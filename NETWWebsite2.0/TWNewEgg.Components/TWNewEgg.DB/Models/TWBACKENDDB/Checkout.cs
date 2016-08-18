using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Checkout")]
    public class Checkout
    {
        public enum TypeEnum
        {
            SO = 1,
            ReturnFactory = 3
        }
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string SoCode { get; set; }
        public string SoitemCode { get; set; }
        public string CheckOutUser { get; set; }
        public Nullable<DateTime> FinalDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> WareHouse { get; set; }
        public Nullable<int> WhID { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> Cause { get; set; }
        public string VouhCode { get; set; }
        public string EndUser { get; set; }
        public string Note { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> DelivType { get; set; }
        public int? StckStatus { get; set; }
        public DateTime? StckstatusDate { get; set; }
        public string StckstatusUser { get; set; }
        public Nullable<int> RefSONumber { get; set; }
        public Nullable<int> SellerID { get; set; } // Add column 2014-06-30 bw52

        public virtual ICollection<Checkoutitem> checkoutitems { get; set; }

        

        
    }
}