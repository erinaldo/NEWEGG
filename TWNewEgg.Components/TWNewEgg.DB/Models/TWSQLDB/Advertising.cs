﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("Advertising")]
    public class Advertising
    {
        public enum AdType
        {
            輪播 = 1,
            右方小廣告 = 2,
            下方廣告 = 3,
            左下廣告 = 5
        };
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CategoryID { get; set; }
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
