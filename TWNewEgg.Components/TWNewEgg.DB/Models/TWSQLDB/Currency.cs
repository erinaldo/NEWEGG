using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("currency")]
    public class Currency
    {
        public Currency()
        {
            CreateDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CountryID { get; set; }
        public string Bank { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public decimal AverageexchangeRate { get; set; }
        public decimal BufferRate { get; set; }
        public string CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}