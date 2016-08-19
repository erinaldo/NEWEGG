using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Seller_AuthTokenRepoAdapters.Interface;
using System.Security.Cryptography;
using TWNewEgg.AuthTokenService.Interface;
using TWNewEgg.Seller_UserRepoAdapters.Interface;
using TWNewEgg.PublicApiModels;

namespace TWNewEgg.AuthTokenService
{
    public class AuthTokenService : IAuthTokenService
    {
        private string TokenTime = System.Configuration.ConfigurationManager.AppSettings["TokenTime"] == null ? "10" : System.Configuration.ConfigurationManager.AppSettings["TokenTime"].ToString();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private ISeller_AuthTokenRepoAdapters _iSeller_AuthTokenRepoAdapters;
        private ISeller_UserRepoAdapters _iSeller_UserRepoAdapters;

        public AuthTokenService(ISeller_AuthTokenRepoAdapters iSeller_AuthTokenRepoAdapters, ISeller_UserRepoAdapters iSeller_UserRepoAdapters)
        {
            this._iSeller_AuthTokenRepoAdapters = iSeller_AuthTokenRepoAdapters;
            this._iSeller_UserRepoAdapters = iSeller_UserRepoAdapters;
        }
        public TokenResponseModel getToken(string account, string fromIP)
        {
            TokenResponseModel resultModel = new TokenResponseModel();
            //記錄使用者
            logger.Info("account: " + account + ", fromIP: " + fromIP);
            if (string.IsNullOrEmpty(account) == true || string.IsNullOrEmpty(fromIP) == true)
            {
                resultModel = this._responseCodeMsg(CodeStatue.No_datas, "No Account or no IP");
                return resultModel;
            }
            try
            {
                //開始產生 token
                resultModel = this.CheckUserValid(account, fromIP);

            }
            catch (Exception ex)
            {
                resultModel = this._responseCodeMsg(CodeStatue.System_Error, "系統錯誤");
                logger.Error(ex.ToString());
            }
            return resultModel;
        }
        private TokenResponseModel CheckUserValid(string account, string fromIP)
        {
            TokenResponseModel _CheckUserValidResultModel = new TokenResponseModel();
            
            #region 檢查使用者是否存在資料庫裡
            var getUserData = this._iSeller_AuthTokenRepoAdapters.GetAll().Where(p => p.AccountIdEmail == account).FirstOrDefault();
            //檢查使用者是否存在資料庫裡
            if (getUserData == null)
            {
                _CheckUserValidResultModel = this._responseCodeMsg(CodeStatue.Account_is_invalid, "使用者 Email(Account) 錯誤");
                return _CheckUserValidResultModel;
            }
            #endregion 
            #region 檢查使用者來源IP是否為合法IP
            //檢查使用者來源IP是否為合法IP
            _CheckUserValidResultModel = this.checkIP(getUserData.StartIP, getUserData.EndIP, fromIP);
            if (_CheckUserValidResultModel.codeNumber != "00")
            {
                return _CheckUserValidResultModel;
            }
            #endregion
            //開始進行產生Token
            string SHA256TokenResult = this.SHA256(account, fromIP);
            //更新資料表的相關資料
            this.updateAuthToken(getUserData, SHA256TokenResult);
            _CheckUserValidResultModel = this._responseCodeMsg(CodeStatue.Success, "Success");
            _CheckUserValidResultModel.token = SHA256TokenResult;
            return _CheckUserValidResultModel;
        }
        
        #region 檢查IP是否合法
        /// <summary>
        /// 檢查IP是否合法
        /// </summary>
        /// <param name="startIP"></param>
        /// <param name="endIP"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private TokenResponseModel checkIP(string startIP, string endIP, string fromIP)
        {
            TokenResponseModel ipResult = new TokenResponseModel();
            long startIPAddress = this.IP2Long(startIP.Replace(" ", ""));
            long endIPAddress = this.IP2Long(endIP.Replace(" ", ""));
            long fromIPAddress = this.IP2Long(fromIP.Replace(" ", ""));
            //int startIPAddress = BitConverter.ToInt32(System.Net.IPAddress.Parse(startIP.Replace(" ", "")).GetAddressBytes(), 0);
            //int endIPAddress = BitConverter.ToInt32(System.Net.IPAddress.Parse(endIP.Replace(" ", "")).GetAddressBytes(), 0);
            //int fromIPAddress = BitConverter.ToInt32(System.Net.IPAddress.Parse(fromIP.Replace(" ", "")).GetAddressBytes(), 0);
            if (fromIPAddress >= startIPAddress && fromIPAddress <= endIPAddress)
            {
                ipResult = this._responseCodeMsg(CodeStatue.Success, "success");
            }
            else
            {
                ipResult = this._responseCodeMsg(CodeStatue.IP_is_invalid, "使用者IP 不合法");
            }
            return ipResult;
        }
        #endregion
        #region 轉換IP為數字
        /// <summary>
        /// 轉換IP為數字
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private long IP2Long(string ip)
        {
            string[] ipBytes;
            double num = 0;
            ipBytes = ip.Split('.');
            for (int i = ipBytes.Length - 1; i >= 0; i--)
            {
                num += ((int.Parse(ipBytes[i]) % 256) * Math.Pow(256, (3 - i)));
            }
            return (long)num;
        }
        #endregion
        #region SHA256 加密
        /// <summary>
        /// 利用 account and ip and time 加密
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string SHA256(string account, string ip)
        {
            string SHA256Result = string.Empty;
            string strTimeSpan = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string toSHAStr = strTimeSpan + account + ip;
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            byte[] source = Encoding.Default.GetBytes(toSHAStr);//將字串轉為Byte[]
            byte[] crypto = sha256.ComputeHash(source);//進行SHA256加密
            SHA256Result = Convert.ToBase64String(crypto);//把加密後的字串從Byte[]轉為字串
            return SHA256Result;
        }
        #endregion
        #region 更新 AuthToken
        /// <summary>
        /// 更新 AuthToken
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        private void updateAuthToken(TWNewEgg.VendorModels.DBModels.Model.Seller_AuthToken model, string token)
        {
            int tokenRangeTime = 0;
            int.TryParse(TokenTime, out tokenRangeTime);
            model.Token = token;
            model.TerminateDate = DateTime.Now.AddMinutes(tokenRangeTime);
            model.CreateDate = DateTime.Now;
            this._iSeller_AuthTokenRepoAdapters.Update(model);
        }
        #endregion
        #region 組合回傳的 model
        private TokenResponseModel _responseCodeMsg(CodeStatue _codeStatue, string _msg)
        {
            TokenResponseModel _result = new TokenResponseModel();

            
            int code = (int)_codeStatue;
            string strCode = code.ToString();
            string strMsg = _codeStatue.ToString().Replace("_", " ");
            if (strCode.Length == 1)
            {
                strCode = "0" + strCode;
            }
            _result.codeMessage = strMsg;
            _result.codeNumber = strCode;
            _result.infoMessage = _msg;
            return _result;
        }
        #endregion
    }
}
