using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb.PrivilegeFilters
{
    public class CheckSecures
    {
        public static bool CheckSSLConnection(HttpContextBase httpContext)
        {
            bool checkSSL = false;
            int sslPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsPort"]);
            if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["IISEnableSSL"], "true", StringComparison.OrdinalIgnoreCase))
            {
                if (httpContext.Request.IsSecureConnection &&
                    string.Equals(httpContext.Request.Url.Host, System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], StringComparison.OrdinalIgnoreCase))
                {
                    checkSSL = true;
                }
            }
            else
            {
                if (string.Equals(httpContext.Request.Url.Host, System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], StringComparison.OrdinalIgnoreCase))
                {
                    checkSSL = true;
                }
            }

            return checkSSL;
        }

        public static bool CheckSSLConnection(HttpContext httpContext)
        {
            bool checkSSL = false;
            int sslPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsPort"]);
            if (string.Equals(System.Configuration.ConfigurationManager.AppSettings["IISEnableSSL"], "true", StringComparison.OrdinalIgnoreCase))
            {
                if (httpContext.Request.IsSecureConnection &&
                    string.Equals(httpContext.Request.Url.Host, System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], StringComparison.OrdinalIgnoreCase))
                {
                    checkSSL = true;
                }
            }
            else
            {
                if (string.Equals(httpContext.Request.Url.Host, System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsDomain"], StringComparison.OrdinalIgnoreCase))
                {
                    checkSSL = true;
                }
            }

            return checkSSL;
        }
    }
}
