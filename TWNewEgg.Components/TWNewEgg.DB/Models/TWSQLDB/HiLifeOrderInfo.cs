using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("hilifeorderinfo")]
    public class HiLifeOrderInfo
    {
        public HiLifeOrderInfo()
        {
            this.ProductNumber = "0";
            this.Updated = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        // Stno(取貨門市編號)
        public string StoreNumber { get; set; }

        // Stnm(取貨店名)
        public string StoreName { get; set; }

        // ODNO(EC訂單編號)
        public string ODNumber { get; set; }

        // DCRONO(路線路順)
        public string DirectRouteNO { get; set; }

        // Prodnm(商品類型)
        public string ProductNumber { get; set; }

        public string CreateUser { get; set; }

        public Nullable<DateTime> CreateDate { get; set; }

        public Nullable<int> Updated { get; set; }

        public string UpdateUser { get; set; }

        public Nullable<DateTime> UpdateDate { get; set; }
    }
}
