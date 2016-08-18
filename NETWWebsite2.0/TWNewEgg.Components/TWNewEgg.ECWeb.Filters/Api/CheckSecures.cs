using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Api
{
    public class CheckSecures
    {
        public static bool CheckSSLConnection(HttpActionContext actionContext)
        {
            bool checkSSL = false;
            int sslPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsPort"]);
            if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["IISEnableSSL"], "true", StringComparison.OrdinalIgnoreCase))
            {
                if (actionContext.Request.RequestUri.Scheme == Uri.UriSchemeHttps &&
                    string.Equals(actionContext.Request.RequestUri.Host, System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], StringComparison.OrdinalIgnoreCase))
                {
                    checkSSL = true;
                }
            }
            else
            {
                if (string.Equals(actionContext.Request.RequestUri.Host, System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], StringComparison.OrdinalIgnoreCase))
                {
                    checkSSL = true;
                }
            }

            return checkSSL;
        }
    }
}
