using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View.Service
{
    public class NetPacketUtilities
    {
        public string GetUserIPAddress()
        {
            //string remoteIPAddr = httpContext.Request.Headers.ToString();
            string clientIPFromNetScale = HttpContext.Current.Request.Headers["ClientIP"];
            string clientIPFromProxy = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string clientIPFromIIS = HttpContext.Current.Request.UserHostAddress;
            if (!string.IsNullOrEmpty(clientIPFromNetScale))
            {
                return clientIPFromNetScale.Trim();
            }
            if (!string.IsNullOrEmpty(clientIPFromProxy))
            {
                string[] proxyAddresses = clientIPFromProxy.Split(',');
                if (proxyAddresses.Length != 0)
                {
                    return proxyAddresses[0].Trim();
                }
            }
            if (!string.IsNullOrEmpty(clientIPFromIIS))
            {
                return clientIPFromIIS.Trim();
            }
            return "::1";
        }
    }
}