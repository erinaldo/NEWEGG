using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    public class Redm
    {
        public Redm()
        {
        }

        [Key]
        public int redm_id { get; set; }

        public string redm_accountid { get; set; }
        public string redm_ordrcode { get; set; }
        public string redm_title { get; set; }
        public int redm_amantinit { get; set; }
        public int redm_amntwaiting { get; set; }
        public int redm_amntvalidated { get; set; }
        public int redm_bln { get; set; }
        public int redm_amntexpired { get; set; }
        public int redm_validtype { get; set; }
        public DateTime? redm_validstart { get; set; }
        public DateTime? redm_validend { get; set; }
        public DateTime? redm_date { get; set; }
        public int redm_visible { get; set; }
        public string redm_note { get; set; }
        public DateTime redm_createdate { get; set; }
        public string redm_createuser { get; set; }
        public int redm_updated { get; set; }
        public DateTime? redm_updatedate { get; set; }
        public string redm_updateuser { get; set; }
    }//end class
}//end namespace