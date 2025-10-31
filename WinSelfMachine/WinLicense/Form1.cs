using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLicense
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //LoadMachineCode();
            InitializeDateTimePickers();
        }

        /// <summary>
        /// 加载并显示机器码
        /// </summary>
        private void LoadMachineCode()
        {
            try
            {
                string machineCode = LicenseHelper.GetMachineCode();
                TxtMachineCode.Text = machineCode;
                TxtMachineCode.ReadOnly = true;
                TxtMachineCode.BackColor = Color.LightGray;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取机器码失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 初始化日期时间选择器
        /// </summary>
        private void InitializeDateTimePickers()
        {
            // 开始时间默认为当前时间
            DtpStartTime.Format = DateTimePickerFormat.Custom;
            DtpStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            DtpStartTime.ShowUpDown = true;
            DtpStartTime.Value = DateTime.Now;

            // 结束时间默认为30天后
            DtpEndTime.Format = DateTimePickerFormat.Custom;
            DtpEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            DtpEndTime.ShowUpDown = true;
            DtpEndTime.Value = DateTime.Now.AddDays(30);

            // 设置默认授权类型
            if (CmbLicenseType.Items.Count > 0)
            {
                CmbLicenseType.SelectedIndex = 0; // 默认为"试用版"
            }
        }

        /// <summary>
        /// 生成License文件
        /// </summary>
        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证输入
                if (string.IsNullOrWhiteSpace(TxtMachineCode.Text))
                {
                    MessageBox.Show("机器码不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DateTime startTime = DtpStartTime.Value;
                DateTime endTime = DtpEndTime.Value;

                if (endTime <= startTime)
                {
                    MessageBox.Show("结束时间必须大于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 创建License信息
                LicenseInfo licenseInfo = new LicenseInfo
                {
                    MachineCode = TxtMachineCode.Text.Trim(),
                    StartTime = startTime,
                    EndTime = endTime,
                    LicenseType = CmbLicenseType.Text,
                    ExtraInfo = TxtExtraInfo.Text
                };

                // 生成License
                string license = LicenseHelper.GenerateLicense(licenseInfo);

                // 保存到文件
                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "License文件 (*.lic)|*.lic|所有文件 (*.*)|*.*",
                    FileName = $"License.lic",
                    Title = "保存License文件"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    LicenseHelper.SaveLicenseToFile(license, saveDialog.FileName);
                    
                    // 显示License信息预览
                    ShowLicensePreview(licenseInfo, license);
                    
                    MessageBox.Show($"License文件已成功生成！\n保存路径：{saveDialog.FileName}", 
                        "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成License失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 显示License预览
        /// </summary>
        private void ShowLicensePreview(LicenseInfo licenseInfo, string encryptedLicense)
        {
            StringBuilder preview = new StringBuilder();
            preview.AppendLine("=== License 信息预览 ===");
            preview.AppendLine($"机器码: {licenseInfo.MachineCode}");
            preview.AppendLine($"开始时间: {licenseInfo.StartTime:yyyy-MM-dd HH:mm:ss}");
            preview.AppendLine($"结束时间: {licenseInfo.EndTime:yyyy-MM-dd HH:mm:ss}");
            preview.AppendLine($"授权类型: {licenseInfo.LicenseType}");
            preview.AppendLine($"额外信息: {licenseInfo.ExtraInfo}");
            preview.AppendLine($"生成时间: {licenseInfo.GenerateTime:yyyy-MM-dd HH:mm:ss}");
            
            TxtPreview.Text = preview.ToString();
        }

        /// <summary>
        /// 验证License文件
        /// </summary>
        private void BtnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog
                {
                    Filter = "License文件 (*.lic)|*.lic|所有文件 (*.*)|*.*",
                    Title = "选择License文件"
                };

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string license = LicenseHelper.ReadLicenseFromFile(openDialog.FileName);
                    LicenseValidateResult result = LicenseHelper.ValidateLicense(license);

                    StringBuilder message = new StringBuilder();
                    message.AppendLine($"验证结果: {(result.IsValid ? "✓ 有效" : "✗ 无效")}");
                    message.AppendLine($"错误信息: {result.ErrorMessage}");
                    
                    if (result.IsValid && result.LicenseInfo != null)
                    {
                        message.AppendLine();
                        message.AppendLine("=== License 详细信息 ===");
                        message.AppendLine($"机器码: {result.LicenseInfo.MachineCode}");
                        message.AppendLine($"开始时间: {result.LicenseInfo.StartTime:yyyy-MM-dd HH:mm:ss}");
                        message.AppendLine($"结束时间: {result.LicenseInfo.EndTime:yyyy-MM-dd HH:mm:ss}");
                        message.AppendLine($"授权类型: {result.LicenseInfo.LicenseType}");
                        message.AppendLine($"额外信息: {result.LicenseInfo.ExtraInfo}");
                        message.AppendLine();
                        message.AppendLine("=== 剩余时间 ===");
                        message.AppendLine($"剩余天数: {result.RemainingDays}");
                        message.AppendLine($"剩余小时: {result.RemainingHours}");
                        message.AppendLine($"剩余分钟: {result.RemainingMinutes}");
                        message.AppendLine($"剩余秒数: {result.RemainingSeconds}");
                    }

                    MessageBox.Show(message.ToString(), "License验证结果", 
                        MessageBoxButtons.OK, result.IsValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"验证License失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 快速设置试用版（30天）
        /// </summary>
        private void BtnTrial30Days_Click(object sender, EventArgs e)
        {
            DtpStartTime.Value = DateTime.Now;
            DtpEndTime.Value = DateTime.Now.AddDays(30);
            CmbLicenseType.Text = "试用版";
            TxtExtraInfo.Text = "试用版授权，有效期30天";
        }

        /// <summary>
        /// 快速设置试用版（7天）
        /// </summary>
        private void BtnTrial7Days_Click(object sender, EventArgs e)
        {
            DtpStartTime.Value = DateTime.Now;
            DtpEndTime.Value = DateTime.Now.AddDays(7);
            CmbLicenseType.Text = "试用版";
            TxtExtraInfo.Text = "试用版授权，有效期7天";
        }

        /// <summary>
        /// 快速设置正式版（1年）
        /// </summary>
        private void BtnFullYear_Click(object sender, EventArgs e)
        {
            DtpStartTime.Value = DateTime.Now;
            DtpEndTime.Value = DateTime.Now.AddYears(1);
            CmbLicenseType.Text = "正式版";
            TxtExtraInfo.Text = "正式版授权，有效期1年";
        }
    }
}
