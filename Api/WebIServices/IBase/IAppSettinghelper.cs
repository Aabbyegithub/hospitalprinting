using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIServices.IBase
{
    /// <summary>
    /// 读取配置文件接口
    /// </summary>
    public interface IAppSettinghelper
    {
        string Get(string key);
    }
}
