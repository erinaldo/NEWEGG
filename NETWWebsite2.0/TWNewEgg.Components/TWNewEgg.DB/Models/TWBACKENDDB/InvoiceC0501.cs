using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceC0501")]
    public class InvoiceC0501
    {
        public InvoiceC0501()
        {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string CancelInvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string SellerId { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string BuyerId { get; set; }
        public string CancelDate{get;set;}
        public string CancelTime { get; set; }
        public string CancelReason { get; set; }
        public string ReturnTaxDocumentNumber { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
