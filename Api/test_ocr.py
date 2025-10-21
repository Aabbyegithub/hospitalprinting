#!/usr/bin/env python3
"""
OCR服务测试脚本
测试本地OCR识别功能
"""

import requests
import os
import time

def test_ocr_api():
    """测试OCR API接口"""
    base_url = "http://localhost:5000/api/OCR"
    
    print("=== OCR服务测试 ===")
    
    # 1. 测试获取支持格式
    print("1. 测试获取支持格式...")
    try:
        response = requests.get(f"{base_url}/formats")
        if response.status_code == 200:
            data = response.json()
            print(f"✅ 支持格式: {data.get('data', [])}")
        else:
            print(f"❌ 获取格式失败: {response.status_code}")
    except Exception as e:
        print(f"❌ 获取格式异常: {e}")
    
    # 2. 测试获取支持语言
    print("\n2. 测试获取支持语言...")
    try:
        response = requests.get(f"{base_url}/languages")
        if response.status_code == 200:
            data = response.json()
            print(f"✅ 支持语言: {data.get('data', [])}")
        else:
            print(f"❌ 获取语言失败: {response.status_code}")
    except Exception as e:
        print(f"❌ 获取语言异常: {e}")
    
    # 3. 测试上传图片识别
    print("\n3. 测试上传图片识别...")
    test_image_path = "test_image.jpg"
    
    if os.path.exists(test_image_path):
        try:
            with open(test_image_path, 'rb') as f:
                files = {'file': f}
                response = requests.post(f"{base_url}/upload?language=chi_sim+eng", files=files)
                
            if response.status_code == 200:
                data = response.json()
                if data.get('success'):
                    result = data.get('data', {})
                    print(f"✅ 识别成功!")
                    print(f"   文字内容: {result.get('text', '')}")
                    print(f"   置信度: {result.get('confidence', 0):.2f}%")
                    print(f"   处理时间: {result.get('processingTimeMs', 0)}ms")
                else:
                    print(f"❌ 识别失败: {data.get('message', '')}")
            else:
                print(f"❌ 上传失败: {response.status_code}")
        except Exception as e:
            print(f"❌ 上传异常: {e}")
    else:
        print(f"⚠️  测试图片不存在: {test_image_path}")
        print("   请准备一张包含文字的图片文件")
    
    # 4. 测试文件路径识别
    print("\n4. 测试文件路径识别...")
    if os.path.exists(test_image_path):
        try:
            response = requests.post(f"{base_url}/recognize?filePath={os.path.abspath(test_image_path)}&language=chi_sim+eng")
            
            if response.status_code == 200:
                data = response.json()
                if data.get('success'):
                    result = data.get('data', {})
                    print(f"✅ 识别成功!")
                    print(f"   文字内容: {result.get('text', '')}")
                    print(f"   置信度: {result.get('confidence', 0):.2f}%")
                    print(f"   处理时间: {result.get('processingTimeMs', 0)}ms")
                else:
                    print(f"❌ 识别失败: {data.get('message', '')}")
            else:
                print(f"❌ 识别失败: {response.status_code}")
        except Exception as e:
            print(f"❌ 识别异常: {e}")
    else:
        print(f"⚠️  测试图片不存在: {test_image_path}")
    
    # 5. 测试批量识别
    print("\n5. 测试批量识别...")
    test_folder = "test_images"
    if os.path.exists(test_folder):
        try:
            response = requests.post(f"{base_url}/batch?folderPath={os.path.abspath(test_folder)}&language=chi_sim+eng")
            
            if response.status_code == 200:
                data = response.json()
                if data.get('success'):
                    results = data.get('data', [])
                    success_count = sum(1 for r in results if r.get('success'))
                    print(f"✅ 批量识别完成: {success_count}/{len(results)} 成功")
                    
                    for i, result in enumerate(results):
                        if result.get('success'):
                            print(f"   文件{i+1}: {result.get('filePath', '')}")
                            print(f"   内容: {result.get('text', '')[:50]}...")
                            print(f"   置信度: {result.get('confidence', 0):.2f}%")
                else:
                    print(f"❌ 批量识别失败: {data.get('message', '')}")
            else:
                print(f"❌ 批量识别失败: {response.status_code}")
        except Exception as e:
            print(f"❌ 批量识别异常: {e}")
    else:
        print(f"⚠️  测试文件夹不存在: {test_folder}")
        print("   请创建test_images文件夹并放入测试图片")

def create_test_image():
    """创建测试图片"""
    try:
        from PIL import Image, ImageDraw, ImageFont
        
        # 创建白色背景图片
        img = Image.new('RGB', (400, 200), color='white')
        draw = ImageDraw.Draw(img)
        
        # 添加文字
        text = "Hello World\n你好世界\nOCR Test"
        
        try:
            # 尝试使用系统字体
            font = ImageFont.truetype("arial.ttf", 24)
        except:
            # 使用默认字体
            font = ImageFont.load_default()
        
        # 绘制文字
        draw.text((50, 50), text, fill='black', font=font)
        
        # 保存图片
        img.save("test_image.jpg")
        print("✅ 测试图片已创建: test_image.jpg")
        
    except ImportError:
        print("⚠️  需要安装PIL库来创建测试图片: pip install Pillow")
    except Exception as e:
        print(f"❌ 创建测试图片失败: {e}")

def main():
    """主函数"""
    print("OCR服务测试工具")
    print("=" * 50)
    
    # 检查服务是否运行
    try:
        response = requests.get("http://localhost:5000/api/OCR/formats", timeout=5)
        if response.status_code != 200:
            print("❌ OCR服务未运行，请先启动服务")
            return
    except:
        print("❌ 无法连接到OCR服务，请确保服务已启动")
        return
    
    # 创建测试图片
    if not os.path.exists("test_image.jpg"):
        print("创建测试图片...")
        create_test_image()
        print()
    
    # 运行测试
    test_ocr_api()
    
    print("\n=== 测试完成 ===")
    print("如需更多测试，请:")
    print("1. 准备更多测试图片")
    print("2. 创建test_images文件夹")
    print("3. 使用不同语言的图片测试")

if __name__ == "__main__":
    main()
