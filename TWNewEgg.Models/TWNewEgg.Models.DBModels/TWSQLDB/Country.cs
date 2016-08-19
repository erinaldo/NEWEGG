using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("country")]
    public class Country
    {
        public enum countryNameTW
        {
            台灣 = 1,
            美國 = 2,
            加拿大 = 3,
            中國 = 4,
            香港 = 5
        }

        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string UsageCurrency { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string ShortName { get; set; }
        public string countryName
        {
            get { return Enum.Parse(typeof(countryNameTW), ID.ToString()).ToString(); }
        }
    }
}