using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Image_log")]
    public class Image_log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string Container { get; set; }
        public string FromPath { get; set; }
        public string TargetPath { get; set; }
        public string FromSystem { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string Status { get; set; }
        public string UserAction { get; set; }
    }
}
