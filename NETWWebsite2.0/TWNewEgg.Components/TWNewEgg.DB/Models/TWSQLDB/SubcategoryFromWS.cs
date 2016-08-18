using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("subcategoryfromws")]
    public class SubcategoryFromWS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubcategoryID { get; set; }
        public int NodeID { get; set; }
        public int StoreID { get; set; }
        public int CategoryID { get; set; }
        public string NValue { get; set; }
        public int CategoryType { get; set; }
        public string Description { get; set; }
        public int Showall { get; set; }
        public int ShowOrder { get; set; }
        public Nullable<int> SN { get; set; }
    }
}