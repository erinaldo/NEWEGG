using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.SMSService.Service;
using System.Net;
using System.Net.Sockets;
using TWNewEgg.GetConfigData.Service;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class OTPService
    {
        TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData WebSiteData = new TWNewEgg.GetConfigData.Models.WebSiteListWebSiteData(0);

        private string GenerateNumAndWord(int length)
        {
            string randCode = string.Empty;

            Random randomSeed = new Random();
            var sequence = Enumerable.Range(0, 35).OrderBy(n => n * (new Random()).Next()).ToList();
            for (int i = 0; i < length; i++)
            {
                var sequenceCode = sequence[randomSeed.Next(0, 35)];
                if (sequenceCode > 9)
                {
                    randCode += Convert.ToChar(sequenceCode + 55);
                }
                else
                {
                    randCode += sequenceCode.ToString();
                }
            }

            return randCode;
        }
        #region OTP
        /// <summary>
        /// 判斷是否已通過簡訊驗證
        /// </summary>
        /// <param name="UserId">帳號</param>
        /// <returns>若通過簡訊驗證(Status=Success)則回傳true，反之回傳false；</returns>
        public bool IsSMSSuccess(int UserId)
        {
            bool boolsta = false;
            TWNewEgg.DB.TWSqlDBContext objDB = null;
            OTPRecord DBRecord = null;

            objDB = new DB.TWSqlDBContext();
            DBRecord = new OTPRecord();
            /* ----------------------------- */

            DBRecord = objDB.OTPRecord.Where(x => x.UserID == UserId).FirstOrDefault();

            if (DBRecord != null && DBRecord.Status == 1)
            {
                boolsta = true;
            }

            return boolsta;
        }

        /// <summary>
        /// 判斷UID是否被Block且金額大於1萬
        /// </summary>
        /// <param name="uID">帳號</param>
        /// <param name="amount">金額</param>
        /// <returns>若此UID被Block住且金額大於1萬回傳true</returns>
        public bool IsSMSBlock(int uID,int amount) 
        {
            bool excebool = false;
            TWNewEgg.DB.TWSqlDBContext odb = null;
            OTPRecord otprecord = null;
            DateTime datetimeNow = new DateTime();
            odb = new DB.TWSqlDBContext();
            otprecord=odb.OTPRecord.Where(x=>x.UserID==uID).FirstOrDefault();
            if(otprecord!=null)
            {
            
                datetimeNow = DateTime.Now;
                //判斷是否jo6BLOCK且已經超過Block時間，若已超過把Status改成Null，並把FailCount歸零
                if(datetimeNow > otprecord.StatusDate.AddDays(1) && otprecord.Status == (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block)
                {
                    otprecord.Status = (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Null;
                    otprecord.FailCount = 0;
                    otprecord.StatusDate = datetimeNow;
                    odb.SaveChanges();
                }
                //判斷是否為Block且金額超過1萬元
                if(otprecord.Status == (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block&&amount>=10000)
                {
                    excebool=true;
                }
            }
            
            return excebool;

        }
        public string GenerateSMSCode(int UID, decimal amount, string cellPhone, string itemIDs)
        {
            TWNewEgg.DB.TWSqlDBContext odb = new DB.TWSqlDBContext();
            //bool flag = false;
            DateTime timeNow = new DateTime();
            string pwd = string.Empty;
            pwd = GenerateNumAndWord(6);
            timeNow = DateTime.UtcNow.AddHours(8);
            var userOTP = odb.OTPRecord.Where(x => x.UserID == UID).FirstOrDefault();
            if (userOTP == null)
            {
                userOTP = new OTPRecord();
                userOTP.UserID = UID;
                userOTP.Items = itemIDs;
                userOTP.CartID = "";
                userOTP.CreateDate = timeNow;
                userOTP.Amount = int.Parse(amount.ToString());
                userOTP.Phone = cellPhone;
                userOTP.Status = (int)OTPRecord.OPTStatus.Null;
                userOTP.StatusDate = timeNow;
                userOTP.FailCount = 0;
                userOTP.Password = pwd;
                odb.OTPRecord.Add(userOTP);
            }
            else
            {
                userOTP.Items = itemIDs;
                userOTP.CartID = "";
                userOTP.Amount = int.Parse(amount.ToString());
                userOTP.Phone = cellPhone;
                userOTP.Status = (int)OTPRecord.OPTStatus.Null;
                userOTP.StatusDate = timeNow;
                userOTP.FailCount = 0;
                userOTP.Password = pwd;
            }
            try
            {
                odb.SaveChanges();
            }
            catch
            {
                pwd = "";
            }
            return pwd;
            //return flag;
        }

        public string ReGenerateSMSCode(int UID)
        {
            TWNewEgg.DB.TWSqlDBContext odb = new DB.TWSqlDBContext();
            //bool flag = false;
            DateTime timeNow = new DateTime();
            string pwd = string.Empty;
            pwd = GenerateNumAndWord(6);
            timeNow = DateTime.Now;
            var userOTP = odb.OTPRecord.Where(x => x.UserID == UID).FirstOrDefault();
            if (userOTP == null)
            {
                pwd = "";
            }
            else
            {
                userOTP.Status = (int)OTPRecord.OPTStatus.Null;
                userOTP.StatusDate = timeNow;
                userOTP.FailCount = 0;
                userOTP.Password = pwd;
            }
            try
            {
                odb.SaveChanges();
            }
            catch
            {
                pwd = "";
            }
            return pwd;
            //return flag;
        }
        public bool SendSMS(string cellPhoneNo, string pwd)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            bool flag = false;
            HiNetSolution SendSMS = new HiNetSolution();
            TWNewEgg.DB.TWSqlDBContext odb = new DB.TWSqlDBContext();
            var otpRecord = odb.OTPRecord.Where(x => x.Phone == cellPhoneNo).FirstOrDefault();
            if (otpRecord == null)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                return flag;
            }
            var sendMessage = String.Format(WebSiteData.SiteName + "客服中心通知，為確保您的權益及防止他人冒用，發送簡訊驗證碼：{0}（密碼不分大小寫），請立即在10分鐘內完成驗證，若有問題歡迎來電洽詢：" + WebSiteData.PhoneNumber2 + "。", pwd);

            var response = SendSMS.EasySendSMSMessage(cellPhoneNo, sendMessage);
            if (response.FirstOrDefault() != null)
            {
                otpRecord.SMSReturnID = response.FirstOrDefault().smsID;
                try
                {
                    odb.SaveChanges();
                    flag = true;
                }
                catch
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            return flag;
        }
        #endregion OTP
        #region Project11500 Promotions for new accounts
        /// <summary>
        /// 判斷新使用者是否已通過簡訊驗證
        /// </summary>
        /// <param name="UserId">帳號</param>
        /// <returns>若通過簡訊驗證(Status=Success)則回傳true，反之回傳false；</returns>
        public bool IsSMSSuccessForRegister(string cellPhone)
        {
            bool boolsta = false;
            TWNewEgg.DB.TWSqlDBContext objDB = new DB.TWSqlDBContext();
            Accountactcheck DBRecord = new Accountactcheck();

            /* ----------------------------- */

            DBRecord = objDB.Accountactcheck.Where(x => x.Phone == cellPhone).FirstOrDefault();

            if (DBRecord != null && DBRecord.Status == (int)Accountactcheck.AccountactcheckStatus.Success)
            {
                boolsta = true;
            }

            return boolsta;
        }

        /// <summary>
        /// 判斷UID是否被Block且金額大於1萬
        /// </summary>
        /// <param name="uID">帳號</param>
        /// <param name="amount">金額</param>
        /// <returns>若此UID被Block住且金額大於1萬回傳true</returns>
        public bool IsSMSBlockForRegister(string cellPhone)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate code for register user and save into db talbe
        /// </summary>
        /// <param name="cellPhone"></param>
        /// <returns></returns>
        public string GenerateSMSCodeForRegister(string cellPhone)
        {
            TWNewEgg.DB.TWSqlDBContext odb = new DB.TWSqlDBContext();
            //bool flag = false;
            DateTime timeNow = new DateTime();
            string pwd = string.Empty;
            pwd = GenerateNumAndWord(6);
            timeNow = DateTime.Now;
            var userRegisterRecord = odb.Accountactcheck.Where(x => x.Phone == cellPhone).FirstOrDefault();
            if (userRegisterRecord == null)
            {
                userRegisterRecord = new Accountactcheck();
                userRegisterRecord.User_id = "";
                userRegisterRecord.CrearDate = timeNow;
                userRegisterRecord.Phone = cellPhone;
                userRegisterRecord.Status = (int)OTPRecord.OPTStatus.Null;
                userRegisterRecord.StatusDate = timeNow;
                userRegisterRecord.FailCount = 0;
                userRegisterRecord.Authenticate = pwd;
                odb.Accountactcheck.Add(userRegisterRecord);
            }
            else
            {
                userRegisterRecord.User_id = "";
                userRegisterRecord.Phone = cellPhone;
                userRegisterRecord.Status = (int)OTPRecord.OPTStatus.Null;
                userRegisterRecord.StatusDate = timeNow;
                userRegisterRecord.FailCount = 0;
                userRegisterRecord.Authenticate = pwd;
            }
            try
            {
                odb.SaveChanges();
            }
            catch
            {
                pwd = "";
            }
            return pwd;
            //return flag;
        }

        /// <summary>
        /// Regenerate Code for register user and save into db talbe
        /// </summary>
        /// <param name="Phone"></param>
        /// <returns></returns>
        public string ReGenerateSMSCodeForRegister(string Phone)
        {
            TWNewEgg.DB.TWSqlDBContext odb = new DB.TWSqlDBContext();
            //bool flag = false;
            DateTime timeNow = new DateTime();
            string pwd = string.Empty;
            pwd = GenerateNumAndWord(6);
            timeNow = DateTime.Now;
            var userRegisterRecord = odb.Accountactcheck.Where(x => x.Phone == Phone).FirstOrDefault();
            if (userRegisterRecord == null)
            {
                pwd = "";
            }
            else
            {
                userRegisterRecord.Status = (int)OTPRecord.OPTStatus.Null;
                userRegisterRecord.StatusDate = timeNow;
                userRegisterRecord.FailCount = 0;
                userRegisterRecord.Authenticate = pwd;
            }
            try
            {
                odb.SaveChanges();
            }
            catch
            {
                pwd = "";
            }
            return pwd;
            //return flag;
        }
        /// <summary>
        /// Send SMS for new register user
        /// </summary>
        /// <param name="cellPhoneNo">傳送新的簡訊的手機號碼</param>
        /// <param name="pwd">驗證碼</param>
        /// <returns></returns>
        public bool SendSMSForRegister(string cellPhoneNo, string pwd)
        {
            bool flag = false;
            HiNetSolution SendSMS = new HiNetSolution();
            TWNewEgg.DB.TWSqlDBContext odb = new DB.TWSqlDBContext();
            var registerRecord = odb.Accountactcheck.Where(x => x.Phone == cellPhoneNo).FirstOrDefault();
            if (registerRecord == null)
            {
                return flag;
            }
            //傳送簡訊的內容
            var sendMessage = String.Format(WebSiteData.Contact1 + "通知，為確保您的權益及防止他人冒用，發送簡訊驗證碼：{0}(密碼不分大小寫)，請立即在10分鐘內完成驗證，若有問題歡迎來電洽詢：" + WebSiteData.PhoneNumber2 + "。", pwd);
            //傳送簡訊
            var response = SendSMS.EasySendSMSMessage(cellPhoneNo, sendMessage);
            if (response.FirstOrDefault() != null)
            {
                registerRecord.SMSReturnID = response.FirstOrDefault().smsID;
                try
                {
                    odb.SaveChanges();
                    flag = true;
                }
                catch
                {
                    flag = false;
                }
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        #endregion Project11500 Promotions for new accounts
    }
}