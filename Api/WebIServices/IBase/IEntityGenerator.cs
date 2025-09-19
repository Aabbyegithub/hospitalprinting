using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebIServices.IBase
{
    public interface IEntityGenerator:IBaseService
    {
        void GenerateEntityFiles(List<string> tableNames,string outputPath);
        void GenerateEntityFiles(string tableName,string outputPath);
    }
}
