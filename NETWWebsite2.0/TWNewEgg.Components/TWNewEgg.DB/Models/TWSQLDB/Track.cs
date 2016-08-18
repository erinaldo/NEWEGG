using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("track")]
    public class Track
    {
        public Track()
        {
            DateTime defaultDate = DateTime.Parse("1990/01/01");
            CreateDate = defaultDate;
        }
        [Key]
        public int ID { get; set; }
        public int ACCID { get; set; }
        public int ItemID { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }  
    }
}