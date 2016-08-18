using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.StorageServices.Interface;
using System.Net.Cache;
using TWNewEgg.PurgeQueueAdapters.Interface;

namespace TWNewEgg.StorageServices
{
    public class AzureCDNAdapter : ICDNAdapter
    {
         private IPurgeQueueAdapters _iPurgeQueueAdapters;

         public AzureCDNAdapter(IPurgeQueueAdapters iPurgeQueueAdapters)
        {
            this._iPurgeQueueAdapters = iPurgeQueueAdapters;
        }

        //您的AAD Domain名稱，可以從舊版Azure Portal查詢
        private string AADTenantDomain 
            = System.Configuration.ConfigurationManager.AppSettings["AADTenantDomain"].ToString();
        
        //將您的應用程式加入到Azure AD後，取得到的用戶端識別碼
        private string AADClientId
            = System.Configuration.ConfigurationManager.AppSettings["AADClientId"].ToString();
        
        //可以管理CDN的AAD的帳戶名稱
        private string AADServiceAccountUserName
            = System.Configuration.ConfigurationManager.AppSettings["AADServiceAccountUserName"].ToString();
        
        //可以管理CDN的AAD的帳戶密碼
        private string AADServiceAccountPassword
            = System.Configuration.ConfigurationManager.AppSettings["AADServiceAccountPassword"].ToString();
        
        //Azure Subscription ID
        private string AADSubscriptionId
            = System.Configuration.ConfigurationManager.AppSettings["AADSubscriptionId"].ToString();
        
        //CDN與儲存體所在的Resource Group Name
        private string AADResourceGroupsName
            = System.Configuration.ConfigurationManager.AppSettings["AADResourceGroupsName"].ToString();
        
        //CDN設定檔名稱
        private string AADCDNProfileName
            = System.Configuration.ConfigurationManager.AppSettings["AADCDNProfileName"].ToString();
        
        //設定在CDN裡的Endpoint 名稱
        private string AADCDNEndpointsName
            = System.Configuration.ConfigurationManager.AppSettings["AADCDNEndpointsName"].ToString();
        
        //Aoken Result
        private static AuthenticationResult authResult = null;

        private static RequestCachePolicy policy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

        public void Purge(List<string> purgeFiles)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest _purgeRequest = new TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest();
            _purgeRequest = this._iPurgeQueueAdapters.readPurgeRequest().OrderByDescending(x => x.ID).Take(1).FirstOrDefault();
            if (_purgeRequest.Status == 0)
            {
                AskRequestResult(_purgeRequest.AsyncCheckStateURL);
            }
            else 
            {
                PurgeToFile(purgeFiles);
            }
        }
        
        private void PurgeToFile(List<string> purgeFiles) 
        {
            TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest _purgeRequest = new TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest();
            _purgeRequest = this._iPurgeQueueAdapters.readPurgeRequest().OrderByDescending(x => x.ID).Take(1).FirstOrDefault();
            _purgeRequest.TryTimes += 1;
            _purgeRequest.CDNEndPointName = AADCDNEndpointsName;
            _purgeRequest.UpdateDate = DateTime.Now;
            this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);

            //取得Azure Service 的Acess Token (無此Token無法操作Azure Rest API)
            string auth = GetAccessToken();

            //組合Azure CDN服務網址, 參考https://msdn.microsoft.com/library/mt634451.aspx
            string stringURI = string.Format("https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Cdn/Profiles/{2}/endpoints/{3}/purge?api-version=2015-06-01"
                 , AADSubscriptionId, AADResourceGroupsName, AADCDNProfileName, AADCDNEndpointsName);

            //建立Post Request
            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(stringURI);
            endpointRequest.Method = "POST";
            endpointRequest.ContentType = "application/json";
            endpointRequest.Headers["Authorization"] = "Bearer " + auth;

            RootObject root = new RootObject();
            root.ContentPaths = new List<string>();
            foreach (string _path in purgeFiles)
            {
                root.ContentPaths.Add(_path);
            }

            string postData = Newtonsoft.Json.JsonConvert.SerializeObject(root);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            endpointRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = endpointRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            SendRequest(endpointRequest);
        
        }
        private void AskRequestResult(string AsyncCheckStateURL)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest _purgeRequest = new TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest();
            _purgeRequest = this._iPurgeQueueAdapters.readPurgeRequest().OrderByDescending(x => x.ID).Take(1).FirstOrDefault();
            _purgeRequest.UpdateDate = DateTime.Now;
            try
            {
                //取得Request結果
                HttpStatusCode sc = GetPurgeResponse(AsyncCheckStateURL);
                if (sc == HttpStatusCode.Accepted) // 判斷是否為 202
                    {
                        Console.WriteLine("HttpStatusCode.Accepted");
                        _purgeRequest.StatusCode = sc.ToString();
                        this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                    }
                    else
                    {
                        Console.WriteLine("HttpStatusCode: " + sc);
                        _purgeRequest.StatusCode = sc.ToString();
                        this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                    }
                
            }
            catch (WebException WebEx)
            {
                var resp = WebEx.Response as HttpWebResponse;
                _purgeRequest.StatusCode = (resp.StatusCode).ToString();
                _purgeRequest.ErrorMessage = resp.StatusDescription;
                this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                Console.WriteLine("WebException  HttpStatusCode: " + ((int)resp.StatusCode).ToString() + "info:" + resp.StatusDescription); 
            }
            catch (Exception ex)
            {
                _purgeRequest.ErrorMessage = ex.ToString();
                this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        private void SendRequest(HttpWebRequest endpointRequest)
        {
            TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest _purgeRequest = new TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest();
            _purgeRequest = this._iPurgeQueueAdapters.readPurgeRequest().OrderByDescending(x => x.ID).Take(1).FirstOrDefault();

            try
            {
                //取得Request結果
                using (HttpWebResponse response = endpointRequest.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.Accepted) // 判斷是否為 202
                    {
                        Console.WriteLine("HttpStatusCode.Accepted");
                        string AzureAsyncLocation = response.Headers["Location"];
                        if (string.IsNullOrEmpty(AzureAsyncLocation))
                        {
                            return;
                        }
                        _purgeRequest.AsyncCheckStateURL = AzureAsyncLocation;
                        _purgeRequest.StatusCode = response.StatusCode.ToString();
                        _purgeRequest.ErrorMessage = "";
                        _purgeRequest.Status = 0;
                        this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                    }
                    else
                    {
                        Console.WriteLine("HttpStatusCode: " + response.StatusCode);
                        _purgeRequest.StatusCode = response.StatusCode.ToString();
                        this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
                    }
                }
            }
            catch (WebException WebEx)
            {
                var resp = WebEx.Response as HttpWebResponse;
                Console.WriteLine("WebException  HttpStatusCode: " + ((int)resp.StatusCode).ToString() + "info:" + resp.StatusDescription);
                _purgeRequest.StatusCode = (resp.StatusCode).ToString();
                _purgeRequest.ErrorMessage = resp.StatusDescription;
                this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                _purgeRequest.ErrorMessage = ex.ToString();
                this._iPurgeQueueAdapters.updatePurgeRequest(_purgeRequest);
            }
        }

        private  HttpStatusCode GetPurgeResponse(string AzureAsyncOperation)
        {
            string auth = GetAccessToken();
            //發送Post Request
            HttpWebRequest OperationRequest = (HttpWebRequest)HttpWebRequest.Create(AzureAsyncOperation);
            OperationRequest.Method = "Get";
            OperationRequest.ContentType = "application/json";
            //帶入Access Token
            OperationRequest.Headers["Authorization"] = "Bearer " + auth;
            OperationRequest.CachePolicy = policy;
            try
            {
                using (HttpWebResponse Operationresponse = OperationRequest.GetResponse() as HttpWebResponse)
                {
                    Console.WriteLine("Purge Results Time: {0}, {1}", DateTime.Now, Operationresponse.StatusCode.ToString());
                    return Operationresponse.StatusCode;
                }
            }
            catch (WebException WebEx)
            {
                var resp = WebEx.Response as HttpWebResponse;
                Console.WriteLine("GetPurgeResponse WebException  HttpStatusCode: " + ((int)resp.StatusCode).ToString() + "info:" + resp.StatusDescription);
                return resp.StatusCode;

            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPurgeResponse Exception: " + ex.ToString());
                return HttpStatusCode.InternalServerError;
            }
        }
        private string GetAccessToken()
        {

            AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + AADTenantDomain);

            if (authResult == null)
            {
                //若尚未取得過Access Token將進行以下動作

                //將string Password 轉為 SecureString
                SecureString SecurePassword = new SecureString();
                if (AADServiceAccountPassword == null)
                    throw new ArgumentNullException("password");
                foreach (char c in AADServiceAccountPassword)
                    SecurePassword.AppendChar(c);
                SecurePassword.MakeReadOnly();

                //將帳密轉為UserCredential
                UserCredential credential = new UserCredential(AADServiceAccountUserName, SecurePassword);
                //透過UserCredential來取得Access Token
                //可參考: https://msdn.microsoft.com/zh-tw/library/mt473613.aspx
                authResult = context.AcquireToken("https://management.core.windows.net/", AADClientId, credential);
                if (authResult == null)
                {
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                }
            }
            else if (authResult.ExpiresOn.UtcDateTime <= DateTime.UtcNow)
            {
                //若取得過authResult，但原本的Access Token過期時(一小時)，可以透過以下API進行RefreshToken的動作
                authResult = context.AcquireTokenByRefreshToken(authResult.RefreshToken, AADClientId);
            }
            //若曾取得過authResult，將直接回傳Token
            return authResult.AccessToken;
        }
    }

    public class RootObject
    {
        [Newtonsoft.Json.JsonProperty("ContentPaths")]
        public List<string> ContentPaths { get; set; }
    }
}
