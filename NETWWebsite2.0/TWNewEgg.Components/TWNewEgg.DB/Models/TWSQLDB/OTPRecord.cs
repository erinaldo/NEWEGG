using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("OTPRecord")]
    public class OTPRecord
    {
        public enum OPTStatus
        {
            Null = 0,
            Success = 1,
            Block = 2,
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }
        public string Items { get; set; }
        public string CartID { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> Amount { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public DateTime StatusDate { get; set; }
        public Nullable<int> FailCount { get; set; }
        public string Password { get; set; }
        public Nullable<int> SMSReturnID { get; set; }
       
    }
}