using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class PricingResponse
    {
        public List<PricingItemInfo> Items { get; set; }
    }

    public class PricingItemInfo
    {
        public string ItemNumber { get; set; }
        public bool Available { get; set; }
        public decimal FinalPrice { get; set; }
        public int VFInventory { get; set; }
        public decimal ShippingCharge { get; set; }
        public List<PricingWarehouse> Warehouses { get; set; }
    }

    public class PricingWarehouse
    {
        public string WHNo { get; set; }
        public int Quantity { get; set; }
    }
}