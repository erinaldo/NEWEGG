using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmtkout")]
    public class Fmtkout
    {
        public Fmtkout()
        {
        }


        [Key]
        public int fmtkout_cause { get; set; }

        public string fmtkout_causenote { get; set; }
        public int fmtkout_amnt { get; set; }
        public DateTime? fmtkout_validstart { get; set; }
        public DateTime? fmtkout_validend { get; set; }
        public int fmtkout_status { get; set; }
        public string fmtkout_statusnote { get; set; }
        public string fmtkout_note { get; set; }
        public DateTime fmtkout_createdate { get; set; }
        public string fmtkout_createuser { get; set; }
        public int fmtkout_updated { get; set; }
        public DateTime? fmtkout_updatedate { get; set; }
        public string fmtkout_updateuser { get; set; }
    }//end Fmtkout
}//end namespace