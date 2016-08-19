using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("CargoStatusTrack")]
    public class CargoStatusTrack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string OrderNo { get; set; }
        public int TrackNo { get; set; }
        public string SubHawb { get; set; }
        public string DeliveryNo { get; set; }
        public string HawbDate { get; set; }
        public string HawbType { get; set; }
        public string HawbSite { get; set; }
        public string HawbMan { get; set; }
        public string HawbManCode { get; set; }
        public string DispatchorSendMan { get; set; }
        public string PreOrNextStation { get; set; }
        public string SignMan { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
        public string CreateUser { get; set; }
        public Nullable<int> Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
    }
}
