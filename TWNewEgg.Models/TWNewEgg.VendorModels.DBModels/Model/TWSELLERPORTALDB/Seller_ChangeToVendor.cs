using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{

    [Table("Seller_ChangeToVendor")]
    public class Seller_ChangeToVendor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Nullable<int> OldSellerID { get; set; }
        public Nullable<int> NewSellerID { get; set; }
        public string BasicInfo { get; set; }
        public string Financial { get; set; }
        public string ContactInfo { get; set; }
        public string Notification { get; set; }
        public string ReturnInfo { get; set; }
        public string SellerPurview { get; set; }
        public string SellerUser { get; set; }
        public Nullable<int> NewItemID { get; set; }
        public Nullable<int> OldItemID { get; set; }
        public Nullable<int> NewProductID { get; set; }
        public Nullable<int> OldProductID { get; set; }
        public string Exception { get; set; }
        public DateTime InDate { get; set; }
    }

}
