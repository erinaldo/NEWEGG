using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TWNewEgg.Framework.ServiceApi.Configuration;

namespace TWNewEgg.CookiesUtilities
{
    public class CookiesUtility
    {
        public static void CreateCookie(string mainKey, string value, string path, string domain, DateTime? expires)
        {
            if (string.IsNullOrEmpty(mainKey) || string.IsNullOrEmpty(value))
            {
                return;
            }
            HttpCookie mainCookie = new HttpCookie(mainKey);

            mainCookie.Value = HttpUtility.UrlEncode(value);

            mainCookie.Expires = expires ?? ConfigurationManager.GetTaiwanTime().AddDays(1);
            if (!string.IsNullOrEmpty(path))
            {
                mainCookie.Path = path;
            }
            if (!string.IsNullOrEmpty(domain))
            {
                mainCookie.Domain = domain;
            }
            HttpContext.Current.Response.Cookies.Add(mainCookie);
        }

        public static void CreateSubCookie(string mainKey, Dictionary<string, string> subCookieKeysValues, string path, string domain, DateTime? expires)
        {
            if (string.IsNullOrEmpty(mainKey) || subCookieKeysValues == null)
            {
                return;
            }
            HttpCookie mainCookie = new HttpCookie(mainKey);
            foreach (var singleKeyValue in subCookieKeysValues)
            {
                mainCookie.Values[singleKeyValue.Key] = HttpUtility.UrlEncode(singleKeyValue.Value);
            }
            mainCookie.Expires = expires ?? ConfigurationManager.GetTaiwanTime().AddDays(1);
            if (!string.IsNullOrEmpty(path))
            {
                mainCookie.Path = path;
            }
            if (!string.IsNullOrEmpty(domain))
            {
                mainCookie.Domain = domain;
            }
            HttpContext.Current.Response.Cookies.Add(mainCookie);
        }

        //public static void UpdateCookies(string mainKey, Dictionary<string, string> subCookieKeysValues, string path, string domain, DateTime? expires)
        //{
        //    if (string.IsNullOrEmpty(mainKey))
        //    {
        //        return;
        //    }
        //    CreateCookie(mainKey, subCookieKeysValues, path, domain, expires);
        //}

        public static string ReadCookies(string mainKey)
        {
            if (string.IsNullOrEmpty(mainKey))
            {
                return null;
            }
            if (HttpContext.Current.Request.Cookies[mainKey] == null)
            {
                return null;
            }

            HttpCookie aCookie = HttpContext.Current.Request.Cookies[mainKey];
            return HttpUtility.UrlDecode(aCookie.Value);
        }

        public static Dictionary<string, string> ReadSubCookies(string mainKey)
        {
            Dictionary<string, string> subCookies = null;
            if (string.IsNullOrEmpty(mainKey))
            {
                return null;
            }
            if (HttpContext.Current.Request.Cookies[mainKey] == null)
            {
                return null;
            }

            HttpCookie aCookie = HttpContext.Current.Request.Cookies[mainKey];
            if (aCookie.HasKeys)
            {
                subCookies = new Dictionary<string, string>();
                System.Collections.Specialized.NameValueCollection subCookieValues = aCookie.Values;
                string[] subCookieValueNames = subCookieValues.AllKeys;
                for (int j = 0; j < subCookieValues.Count; j++)
                {
                    var subkeyName = subCookieValueNames[j];
                    var subkeyValue = HttpUtility.UrlDecode(subCookieValues[j]);
                    subCookies.Add(subkeyName, subkeyValue);
                }
            }
            return subCookies;
        }

        public static void DeleteSubCookies(string mainKey, List<string> subKeys, string domain, DateTime? expires)
        {
            if (string.IsNullOrEmpty(mainKey) || subKeys == null)
            {
                return;
            }
            if (HttpContext.Current.Request.Cookies[mainKey] == null)
            {
                return;
            }
            HttpCookie aCookie = HttpContext.Current.Request.Cookies[mainKey];
            foreach (var singleSubKey in subKeys)
            {
                aCookie.Values.Remove(singleSubKey);
            }
            if (!string.IsNullOrEmpty(domain))
            {
                aCookie.Domain = domain;
            }
            aCookie.Expires = expires ?? ConfigurationManager.GetTaiwanTime().AddDays(1);
            HttpContext.Current.Response.Cookies.Add(aCookie);
        }

        public static void RemoveMainCookie(string mainKey, string domain)
        {
            if (string.IsNullOrEmpty(mainKey))
            {
                return;
            }
            if (HttpContext.Current.Request.Cookies[mainKey] == null)
            {
                return;
            }
            //HttpCookie aCookie = HttpContext.Current.Response.Cookies[mainKey];
            //aCookie.Expires = ConfigurationManager.GetTaiwanTime().AddDays(-1);
            //HttpContext.Current.Response.Cookies.Add(aCookie);
            if (!string.IsNullOrEmpty(domain))
            {
                HttpContext.Current.Response.Cookies[mainKey].Domain = domain;
            }
            HttpContext.Current.Response.Cookies[mainKey].Expires = ConfigurationManager.GetTaiwanTime().AddDays(-1);
        }
    }
}
