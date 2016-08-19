using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("AdditionalItemForItem")]
    public class AdditionalItemForItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int ParentID { get; set; }
    }
}
