using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("tradecategory")]
    public class TradeCategory
    {
        public TradeCategory()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        [Key]
        public int ID { get; set; }
        public int SubcategoryID { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}