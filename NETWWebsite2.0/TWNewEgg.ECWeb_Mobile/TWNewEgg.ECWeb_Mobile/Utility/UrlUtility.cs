using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb_Mobile.Utility
{
    public class UrlUtility
    {
        public static string GetCategoryUrl(int categoryID)
        {
            if (categoryID <= 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("/Category?ItemId=");
            builder.Append(categoryID);

            return builder.ToString();
        }

        public static string GetFlashListUrl()
        {
            return "/Flash";
        }

        public static string GetItemPageUrl(int itemId)
        {
            if (itemId <= 0)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("/Item?ItemId=");
            builder.Append(itemId);

            return builder.ToString();
        }

        public static string GetDeskTopHomePageUrl()
        {
            return ConfigurationManager.AppSettings["ECWebDeskDomain"];
        }

        public static string GetShoppingCartUrl()
        {
            if (ECWeb_Mobile.Auth.NEUser.ID == 0)
            {
                return "/MyAccount/Login";
            }
            else
            {
                return ConfigurationManager.AppSettings["ECWebDeskDomain"] + "/Cart?typeid=1";
            }
        }

        public static string GetShoppingCartUsaUrl()
        {
            if (ECWeb_Mobile.Auth.NEUser.ID == 0)
            {
                return "/MyAccount/Login";
            }
            else
            {
                return ConfigurationManager.AppSettings["ECWebDeskDomain"] + "/Cart?typeid=2";
            }
        }

        public static string GetShoppingCartChooseAnyUrl()
        {
            if (ECWeb_Mobile.Auth.NEUser.ID == 0)
            {
                return "/MyAccount/Login";
            }
            else
            {
                return ConfigurationManager.AppSettings["ECWebDeskDomain"] + "/Cart?typeid=3";
            }
        }
    }
}