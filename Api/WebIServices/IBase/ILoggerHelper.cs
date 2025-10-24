using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIServices.IBase
{
    public interface ILoggerHelper
    {
        Task LogInfo(string message,string moduleName = "INFO");
        Task LogWarning(string message);
        Task LogError(string message);
    }
}
