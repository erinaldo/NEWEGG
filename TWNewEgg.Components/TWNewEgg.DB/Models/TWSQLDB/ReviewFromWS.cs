using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("reviewfromws")]
    public class ReviewFromWS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int ProductfromwsID { get; set; }
        public string Title { get; set; }
        public Nullable<int> Rating { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public string Nickname { get; set; }
        public string PROS { get; set; }
        public string CONS { get; set; }
        public string Comments { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}