using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceD0401ProductItem")]
    public class InvoiceD0401ProductItem
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string AllowanceNumber { get; set; }
        public string OriginalInvoiceNumber { get; set; }
        public string OriginalInvoiceDate { get; set; }
        public string OriginalSequenceNumber { get; set; }
        public string OriginalDescription { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public string AllowanceSequenceNumber { get; set; }
        public string TaxType { get; set; }
    }
}
