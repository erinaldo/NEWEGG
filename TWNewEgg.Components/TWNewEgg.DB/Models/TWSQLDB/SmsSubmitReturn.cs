using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("smssubmitreturn")]
    public class SmsSubmitReturn
    {
        public SmsSubmitReturn()
        {
            SmsDate = DateTime.UtcNow.AddHours(8); ;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SmsID { get; set; }
        public string ToNumber { get; set; }
        public string ReturnCode { get; set; }
        public string MessageID { get; set; }
        public string ReturnDescription { get; set; }
        public System.DateTime SmsDate { get; set; }

    }
}
