using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("searchWord")]
    public class SearchWord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string KeyWords { get; set; }
        public int SearchTime { get; set; }
        public string SearchPath { get; set; }
        public string CacheLoc { get; set; }
        public Nullable<System.DateTime> CacheDate { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
