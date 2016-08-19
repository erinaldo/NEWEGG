using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("POItemDetail")]
    public class POItemDetail
    {
        [Key]
        public string process_id { get; set; }
        public string cart_id { get; set; }
        public string purchaseorderitem_code { get; set; }
        public Nullable<int> product_id { get; set; }
        public Nullable<int> product_sellerid { get; set; }
        public Nullable<decimal> proc_price { get; set; }
        public Nullable<int> proc_qty { get; set; }
        public Nullable<int> purchaseorderitem_qty { get; set; }
        public Nullable<int> purchaseorder_delivtype { get; set; }
        public Nullable<int> cart_shiptype { get; set; }
    }
}