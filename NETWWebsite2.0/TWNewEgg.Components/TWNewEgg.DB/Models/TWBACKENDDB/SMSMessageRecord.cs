using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("SMSMessageRecord")]
    public class SMSMessageRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID {get;set;}
        public string SalesOrderCode{get;set;}
        public string ProblemCode{get;set;}
        public int SMSMemberGroupID{get;set;}
        public string ReceiverName{get;set;}
        public string ReceiverMobilePhone{ get; set; }
        public int MessageTypeID{get;set;}
        public string MessageContent{get;set;}
        public Nullable<DateTime> SendDate{get;set;}
        public bool IsSend{get;set;}
        public string SenderName{get;set;}
        public DateTime CreateDate { get; set; }
    }
}
