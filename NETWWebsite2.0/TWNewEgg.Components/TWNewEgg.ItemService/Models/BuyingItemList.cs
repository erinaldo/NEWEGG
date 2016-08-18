using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ItemService.Models
{
    public class BuyingItemList
    {
        public int buyItemlistID { get; set; }
        public int? item_AttrID { get; set; }
        public int buyingNumber { get; set; }
        public decimal buyItemListWeight { get; set; }
        public decimal buyItemListShipping { get; set; }
    }
}
