using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceClass.Services
{
    public class DbContextShop:DbContext
    {
        public DbContextShop() { }
        public DbContextShop(DbContextOptions<DbContextShop> options):base(options) { }
    }
}
