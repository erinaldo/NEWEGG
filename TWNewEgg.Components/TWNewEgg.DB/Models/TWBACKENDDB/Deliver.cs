using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Deliver")]
    public class Deliver
    {
        public enum ConvenienceStoreCode
        {
            HiLife = 811,
            SevenEleven = 812,
            FamilyMart = 813,
            OKMart = 814
        }

        public enum ShippingCompany
        {
            HCT新竹貨運 = 801,
            HCTCOD新竹貨運貨到付款 = 802,
            TCAT黑貓宅急便 = 803,
            TCATCOD黑貓宅急便貨到付款 = 804
        }

        public enum Type
        {
            Delivery = 0,
            StorePickUP = 1
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// 貨運代碼
        /// </summary>
        [Key]
        public int code { get; set; }
        /// <summary>
        /// 貨運名稱
        /// </summary>
        public string Name { get; set; }
        public string WebSite { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string CreateUser { get; set; }
        public int? DeliverType { get; set; }
    }
}