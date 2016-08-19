using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("ItemGroupDetailProperty")]
    public class ItemGroupDetailProperty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int GroupID { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> ItemTempID { get; set; }
        public int SellerID { get; set; }

        [Key, Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MasterPropertyID { get; set; }

        
        public int? PropertyID { get; set; }
        [Key, Column(Order = 2)]
        public int GroupValueID { get; set; }
        [Key, Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ValueID { get; set; }
        public string ValueName { get; set; }
        public string InputValue { get; set; }        
        public System.DateTime CreateDate { get; set; }
        public int InUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
