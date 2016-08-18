using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceC0701")]
    public class InvoiceC0701
    {
        public InvoiceC0701()
        {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string VoidInvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string SellerId { get; set; }
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string BuyerId { get; set; }
        public string VoidDate { get; set; }
        public string VoidTime { get; set; }
        public string VoidReason { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
