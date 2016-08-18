using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Redmio")]
    public class Redmio
    {
        public Redmio()
        {
        }

        [Key]
        public int redmio_id { get; set; }

        public int redmio_type { get; set; }
        public string redmio_blng { get; set; }
        public int redmio_blngid { get; set; }
        public int redmio_blngseq { get; set; }
        public int redmio_redmbatchid { get; set; }
        public int redmio_redmid { get; set; }
        public string redmio_ordrcode { get; set; }
        public string redmio_ordritemcode { get; set; }
        public string redmio_accountid { get; set; }
        public string redmio_title { get; set; }
        public int redmio_amnt { get; set; }
        public int redmio_mredmbln { get; set; }
        public int redmio_validtype { get; set; }
        public DateTime? redmio_validstart { get; set; }
        public DateTime? redmio_validend { get; set; }
        public DateTime? redmio_date { get; set; }
        public DateTime? redmio_syncdate { get; set; }
        public string redmio_note { get; set; }
        public DateTime redmio_createdate { get; set; }
        public string redmio_createuser { get; set; }
        public int redmio_updated { get; set; }
        public DateTime? redmio_updatedate { get; set; }
        public string redmio_updateuser { get; set; }
    }//end class
}//end namespace