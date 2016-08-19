using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceList")]
    public class InvoiceList
    {
        public InvoiceList()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
            this.InDate = DateTime.Now;
            this.EditDate = defaultDate;
        }
        [Key]
        public int SN { get; set; }
        public string InvoiceNumber { get; set; }
        public string SONumber { get; set; }
        public decimal SOPrice { get; set; }
        public decimal SOTax { get; set; }
        public decimal SOShip { get; set; }
        public decimal SOServicePrice { get; set; }
        public decimal? SOInstallmentFee { get; set; }
        public string InUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string EditUser { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
    }
}