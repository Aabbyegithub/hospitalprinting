using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BarcodePrintCapture
{
    /// <summary>
    /// 悬停窗口类
    /// </summary>
    public partial class FormHoverWindow : Form
    {
        private FormBarcodeCapture _mainForm;
        private List<string> _nameList = new List<string>();
        private Label[] _labels;
        private CheckBox[] _checkBoxes;
        private Button _btnPrint;

        public FormHoverWindow(FormBarcodeCapture mainForm)
        {
            _mainForm = mainForm;
            InitializeCustomComponents();
        }

        /// <summary>
        /// 初始化自定义控件
        /// </summary>
        private void InitializeCustomComponents()
        {
            this.Text = "条码手动打印";
            this.Size = new Size(200, 200);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - 220, 100);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.TopMost = true;

            _labels = new Label[3];
            _checkBoxes = new CheckBox[3];

            // 创建标签和复选框
            for (int i = 0; i < 3; i++)
            {
                _labels[i] = new Label
                {
                    Text = $"测试{i + 1}",
                    Location = new Point(10, 10 + i * 30),
                    Size = new Size(150, 20),
                    AutoSize = false
                };
                this.Controls.Add(_labels[i]);

                _checkBoxes[i] = new CheckBox
                {
                    Location = new Point(170, 10 + i * 30),
                    Size = new Size(20, 20)
                };
                this.Controls.Add(_checkBoxes[i]);
            }

            // 默认选中最新的
            _checkBoxes[2].Checked = true;

            // 创建打印按钮
            _btnPrint = new Button
            {
                Text = "打印条码",
                Location = new Point(10, 110),
                Size = new Size(180, 40)
            };
            _btnPrint.Click += BtnPrint_Click;
            this.Controls.Add(_btnPrint);
        }

        /// <summary>
        /// 更新姓名列表
        /// </summary>
        public void UpdateName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            // 添加到列表（避免重复）
            _nameList.Remove(name);
            _nameList.Insert(0, name);

            // 保持最多3个
            if (_nameList.Count > 3)
            {
                _nameList.RemoveAt(3);
            }

            // 更新显示（自下而上）
            for (int i = 0; i < Math.Min(_nameList.Count, 3); i++)
            {
                int displayIndex = 2 - i; // 逆序显示
                if (displayIndex >= 0 && displayIndex < 3)
                {
                    _labels[displayIndex].Text = _nameList[i];
                }
            }
        }

        /// <summary>
        /// 打印按钮点击事件
        /// </summary>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            // 查找选中的项
            for (int i = 0; i < _checkBoxes.Length; i++)
            {
                if (_checkBoxes[i].Checked)
                {
                    string selectedName = _labels[i].Text;
                    if (!string.IsNullOrEmpty(selectedName) && _nameList.Contains(selectedName))
                    {
                        int index = _nameList.IndexOf(selectedName);
                        if (index >= 0 && index < _nameList.Count)
                        {
                            // 这里应该调用打印方法，需要从配置中获取ID
                            // 简化处理：只打印姓名
                            MessageBox.Show($"打印条码：{selectedName}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    break;
                }
            }
        }
    }
}

