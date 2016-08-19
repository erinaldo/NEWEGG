using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("ItemPropertyGroup")]
    public partial class ItemPropertyGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int? CategoryID { get; set; }

        public string GroupName { get; set; }
        public string GroupNameTW { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int? Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
