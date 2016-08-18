using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt
{
    [Table("PMComponentInfo")]
    public class ComponentInfo
    {
        public class ComponentStatus
        {
            public static string Saved = "S";
            public static string Edit = "E";
            public static string New = "N";
            public static string Delete = "D";
        }

        [Key]
        public int ComponentID { get; set; }
        public string Index { get; set; }
        public int PageID { get; set; }
        public string ObjectType { get; set; }
        public int ObjectID { get; set; }
        public int HitCount { get; set; }
        public string Status { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ZIndex { get; set; }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public string InUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string LastEditUser { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
    }
}
