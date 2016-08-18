using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("cartitemtemp")]
    public class CartItemTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CartTempID { get; set; }
        public int ItemID { get; set; }
        public int Qty { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<decimal> DisplayPrice { get; set; }
        public Nullable<decimal> PriceCash { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> ShippingPrice { get; set; }
        public Nullable<decimal> ServicePrice { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
