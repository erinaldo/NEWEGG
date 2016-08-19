using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("cartcoupontemp")]
    public class CartCouponTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CartTempID { get; set; }
        public int ItemID { get; set; }
        public string CouponNumber { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
