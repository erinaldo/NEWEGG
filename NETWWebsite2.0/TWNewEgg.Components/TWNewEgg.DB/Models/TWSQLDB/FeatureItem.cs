using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("featureitem")]
    public class FeatureItem
    {
        public FeatureItem()
        {
            CreateDate = DateTime.Now;
        }

        [Key]
        public int ID { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int ItemID { get; set; }
        public int FeatureitemOrder { get; set; }
        public Nullable<System.DateTime> DateStart { get; set; }
        public Nullable<System.DateTime> DateEnd { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }

}