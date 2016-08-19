using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_Settlements")]
    public class Seller_Settlements
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SettlementID { get; set; }
        public int SellerID { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateEnd { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> ShipFee { get; set; }
        public Nullable<decimal> RMAPrice { get; set; }
        public Nullable<decimal> AdjPrice { get; set; }
        public Nullable<decimal> NewEggFee { get; set; }
        public Nullable<decimal> CommissionFee { get; set; }
        public Nullable<decimal> TransFee { get; set; }
        public Nullable<decimal> StorageFee { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
