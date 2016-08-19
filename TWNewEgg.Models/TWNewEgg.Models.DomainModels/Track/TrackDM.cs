using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Track
{
    public class TrackDM
    {
        public int ID { get; set; }
        public int ACCID { get; set; }
        public int ItemID { get; set; }
        public int Status { get; set; }
        public int Qty { get; set; }
        public int? CategoryID { get; set; }
        public int? CategoryType { get; set; }
    }
}
