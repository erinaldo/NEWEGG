using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmnew")]
    public class Fmnew
    {
        public Fmnew()
        {
        }

        [Key]
        public int fmnew_id { get; set; }

        public string fmnew_accountid { get; set; }
        public int fmnew_cause { get; set; }
        public string fmnew_causenote { get; set; }
        public string fmnew_title { get; set; }
        public int fmnew_amnt { get; set; }
        public DateTime? fmnew_validstart { get; set; }
        public DateTime? fmnew_validend { get; set; }
        public int fmnew_status { get; set; }
        public string fmnew_statusnote { get; set; }
        public string fmnew_note { get; set; }
        public DateTime fmnew_createdate { get; set; }
        public string fmnew_createuser { get; set; }
        public int fmnew_updated { get; set; }
        public DateTime? fmnew_updatedate { get; set; }
        public string fmnew_updateuser { get; set; }
    }//end class
}//end namespace