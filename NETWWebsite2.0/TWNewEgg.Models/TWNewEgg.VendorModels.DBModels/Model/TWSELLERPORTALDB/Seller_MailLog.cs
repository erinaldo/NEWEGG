using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_MailLog")]
    public class Seller_MailLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SN { get; set; }
        public string MailType { get; set; }
        public string ID { get; set; }
        public string Email { get; set; }
        public string IsSuccess { get; set; }
        public string Msg { get; set; }

        public Nullable<int> InUserID { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> UpdateUserID { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
    }
}
