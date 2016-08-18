using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels
{
    public class DbItemInfo
    {
        public Item item { get; set; }
        public Product product { get; set; }
        public Seller seller { get; set; }
    }
}
