using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemStock
    {
        public ItemStock()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int Qty { get; set; }
        public int QtyReg { get; set; }
        public int SafeQty { get; set; }
        public int Fdbcklmt { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
