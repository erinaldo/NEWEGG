using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class OTPClientModel
    {
        public int UID { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
        public int FailCount { get; set; }

    }
}