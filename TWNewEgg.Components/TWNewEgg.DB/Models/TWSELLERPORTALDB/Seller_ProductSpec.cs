using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_ProductSpec")]
    public class Seller_ProductSpec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string MenufacturePartNum { get; set; }
        public string SellerProductID { get; set; }
        public string UPC { get; set; }
        public int SpecItem { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
