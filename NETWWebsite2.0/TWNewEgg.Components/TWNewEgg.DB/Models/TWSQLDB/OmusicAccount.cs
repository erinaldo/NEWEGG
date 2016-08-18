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
    [Table("OmusicAccount")]
    public class OmusicAccount
    {
        public OmusicAccount()
        {
            UpdateDate = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string OmusicSN { get; set; }
        public int? AccountID { get; set; }
        public DateTime? TakeDate { get; set; }
        public int? CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? UpdateUserID { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
