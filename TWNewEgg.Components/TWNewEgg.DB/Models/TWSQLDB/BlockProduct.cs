using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("blockproduct")]
    public class BlockProduct
    {
        public BlockProduct()
        {
            this.InDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public int ProductID { get; set; }
        public string Action { get; set; }
        public DateTime InDate { get; set; }
    }
}
