using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class FreightCompany
    {
        public int FreightCompanyID { get; set; }
        public string FreightCompanyName { get; set; }
    }
    public class StatusSelect
    {
        public string StatusId { get; set; }
        public string StatusDes { get; set; }
    }
}