using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("CargoStatus")]
    public class CargoStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string OrderNo { get; set; }
        public string ShipID { get; set; }
        public string SellerID { get; set; }
        public string ShipStatus { get; set; }
        public string ShipIssue { get; set; }
        public string Note { get; set; }
        public Nullable<int> SubHawbNums { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
    }
}
