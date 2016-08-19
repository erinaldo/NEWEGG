using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class ShipTaxService
    {
        public Dictionary<string, decimal> ShippingCost { get; set; }
        public Dictionary<string, List<GetItemTaxDetail>> ShippingTaxCost { get; set; }
        //public List<BuyingItems> PostData { get; set; }
    }
}