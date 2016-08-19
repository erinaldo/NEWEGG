using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("SMSMember")]
    public class SMSMember
    {
        public SMSMember() {
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SMSMemberGroupID { get; set; }
        public string Name { get; set; }
        public string MobilePhone { get; set; }
        public string CreateUser{get;set;}
        public DateTime CreateDate { get; set; }
    }
}
