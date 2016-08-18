using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Redmacnt")]
    public class Redmacnt
    {
        public Redmacnt()
        { }

        [Key]
        public int redmacnt_id { get; set; }

        public string redmacnt_accountid { get; set; }
        public int redmacnt_bln { get; set; }
        public int redmacnt_amntwaiting { get; set; }
        public DateTime? redmacnt_dueupdated { get; set; }
        public DateTime? redmacnt_duedate { get; set; }
        public int redmacnt_dueamnt { get; set; }
        public string redmacnt_note { get; set; }
        public DateTime redmacnt_createdate { get; set; }
        public string redmacnt_createuser { get; set; }
        public int redmacnt_updated { get; set; }
        public DateTime? redmacnt_updatedate { get; set; }
        public string redmacnt_updateuser { get; set; }
    }//end Redmacnt
}//end namespace