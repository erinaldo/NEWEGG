using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt
{
    [Table("PMTextObject")]
    public class TextObject
    {
        [Key]
        public int TextID { get; set; }
        public string Content { get; set; }
        public int HitCount { get; set; }
        public string InUser { get; set; }
        public System.DateTime InDate { get; set; }
        public string LastEditUser { get; set; }
        public Nullable<System.DateTime> LastEditDate { get; set; }
    }
}
