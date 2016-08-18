using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class TrackCart
    {

        public string accID{ get; set; }
        public int trackStatus { get; set; }
        public List<int> itemID { get; set; }
        public List<int> itemlistID { get; set; }
        public int opCode { get; set; }


    }
}