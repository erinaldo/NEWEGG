using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmblk")]
    public class Fmblk
    {
        public Fmblk()
        {
        }

        [Key]
        public int fmblik_id { get; set; }

        public int fmblk_cmdtype { get; set; }
        public string fmblk_cmddb { get; set; }
        public string fmblk_cmdtxt { get; set; }
        public DateTime? fmblk_cmddate { get; set; }
        public int fmblk_cmditmcnt { get; set; }
        public string fmblk_title { get; set; }
        public DateTime? fmblk_validstart { get; set; }
        public DateTime? fmblk_validend { get; set; }
        public int fmblk_amnt { get; set; }
        public DateTime? fmblk_date { get; set; }
        public int fmblk_status { get; set; }
        public string fmblk_statususer { get; set; }
        public string fmblk_note { get; set; }
        public DateTime fmblk_createdate { get; set; }
        public string fmblk_createuser { get; set; }
        public int fmblk_updated { get; set; }
        public DateTime? fmblk_updatedate { get; set; }
        public string fmblk_updateuser { get; set; }

    }//end class
}//end namespace
