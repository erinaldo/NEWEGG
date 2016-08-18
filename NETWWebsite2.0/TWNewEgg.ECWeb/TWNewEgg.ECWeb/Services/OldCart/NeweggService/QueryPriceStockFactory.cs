using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class QueryPriceStockFactory
    {
        public enum Provider
        {
            APIsNewegg = 0,
            Q4S = 1
        }

        private Provider _provider;

        public QueryPriceStockFactory(Provider provider)
        {
            _provider = provider;
        }

        public IProductInfoProvider CreateProvider()
        {
            IProductInfoProvider providerInstance = null;
            switch (_provider)
            {
                case Provider.APIsNewegg:
                    providerInstance = new Pricing();
                    break;
                case Provider.Q4S:
                    providerInstance = new Q4S();
                    break;
            }
            return providerInstance;
        }
    }
}