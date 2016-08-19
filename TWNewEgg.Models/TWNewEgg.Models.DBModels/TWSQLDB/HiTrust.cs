using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("HiTrust")]
    public class HiTrust
    {
        public string MerConfigName { get; set; }
        [Key, Column("BnkID", Order = 0)]
        public string BnkID { get; set; }
        [Key, Column("DateStart", Order = 1)]
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        [Key, Column("IsOnce", Order = 2)]
        public int IsOnce { get; set; }
        public string UpdateUrl { get; set; }
        public string merupdateURL { get; set; }
        public string returnURL { get; set; }
        public string HiServer { get; set; }
        public string MerConfig { get; set; }
        public string QueryFlag { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
