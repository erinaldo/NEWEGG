using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("trackitem")]
    public class TrackItem
    {
        [Key]
        public int ID { get; set; }
        public int TrackID { get; set; }
        public int ItemlistID { get; set; }
        public int Status { get; set; }
    }
}