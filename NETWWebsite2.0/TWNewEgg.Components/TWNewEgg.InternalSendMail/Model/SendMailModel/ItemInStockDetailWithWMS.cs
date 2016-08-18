using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Model.SendMailModel
{
    public class ItemInStockOnlyQty
    {
        public int InstockQuantity { get; set; }
        public int RMABad { get; set; }
        public int RMADead { get; set; }
    }

    public class WMSItemInStock
    {
        public Nullable<int> InstockQuantity { get; set; }
        public Nullable<int> RMABad { get; set; }
        public Nullable<int> RMADead { get; set; }
    }

    public class ItemInStockDetailWithWMS
    {
        public ItemInStockOnlyQty itemInStockOnlyQty { get; set; }
        public WMSItemInStock WMSItemInStock { get; set; }
        public int ProductID { get; set; }
        public int Sellquantity { get; set; }
        public Nullable<int> WMSsellquantity { get; set; }
        public int InstockQuantityDifference { get; set; }
        public int RMABadDifference { get; set; }
        public int RMADeadDifference { get; set; }       
    }
}
