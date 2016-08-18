using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmevnt")]
    public class Fmevnt
    {
        public Fmevnt()
        {
        }

        [Key]
        public int fmevnt_id { get; set; }

        public string fmevnt_blng { get; set; }
        public int fmevnt_blngid { get; set; }
        public int fmevnt_cause { get; set; }
        public string fmevnt_causenote { get; set; }
        public string fmevnt_causeuser { get; set; }
        public int fmevnt_solve { get; set; }
        public string fmevnt_solvenote { get; set; }
        public string fmevnt_solveuser { get; set; }
        public int fmevnt_status { get; set; }
        public string fmevnt_statusnote { get; set; }
        public string fmevnt_statususer { get; set; }
        public string fmevnt_note { get; set; }
        public DateTime fmevnt_createdate { get; set; }
        public string fmevnt_createuser { get; set; }
        public int fmevnt_updated { get; set; }
        public DateTime? fmevnt_updatedate { get; set; }
        public string fmevnt_updateuser { get; set; }
    }//end class
}//end namespace