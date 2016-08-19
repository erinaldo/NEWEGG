using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSELLERPORTALDB.Models
{
    [Table("Seller_Notification")]
    public partial class Seller_Notification
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }

        [Key, Column(Order = 0)]
        //[ForeignKey("SellerID")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SellerID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string NotificationTypeCode { get; set; }

        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }
        public string Enabled { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> InUserID { get; set; }
    }
}
