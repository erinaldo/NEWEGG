using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_DelvTrack")]
    public class Seller_DelvTrack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int SellerID { get; set; }
        public string ProcessID { get; set; }
        public int ProductID { get; set; }
        public string UPC { get; set; }
        public string SellerProductID { get; set; }
        public int ManufactureID { get; set; }
        public string ManufacturePartNum { get; set; }

        public int Qty { get; set; }
        public int DeliverID { get; set; }
        public System.DateTime DelvDate { get; set; }
        public string TrackingNum { get; set; }
        public string Address { get; set; }
        public Nullable<int> InUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
