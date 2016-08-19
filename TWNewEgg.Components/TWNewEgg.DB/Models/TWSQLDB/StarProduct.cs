using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("starproduct")]
    public class StarProduct
    {
        public StarProduct()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            DateStart = defaultDate;
            DateEnd = defaultDate;
            CreateDate = defaultDate;
            UpdateDate = defaultDate;
                

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public int CategoryLayer { get; set; }
        public int ItemID { get; set; }
        public int SellerID { get; set; }
        public int ShowType { get; set; }
        public int StarProductOrder { get; set; }
        public int ManufactureID { get; set; }
        public System.DateTime DateStart { get; set; }
        public System.DateTime DateEnd { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}