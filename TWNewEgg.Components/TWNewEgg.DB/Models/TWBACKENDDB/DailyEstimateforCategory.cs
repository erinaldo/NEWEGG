using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("DailyEstimateforCategory")]
    public class DailyEstimateforCategory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        [Key]
        [Column(Order = 0)]
        public int Year { get; set; }

        [Key]
        [Column(Order = 1)]
        public int Mouth { get; set; }
        
        [Key]
        [Column(Order = 2)]
        public string CategoryName { get; set; }

        [Key]
        [Column(Order = 3)]
        public int CategoryID { get; set; }

        public decimal? Estimate { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateDate { get; set; }
        //public DateTime SetCreateDate { get; set; }
        //public DateTime? SetCreateDate { get; set; }
    }
}
