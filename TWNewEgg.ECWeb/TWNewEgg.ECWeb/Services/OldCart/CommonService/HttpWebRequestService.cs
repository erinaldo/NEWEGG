using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Security;

namespace TWNewEgg.Website.ECWeb.Service.CommonService
{
    public class HttpWebRequestService
    {
        //private static string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
        //private static string googlereCaptcha = "{\n  \"success\": true\n}";
        //private static string googlereCaptcha = System.Configuration.ConfigurationManager.AppSettings[environment + "_googlereCaptcha"];
        //public const string GOOGLE_SECRET_KEY = "6LdBUgATAAAAAJ3dFuFpoKre2fZ99J4ScwUAL50s";
        //public static string GOOGLE_SECRET_KEY = System.Configuration.ConfigurationManager.AppSettings[environment + "_GOOGLE_SECRET_KEY"];

        private static string environment = string.Empty;
        private static string googlereCaptcha = string.Empty;
        public static string GOOGLE_SECRET_KEY = string.Empty;

        public HttpWebRequestService()
        {
            environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            var googlereCaptchaString = System.Configuration.ConfigurationManager.AppSettings[environment + "_googlereCaptcha"];
            if (string.IsNullOrEmpty(googlereCaptchaString))
            {
                googlereCaptcha = "";
            }
            else
            {
                googlereCaptcha = string.Join("; ", googlereCaptchaString.Split(';').Select(x => "\"" + x.Split(',')[0] + "\": " + x.Split(',')[1]).ToArray());
            }
            GOOGLE_SECRET_KEY = System.Configuration.ConfigurationManager.AppSettings[environment + "_GOOGLE_SECRET_KEY"];
        }

        /// <summary>
        /// 設定 HTTPS 連線時，不要理會憑證的有效性問題
        /// </summary>
        /// <param name="sender">傳出物件</param>
        /// <param name="certificate">憑證驗證</param>
        /// <param name="chain">憑證鏈</param>
        /// <param name="sslPolicyErrors">SSL規則錯誤</param>
        /// <returns>直接回傳認證</returns>
        private static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public bool CheckGooglereCaptChaMessage(string response)
        {
            bool flag = false;
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

        public string Get(string requestUrl, Dictionary<string, string> parameters)
        {
            string response = string.Empty;

            //// 設定 HTTPS 連線時，不要理會憑證的有效性問題, 做完中華電信簡訊之後, 加密模式要切回TLS
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            string parameterUrl = string.Empty;
            foreach (var singleParameter in parameters)
            {
                parameterUrl += singleParameter.Key + "=" + HttpUtility.UrlEncode(singleParameter.Value) + "&";
            }

            // 建立WebRequest物件
            var reqUrl = requestUrl + "?" + parameterUrl;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(reqUrl);

            webRequest.Method = "GET";
            // 連線中斷時間設為10秒
            webRequest.Timeout = 10000;
            // 設定URL編碼字串是以x-www-form-urlencoded方式編碼的字串
            webRequest.ContentType = "application/x-www-form-urlencoded";

            try
            {
                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    StreamReader reader = new StreamReader(webResponse.GetResponseStream());
                    response = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                response = string.Empty;
            }

            // 做完中華電信簡訊之後, 加密模式要切回TLS
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return response;
        }
        public string Post(string requestUrl, string apiPath, Dictionary<string, string> cookiesParameters, object parameters)
        {
            string responseMessage = string.Empty;
            string postData = string.Empty;
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            postData = js.Serialize(parameters);

            var baseUrl = new Uri(requestUrl);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            {
                using (var client = new HttpClient(handler) { BaseAddress = baseUrl })
                {

                    foreach (var singleCookies in cookiesParameters)
                    {
                        cookieContainer.Add(baseUrl, new Cookie(singleCookies.Key, singleCookies.Value));
                    }

                    var result = client.PostAsync(apiPath, parameters, new JsonMediaTypeFormatter()).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        responseMessage = result.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        responseMessage = result.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            return responseMessage; 
        }
    }
}