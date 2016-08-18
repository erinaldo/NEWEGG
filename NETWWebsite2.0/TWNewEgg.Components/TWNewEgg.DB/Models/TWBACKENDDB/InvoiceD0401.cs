using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("InvoiceD0401")]
    public class InvoiceD0401
    {
        public InvoiceD0401()
        {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public string AllowanceNumber { get; set; }
        public string AllowanceDate { get; set; }
        public string SellerIdentifier { get; set; }
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public string SellerPersonInCharge { get; set; }
        public string SellerTelephoneNumber { get; set; }
        public string SellerFacsimileNumber { get; set; }
        public string SellerEmailAddress { get; set; }
        public string SellerCustomerNumber { get; set; }
        public string BuyerIdentifier { get; set; }
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerPersonInCharge { get; set; }
        public string BuyerTelephoneNumber { get; set; }
        public string BuyerFacsimileNumber { get; set; }
        public string BuyerEmailAddress { get; set; }
        public string BuyerCustomerNumber { get; set; }
        public string AllowanceType { get; set; }
        public string Attachment { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
    }
}
