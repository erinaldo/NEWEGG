using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceC0401ProductItem")]
    public class InvoiceC0401ProductItem
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string InvoiceNumber { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> TaxField { get; set; }
        public string SequenceNumber { get; set; }
        public string Remark { get; set; }
        public string RelateNumber { get; set; }
    }
}
