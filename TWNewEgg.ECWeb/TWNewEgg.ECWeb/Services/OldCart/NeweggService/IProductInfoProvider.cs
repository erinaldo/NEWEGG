using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Service
{
    public interface IProductInfoProvider
    {
        Dictionary<string, bool> GetStatus(List<string> sellerProductIdList);
        Dictionary<string, decimal> GetPrice(List<string> sellerProductIdList);
        Dictionary<string, decimal> GetPriceWithShippingCharge(List<string> sellerProductIdList);
        Dictionary<string, int> GetStock(List<string> sellerProductIdList, string warehouseNumber);
    }
}