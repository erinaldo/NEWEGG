using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CookiesUtilities;
using TWNewEgg.Framework.Common;
using TWNewEgg.Framework.Common.Cryptography;

namespace TWNewEgg.ECWeb.PrivilegeFilters.Core
{
    public class EncapsulationAuthCookies : IEncapsulationAuthCookies
    {
        public const string AUTHMAINCOOKIEKEY = "neui";
        public const string AUTHSUBCOOKIEKEY_ACCOUNTINFO = "ai";
        public const string AUTHSUBCOOKIEKEY_ACCOUNTMAIL = "mail";
        public const string FAILED = "FAILED.";
        public string Encapsulate(Models.AccountInfo source, string path, string domain, DateTime? expires)
        {
            string encapCode = JSONSerialization.Serializer(source);
            string randIV = RandomUtility.GenerateNumAndWord(3);
            string encToken = AESCryptography.AESEnprypt(encapCode, ivString: randIV);
            encapCode = string.Format("{0}{1}", randIV, encToken);
            Dictionary<string, string> subCookies = new Dictionary<string, string>();
            subCookies.Add(AUTHSUBCOOKIEKEY_ACCOUNTINFO, encapCode);
            subCookies.Add(AUTHSUBCOOKIEKEY_ACCOUNTMAIL, source.Email);
            CookiesUtility.CreateSubCookie(AUTHMAINCOOKIEKEY, subCookies, null, domain, expires);
            return encapCode;
        }

        public string DeEncapsulate()
        {
            Dictionary<string, string> value = CookiesUtility.ReadSubCookies(AUTHMAINCOOKIEKEY);
            if (value == null || !value.ContainsKey(AUTHSUBCOOKIEKEY_ACCOUNTINFO))
            {
                return null;
            }
            if (value[AUTHSUBCOOKIEKEY_ACCOUNTINFO].Length < 4)
            {
                return null;
            }
            string createIV = value[AUTHSUBCOOKIEKEY_ACCOUNTINFO].Substring(0, 3);
            string encTok = value[AUTHSUBCOOKIEKEY_ACCOUNTINFO].Substring(3);
            string decString = AESCryptography.AESDecrypt(encTok, ivString: createIV);
            return decString;
            //return JSONSerialization.Deserializer<Models.AccountInfo>(decString);
        }


        public void ClearAll(string domain)
        {
            CookiesUtility.RemoveMainCookie(AUTHMAINCOOKIEKEY, domain);
        }
    }
}
