using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSelfMachine.Controls
{
    public partial class TableLayoutControl : UserControl
    {
        #region 私有字段
        private TableLayoutPanel tableLayoutPanel;
        private List<string> columnHeaders;
        private List<List<object>> tableData;
        private List<CheckBox> checkBoxes;
        private List<int> selectedRows;
        private List<float> columnWidths;
        private Color headerBackColor = Color.FromArgb(240, 240, 240);
        private Color headerForeColor = Color.Black;
        private Color rowBackColor = Color.White;
        private Color rowForeColor = Color.Black;
        private Color selectedRowBackColor = Color.FromArgb(200, 220, 255);
        private Font headerFont = new Font("Microsoft YaHei", 12F, FontStyle.Bold);
        private Font cellFont = new Font("Microsoft YaHei", 11F);
        private int rowHeight = 30;
        private int headerHeight = 30;
        #endregion

        #region 公共属性
        [Category("表格设置")]
        [Description("表头背景色")]
        public Color HeaderBackColor
        {
            get { return headerBackColor; }
            set
            {
                headerBackColor = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("表头文字颜色")]
        public Color HeaderForeColor
        {
            get { return headerForeColor; }
            set
            {
                headerForeColor = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("行背景色")]
        public Color RowBackColor
        {
            get { return rowBackColor; }
            set
            {
                rowBackColor = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("行文字颜色")]
        public Color RowForeColor
        {
            get { return rowForeColor; }
            set
            {
                rowForeColor = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("选中行背景色")]
        public Color SelectedRowBackColor
        {
            get { return selectedRowBackColor; }
            set
            {
                selectedRowBackColor = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("表头字体")]
        public Font HeaderFont
        {
            get { return headerFont; }
            set
            {
                headerFont = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("单元格字体")]
        public Font CellFont
        {
            get { return cellFont; }
            set
            {
                cellFont = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("行高")]
        public int RowHeight
        {
            get { return rowHeight; }
            set
            {
                rowHeight = value;
                RefreshTable();
            }
        }

        [Category("表格设置")]
        [Description("表头高度")]
        public int HeaderHeight
        {
            get { return headerHeight; }
            set
            {
                headerHeight = value;
                RefreshTable();
            }
        }

        [Browsable(false)]
        public List<int> SelectedRows
        {
            get { return selectedRows; }
        }

        [Browsable(false)]
        public List<List<object>> SelectedData
        {
            get
            {
                List<List<object>> selectedData = new List<List<object>>();
                foreach (int rowIndex in selectedRows)
                {
                    if (rowIndex >= 0 && rowIndex < tableData.Count)
                    {
                        selectedData.Add(new List<object>(tableData[rowIndex]));
                    }
                }
                return selectedData;
            }
        }

        [Category("布局设置")]
        [Description("是否自动填充父容器")]
        public bool AutoFill
        {
            get { return this.Dock == DockStyle.Fill; }
            set
            {
                this.Dock = value ? DockStyle.Fill : DockStyle.None;
            }
        }

        [Category("布局设置")]
        [Description("是否显示剩余空间留白")]
        public bool ShowRemainingSpace { get; set; } = true;

        [Category("布局设置")]
        [Description("复选框大小")]
        public int CheckBoxSize { get; set; } = 16;
        #endregion

        #region 事件
        public event EventHandler<RowSelectionChangedEventArgs> RowSelectionChanged;
        #endregion

        public TableLayoutControl()
        {
            InitializeComponent();
            InitializeControl();
        }

        private void InitializeControl()
        {
            // 默认不自动填充，让控件可以自由调整大小和位置
            this.Dock = DockStyle.None;
            this.BackColor = Color.White;
            this.Size = new Size(400, 300); // 设置默认大小
            this.Location = new Point(0, 0);
            
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.AutoScroll = true;
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.BackColor = Color.White;
            
            this.Controls.Add(tableLayoutPanel);
            
            columnHeaders = new List<string>();
            tableData = new List<List<object>>();
            checkBoxes = new List<CheckBox>();
            selectedRows = new List<int>();
            columnWidths = new List<float>();
        }

        /// <summary>
        /// 设置表头
        /// </summary>
        /// <param name="headers">表头数组</param>
        public void SetHeaders(params string[] headers)
        {
            columnHeaders.Clear();
            columnHeaders.AddRange(headers);
            columnHeaders.Add("选择"); // 添加复选框列
            
            // 设置默认列宽（等宽）
            columnWidths.Clear();
            float defaultWidth = 100f / columnHeaders.Count;
            for (int i = 0; i < columnHeaders.Count; i++)
            {
                columnWidths.Add(defaultWidth);
            }
            
            RefreshTable();
        }

        /// <summary>
        /// 设置表头和列宽
        /// </summary>
        /// <param name="headers">表头数组</param>
        /// <param name="widths">列宽数组（百分比）</param>
        public void SetHeadersWithWidths(string[] headers, float[] widths)
        {
            columnHeaders.Clear();
            columnHeaders.AddRange(headers);
            columnHeaders.Add("选择"); // 添加复选框列
            
            columnWidths.Clear();
            columnWidths.AddRange(widths);
            columnWidths.Add(15f); // 复选框列固定15%宽度
            
            RefreshTable();
        }

        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="widths">列宽数组（百分比）</param>
        public void SetColumnWidths(params float[] widths)
        {
            if (widths.Length != columnHeaders.Count - 1) // 不包括复选框列
            {
                throw new ArgumentException("列宽数量必须与数据列数量一致");
            }
            
            columnWidths.Clear();
            columnWidths.AddRange(widths);
            columnWidths.Add(15f); // 复选框列固定15%宽度
            
            RefreshTable();
        }

        /// <summary>
        /// 添加数据行
        /// </summary>
        /// <param name="rowData">行数据</param>
        public void AddRow(params object[] rowData)
        {
            List<object> row = new List<object>(rowData);
            tableData.Add(row);
            RefreshTable();
        }

        /// <summary>
        /// 添加多行数据
        /// </summary>
        /// <param name="rows">多行数据</param>
        public void AddRows(List<List<object>> rows)
        {
            tableData.AddRange(rows);
            RefreshTable();
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        public void ClearData()
        {
            tableData.Clear();
            selectedRows.Clear();
            checkBoxes.Clear();
            columnWidths.Clear();
            RefreshTable();
        }

        /// <summary>
        /// 删除选中的行
        /// </summary>
        public void DeleteSelectedRows()
        {
            if (selectedRows.Count == 0) return;

            // 按降序排序，从后往前删除
            selectedRows.Sort((a, b) => b.CompareTo(a));
            
            foreach (int rowIndex in selectedRows)
            {
                if (rowIndex >= 0 && rowIndex < tableData.Count)
                {
                    tableData.RemoveAt(rowIndex);
                }
            }
            
            selectedRows.Clear();
            RefreshTable();
        }

        /// <summary>
        /// 刷新表格
        /// </summary>
        private void RefreshTable()
        {
            if (tableLayoutPanel == null) return;

            // 清除现有控件
            tableLayoutPanel.Controls.Clear();
            checkBoxes.Clear();

            if (columnHeaders.Count == 0) return;

            // 设置列数和行数
            tableLayoutPanel.ColumnCount = columnHeaders.Count;
            tableLayoutPanel.RowCount = tableData.Count + 1; // +1 for header

            // 设置列宽
            for (int i = 0; i < columnHeaders.Count; i++)
            {
                float width = i < columnWidths.Count ? columnWidths[i] : 100f / columnHeaders.Count;
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, width));
            }

            // 添加表头
            for (int col = 0; col < columnHeaders.Count; col++)
            {
                Label headerLabel = new Label();
                headerLabel.Text = columnHeaders[col];
                headerLabel.TextAlign = ContentAlignment.MiddleCenter;
                headerLabel.BackColor = headerBackColor;
                headerLabel.ForeColor = headerForeColor;
                headerLabel.Font = headerFont;
                headerLabel.Dock = DockStyle.Fill;
                headerLabel.Margin = new Padding(0);
                headerLabel.AutoSize = false; // 禁用自动调整大小
                
                tableLayoutPanel.Controls.Add(headerLabel, col, 0);
            }

            // 添加数据行
            for (int row = 0; row < tableData.Count; row++)
            {
                for (int col = 0; col < columnHeaders.Count; col++)
                {
                    Control cellControl;
                    
                    if (col == columnHeaders.Count - 1) // 最后一列是复选框
                    {
                        CheckBox checkBox = new CheckBox();
                        checkBox.TextAlign = ContentAlignment.MiddleCenter;
                        checkBox.Dock = DockStyle.Fill;
                        checkBox.Margin = new Padding(0);
                        checkBox.Tag = row; // 存储行索引
                        checkBox.CheckedChanged += CheckBox_CheckedChanged;
                        
                        // 设置复选框大小
                        checkBox.Width = CheckBoxSize;
                        checkBox.Height = CheckBoxSize;
                        checkBox.AutoSize = false; // 禁用自动调整大小
                        
                        checkBoxes.Add(checkBox);
                        cellControl = checkBox;
                    }
                    else
                    {
                        Label cellLabel = new Label();
                        cellLabel.Text = row < tableData.Count && col < tableData[row].Count ? 
                                       tableData[row][col]?.ToString() ?? "" : "";
                        cellLabel.TextAlign = ContentAlignment.MiddleLeft;
                        cellLabel.BackColor = selectedRows.Contains(row) ? selectedRowBackColor : rowBackColor;
                        cellLabel.ForeColor = rowForeColor;
                        cellLabel.Font = cellFont;
                        cellLabel.Dock = DockStyle.Fill;
                        cellLabel.Margin = new Padding(2, 0, 2, 0);
                        cellLabel.AutoSize = false; // 禁用自动调整大小
                        cellControl = cellLabel;
                    }
                    
                    tableLayoutPanel.Controls.Add(cellControl, col, row + 1);
                }
            }

            // 清除现有的行样式
            tableLayoutPanel.RowStyles.Clear();
            
            // 设置行高 - 确保每行高度完全一致
            for (int i = 0; i < tableLayoutPanel.RowCount; i++)
            {
                if (i == 0)
                {
                    // 表头行使用固定高度
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, headerHeight));
                }
                else
                {
                    // 数据行使用固定高度，确保完全一致
                    tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
                }
            }
            
            // 根据设置决定是否添加剩余空间留白
            if (ShowRemainingSpace && tableData.Count > 0)
            {
                tableLayoutPanel.RowCount += 1;
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            }
            
            // 强制设置所有行的高度，确保一致性
            for (int i = 0; i < tableLayoutPanel.RowCount; i++)
            {
                if (i < tableLayoutPanel.RowStyles.Count)
                {
                    if (i == 0)
                    {
                        tableLayoutPanel.RowStyles[i].Height = headerHeight;
                    }
                    else if (i < tableData.Count + 1)
                    {
                        tableLayoutPanel.RowStyles[i].Height = rowHeight;
                    }
                }
            }
        }

        /// <summary>
        /// 复选框选中状态改变事件
        /// </summary>
        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) return;

            int rowIndex = (int)checkBox.Tag;
            
            if (checkBox.Checked)
            {
                if (!selectedRows.Contains(rowIndex))
                {
                    selectedRows.Add(rowIndex);
                }
            }
            else
            {
                selectedRows.Remove(rowIndex);
            }

            // 更新行的背景色
            UpdateRowBackground(rowIndex, checkBox.Checked);
            
            // 触发选择改变事件
            OnRowSelectionChanged(new RowSelectionChangedEventArgs(selectedRows, SelectedData));
        }

        /// <summary>
        /// 更新行背景色
        /// </summary>
        private void UpdateRowBackground(int rowIndex, bool isSelected)
        {
            if (rowIndex < 0 || rowIndex >= tableData.Count) return;

            for (int col = 0; col < columnHeaders.Count - 1; col++) // 不包括复选框列
            {
                Control cell = tableLayoutPanel.GetControlFromPosition(col, rowIndex + 1);
                if (cell is Label label)
                {
                    label.BackColor = isSelected ? selectedRowBackColor : rowBackColor;
                }
            }
        }

        /// <summary>
        /// 触发行选择改变事件
        /// </summary>
        protected virtual void OnRowSelectionChanged(RowSelectionChangedEventArgs e)
        {
            RowSelectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 全选/取消全选
        /// </summary>
        public void SelectAll(bool select)
        {
            foreach (CheckBox checkBox in checkBoxes)
            {
                checkBox.Checked = select;
            }
        }

        /// <summary>
        /// 获取指定行的数据
        /// </summary>
        public List<object> GetRowData(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < tableData.Count)
            {
                return new List<object>(tableData[rowIndex]);
            }
            return new List<object>();
        }

        /// <summary>
        /// 更新指定行的数据
        /// </summary>
        public void UpdateRowData(int rowIndex, params object[] newData)
        {
            if (rowIndex >= 0 && rowIndex < tableData.Count)
            {
                tableData[rowIndex] = new List<object>(newData);
                RefreshTable();
            }
        }

        /// <summary>
        /// 强制刷新表格布局
        /// </summary>
        public void ForceRefresh()
        {
            RefreshTable();
            this.Invalidate();
            this.Update();
        }

        /// <summary>
        /// 强制重新计算行高，确保一致性
        /// </summary>
        public void ForceRecalculateRowHeights()
        {
            if (tableLayoutPanel == null) return;

            // 强制重新计算所有行的高度
            for (int i = 0; i < tableLayoutPanel.RowStyles.Count; i++)
            {
                if (i == 0)
                {
                    tableLayoutPanel.RowStyles[i].SizeType = SizeType.Absolute;
                    tableLayoutPanel.RowStyles[i].Height = headerHeight;
                }
                else if (i <= tableData.Count)
                {
                    tableLayoutPanel.RowStyles[i].SizeType = SizeType.Absolute;
                    tableLayoutPanel.RowStyles[i].Height = rowHeight;
                }
            }

            // 强制重新布局
            tableLayoutPanel.PerformLayout();
            this.Invalidate();
            this.Update();
        }
    }

    /// <summary>
    /// 行选择改变事件参数
    /// </summary>
    public class RowSelectionChangedEventArgs : EventArgs
    {
        public List<int> SelectedRowIndices { get; }
        public List<List<object>> SelectedData { get; }

        public RowSelectionChangedEventArgs(List<int> selectedRowIndices, List<List<object>> selectedData)
        {
            SelectedRowIndices = selectedRowIndices;
            SelectedData = selectedData;
        }
    }
}
