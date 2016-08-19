using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceD0501")]
    public class InvoiceD0501
    {
        public InvoiceD0501()
        {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string CancelAllowanceNumber { get; set; }
        public string AllowanceDate { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string SellerId { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string BuyerId { get; set; }
        public string CancelDate { get; set; }
        public string CancelTime { get; set; }
        public string CancelReason { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
