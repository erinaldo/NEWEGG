using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("purchaseorderitemext")]
    public partial class PurchaseOrderitemexTWBACK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string PurchaseOrderItemCode { get; set; }
        public string PSProductID { get; set; }
        public string PSMProductid { get; set; }
        public Nullable<decimal> PSORIPrice { get; set; }
        public string PSSellCatID { get; set; }
        public string PSAttribName { get; set; }
        public string PSModelNO { get; set; }
        public Nullable<int> PSCost { get; set; }
        public Nullable<int> PSFvf { get; set; }
    }
}
