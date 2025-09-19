using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIServices.IBase
{
    public interface IRedisCacheService:IBaseService
    {
        Task SetStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
        Task RemoveAsync(string key);
    }
}
