using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.ImageSharp;
using System.Drawing;

namespace WinFormsOCR
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            new DicomSetupBuilder()
                .RegisterServices(s => s.AddFellowOakDicom().AddImageManager<ImageSharpImageManager>())
            .Build();
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}