using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb.Utility
{
    public class ImageUtility
    {
        private const string HttpContextImageHost = "Http-Context-Image-Host";

        public static string GetImagePath(string imagePath)
        {
            Uri imageUri;
            if (Uri.TryCreate(imagePath, UriKind.Absolute, out imageUri))
            {
                return imageUri.AbsoluteUri;
            }

            if (!string.IsNullOrEmpty(imagePath))
            {
                return GetImageHost() + imagePath;
            }

            return string.Empty;
        }

        private static string GetImageHost()
        {
            string hostString = ContextItemUility.GetItemValue(HttpContextImageHost);

            if (string.IsNullOrEmpty(hostString))
            {
                try
                {
                    hostString = 
                    TWNewEgg.ECWeb.PrivilegeFilters.CheckSecures.CheckSSLConnection(HttpContext.Current) ?
                    (System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsImgDomain"]) :
                    (System.Configuration.ConfigurationManager.AppSettings["ECWebHttpImgDomain"]);

                    ContextItemUility.SetItemValue(HttpContextImageHost, hostString);
                }
                catch (Exception)
                {
                    hostString = string.Empty;
                }
            }

            return hostString;
        }
    }
}