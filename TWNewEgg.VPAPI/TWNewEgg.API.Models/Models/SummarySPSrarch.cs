using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class SummarySPSrarch
    {   
        /// <summary>
        /// 廠商ID
        /// </summary>
        public string inputSellerID {get;set;}

        /// <summary>
        /// 資料起始日期
        /// </summary>
        public string inputStartDate { get; set; }

        /// <summary>
        /// 資料結束日期
        /// </summary>
        public string inputEndDate { get; set; }
    }
}
