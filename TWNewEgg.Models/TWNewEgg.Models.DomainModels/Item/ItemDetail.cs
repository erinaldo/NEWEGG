using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemDetail
    {
        public ItemInfo Main { get; set; }
        public ItemPrice Price { get; set; }
        public int SellingQty { get; set; }
        public List<int> DeliverTypes { get; set; }
        public List<int> PayTypes { get; set; }
        public DateTime? GroupBuyEndDate { get; set; }
        public List<Models.DomainModels.Redeem.PromotionDetail> PromotionBasicList { get; set; }
        public String BrandStory { get; set; }
    }
}
