using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ItemService.Models
{   
    public class BuyingItems
    {
        public BuyingItems()
        {
            this.buyItemLists = new List<BuyingItemList>();
            this.Coupons = new Dictionary<string, decimal>();
        }

        public int buyItemID_DelvType { get; set; }
        public int buyItemID_Seller { get; set; }
        public int buyItemID { get; set; }
        public int? item_AttrID { get; set; }
        public int buyingNumber { get; set; }
        public List<BuyingItemList> buyItemLists { get; set; }
        public decimal buyItemWeight { get; set; }
        public decimal buyItemShipping { get; set; }
        public Dictionary<string, decimal> Coupons { get; set; }
    }
}
