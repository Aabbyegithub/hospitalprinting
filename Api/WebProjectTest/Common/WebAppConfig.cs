using WebIServices.IBase;

namespace WebProjectTest.Common
{
    public class WebAppConfig: IAppSettinghelper
    {
        public string Get(string key) => AppSettings.GetConfig(key);
    }
}
