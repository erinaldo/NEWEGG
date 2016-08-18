using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.VendorModels.DBModels.Model
{
    [Table("Seller_AuthToken")]
    public class Seller_AuthToken
    {
        public Seller_AuthToken()
        {
            CreateDate = DateTime.Now;
            TerminateDate = DateTime.Now.AddMinutes(10);
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AccountIdEmail { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public Nullable<DateTime> TerminateDate { get; set; }
        public string StartIP { get; set; }
        public string EndIP { get; set; }
        public string Token { get; set; }

    }
}
