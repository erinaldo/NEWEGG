using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("itemdisplayprice")]
    public class ItemDisplayPrice
    {
        public enum PriceTypeEnum : int //Highest Pirority Must Set Maximum Number, also 1 is the lowest number!!!!!!!!!!!!!!!!!
        {
            系統自動 = 1,
            人工建立 = 2
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("流水號")]
        public int ID { get; set; }

        [DisplayName("Item ID")]
        public int ItemID { get; set; }

        [DisplayName("價格型態，依據PriceTypeEnum")]
        public int PriceType { get; set; }

        [DisplayName("最小數量限制")]
        public int MinNumber { get; set; }

        [DisplayName("最大數量限制")]
        public int MaxNumber { get; set; }

        [DisplayName("起始時間")]
        public DateTime StartDate { get; set; }

        [DisplayName("結束時間")]
        public DateTime EndDate { get; set; }

        [DisplayName("數量限制與時間限制內售價")]
        public decimal DisplayPrice { get; set; }

        [DisplayName("數量限制與時間限制內稅率")]
        public decimal DisplayTax { get; set; }

        [DisplayName("數量限制與時間限制內運費")]
        public decimal DisplayShipping { get; set; }

        [DisplayName("數量限制與時間限制內成本(原始售價未轉匯匯率)")]
        public decimal ItemCost { get; set; }

        [DisplayName("數量限制與時間限制內成本(台幣TWD)")]
        public decimal ItemCostTW { get; set; }

        [DisplayName("數量限制與時間限制內賺取%數")]
        public Nullable<decimal> ItemProfitPercent { get; set; }

        [DisplayName("建立時間")]
        public System.DateTime CreateDate { get; set; }

        [DisplayName("建立使用者")]
        public string CreateUser { get; set; }

        [DisplayName("更新次數")]
        public int Updated { get; set; }

        [DisplayName("更新時間")]
        public Nullable<System.DateTime> UpdateDate { get; set; }

        [DisplayName("更新使用者")]
        public string UpdateUser { get; set; }
    }
}
