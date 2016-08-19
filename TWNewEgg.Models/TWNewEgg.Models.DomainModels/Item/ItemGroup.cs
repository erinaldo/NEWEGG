using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemGroup
    {
        public int ID { get; set; }
        public int SellerID { get; set; }
        public int Amount { get; set; }
        public int MasterPropertyID { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int InUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
