using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("spexeditinvoice")]
    public class SpexEditInvoice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SO { get; set; }
        public string BoxNo { get; set; }
        public string InvoiceItem { get; set; }
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }
        public string BoxWeight { get; set; }
        public string FileName { get; set; }
    }
}