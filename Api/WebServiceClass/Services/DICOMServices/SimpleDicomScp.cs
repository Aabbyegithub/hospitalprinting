using FellowOakDicom;
using FellowOakDicom.Network;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceClass.Services.DICOMServices
{
    public class SimpleDicomScp : DicomService, IDicomServiceProvider, IDicomCStoreProvider,IDicomCEchoProvider

    {
        private readonly string _saveDirectory;
        private readonly string _aet;

        // ✅ fo-dicom 5.2.4 的正确构造函数签名
        public SimpleDicomScp(INetworkStream stream, Encoding fallbackEncoding, ILogger log, DicomServiceDependencies dependencies, object userState)
            : base(stream, fallbackEncoding, log, dependencies)
        {
            if (userState is object[] arr && arr.Length >= 2)
            {
                _saveDirectory = arr[0]?.ToString() ?? Path.Combine(AppContext.BaseDirectory, "Received");
                _aet = arr[1]?.ToString() ?? "DEFAULT_AE";
            }
            else
            {
                _saveDirectory = Path.Combine(AppContext.BaseDirectory, "Received");
                _aet = "DEFAULT_AE";
            }

            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
        }

        // ✅ 关联请求（异步）
        public async Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            Console.WriteLine($"收到关联请求：Calling AE={association.CallingAE}, Called AE={association.CalledAE}");

            // 接受所有存储类型请求
            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax.IsImageStorage)
                    pc.AcceptTransferSyntaxes(DicomTransferSyntax.ImplicitVRBigEndian);
                else if (pc.AbstractSyntax == DicomUID.Verification)
                    pc.AcceptTransferSyntaxes(DicomTransferSyntax.ImplicitVRLittleEndian);
            }

            await SendAssociationAcceptAsync(association);
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            Console.WriteLine("客户端请求释放连接");
            return SendAssociationReleaseResponseAsync();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            Console.WriteLine($"连接中止: {reason}");
        }

        public void OnConnectionClosed(Exception exception)
        {
            Console.WriteLine("连接关闭");
        }

        // ✅ 接收DICOM文件（C-STORE）
        public DicomCStoreResponse OnCStoreRequest(DicomCStoreRequest request)
        {
            try
            {
                string sopInstanceUid = request.File.Dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, Guid.NewGuid().ToString());
                string fileName = $"{sopInstanceUid}_{DateTime.Now:yyyyMMddHHmmss}.dcm";
                string filePath = Path.Combine(_saveDirectory, fileName);
                request.File.Save(filePath);
                Console.WriteLine($"已保存 DICOM 文件: {filePath}");
                return new DicomCStoreResponse(request, DicomStatus.Success);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存失败: {ex.Message}");
                return new DicomCStoreResponse(request, DicomStatus.ProcessingFailure);
            }
        }

        public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
            => Task.FromResult(OnCStoreRequest(request));

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
        {
            Console.WriteLine($"C-STORE 异常: {e.Message}");
            return Task.CompletedTask;
        }

        // ✅ 启动方法
        public static void Start(string ip, int port, string aet, string saveDirectory)
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            if (string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out _))
                throw new ArgumentException("无效的IP地址");
            if (port < 1 || port > 65535)
                throw new ArgumentException("端口号必须在1-65535之间");
            var server = DicomServer.Create<SimpleDicomScp>(
                ip,
                port,
                null,
                new object[] { saveDirectory, aet }
            );

            Console.WriteLine($"✅ DICOM SCP 已启动 - IP: {ip}, 端口: {port}, AE: {aet}");
            Console.WriteLine($"📁 保存路径: {saveDirectory}");
            Console.WriteLine("按 ENTER 退出...");
            Console.ReadLine();

            server.Dispose();
        }

        public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
        {
            Console.WriteLine(); return (Task<DicomCEchoResponse>)Task.CompletedTask;
        }
    }
}
