using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmcncl")]
    public class Fmcncl
    {
        public Fmcncl()
        {
        }

        [Key]
        public int fmcncl_id { get; set; }

        public int fmcncl_type { get; set; }
        public string fmcncl_frm { get; set; }
        public int fmcncl_frmid { get; set; }
        public int fmcncl_processid { get; set; }
        public string fmcncl_accountid { get; set; }
        public string fmcncl_title { get; set; }
        public int fmcncl_fdbck { get; set; }
        public int fmcncl_tkout { get; set; }
        public int fmcncl_redmbatchid { get; set; }
        public string fmcncl_execrslt { get; set; }
        public int fmcncl_status { get; set; }
        public DateTime? fmcncl_statusdate { get; set; }
        public string fmcncl_statususer { get; set; }
        public string fmcncl_note { get; set; }
        public DateTime fmcncl_createdate { get; set; }
        public string fmcncl_createuser { get; set; }
        public int fmcncl_updated { get; set; }
        public DateTime? fmcncl_updatedate { get; set; }
        public string fmcncl_updateuser { get; set; }

    }//end class
}//end namespace