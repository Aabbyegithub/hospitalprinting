using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinSelfMachine.Model
{
    public class HolModel
    {
        public class WaitTimeList
        {
            public int Id { get; set; }
            public string Size { get; set; }
            public int WaitTime { get; set; }
        }

        public class FilmList
        {
            public int Id { get; set; }
            public string Size { get; set; }
            public int PageSum { get; set; }
        }
    }
}
