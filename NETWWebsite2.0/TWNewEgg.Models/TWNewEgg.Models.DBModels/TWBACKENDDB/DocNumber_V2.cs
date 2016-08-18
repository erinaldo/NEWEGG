using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("DocNumber_V2")]
    public class DocNumber_V2
    {
        public enum DOCTypeEnum
        {
            XQ,
            XD,
            XI,
            XIRMA,
            TrID
            //XP,
            //XV
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string DocType { get; set; }
        public int CurrentNumber { get; set; }
        public string StartNumber { get; set; }
        public string EndNumber { get; set; }
        public string CreateUser { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<int> Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<DateTime> StartUsingDate { get; set; }
        public Nullable<DateTime> EndUsingDate { get; set; }
        public string CurrentNumberV2 { get; set; }
        public Nullable<int> IsChange { get; set; }
    }
}
