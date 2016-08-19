using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class CathayUnitedBank
    {
        public CathayUnitedBank()
        {
            this.CompanyID = "010230101000";
        }
        public string CompanyID { get; set; }
        public string orderNoGenDate { get; set; }
        public string PtrAcno { get; set; }
        public string AcqDate { get; set; }
        public string AcqTime { get; set; }
        public string ItemNo { get; set; }
        public int? PurQuantity { get; set; }
        public decimal amount { get; set; }
        public string MerchantKey { get; set; }
        public string salesorder_code { get; set; }
        public string TrsCode { get; set; }
        public string rtnCode { get; set; }
    }
}