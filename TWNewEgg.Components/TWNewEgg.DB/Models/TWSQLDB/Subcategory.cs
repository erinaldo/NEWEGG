using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("subcategory")]
    public class Subcategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int subcategory_nodeid { get; set; }
        public int subcategory_storeid { get; set; }
        public int subcategory_categoryid { get; set; }
        public int subcategory_categorytype { get; set; }
        public string subcategory_description { get; set; }
        public int subcategory_showall { get; set; }
        public int subcategory_updated { get; set; }
        public int subcategory_showorder { get; set; }
    }
}