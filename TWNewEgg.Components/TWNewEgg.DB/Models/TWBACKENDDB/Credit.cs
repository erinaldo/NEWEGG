using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Credit")]
    public class Credit
    {
        public Credit()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public Nullable<int> StoreID { get; set; }
        public Nullable<int> DealType { get; set; }
        public Nullable<int> Dealid { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> CreditConfirm { get; set; }
        public Nullable<System.DateTime> Reply { get; set; }
        public string ReplyUser { get; set; }
        public Nullable<int> ReplyAmnt { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> CheckType { get; set; }
        public string CheckNote { get; set; }
        public Nullable<int> PayType { get; set; }
        public Nullable<System.DateTime> PayDate { get; set; }
        public string InvoiceNO { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<System.DateTime> FinalDate { get; set; }
        public Nullable<byte> AuthType { get; set; }
        public string AuthCode { get; set; }
        public string AuthSN { get; set; }
        public Nullable<System.DateTime> VerifyEmail1 { get; set; }
        public Nullable<System.DateTime> VerifyEmail2 { get; set; }
        public string VerifyNote { get; set; }
        public Nullable<System.DateTime> Chequerecv { get; set; }
        public Nullable<System.DateTime> ChequePay { get; set; }
        public Nullable<int> InvoiceLock { get; set; }
        public Nullable<decimal> InvoAmnt { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public Nullable<System.DateTime> SysDate { get; set; }
        public Nullable<byte> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
