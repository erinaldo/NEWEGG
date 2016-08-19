using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View
{
    public class OrderInfoAddDes:TWNewEgg.API.Models.OrderInfo
    {
        public string Des { get; set; }
        public string Detail { get; set; }
        public string Total { get; set; }
        
    }
}