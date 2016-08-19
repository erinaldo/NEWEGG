using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class UnShipList
    {
        public int SellerID { get; set; }

        public string SellerEmail { get; set; }

        public string SellerName { get; set; }

        public int Unshipcount { get; set; }

        public string CartID { get; set; }
    }
}
