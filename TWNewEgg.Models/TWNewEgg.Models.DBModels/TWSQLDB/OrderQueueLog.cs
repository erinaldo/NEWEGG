using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("OrderQueueLog")]
    public class OrderQueueLog
    {
        public OrderQueueLog()
        {
            CreateDate = DateTime.Now;
            CreateUser = string.Empty;
            LockedBy = string.Empty;
            Status = -1;
            ErrMsg = string.Empty;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int OrderNumber { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public string LockedBy { get; set; }
        public int Status { get; set; }
        public string ErrMsg { get; set; }
    }
    public enum StatusType
    {
        error = 0,
        success = 1,
    }
}
