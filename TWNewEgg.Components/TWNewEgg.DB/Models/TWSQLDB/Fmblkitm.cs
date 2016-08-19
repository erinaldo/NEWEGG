using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Fmblkitm")]
    public class Fmblkitm
    {
        public Fmblkitm()
        {
        }

        [Key]
        public int fmblkitm_id { get; set; }

        public int fmblkitm_fmblkid { get; set; }
        public string fmblkitm_accountid { get; set; }
        public string fmblkitm_email { get; set; }
        public DateTime? fmblkitm_execdate { get; set; }
        public string fmblkitm_execrslt { get; set; }
        public DateTime? fmblkitm_emaildate { get; set; }
        public DateTime fmblkitm_createdate { get; set; }
        public int fmblkitm_updated { get; set; }
        public DateTime? fmblkitm_updatedate { get; set; }
        public string fmblkitm_updateuser { get; set; }
    }//end class
}//end namespace