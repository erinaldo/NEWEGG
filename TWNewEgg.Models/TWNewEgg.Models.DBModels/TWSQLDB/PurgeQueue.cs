using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("PurgeQueue")]
    public class PurgeQueue
    {
        public PurgeQueue()
        {
            CreateDate = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string URL { get; set; }
        public int isPurged { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<int> PurgeRequestID { get; set; }
    }
    public enum PurgeType
    {
        purgeError = 0,
        purgeSuccess = 1
    }
}
