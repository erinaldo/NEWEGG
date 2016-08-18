using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{   [Table ("deliveryitem")]
    public partial class DeliveryItem
    {
        [Key]
        public int ID { get; set; }
        public Nullable<int> DeliveryID { get; set; }
        public string ProcessID { get; set; }
        public Nullable<int> ProductID { get; set; }
        public Nullable<int> deliveryitem_pktid { get; set; }
        public string deliveryitem_title { get; set; }
        public string deliveryitem_attribs { get; set; }
        public Nullable<decimal> deliveryitem_price { get; set; }
        public Nullable<int> deliveryitem_qty { get; set; }
        public string deliveryitem_invoiceno { get; set; }
        public string deliveryitem_invoicetitle { get; set; }
        public string deliveryitem_invoicenum { get; set; }
        public Nullable<System.DateTime> deliveryitem_stckdate { get; set; }
        public string deliveryitem_stckuser { get; set; }
        public Nullable<System.DateTime> deliveryitem_erasedate { get; set; }
        public Nullable<int> deliveryitem_erasecause { get; set; }
        public string deliveryitem_erasecausenote { get; set; }
        public Nullable<System.DateTime> deliveryitem_sysdate { get; set; }
        public Nullable<int> deliveryitem_updated { get; set; }
        public Nullable<System.DateTime> deliveryitem_updateddate { get; set; }
        public string deliveryitem_updateduser { get; set; }
    }
}