using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("SMSMessageSample")]
    public class SMSMessageSample
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MessageTypeName{get;set;}
        public string MessageContent { get; set; }
    }
}
