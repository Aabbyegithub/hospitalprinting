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
        private const int licenseCheckInterval = 60000; // 60����һ��
        private static string licenseFilePath;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ��ʼ�� License �ļ�·��
            licenseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "license.lic");
            if (!File.Exists(licenseFilePath))
            {
                // ����License��Ϣ
                LicenseInfo licenseInfo = new LicenseInfo
                {
                    MachineCode = LicenseHelper.GetMachineCode(),
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(7),
                    LicenseType = "���ð�",
                    ExtraInfo = "���ð���Ȩ����Ч��7��"
                };

                // ����License
                string license1 = LicenseHelper.GenerateLicense(licenseInfo);
                LicenseHelper.SaveLicenseToFile(license1, licenseFilePath);
            }

            // ��ʼ��֤ License
            string license = LicenseHelper.ReadLicenseFromFile(licenseFilePath);
            LicenseValidateResult result = LicenseHelper.ValidateLicense(license);

            if (!result.IsValid)
            {
                MessageBox.Show($"License��֤ʧ�ܣ�{result.ErrorMessage}\n�����룺{LicenseHelper.GetMachineCode()}", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddImageManager<ImageSharpImageManager>())
            .Build();
            ApplicationConfiguration.Initialize();

            // ���� License ��ʱ���
            StartLicenseCheckTimer();
            Application.Run(new Form1());
        }

        /// <summary>
        /// ���� License ��ʱ���
        /// </summary>
        private static void StartLicenseCheckTimer()
        {
            licenseCheckTimer = new Timer();
            licenseCheckTimer.Interval = licenseCheckInterval;
            licenseCheckTimer.Tick += LicenseCheckTimer_Tick;
            licenseCheckTimer.Start();
        }

        /// <summary>
        /// License У�鶨ʱ���¼�
        /// </summary>
        private static void LicenseCheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // ֹͣ��ʱ�������ظ�����
                licenseCheckTimer.Stop();

                // ��� License �Ƿ����
                string license = LicenseHelper.ReadLicenseFromFile(licenseFilePath);
                LicenseValidateResult result = LicenseHelper.ValidateLicense(license);

                // ��� License ��Ч����ڣ��˳�����
                if (!result.IsValid)
                {
                    MessageBox.Show(
                        $"����ѹ��ڣ�{result.ErrorMessage}\n�����Զ��˳���",
                        "��Ȩ����",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    // �˳�Ӧ�ó���
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                // License �ļ������ڻ���������Ҳ�˳�����
                MessageBox.Show(
                    $"License ��֤ʧ�ܣ�{ex.Message}\n�����Զ��˳���",
                    "��Ȩ����",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                Application.Exit();
            }
            finally
            {
                // ������������У�����������ʱ��
                if (licenseCheckTimer != null)
                {
                    licenseCheckTimer.Start();
                }
            }
        }
    }
}