using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("smssubmit")]
    public class SmsSubmit
    {
        public SmsSubmit()
        {
            SmsDate = DateTime.UtcNow.AddHours(8); ;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SmsID { get; set; }
        public string ToNumber { get; set; }
        public int MessageNo { get; set; }
        public string MessageContent { get; set; }
        public System.DateTime SmsDate { get; set; }

    }
}
