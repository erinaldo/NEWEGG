using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Checkin")]
    public class Checkin
    {
        public enum TypeEnum
        {
            PO = 1, 
            ReturnGood = 3, 
            ADJ = 5, 
            CSApply = 7
        }
        [Key]
        public int ID { get; set; }
        public string PoCode { get; set; }
        public string PoItemCode { get; set; }
        public Nullable<int> RetID { get; set; }
        public Nullable<int> WhID { get; set; }
        public Nullable<int> SCMEnabled { get; set; }
        public string Code { get; set; }
        public string CheckInUser { get; set; }
        public Nullable<DateTime> FinalDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> SupplierID { get; set; }
        public Nullable<int> SupplierType { get; set; }
        public Nullable<int> WareHouse { get; set; }
        public Nullable<int> Type { get; set; }
        public string VouhCode { get; set; }
        public string EndUser { get; set; }
        public string Note { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> StckStatus { get; set; }
        public DateTime? StckstatusDate { get; set; }
        public string StckstatusUser { get; set; }
        public Nullable<int> DelivType { get; set; }
        public Nullable<int> AsnNumber { get; set; }
        public virtual ICollection<Checkinitem> Checkinitems { get; set; }
    }
}