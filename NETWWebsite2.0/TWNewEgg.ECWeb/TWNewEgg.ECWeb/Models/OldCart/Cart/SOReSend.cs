using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class SOReSend
    {
        public string Environment { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string SalesOrderCode { get; set; }
        public string PurchaseOrderCode { get; set; }
        public string ItemID { get; set; }
        public string ProductID { get; set; }
        public string SellerProductID { get; set; }
        public string ItemName { get; set; }
        public string DelvType { get; set; }
        public string PayType { get; set; }
        public string Status { get; set; }
    }
}