using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("WindowBlocks")]
    public class WindowBlocks
    {
        public enum Zone
        {
            Zone1 = 1,
            Zone2 = 2,
            Zone3 = 3
        };
        public enum Style
        {
            樣板1 = 1,
            樣板2 = 2,
            樣板3 = 3,
            樣板4 = 4,
            樣板5 = 5,
            //樣板6 = 6,
            樣板7 = 7,
            樣板9 = 9
        };
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
