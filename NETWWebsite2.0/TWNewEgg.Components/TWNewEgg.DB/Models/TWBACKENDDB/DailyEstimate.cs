using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{    [Table("DailyEstimate")]
    public class DailyEstimate
    {
        public DailyEstimate()
        {
            DateTime defaultDate = DateTime.Parse("1900/01/01 00:00:00");
            //cart_date = defaultDate;
            //cart_createdate = DateTime.Now;
            //cart_updatedate = defaultDate;
        }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Key]
        [Column(Order = 0)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 1)]
        public int Mouth { get; set; }
        [Key]
        [Column(Order =2)]
        public int Date { get; set; }
        public decimal? Estimate { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime SetCreateDate { get; set; }
    }
}
