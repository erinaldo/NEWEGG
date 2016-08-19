using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Subcategorylogo")]
    public class Subcategorylogo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int SubCategoryID { get; set; }
        public int ManufactureID { get; set; }
        public int Showorder { get; set; }
        public int ShowAll { get; set; }
        public string ImageUrl { get; set; }
        public string Clickpath { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
