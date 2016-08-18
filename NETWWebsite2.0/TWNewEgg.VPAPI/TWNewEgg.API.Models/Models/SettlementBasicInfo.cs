using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models.Models
{
    public class SettlementBasicInfo
    {
        //[DataMember]
        public DateTime From { get; set; }

        //[DataMember]
        public DateTime To { get; set; }

        //[DataMember]
        public string ClearDocument { get; set; }

        //[DataMember]
        public string CheckNumber { get; set; }

        //[DataMember]
        public string SellerID { get; set; }

        //[DataMember]
        public string Description
        {
            get
            {
                return string.Format("{0}", To.ToString("d"));
                //return string.Format("{0} - {1}",
                //    From.ToString("d"),
                //    To.ToString("d"));
            }
            set { }
        }
    }
}
