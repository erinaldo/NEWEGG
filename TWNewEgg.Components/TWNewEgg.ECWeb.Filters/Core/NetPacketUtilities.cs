using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Collections;
using System.Web.Configuration;
using System.Collections.Specialized;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Core
{
    public class NetPacketUtilities
    {
        public static string GetUserIPAddress()
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

        public static string GetUserBrowser()
        {
            string userAgent = HttpContext.Current.Request.UserAgent;
            HttpBrowserCapabilities userBrowser = new HttpBrowserCapabilities { Capabilities = new Hashtable { { string.Empty, userAgent } } };
            var factory = new BrowserCapabilitiesFactory();
            factory.ConfigureBrowserCapabilities(new NameValueCollection(), userBrowser);
            return string.Format("{0}{1}", userBrowser.Browser, userBrowser.Version);
        }
    }
}
