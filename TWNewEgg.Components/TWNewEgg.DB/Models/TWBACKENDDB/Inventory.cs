using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        public int inv_id { get; set; }
        public int inv_productid { get; set; }
        public string inv_sellerproductid { get; set; }
        public int? inv_delivtype { get; set; }
        public int inv_instockqty { get; set; }
        public int inv_wmsqty { get; set; }
        public int? inv_whid { get; set; }
        public DateTime? inv_createdate { get; set; }
        public string inv_createuser { get; set; }
        public DateTime? inv_updatedate { get; set; }
        public string inv_updateuser { get; set; }
        public int? inv_updated { get; set; }
        public virtual ICollection<Inventoryitem> inventoryitems { get; set; }
    }
}