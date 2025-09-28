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

        }

        #region  按钮操作
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
        private void BtnReturn_Click(object sender, EventArgs e)
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
        /// 查询界面打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrint_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 查询报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelect_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 检验报告查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReportQuery_Click(object sender, EventArgs e)
        {

        }
        #endregion
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
                if (roundedContainer3 != null) roundedContainer3.Visible = visible;
                if (Table != null) Table.Visible = visible;
                if (BtnPrint != null) BtnPrint.Visible = visible;
                
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

        }
        /// <summary>
        /// 确保查询页面控件置于最前面
        /// </summary>
        private void BringQueryPageControlsToFront()
        {
            // 批量处理控件置前，提高效率
            if (roundedContainer1 != null && roundedContainer1.Visible) roundedContainer1.BringToFront();
            if (roundedContainer2 != null && roundedContainer2.Visible) roundedContainer2.BringToFront();
            if (roundedContainer3 != null && roundedContainer3.Visible) roundedContainer3.BringToFront();
            if (label3 != null && label3.Visible) label3.BringToFront();
            if (roundTextBox1 != null && roundTextBox1.Visible) roundTextBox1.BringToFront();
            if (BtnSelect != null && BtnSelect.Visible) BtnSelect.BringToFront();
            if (label1 != null && label1.Visible) label1.BringToFront();
            if (BtnReturn != null && BtnReturn.Visible) BtnReturn.BringToFront();
            if (Table != null && Table.Visible) Table.BringToFront();
            if (BtnPrint != null && BtnPrint.Visible) BtnPrint.BringToFront();
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

                // 调整roundedContainer3（左侧身份检索容器）
                if (roundedContainer3 != null)
                {
                    int leftContainerX = queryPageX + (int)(20 * scaleX);
                    int leftContainerY = queryPageY + (int)(20 * scaleY);
                    int leftContainerWidth = leftColumnWidth - (int)(0 * scaleX);
                    int leftContainerHeight = queryPageHeight - (int)(40 * scaleY);
                    
                    roundedContainer3.Location = new Point(leftContainerX, leftContainerY);
                    roundedContainer3.Size = new Size(leftContainerWidth, leftContainerHeight);
                    roundedContainer3.TitleText = "身份检索";
                    float titleFontSize = Math.Max(12f, 18f * averageScale);
                    roundedContainer3.TitleFont = new Font("宋体", titleFontSize, FontStyle.Bold);
                }

                // 调整label3（身份检索标签）- 左侧标题
                if (label3 != null)
                {
                    int label3X = queryPageX + (int)(40 * scaleX);
                    int label3Y = queryPageY + (int)(50 * scaleY);
                    int label3Width = (int)(150 * scaleX);
                    int label3Height = (int)(30 * scaleY);
                    
                    label3.Location = new Point(label3X, label3Y);
                    label3.Size = new Size(label3Width, label3Height);
                    float label3FontSize = Math.Max(10f, 16f * averageScale);
                    label3.Font = new Font(label3.Font.FontFamily, label3FontSize, FontStyle.Bold);
                    label3.Text = "身份检索";
                }

                // 调整roundTextBox1（输入框）- 左侧输入框
                if (roundTextBox1 != null)
                {
                    int textBoxX = queryPageX + (int)(40 * scaleX);
                    int textBoxY = queryPageY + (int)(600 * scaleY);
                    int textBoxWidth = leftColumnWidth - (int)(80 * scaleX);
                    int textBoxHeight = (int)(50 * scaleY);
                    
                    roundTextBox1.Location = new Point(textBoxX, textBoxY);
                    roundTextBox1.Size = new Size(textBoxWidth, textBoxHeight);
                    float textBoxFontSize = Math.Max(10f, 16f * averageScale);
                    roundTextBox1.FontSize = (int)textBoxFontSize;
                }

                // 调整BtnSelect（查询按钮）- 左侧查询按钮
                if (BtnSelect != null)
                {
                    int btnX = queryPageX + (int)(40 * scaleX);
                    int btnY = queryPageY + (int)(700 * scaleY);
                    int btnWidth = leftColumnWidth - (int)(80 * scaleX);
                    int btnHeight = (int)(50 * scaleY);
                    
                    BtnSelect.Location = new Point(btnX, btnY);
                    BtnSelect.Size = new Size(btnWidth, btnHeight);
                    float btnSelectFontSize = Math.Max(10f, 16f * averageScale);
                    BtnSelect.Font = new Font(BtnSelect.Font.FontFamily, btnSelectFontSize, BtnSelect.Font.Style);
                    BtnSelect.ButtonText = "查询";
                }

                // 调整roundedContainer2（右侧查询结果容器）
                if (roundedContainer2 != null)
                {
                    int rightContainerX = rightColumnX + (int)(20 * scaleX);
                    int rightContainerY = queryPageY + (int)(20 * scaleY);
                    int rightContainerWidth = rightColumnWidth - (int)(40 * scaleX);
                    int rightContainerHeight = queryPageHeight - (int)(40 * scaleY);
                    
                    roundedContainer2.Location = new Point(rightContainerX, rightContainerY);
                    roundedContainer2.Size = new Size(rightContainerWidth, rightContainerHeight);
                    roundedContainer2.TitleText = "查询结果";
                    float titleFontSize = Math.Max(12f, 18f * averageScale);
                    roundedContainer2.TitleFont = new Font("宋体", titleFontSize, FontStyle.Bold);
                }

                // 调整label1（患者信息标签）- 位于查询结果标题下方，左侧
                if (label1 != null)
                {
                    int label1X = rightColumnX + (int)(40 * scaleX);
                    int label1Y = queryPageY + (int)(90 * scaleY);  // 在查询结果标题下方
                    int label1Width = (int)(120 * scaleX);
                    int label1Height = (int)(30 * scaleY);
                    
                    label1.Location = new Point(label1X, label1Y);
                    label1.Size = new Size(label1Width, label1Height);
                    float label1FontSize = Math.Max(10f, 14f * averageScale);
                    label1.Font = new Font(label1.Font.FontFamily, label1FontSize, FontStyle.Regular);
                    label1.Text = "患者信息：";
                }

                // 调整label3（检验单号标签）- 与患者信息在同一行，右侧
                if (label3 != null)
                {
                    int examInfoX = rightColumnX + (int)(300 * scaleX);  // 在患者信息右侧
                    int examInfoY = queryPageY + (int)(90 * scaleY);     // 与患者信息同一行
                    int examInfoWidth = (int)(150 * scaleX);
                    int examInfoHeight = (int)(30 * scaleY);
                    
                    label3.Location = new Point(examInfoX, examInfoY);
                    label3.Size = new Size(examInfoWidth, examInfoHeight);
                    float examInfoFontSize = Math.Max(10f, 14f * averageScale);
                    label3.Font = new Font(label3.Font.FontFamily, examInfoFontSize, FontStyle.Regular);
                    label3.Text = "检验单号：";
                }



                // 调整BtnReturn（返回按钮）- 右上角返回按钮
                if (BtnReturn != null)
                {
                    int returnBtnX = rightColumnX + rightColumnWidth - (int)(120 * scaleX);
                    int returnBtnY = queryPageY + (int)(30 * scaleY);
                    int returnBtnWidth = (int)(80 * scaleX);
                    int returnBtnHeight = (int)(40 * scaleY);
                    
                    BtnReturn.Location = new Point(returnBtnX, returnBtnY);
                    BtnReturn.Size = new Size(returnBtnWidth, returnBtnHeight);
                    float btnReturnFontSize = Math.Max(10f, 14f * averageScale);
                    BtnReturn.Font = new Font(BtnReturn.Font.FontFamily, btnReturnFontSize, BtnReturn.Font.Style);
                    BtnReturn.ButtonText = "返回";
                }

                // 调整BtnPrint（打印按钮）- 右上角打印按钮
                if (BtnPrint != null)
                {
                    int printBtnX = rightColumnX + rightColumnWidth - (int)(120 * scaleX);
                    int printBtnY = queryPageY + (int)(80 * scaleY);
                    int printBtnWidth = (int)(80 * scaleX);
                    int printBtnHeight = (int)(40 * scaleY);
                    
                    BtnPrint.Location = new Point(printBtnX, printBtnY);
                    BtnPrint.Size = new Size(printBtnWidth, printBtnHeight);
                    float btnPrintFontSize = Math.Max(10f, 14f * averageScale);
                    BtnPrint.Font = new Font(BtnPrint.Font.FontFamily, btnPrintFontSize, BtnPrint.Font.Style);
                    BtnPrint.ButtonText = "打印";
                }


                // 调整Table（表格控件）- 报告内容区域
                if (Table != null)
                {
                    // 表格位于右侧区域，在患者信息和检验单号标签下方
                    int tableX = rightColumnX + (int)(40 * scaleX);
                    int tableY = queryPageY + (int)(130 * scaleY);  // 在标签下方
                    int tableWidth = rightColumnWidth - (int)(80 * scaleX);
                    int tableHeight = queryPageHeight - (int)(170 * scaleY); // 为其他控件留出空间
                    
                    Table.Location = new Point(tableX, tableY);
                    Table.Size = new Size(tableWidth, tableHeight);
                    float tableFontSize = Math.Max(10f, 14f * averageScale);
                    Table.Font = new Font(Table.Font.FontFamily, tableFontSize, Table.Font.Style);
                    
                    // 调整列宽
                    if (Table.Columns.Count >= 4)
                    {
                        Table.Columns[0].Width = (int)(80 * scaleX);  // 序号列
                        Table.Columns[1].Width = (int)((tableWidth - 80 * scaleX) * 0.3f);  // 科室列
                        Table.Columns[2].Width = (int)((tableWidth - 80 * scaleX) * 0.5f);  // 报告名称列
                        Table.Columns[3].Width = (int)((tableWidth - 80 * scaleX) * 0.2f);  // 操作列
                    }
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
