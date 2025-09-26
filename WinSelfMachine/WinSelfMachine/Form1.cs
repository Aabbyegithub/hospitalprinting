using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSelfMachine.Controls;

namespace WinSelfMachine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Resize += Form1_Resize;
            
            // 启用双缓冲，减少闪烁
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint | 
                         ControlStyles.DoubleBuffer | 
                         ControlStyles.ResizeRedraw, true);
            
            // 初始化查询页面控件状态
            InitializeQueryPageControls();
        }

        /// <summary>
        /// 初始化查询页面控件状态
        /// </summary>
        private void InitializeQueryPageControls()
        {
            // 确保查询页面控件初始状态为隐藏
            BtnQuery(false);
            TableData();

        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnQuery_Click(object sender, EventArgs e)
        {
            // 暂停窗体重绘，提高流畅度
            this.SuspendLayout();
            
            try
            {
                // 隐藏主页面控件
                MainControl(false);
                
                // 显示查询页面控件
                BtnQuery(true);
            }
            finally
            {
                // 恢复窗体重绘
                this.ResumeLayout(true);
            }
        }

        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Layout_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 返回上一级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReturn_Load(object sender, EventArgs e)
        {
            // 暂停窗体重绘，提高流畅度
            this.SuspendLayout();
            
            try
            {
                // 隐藏查询页面控件
                BtnQuery(false);
                
                // 显示主页面控件
                MainControl(true);
            }
            finally
            {
                // 恢复窗体重绘
                this.ResumeLayout(true);
            }
        }
        /// <summary>
        /// 界面控件是否显示 - 优化版本，避免闪烁
        /// </summary>
        /// <param name="visible"></param>
        private void MainControl(bool visible)
        {
            // 暂停重绘，避免闪烁
            this.SuspendLayout();
            
            try
            {
                // 批量设置控件可见性
                _3DButton1.Visible = visible;
                _3DButton2.Visible = visible;
                _3DButton3.Visible = visible;
                TxtQuery.Visible = visible;
                TxtBnr.Visible = visible;
                BtnReportQuery.Visible = visible;
                label2.Visible = visible;
            }
            finally
            {
                // 恢复重绘，一次性更新
                this.ResumeLayout(true);
            }
        }

        private void BtnQuery(bool visible)
        {
            // 暂停重绘，避免闪烁
            this.SuspendLayout();

            try
            {
                // 批量设置控件可见性
                if (roundedContainer1 != null) roundedContainer1.Visible = visible;
                if (label3 != null) label3.Visible = visible;
                if (roundTextBox1 != null) roundTextBox1.Visible = visible;
                if (BtnSelect != null) BtnSelect.Visible = visible;
                if (label1 != null) label1.Visible = visible;
                if (BtnReturn != null) BtnReturn.Visible = visible;
                if (roundedContainer2 != null) roundedContainer2.Visible = visible;
                //if (Table != null) Table.Visible = visible;
                
                // 确保控件置于最前面
                if (visible)
                {
                    BringQueryPageControlsToFront();
                }
            }
            finally
            {
                // 恢复重绘，一次性更新
                this.ResumeLayout(true);
            }
        }

        /// <summary>
        /// 加载表格数据
        /// </summary>
        private void TableData()
        {
            // 示例：使用自定义表格控件
            if (Table is TableLayoutControl tableControl)
            {
                // 设置表头
                tableControl.SetHeaders("姓名", "年龄", "性别", "科室", "检查项目");

                // 添加示例数据
                tableControl.AddRow("张三", 35, "男", "内科", "血常规");
                tableControl.AddRow("李四", 28, "女", "妇科", "B超检查");
                tableControl.AddRow("王五", 42, "男", "外科", "CT扫描");
                tableControl.AddRow("赵六", 31, "女", "儿科", "心电图");
                tableControl.AddRow("钱七", 55, "男", "心内科", "心脏彩超");

                // 订阅选择改变事件
                tableControl.RowSelectionChanged += TableControl_RowSelectionChanged;
            }
        }

        /// <summary>
        /// 表格选择改变事件处理
        /// </summary>
        private void TableControl_RowSelectionChanged(object sender, RowSelectionChangedEventArgs e)
        {
            // 获取选中的行索引
            List<int> selectedRows = e.SelectedRowIndices;
            
            // 获取选中的数据
            List<List<object>> selectedData = e.SelectedData;
            
            // 可以在这里处理选中的数据
            System.Diagnostics.Debug.WriteLine($"选中了 {selectedRows.Count} 行数据");
            foreach (var rowData in selectedData)
            {
                System.Diagnostics.Debug.WriteLine($"数据: {string.Join(", ", rowData)}");
            }
        }


        /// <summary>
        /// 确保查询页面控件置于最前面
        /// </summary>
        private void BringQueryPageControlsToFront()
        {
            // 批量处理控件置前，提高效率
            if (roundedContainer1 != null && roundedContainer1.Visible) roundedContainer1.BringToFront();
            if (label3 != null && label3.Visible) label3.BringToFront();
            if (roundTextBox1 != null && roundTextBox1.Visible) roundTextBox1.BringToFront();
            if (BtnSelect != null && BtnSelect.Visible) BtnSelect.BringToFront();
            if (label1 != null && label1.Visible) label1.BringToFront();
            if (BtnReturn != null && BtnReturn.Visible) BtnReturn.BringToFront();
            if (roundedContainer2 != null && roundedContainer2.Visible) roundedContainer2.BringToFront();
            if (Table != null && Table.Visible) Table.BringToFront();
        }

        #region 控制窗体适应不同的分辨率
        /// <summary>
        /// 窗体大小变化时调整控件布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            // 获取当前窗体大小
            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;

            // 计算缩放比例（基于原始设计尺寸 1595x1017）
            float scaleX = (float)formWidth / 1595f;
            float scaleY = (float)formHeight / 1017f;
            
            // 使用平均缩放比例，确保字体大小变化更明显
            float averageScale = (scaleX + scaleY) / 2f;

            // 调整3D按钮的位置和大小
            if (_3DButton1 != null)
            {
                _3DButton1.Location = new Point((int)(213 * scaleX), (int)(523 * scaleY));
                _3DButton1.Size = new Size((int)(297 * scaleX), (int)(285 * scaleY));
                float newFontSize = Math.Max(8f, 15f * averageScale);
                _3DButton1.UpdateFontSize(newFontSize); // 使用强制更新方法
            }

            if (_3DButton2 != null)
            {
                _3DButton2.Location = new Point((int)(642 * scaleX), (int)(523 * scaleY));
                _3DButton2.Size = new Size((int)(297 * scaleX), (int)(285 * scaleY));
                float newFontSize2 = Math.Max(8f, 15f * averageScale);
                _3DButton2.UpdateFontSize(newFontSize2); // 使用强制更新方法
            }

            if (_3DButton3 != null)
            {
                _3DButton3.Location = new Point((int)(1071 * scaleX), (int)(523 * scaleY));
                _3DButton3.Size = new Size((int)(297 * scaleX), (int)(285 * scaleY));
                float newFontSize3 = Math.Max(8f, 15f * averageScale);
                _3DButton3.UpdateFontSize(newFontSize3); // 使用强制更新方法
            }

            // 调整标签位置和字体大小
            if (TxtQuery != null)
            {
                TxtQuery.Location = new Point((int)(240 * scaleX), (int)(383 * scaleY));
                float labelFontSize = Math.Max(8f, 15f * averageScale);
                TxtQuery.Font = new Font(TxtQuery.Font.FontFamily, labelFontSize, TxtQuery.Font.Style);
            }

            if (label2 != null)
            {
                label2.Location = new Point(0, (int)(203 * scaleY));
                label2.Size = new Size(formWidth, (int)(84 * scaleY));
                float titleFontSize = Math.Max(12f, 36f * averageScale);
                label2.Font = new Font(label2.Font.FontFamily, titleFontSize, label2.Font.Style);
            }

            if (TxtTitle != null)
            {
                TxtTitle.Location = new Point((int)(132 * scaleX), (int)(52 * scaleY));
                float titleFontSize = Math.Max(10f, 21.75f * averageScale);
                TxtTitle.Font = new Font(TxtTitle.Font.FontFamily, titleFontSize, TxtTitle.Font.Style);
            }

            // 调整按钮位置和字体大小
            if (BtnReportQuery != null)
            {
                BtnReportQuery.Location = new Point((int)(1142 * scaleX), (int)(369 * scaleY));
                BtnReportQuery.Size = new Size((int)(187 * scaleX), (int)(63 * scaleY));
                float buttonFontSize = Math.Max(8f, 12f * averageScale);
                BtnReportQuery.Font = new Font(BtnReportQuery.Font.FontFamily, buttonFontSize, BtnReportQuery.Font.Style);
            }

            // 调整图片框位置
            if (pictureBox1 != null)
            {
                pictureBox1.Location = new Point((int)(22 * scaleX), (int)(44 * scaleY));
                pictureBox1.Size = new Size((int)(66 * scaleX), (int)(66 * scaleY));
            }

            // 调整TxtBnr控件位置、大小和字体
            if (TxtBnr != null)
            {
                TxtBnr.Location = new Point((int)(348 * scaleX), (int)(369 * scaleY));
                TxtBnr.Size = new Size((int)(761 * scaleX), (int)(63 * scaleY));
                float txtBnrFontSize = Math.Max(8f, 15f * averageScale);
                //TxtBnr.UpdateFontSize(txtBnrFontSize);
            }

            // 调整查询页面控件布局
            AdjustQueryPageControls(scaleX, scaleY, averageScale);
        }

        /// <summary>
        /// 调整查询页面控件布局
        /// </summary>
        /// <param name="scaleX">X轴缩放比例</param>
        /// <param name="scaleY">Y轴缩放比例</param>
        /// <param name="averageScale">平均缩放比例</param>
        private void AdjustQueryPageControls(float scaleX, float scaleY, float averageScale)
        {
            try
            {
                // 基于原始设计尺寸计算查询页面区域
                // 原始窗体尺寸: 1595x1017
                // 查询页面占据窗体的主要区域，留出适当边距
                int queryPageWidth = (int)(1400 * scaleX);   // 约87%的窗体宽度
                int queryPageHeight = (int)(800 * scaleY);   // 约78%的窗体高度
                int queryPageX = (int)(100 * scaleX);        // 左边距
                int queryPageY = (int)(130 * scaleY);        // 上边距
                
                // 左右分栏比例：左侧35%，右侧65%
                int leftColumnWidth = (int)(queryPageWidth * 0.35f);
                int rightColumnWidth = (int)(queryPageWidth * 0.65f);
                int rightColumnX = queryPageX + leftColumnWidth;
                
                // 调整roundedContainer1（主容器）- 整个查询页面的背景容器
                if (roundedContainer1 != null)
                {
                    roundedContainer1.Location = new Point(queryPageX, queryPageY);
                    roundedContainer1.Size = new Size(queryPageWidth, queryPageHeight);
                }

                // 调整label3（身份检索标签）- 左侧标题
                if (label3 != null)
                {
                    int label3X = queryPageX + (int)(20 * scaleX);
                    int label3Y = queryPageY + (int)(30 * scaleY);
                    int label3Width = (int)(150 * scaleX);
                    int label3Height = (int)(30 * scaleY);
                    
                    label3.Location = new Point(label3X, label3Y);
                    label3.Size = new Size(label3Width, label3Height);
                    float label3FontSize = Math.Max(8f, 16f * averageScale);
                    label3.Font = new Font(label3.Font.FontFamily, label3FontSize, FontStyle.Bold);
                }

                // 调整roundTextBox1（输入框）- 左侧输入框
                if (roundTextBox1 != null)
                {
                    int textBoxX = queryPageX + (int)(20 * scaleX);
                    int textBoxY = queryPageY + (int)(80 * scaleY);
                    int textBoxWidth = leftColumnWidth - (int)(40 * scaleX);
                    int textBoxHeight = (int)(40 * scaleY);
                    
                    roundTextBox1.Location = new Point(textBoxX, textBoxY);
                    roundTextBox1.Size = new Size(textBoxWidth, textBoxHeight);
                    float textBoxFontSize = Math.Max(8f, 14f * averageScale);
                    roundTextBox1.FontSize = (int)textBoxFontSize;
                }

                // 调整BtnSelect（查询按钮）- 左侧查询按钮
                if (BtnSelect != null)
                {
                    int btnX = queryPageX + (int)(20 * scaleX);
                    int btnY = queryPageY + (int)(140 * scaleY);
                    int btnWidth = leftColumnWidth - (int)(40 * scaleX);
                    int btnHeight = (int)(40 * scaleY);
                    
                    BtnSelect.Location = new Point(btnX, btnY);
                    BtnSelect.Size = new Size(btnWidth, btnHeight);
                    float btnSelectFontSize = Math.Max(8f, 14f * averageScale);
                    BtnSelect.Font = new Font(BtnSelect.Font.FontFamily, btnSelectFontSize, BtnSelect.Font.Style);
                }

                // 调整label1（检验报告单标签）- 右侧标题
                if (label1 != null)
                {
                    int label1X = rightColumnX + (int)(20 * scaleX);
                    int label1Y = queryPageY + (int)(30 * scaleY);
                    int label1Width = (int)(200 * scaleX);
                    int label1Height = (int)(30 * scaleY);
                    
                    label1.Location = new Point(label1X, label1Y);
                    label1.Size = new Size(label1Width, label1Height);
                    float label1FontSize = Math.Max(8f, 16f * averageScale);
                    label1.Font = new Font(label1.Font.FontFamily, label1FontSize, FontStyle.Bold);
                }

                // 调整BtnReturn（返回按钮）- 右上角返回按钮
                if (BtnReturn != null)
                {
                    int returnBtnX = rightColumnX + rightColumnWidth - (int)(120 * scaleX);
                    int returnBtnY = queryPageY + (int)(30 * scaleY);
                    int returnBtnWidth = (int)(100 * scaleX);
                    int returnBtnHeight = (int)(30 * scaleY);
                    
                    BtnReturn.Location = new Point(returnBtnX, returnBtnY);
                    BtnReturn.Size = new Size(returnBtnWidth, returnBtnHeight);
                    float btnReturnFontSize = Math.Max(8f, 14f * averageScale);
                    BtnReturn.Font = new Font(BtnReturn.Font.FontFamily, btnReturnFontSize, BtnReturn.Font.Style);
                }

                // 调整roundedContainer2（报告显示容器）- 右侧报告区域
                if (roundedContainer2 != null)
                {
                    int reportX = rightColumnX + (int)(20 * scaleX);
                    int reportY = queryPageY + (int)(80 * scaleY);
                    int reportWidth = rightColumnWidth - (int)(40 * scaleX);
                    int reportHeight = queryPageHeight - (int)(100 * scaleY);
                    
                    roundedContainer2.Location = new Point(reportX, reportY);
                    roundedContainer2.Size = new Size(reportWidth, reportHeight);
                }

                // 调整Table（表格控件）- 报告内容区域
                if (Table != null)
                {
                    int tableX = rightColumnX + (int)(40 * scaleX);
                    int tableY = queryPageY + (int)(100 * scaleY);
                    int tableWidth = rightColumnWidth - (int)(80 * scaleX);
                    int tableHeight = queryPageHeight - (int)(140 * scaleY);

                    Table.Location = new Point(tableX, tableY);
                    Table.Size = new Size(tableWidth, tableHeight);
                    float tableFontSize = Math.Max(8f, 12f * averageScale);
                    Table.Font = new Font(Table.Font.FontFamily, tableFontSize, Table.Font.Style);
                }
            }
            catch (Exception ex)
            {
                // 静默处理异常，避免影响用户体验
            }
        }

        #endregion
    }
}
