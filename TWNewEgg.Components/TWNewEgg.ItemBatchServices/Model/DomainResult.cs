using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ItemBatchServices.Models
{
    public class DomainResult
    {
        public int? ItemID { get; set; }
        public int? ProductID { get; set; }
        public string SellerProductID { get; set; }
        public bool IsSuccess { get; set; }
        public int UpdateTypeID { get; set; }
        public string UpdateTypeName { get; set; }
        public string Log { get; set; }
    }
}
