using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    /// <summary>
    /// refer DBModel.ItemDisplayPrice
    /// </summary>
    public class ItemPrice
    {
        public enum PriceTypeEnum : int //Highest Pirority Must Set Maximum Number, also 1 is the lowest number!!!!!!!!!!!!!!!!!
        {
            系統自動 = 1,
            人工建立 = 2
        };

        public int ID { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public decimal DisplayPrice { get; set; }
        public decimal DisplayShipping { get; set; }
        public decimal DisplayTax { get; set; }
        public DateTime EndDate { get; set; }
        public decimal ItemCost { get; set; }
        public decimal ItemCostTW { get; set; }
        public int ItemID { get; set; }
        public decimal? ItemProfitPercent { get; set; }
        public int MaxNumber { get; set; }
        public int MinNumber { get; set; }
        public int PriceType { get; set; }
        public DateTime StartDate { get; set; }
        public int Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
