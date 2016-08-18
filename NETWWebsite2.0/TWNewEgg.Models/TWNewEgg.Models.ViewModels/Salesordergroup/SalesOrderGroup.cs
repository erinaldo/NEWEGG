using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Salesordergroup
{
    public class SalesOrderGroup
    {
        public SalesOrderGroup()
        {
            CreateDate = DateTime.Now;
            CreateUser = string.Empty;
            Updated = 0;
            UpdateUser = string.Empty;
            UpdateDate = DateTime.Now;
            
        }
        public string Vaccunt { get; set; }
        public int PriceSum { get; set; }
        public int OrderNum { get; set; }
        public string Note { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
