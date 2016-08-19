using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    
    [Table("Inventoryitem")]
    public class Inventoryitem
    {
        [Key]
        public int invitem_id { get; set; }

        [ForeignKey("inventory")]
        public int invitem_invid { get; set; }
        public virtual Inventory inventory { get; set; }

        public int invitem_productid { get; set; }
        public string invitem_sellerproductid { get; set; }
        public int? invitem_delivtype { get; set; }
        public int invitem_qty { get; set; }
        public string invitem_source { get; set; }
        public DateTime? invitem_createdate { get; set; }
        public string invitem_createuser { get; set; }
        public DateTime? invitem_updatedate { get; set; }
        public string invitem_updateuser { get; set; }
        public int? invitem_updated { get; set; }
    }
}