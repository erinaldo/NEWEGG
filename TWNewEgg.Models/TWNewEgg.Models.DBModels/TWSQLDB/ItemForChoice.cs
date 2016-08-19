using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("ItemForChoice")]
    public class ItemForChoice
    {
        [Key]
        [Column(Order = 0)]
        public int CategoryID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ItemID { get; set; }
        public int Showorder { get; set; }
        public int Showall { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
