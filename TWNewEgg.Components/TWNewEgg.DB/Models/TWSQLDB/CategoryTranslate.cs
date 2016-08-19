using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("categorytranslate")]
    public class CategoryTranslate
    {
        public CategoryTranslate()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            Createdate = defaultDate;
            Updatedate = defaultDate;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CountryID { get; set; }
        public string Description { get; set; }
        public string Createuser { get; set; }
        public System.DateTime Createdate { get; set; }
        public string Updateuser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> Updatedate { get; set; }
    }
}