using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("salesordergroup")]
    public class SalesOrderGroup
    {
        public SalesOrderGroup()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
            UpdateDate = defaultDate;
        }
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Vaccunt { get; set; }
        public int PriceSum { get; set; }
        public int OrderNum { get; set; }
        public string Note { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Updated { get; set; }
        public string UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        //public string Status { get; set; }
    }
}