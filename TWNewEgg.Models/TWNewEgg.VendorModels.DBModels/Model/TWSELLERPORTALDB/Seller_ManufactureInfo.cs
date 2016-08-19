using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_ManufactureInfo")]
    public class Seller_ManufactureInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SN { get; set; }
        public int SellerID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string ManufactureStatus { get; set; }
        public string ManufactureName { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ManufactureURL { get; set; }

        public string SupportEmail { get; set; }
        public string PhoneRegion { get; set; }
        public string Phone { get; set; }
        public string PhoneExt { get; set; }
        public string supportURL { get; set; }
        public Nullable<int> DeclineReasonType { get; set; }
        public string DeclineReason { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
