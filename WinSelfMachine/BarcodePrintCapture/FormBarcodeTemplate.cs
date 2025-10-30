using System;
using System.IO;
using System.Windows.Forms;

namespace BarcodePrintCapture
{
    public partial class FormBarcodeTemplate : Form
    {
        private string _configPath;

        public FormBarcodeTemplate()
        {
            InitializeComponent();
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BarcodePrint.ini");
        }

        private void FormBarcodeTemplate_Load(object sender, EventArgs e)
        {
            LoadTemplateConfig();
        }

        /// <summary>
        /// 加载模板配置
        /// </summary>
        private void LoadTemplateConfig()
        {
            try
            {
                using (var iniHelper = new IniFileHelper(_configPath))
                {
                    textBoxTitle.Text = iniHelper.Read("Template", "Title", "{HOSPITAL_NAME}");
                    textBoxIdLabel.Text = iniHelper.Read("Template", "IdLabel", "编号：");
                    textBoxNameLabel.Text = iniHelper.Read("Template", "NameLabel", "姓名：");
                    textBoxTimeLabel.Text = iniHelper.Read("Template", "TimeLabel", "登记时间：");
                }
            }
            catch { }
        }

        /// <summary>
        /// 保存按钮点击事件
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (var iniHelper = new IniFileHelper(_configPath))
                {
                    iniHelper.Write("Template", "Title", textBoxTitle.Text);
                    iniHelper.Write("Template", "IdLabel", textBoxIdLabel.Text);
                    iniHelper.Write("Template", "NameLabel", textBoxNameLabel.Text);
                    iniHelper.Write("Template", "TimeLabel", textBoxTimeLabel.Text);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存模板配置失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 取消按钮点击事件
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

