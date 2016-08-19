using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Seller;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemInfo
    {
        public ItemBase ItemBase { get; set; }
        public SellerBase SellerBase { get; set; }
        public ProductBase ProductBase { get; set; }
        public int? Qty { get; set; }
        public int? NowPage { get; set; }
        public int? TotalPage { get; set; }
        /// <summary>
        /// 是否任選館商品
        /// </summary>
        public int IsChooseAny { get; set; }
    }
}
