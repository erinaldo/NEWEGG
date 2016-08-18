using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class UpdateTWTrackingNum
    {
        public string SalesOrderCode { get; set; }
        public string InvoiceNO { get; set; }
        public int? RefSONumber { get; set; }
        public string TrackNO { get; set; }
        public int? ForwarderID { get; set; }
        public string UpdateNote { get; set; }
    }
}
