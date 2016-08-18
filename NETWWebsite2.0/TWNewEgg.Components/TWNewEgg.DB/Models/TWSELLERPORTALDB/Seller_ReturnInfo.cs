using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_ReturnInfo")]
    public partial class Seller_ReturnInfo
    {
        [Key]
        //[ForeignKey("SellerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerID { get; set; }
        public int RefundPeriod { get; set; }
        public int ReplacementPeriod { get; set; }
        public int RestockingFee { get; set; }
        public string ReturnPolicy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
