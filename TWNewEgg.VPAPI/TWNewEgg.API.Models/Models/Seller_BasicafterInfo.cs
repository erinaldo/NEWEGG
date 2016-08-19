using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class Seller_BasicafterInfo
    {
        public int SellerID { get; set; }
        public string AboutInfo { get; set; }
        public string SellerLogoURL { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? ActiveatedDate { get; set; }
        public int? ActiveatedUserID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateUserID { get; set; }
        public DateTime? InDate { get; set; }
        public int? InUserID { get; set; }
    }
}
