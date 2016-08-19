using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("ImageUrlReference")]
    public class ImageUrlReference
    {
        [Key, Column(Order = 0)]
        public int ItemID { get; set; }
        [Key, Column(Order = 1)]
        public int Size { get; set; }
        [Key, Column(Order = 2)]
        public int SizeIndex { get; set; }
        public string ImageUrl { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }
}
