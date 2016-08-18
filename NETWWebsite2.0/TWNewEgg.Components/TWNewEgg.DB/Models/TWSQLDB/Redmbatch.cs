using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Redmbatch")]
    public class Redmbatch
    {
        public Redmbatch()
        {
        }

        [Key]
        public int redmbatch_id { get; set; }

        public int redmbatch_type { get; set; }
        public string redmbatch_name { get; set; }
        public int redmbatch_oknum { get; set; }
        public int redmbatch_okamnt { get; set; }
        public int redmbatch_oktkout { get; set; }
        public int redmbatch_failnum { get; set; }
        public int redmbatch_failamnt { get; set; }
        public int redmbatch_failtkout { get; set; }
        public string redmbatch_serverip { get; set; }
        public string redmbatch_note { get; set; }
        public DateTime redmbatch_createdate { get; set; }
        public string redmbatch_createuser { get; set; }
        public int redmbatch_updated { get; set; }
        public DateTime? redmbatch_updatedate { get; set; }
        public string redmbatch_updateuser { get; set; }
    }//end class
}//end  namespace