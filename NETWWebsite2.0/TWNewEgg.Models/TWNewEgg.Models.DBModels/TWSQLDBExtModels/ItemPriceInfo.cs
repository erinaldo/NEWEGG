using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWSQLDBExtModels
{
    public class ItemPriceInfo
    {
        public Item item { get; set; }
        public ItemDisplayPrice itemDisplayPrice { get; set; }
        public int ID { get; set; }
        public int QtyReg { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal RedmfdbckRate { get; set; }
        public decimal Price { get; set; }
        public decimal DisplayPrice { get; set; }
        public string ImgPath { get; set; }
        public int? Qty { get; set; }
        public int? NowPage { get; set; }
        public int? TotalPage { get; set; }
    }

    public class ItemPriceInfoSimplify
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int QtyReg { get; set; }
        public int ManufactureID { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal RedmfdbckRate { get; set; }
        public decimal? MarketPrice { get; set; }
        public decimal PriceCash { get; set; }
        public decimal DisplayPrice { get; set; }
        public string ImgPath { get; set; }
        public int SellingQty { get; set; }
        /// <summary>
        /// 以可銷售數量做排序的數量
        /// </summary>
        public int SortSellingQty { get; set; }
        public int CategoryID { get; set; }
    }
    
}
