import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { viteStaticCopy } from 'vite-plugin-static-copy'

export default defineConfig({
  plugins: [
    vue(),
    viteStaticCopy({
      targets: [
        {
          src: 'src/assets/**/*', // 复制所有资源文件
          dest: 'assets'          // 目标目录
        }
      ]
    })
  ],
  server: {
    host: '0.0.0.0', // 允许外部访问
    port: 8081,      // 端口号
    open: true,      // 自动打开浏览器
    cors: true,      // 启用CORS
    
  }
})