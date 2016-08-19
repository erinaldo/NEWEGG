using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class QueryPriceStock
    {
        private QueryPriceStockFactory _factory;

        public QueryPriceStock(QueryPriceStockFactory factory)
        {
            _factory = factory;
        }

        public Dictionary<string, bool> GetStatus(List<string> sellerProductIdList)
        {
            return _factory.CreateProvider().GetStatus(sellerProductIdList);
        }

        public Dictionary<string, int> GetStock(List<string> sellerProductIdList, string warehouseNumber)
        {
            return _factory.CreateProvider().GetStock(sellerProductIdList, warehouseNumber);
        }

        public Dictionary<string, decimal> GetPrice(List<string> sellerProductIdList)
        {
            return _factory.CreateProvider().GetPrice(sellerProductIdList);
        }
    }
}