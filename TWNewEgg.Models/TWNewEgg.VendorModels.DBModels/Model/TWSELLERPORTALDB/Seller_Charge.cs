using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_Charge")]
    public class Seller_Charge
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }

        [Key, Column(Order = 0)]
        //[ForeignKey("SellerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerID { get; set; }

        [Key, Column(Order = 1)]
        //[ForeignKey("CountryCode")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CountryCode { get; set; }

        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ChargeType { get; set; }

        [Key, Column(Order = 3)]
        //[ForeignKey("CategoryID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryID { get; set; }

        public decimal Commission { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
