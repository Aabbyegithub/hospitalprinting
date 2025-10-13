using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelClassLibrary.Model.Dto.HolDto
{
    /// <summary>
    /// 文件夹监听配置DTO
    /// </summary>
    public class HolFolderMonitorDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string ip_address { get; set; }
        public List<string> folder_paths { get; set; } = new();
        public int status { get; set; }
        public string? remark { get; set; }
    }
}
