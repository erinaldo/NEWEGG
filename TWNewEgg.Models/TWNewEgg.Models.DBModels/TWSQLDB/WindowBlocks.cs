using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("WindowBlocks")]
    public class WindowBlocks
    {
        [Key, Column(Order = 0)]
        public int SubCategoryID { get; set; }
        [Key, Column(Order = 1)]
        public int ZoneID { get; set; }
        public int ZoneStyle { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
