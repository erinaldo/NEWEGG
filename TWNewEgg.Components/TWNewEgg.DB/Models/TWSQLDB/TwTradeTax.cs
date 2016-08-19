using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("twtradetax")]
    public class TwTradeTax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ID { get; set; }
        public string Name { get; set; }
        public string ENGName { get; set; }
        public string Rate1 { get; set; }
        public string Rate2 { get; set; }
        public string Rate3 { get; set; }
        public string StaticUnit { get; set; }
        public string WeightUnit { get; set; }
        public string TWTradeTaxRule { get; set; }
        public string ImportRule { get; set; }
        public string ExportRule { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}