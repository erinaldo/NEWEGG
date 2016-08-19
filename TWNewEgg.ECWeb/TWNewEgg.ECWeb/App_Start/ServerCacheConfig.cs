using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.ActionFilters.Model;
using TWNewEgg.Framework.Cache;

namespace TWNewEgg.ECWeb.App_Start
{
    public class ServerCacheConfig
    {
        public static void Bootstrapper()
        {
            if (CacheConfiguration.Instance.GetFromCache<VarnishHeads>(FilterConst.VARNISHHEADLOC, null, false) == null)
            {
                throw new Exception("Varnish Configuration XML Load Failed.");
            }
        }
    }
}