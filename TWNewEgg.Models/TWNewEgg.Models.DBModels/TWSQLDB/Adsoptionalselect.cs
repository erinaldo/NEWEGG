using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("adsoptionalselect")]
    public class Adsoptionalselect
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int OptionalSelectID { get; set; }
        public int BannerType { get; set; }
        public string Description { get; set; }
        public int Showorder { get; set; }
        public int ShowAll { get; set; }
        public string Imagepath { get; set; }
        public string Clickpath { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
