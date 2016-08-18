using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("PurgeRequest")]
    public class PurgeRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int TryTimes { get; set; }
        public string AsyncCheckStateURL { get; set; }
        public string StatusCode { get; set; }
        public int Status { get; set; }
        public string CDNEndPointName { get; set; }
        public string ErrorMessage { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
    }
}
