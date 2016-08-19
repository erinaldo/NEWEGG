using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("nodefromws")]
    public class NodeFromWS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public int SubcategoryID { get; set; }
        public int NodeID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ItemCount { get; set; }
        public string WebURL { get; set; }
    }
}