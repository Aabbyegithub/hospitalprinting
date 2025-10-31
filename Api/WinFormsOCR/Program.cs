using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.ImageSharp;
using System.Drawing;
using Timer = System.Windows.Forms.Timer;

namespace WinFormsOCR
{
    internal static class Program
    {
        private static Timer licenseCheckTimer;
        private const int licenseCheckInterval = 60000; // 60秒检测一次
        private static string licenseFilePath;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 初始化 License 文件路径
            licenseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.lic");
            if (!File.Exists(licenseFilePath))
            {
                // 创建License信息
                LicenseInfo licenseInfo = new LicenseInfo
                {
                    MachineCode = LicenseHelper.GetMachineCode(),
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(7),
                    LicenseType = "试用版",
                    ExtraInfo = "试用版授权，有效期7天"
                };

                // 生成License
                string license1 = LicenseHelper.GenerateLicense(licenseInfo);
                LicenseHelper.SaveLicenseToFile(license1, licenseFilePath);
            }

            // 初始验证 License
            string license = LicenseHelper.ReadLicenseFromFile(licenseFilePath);
            LicenseValidateResult result = LicenseHelper.ValidateLicense(license);

            if (!result.IsValid)
            {
                MessageBox.Show($"License验证失败：{result.ErrorMessage}\n机器码：{LicenseHelper.GetMachineCode()}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddImageManager<ImageSharpImageManager>())
            .Build();
            ApplicationConfiguration.Initialize();

            // 启动 License 定时检测
            StartLicenseCheckTimer();
            Application.Run(new Form1());
        }

        /// <summary>
        /// 启动 License 定时检测
        /// </summary>
        private static void StartLicenseCheckTimer()
        {
            licenseCheckTimer = new Timer();
            licenseCheckTimer.Interval = licenseCheckInterval;
            licenseCheckTimer.Tick += LicenseCheckTimer_Tick;
            licenseCheckTimer.Start();
        }

        /// <summary>
        /// License 校验定时器事件
        /// </summary>
        private static void LicenseCheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // 停止定时器避免重复触发
                licenseCheckTimer.Stop();

                // 检查 License 是否过期
                string license = LicenseHelper.ReadLicenseFromFile(licenseFilePath);
                LicenseValidateResult result = LicenseHelper.ValidateLicense(license);

                // 如果 License 无效或过期，退出程序
                if (!result.IsValid)
                {
                    MessageBox.Show(
                        $"软件已过期：{result.ErrorMessage}\n程序将自动退出。",
                        "授权过期",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // 退出应用程序
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                // License 文件不存在或其他错误，也退出程序
                MessageBox.Show(
                    $"License 验证失败：{ex.Message}\n程序将自动退出。",
                    "授权错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                Application.Exit();
            }
            finally
            {
                // 如果程序还在运行，重新启动定时器
                if (licenseCheckTimer != null)
                {
                    licenseCheckTimer.Start();
                }
            }
        }
    }
}