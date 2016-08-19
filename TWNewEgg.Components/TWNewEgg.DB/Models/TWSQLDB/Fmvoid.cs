using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmvoid")]
    public class Fmvoid
    {
        public Fmvoid()
        {
        }

        [Key]
        public int fmvoid_id { get; set; }

        public int fmvoid_redmid { get; set; }
        public int fmvoid_cause { get; set; }
        public string fmvoid_causenote { get; set; }
        public string fmvoid_title { get; set; }
        public int fmvoid_amnt { get; set; }
        public DateTime? fmvoid_validstart { get; set; }
        public DateTime? fmvoid_validend { get; set; }
        public int fmvoid_status { get; set; }
        public string fmvoid_statusnote { get; set; }
        public string fmvoid_note { get; set; }
        public DateTime fmvoid_createdate { get; set; }
        public string fmvoid_createuser { get; set; }
        public int fmvoid_updated { get; set; }
        public DateTime? fmvoid_updatedate { get; set; }
        public string fmvoid_updateuser { get; set; }

    }//end class
}//end namespace