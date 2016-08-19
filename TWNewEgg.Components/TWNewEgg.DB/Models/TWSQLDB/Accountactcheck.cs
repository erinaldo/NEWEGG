using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace TWNewEgg.DB.TWSQLDB.Models
{
    [Table("accountactcheck")]
    public class Accountactcheck
    {
        public enum AccountactcheckStatus
        {
            Null = 0,
            Success = 1,
            Block = 2,
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Phone { get; set; }
        public string User_id { get; set; }
        public Nullable<System.DateTime> CrearDate { get; set; }
        public Nullable<System.DateTime> StatusDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> FailCount { get; set; }
        public string Authenticate { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Nullable<int> SMSReturnID { get; set; }
    }
}
