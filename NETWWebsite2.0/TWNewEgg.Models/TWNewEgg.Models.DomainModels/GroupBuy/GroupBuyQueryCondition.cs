using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Models.DomainModels.GroupBuy
{
    public class GroupBuyQueryCondition
    {
        public int GroupBuyID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}