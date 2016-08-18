using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("purchaseordergroupext")]
    public partial class PurchaseOrdergroupextTWBACK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int ID { get; set; }
        public int PurchaseorderGroupID { get; set; }
        public Nullable<int> PsCartID { get; set; }
        public string PSSellerID { get; set; }
        public string PSCarryNote { get; set; }
        public Nullable<int> Pshasact { get; set; }
        public Nullable<int> PshasPartialAuth { get; set; }
        public int Status { get; set; }
    }
}
