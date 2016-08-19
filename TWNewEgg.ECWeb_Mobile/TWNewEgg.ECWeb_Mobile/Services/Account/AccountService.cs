using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Models.ViewModels.Login;
using TWNewEgg.Models.ViewModels.Account;
using TWNewEgg.ECWeb.PrivilegeFilters.Core;
using TWNewEgg.ECWeb.PrivilegeFilters.Models;
using TWNewEgg.Models.ViewModels.Register;
using TWNewEgg.Framework.HttpMethod;
using TWNewEgg.Framework.Common.Cryptography;
using TWNewEgg.Framework.ServiceApi.Configuration;
using System.Text.RegularExpressions;
using TWNewEgg.CookiesUtilities;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.ECWeb.PrivilegeFilters;

namespace TWNewEgg.ECWeb_Mobile.Services.Account
{
    public class AccountService
    {
        public const string WHOLESCOPE = "WHOLE_ACTIONS";
        private readonly string forgetKey = "IFORGOTIT";
        private readonly string forgetiv = "WHATSHAPPEN";
        public readonly string head = "head";
        public readonly string body = "body";
        public readonly string foot = "foot";
        IAccountAuth _authService;

        public AccountService(string type)
        {
            AccountAuthFactory factory = new AccountAuthFactory();
            _authService = factory.SwitchAuthService(type);
        }
        /// <summary>
        /// Check user trying login times.
        /// </summary>
        /// <param name="randCode"></param>
        /// <returns></returns>
        public bool IsTryManyTime(string[] randCode)
        {
            if (randCode.Count() < 2)
            {
                return true;
            }
            int tryCount = new int();
            if (!int.TryParse(randCode[0], out tryCount) || tryCount > 2)
            {
                return true;
            }
            DateTime datetime = new DateTime();
            if (!DateTime.TryParse(randCode[1], out datetime) || DateTime.Compare(datetime, ConfigurationManager.GetTaiwanTime().AddMinutes(-5)) < 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Decrypt random string.
        /// </summary>
        /// <param name="randCode"></param>
        /// <returns></returns>
        public string[] DecryptRandString(string randCode)
        {
            string[] content = AESCryptography.AESDecrypt(randCode).Split('~');
            if (content.Length < 3)
            {
                return new string[] { "" };
            }
            int tryCount = new int();
            if (!int.TryParse(content[0], out tryCount))
            {
                return new string[] { "" };
            }
            DateTime datetime = new DateTime();
            if (!DateTime.TryParse(content[1], out datetime))
            {
                return new string[] { "" };
            }
            return content;
        }

        /// <summary>
        /// Generate user login times.
        /// </summary>
        /// <param name="randTime"></param>
        /// <returns></returns>
        public string GenerateTryTime(string randTime)
        {
            return AESCryptography.AESEnprypt(randTime);
        }

        /// <summary>
        /// Generate user forget password link.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="datetiemString"></param>
        /// <returns></returns>
        public string GenerateResetPasswordLink(string email, string datetiemString)
        {
            Dictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add(head, datetiemString);
            obj.Add(body, email);
            obj.Add(foot, datetiemString);
            string json = TWNewEgg.Framework.Common.JSONSerialization.Serializer(obj);
            return AESCryptography.AESEnprypt(json, forgetKey, forgetiv);
        }
        /// <summary>
        /// Decode user forget password link.
        /// </summary>
        /// <param name="idontknow"></param>
        /// <returns>email</returns>
        public Dictionary<string, string> DecodeResetPasswordLink(string idontknow)
        {
            string json = AESCryptography.AESDecrypt(idontknow, forgetKey, forgetiv);
            Dictionary<string, string> obj = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<Dictionary<string, string>>(json);
            if (obj != null && obj.ContainsKey(body) && obj.ContainsKey(head))
            {
                return obj;
            }
            return new Dictionary<string, string>();
        }

        public bool CheckGoogleReCaptcha(string googleCaptcha)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("secret", System.Configuration.ConfigurationManager.AppSettings["GOOGLE_SECRET_KEY"] ?? "");
            parameters.Add("response", googleCaptcha ?? "");

            //Web.Config must add 
            //<system.net>
            //  <defaultProxy enabled="true" useDefaultCredentials="true">
            //    <!-- stproxy -->
            //    <!-- s1firewall -->
            //    <!-- <proxy proxyaddress="http://s1firewall:8080" bypassonlocal="True" usesystemdefault="True" /> -->
            //  </defaultProxy>
            //</system.net>
            var result = HttpClientMethod.Get("https://www.google.com/recaptcha/api/siteverify", string.Format("?secret={0}&response={1}", parameters["secret"], parameters["response"]), null);

            if (!this.CheckGooglereCaptChaMessage(result))
            {
                return false;
            }
            return true;
        }

        private bool CheckGooglereCaptChaMessage(string response)
        {
            bool flag = false;
            string googlereCaptcha = string.Empty;
            var googlereCaptchaString = System.Configuration.ConfigurationManager.AppSettings["googlereCaptcha"];
            if (string.IsNullOrEmpty(googlereCaptchaString))
            {
                googlereCaptcha = "";
            }
            else
            {
                googlereCaptcha = string.Join("; ", googlereCaptchaString.Split(';').Select(x => "\"" + x.Split(',')[0] + "\": " + x.Split(',')[1]).ToArray());
            }
            if (response.Contains(googlereCaptcha))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            return flag;
        }

        /// <summary>
        /// Check Login status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public AccountVM CheckLogin(Login model)
        {
            if (!CorrectEmailFormat(model.user) || string.IsNullOrEmpty(model.pass))
            {
                return null;
            }

            AccountVM result = _authService.CheckAuth(model);

            if (result == null)
            {
                return null;
            }

            WriteToCookies(result, model.remb);

            return result;
        }

        /// <summary>
        /// Check Login status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Logout()
        {
            ClearToCookies(EncapsulationAuthCookies.AUTHMAINCOOKIEKEY, System.Configuration.ConfigurationManager.AppSettings["ECWebDomain"]);
            ClearToCookies("sc", System.Configuration.ConfigurationManager.AppSettings["ECWebDomain"]);
            return true;
        }

        private void WriteToCookies(AccountVM result, string rememberMe)
        {
            IEncapsulationAuthCookies ecapCookies = new EncapsulationAuthCookies();
            Dictionary<string, string> subCookies = new Dictionary<string, string>();
            AccountInfo loginStatus = new AccountInfo();
            loginStatus.ID = result.ID;
            loginStatus.Name = result.Name;
            loginStatus.Email = result.Email;
            loginStatus.Loginon = result.Loginon.Value;
            loginStatus.NickName = result.Nickname;
            loginStatus.Scopes = WHOLESCOPE;
            loginStatus.IPAddress = NetPacketUtilities.GetUserIPAddress();
            loginStatus.Browser = NetPacketUtilities.GetUserBrowser();

            DateTime expires = ConfigurationManager.GetTaiwanTime().AddDays(1);
            if (!string.IsNullOrEmpty(rememberMe))
            {
                expires = ConfigurationManager.GetTaiwanTime().AddYears(1);
            }

            ecapCookies.Encapsulate(loginStatus, null, System.Configuration.ConfigurationManager.AppSettings["ECWebDomain"], expires);
            string authValue = TWNewEgg.Framework.Common.JSONSerialization.Serializer(loginStatus);
            HttpContext.Current.User = new CustomPrincipal(authValue);
        }

        private void ClearToCookies(string cookiesKey, string domain)
        {
            if (!string.IsNullOrEmpty(cookiesKey))
            {
                CookiesUtility.RemoveMainCookie(cookiesKey, domain);
            }
        }

        /// <summary>
        /// Check Register status.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public RegistrationError CheckRegister(RegisterVM RegisterVM)
        {
            //錯誤訊息Model
            RegistrationError result = new RegistrationError();

            //Account Email 檢查
            if (!CorrectEmailFormat(RegisterVM.Email)) {
                result.error = true;
                result.account.error = true;
                result.account.accountFormat = true;
                result.account.errormessage = "帳號格式錯誤\n";   
                result.errormessage = result.errormessage + "帳號格式錯誤\n";   
            }
            string IsEmailExisted = Processor.Request<string, string>("AccountService", "EmailExisted", RegisterVM.Email).results;
            if (IsEmailExisted != "false")
            {
                result.error = true;
                result.account.error = true;
                result.account.accountExisted = true;
                result.account.errormessage = "Email已存在\n";
                result.errormessage = result.errormessage + "Email已存在\n";
            }
            if (RegisterVM.PWD != RegisterVM.confirmPWD)
            {
                result.error = true;
                result.confirmpassword = true;
                result.errormessage = result.errormessage + "確認密碼與密碼不符\n";               
            }

            //密碼檢查
            PasswordError PasswordError = _authService.CheckPassword(RegisterVM);
            if (PasswordError != null)
            {
                if (PasswordError.error)
                {
                    result.error = true;
                    result.passwordError = PasswordError;
                    result.password = true;
                    result.errormessage = result.errormessage + "密碼有誤\n";
                }
            }
            else {
                result.password = true;
                result.errormessage = result.errormessage + "密碼有誤\n";
            }

            //我已閱讀完畢，並同意會員條款(需勾選才能加入會員)
            if (!_authService.CheckAgreePaper(RegisterVM))
            {
                result.error = true;
                result.agreePaper = true;
                result.errormessage = result.errormessage + "未同意會員條款\n";
            }

            //判斷日期型態
            if (RegisterVM.Birthday != null)
            {
                DateTime defaultDate = DateTime.Parse("1900/01/01");
                if (RegisterVM.Birthday < defaultDate || RegisterVM.Birthday > DateTime.Today)
                {
                    result.error = true;
                    result.birthday = true;
                    result.errormessage = result.errormessage + "日期輸入錯誤\n";
                }                
            }

            if (result == null)
            {
                result.error = true;
                result.birthday = true;
                result.errormessage = result.errormessage + "資料錯誤\n";
            }

            if (result.error == false)
            { 
            //新增到資料庫
            }

            return result;
        }

        /// <summary>
        /// Get Activity now.
        /// </summary>
        /// <returns></returns>
        public string GetActivityNow()
        {
            return string.Empty;
        }

        public bool CorrectEmailFormat(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            Regex regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", RegexOptions.IgnoreCase);
            Match match = regex.Match(email);
            if (!match.Success)
            {
                return false;
            }
            return true;
        }

    }
}