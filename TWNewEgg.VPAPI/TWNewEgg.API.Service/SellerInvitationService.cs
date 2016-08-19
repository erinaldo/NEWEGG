using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// Seller Manage Service
    /// </summary>
    public class SellerInvitationService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }
        private log4net.ILog logger;
        
        #region 移除

        /* 11/1移除部分

        /// <summary>
        /// 取得DB內的資料
        /// </summary>
        /// <returns></returns>
        ///
        public API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> getChargeList()
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> result = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();

            List<DB.TWSELLERPORTALDB.Models.Seller_Charge> chargeList = new List<DB.TWSELLERPORTALDB.Models.Seller_Charge>();

            chargeList = spdb.Seller_Charge.Where(x => x.SellerID != 0).ToList<DB.TWSELLERPORTALDB.Models.Seller_Charge>();

            if (chargeList.Count == 0)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無資料";
            }
            else
            {
                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "List資料傳回";
                result.Body = chargeList.OrderBy(x => x.InDate).ToList();
            }

            return result;
        }
        /// <summary>
        /// 取得DB內的資料，包含標準 Commission Rate
        /// </summary>
        /// <returns></returns>
        public API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> showStandardList()
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> result = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();

            List<DB.TWSELLERPORTALDB.Models.Seller_Charge> chargeList = new List<DB.TWSELLERPORTALDB.Models.Seller_Charge>();

            chargeList = spdb.Seller_Charge.Where(x => x.SellerID == 0).ToList<DB.TWSELLERPORTALDB.Models.Seller_Charge>();

            if (chargeList.Count == 0)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "查無資料";
            }
            else
            {
                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "List資料傳回";
                result.Body = chargeList.OrderBy(x => x.InDate).ToList();
            }

            return result;
        }

        public API.Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> SellerInvitationInfo(Models.SellerInvitation sellerInvitation)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            API.Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> basicInfoResult = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>();
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> chargeResult = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();
            TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo basicInfo = new DB.TWSELLERPORTALDB.Models.Seller_BasicInfo();
            //紀錄Email格式
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$");
            //判斷商家是否存在
            var checkSeller = spdb.Seller_BasicInfo.Where(x => x.SellerEmail == sellerInvitation.SellerEmail).FirstOrDefault();

            try
            {
                //判斷商家是否存在，開發先拿掉

                if (checkSeller != null)
                {
                    basicInfoResult.Code = (int)ResponseCode.Error;
                    basicInfoResult.IsSuccess = false;
                    basicInfoResult.Msg = checkSeller.SellerEmail + " 商家已存在";
                }

                else if (regex.IsMatch(sellerInvitation.SellerEmail))
                {
                    //邀請填寫
                    basicInfo.SellerEmail = sellerInvitation.SellerEmail;
                    basicInfo.SellerCountryCode = sellerInvitation.SellerCountryCode;
                    basicInfo.LanguageCode = sellerInvitation.LanguageCode;
                    basicInfo.SellerStatus = sellerInvitation.SellerStatus;
                    //必填欄位
                    basicInfo.Authority = "1";
                    basicInfo.AccountTypeCode = "S";
                    //非必填
                    basicInfo.CreateDate = DateTime.UtcNow.AddHours(8);

                    //須儲存basciInfo，才有ID能寫入seller charge
                    spdb.Seller_BasicInfo.Add(basicInfo);
                    spdb.SaveChanges();
                    sellerInvitation.SellerID = basicInfo.SellerID;

                    //將seller charge做判斷，存入資料庫
                    chargeResult = SaveSellerCharge(sellerInvitation);

                    if (chargeResult.IsSuccess)
                    {
                        basicInfoResult.Code = (int)ResponseCode.Success;
                        basicInfoResult.IsSuccess = true;
                        basicInfoResult.Msg = "商家邀請成功";
                        basicInfoResult.Body = basicInfo;
                    }
                    else
                    {
                        //回復 seller charge失敗訊息
                        basicInfoResult.Code = chargeResult.Code;
                        basicInfoResult.IsSuccess = chargeResult.IsSuccess;
                        basicInfoResult.Msg = chargeResult.Msg;
                        //seller charge判斷失敗，將為了取得 Seller ID而儲存的basciInfo砍掉
                        spdb.Seller_BasicInfo.Remove(basicInfo);
                        spdb.SaveChanges();
                    }
                }
                else if (regex.IsMatch(sellerInvitation.SellerEmail) == false)
                {
                    basicInfoResult.Code = (int)ResponseCode.Error;
                    basicInfoResult.IsSuccess = false;
                    basicInfoResult.Msg = "Email格式錯誤，商家邀請失敗";
                }
            }
            catch (Exception ex)
            {
                basicInfoResult.Code = (int)ResponseCode.Error;
                basicInfoResult.IsSuccess = false;
                basicInfoResult.Msg = "商家邀請失敗 " + ex.Message ?? "" + ((ex.InnerException != null) ? ex.InnerException.Message : "");
            }

            return basicInfoResult;
        }

        private API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> SaveSellerCharge(Models.SellerInvitation sellerInvitation)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>> result = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Charge>>();
            List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_Charge> sellerChargeList = new List<DB.TWSELLERPORTALDB.Models.Seller_Charge>();
            List<int> categoryID = db.Category.Where(x => x.Layer == 0).Select(x => x.ID).ToList();

            try
            {
                for (int i = 0; i < categoryID.Count; i++)
                {
                    TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_Charge sellerCharge = new DB.TWSELLERPORTALDB.Models.Seller_Charge();
                    sellerCharge.SellerID = sellerInvitation.SellerID;
                    sellerCharge.CategoryID = categoryID[i];
                    sellerCharge.CountryCode = sellerInvitation.SellerCountryCode;
                    sellerCharge.InDate = DateTime.UtcNow.AddHours(8);
                    sellerCharge.ChargeType = sellerInvitation.ChargeType;

                    sellerCharge.Commission = CategoryCommission(sellerCharge.ChargeType, sellerInvitation.CategoryCommisson[i]);
                    sellerChargeList.Add(sellerCharge);

                    spdb.Seller_Charge.Add(sellerCharge);
                }
                spdb.SaveChanges();

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "Seller Charge Saved";
                result.Body = sellerChargeList;
            }

            catch (Exception ex)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = ex.Message ?? "" + ((ex.InnerException != null) ? ex.InnerException.Message : "");
            }

            return result;
        }
        //回傳對應chargeType底下類別的佣金
        private decimal CategoryCommission(string chargeType, Models.SellerInvitation.SellerTypeInfo sellerCharge)
        {
            decimal commissionRate = 0;
            switch (chargeType)
            {
                case "S":
                    if (sellerCharge.CategoryID == 1)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 2)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 3)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 4)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 5)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 6)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 7)
                    {
                        commissionRate = 0.14m;
                    }
                    else if (sellerCharge.CategoryID == 264)
                    {
                        commissionRate = 0.14m;
                    }
                    else
                    {
                        commissionRate = 0;
                    }
                    break;

                case "A":
                    if (sellerCharge.CategoryID == 1)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 2)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 3)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 4)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 5)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 6)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 7)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else if (sellerCharge.CategoryID == 264)
                    {
                        commissionRate = sellerCharge.Commission;
                    }
                    else
                    {
                        commissionRate = 0;
                    }
                    break;
            }

            return commissionRate;
        }
        */

        #endregion 移除

        public API.Models.ActionResponse<List<Models.GetSellerChargeResult>> GetChargeList(TWNewEgg.API.Models.GetSellerCharge getSellerCharge)
        {

            DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();

            API.Models.ActionResponse<List<Models.GetSellerChargeResult>> apiResult = new Models.ActionResponse<List<Models.GetSellerChargeResult>>();

            List<DB.TWSELLERPORTALDB.Models.Seller_Charge> chargeList = new List<DB.TWSELLERPORTALDB.Models.Seller_Charge>();
            List<Models.GetSellerChargeResult> result = new List<Models.GetSellerChargeResult>();


            #region 2013.11.06 新增判斷式

            var query = db.Seller_Charge.AsEnumerable();

            if (getSellerCharge.SellerID.HasValue)
            {
                query = query.Where(x => x.SellerID == getSellerCharge.SellerID);
            }

            if (!string.IsNullOrEmpty(getSellerCharge.CountryCode))
            {
                query = query.Where(x => x.CountryCode == getSellerCharge.CountryCode);
            }

            if (!string.IsNullOrEmpty(getSellerCharge.ChargeType))
            {
                query = query.Where(x => x.ChargeType == getSellerCharge.ChargeType);
            }

            if (getSellerCharge.CategoryID.HasValue)
            {
                query = query.Where(x => x.CategoryID == getSellerCharge.CategoryID);
            }

            chargeList = query.ToList();

            #endregion 2013.11.06 新增

            #region 查詢類別名稱 by Jack Lin 2013.12.20
            DB.TWSqlDBContext twdb = new DB.TWSqlDBContext();

            foreach (var item in chargeList)
            {
                Models.GetSellerChargeResult newItem = new Models.GetSellerChargeResult();

                newItem.CategoryID = item.CategoryID;
                newItem.ChargeType = item.ChargeType;
                newItem.Commission = item.Commission;
                newItem.CountryCode = item.CountryCode;
                newItem.SellerID = item.SellerID;

                var category = twdb.Category.Where(x => x.ID == item.CategoryID).FirstOrDefault();
                newItem.CategoryTitle = category.Title;
                newItem.CategoryDescription = category.Description;

                result.Add(newItem);
            }
            #endregion

            if (result.Count == 0)
            {
                apiResult.Code = (int)ResponseCode.Error;
                apiResult.IsSuccess = false;
                apiResult.Msg = "查無資料";
            }
            else
            {
                apiResult.Code = (int)ResponseCode.Success;
                apiResult.IsSuccess = true;
                apiResult.Msg = "List資料傳回";
                apiResult.Body = result;
            }

            return apiResult;
        }

        public API.Models.ActionResponse<string> SaveSellerCharge(TWNewEgg.API.Models.SaveSellerCharge saveSellerCharge)
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            API.Models.ActionResponse<string> apiResult = new Models.ActionResponse<string>();
            DB.TWSELLERPORTALDB.Models.Seller_Charge charge;
            log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

            //檢查佣金費率是否合法
            int error = 0;
            string errorItem = "";
            for (int i = 0; i < saveSellerCharge.CommissionRate.Count(); i++)
            {
                if (saveSellerCharge.CommissionRate[i].Commission > 1 || saveSellerCharge.CommissionRate[i].Commission < 0)
                {
                    if (error != 0)
                        errorItem += ", ";

                    errorItem += i.ToString();

                    error++;
                }
            }

            if (error != 0)
            {
                apiResult.Code = (int)ResponseCode.Error;
                apiResult.IsSuccess = false;
                apiResult.Msg = "Commission rate field has " + error + "error(s).";
                apiResult.Body = errorItem;
            }
            else
            {
                decimal commission;
                for (int i = 0; i < saveSellerCharge.CommissionRate.Count(); i++)
                {
                    charge = new DB.TWSELLERPORTALDB.Models.Seller_Charge();

                    charge.SellerID = saveSellerCharge.SellerID;
                    charge.CountryCode = saveSellerCharge.SellerCountryCode;
                    charge.ChargeType = saveSellerCharge.ChargeType;
                    charge.CategoryID = saveSellerCharge.CommissionRate[i].CategoryID;

                    //Commission 四捨五入 2014.2.14
                    //charge.Commission = saveSellerCharge.CategoryCommisson.Where(x => x.CategoryID == charge.CategoryID).Select(x => x.Commission).FirstOrDefault();
                    commission = saveSellerCharge.CommissionRate[i].Commission;
                    commission = Decimal.Floor(commission * 10000 + (decimal)0.5) / 10000;
                    charge.Commission = commission;

                    charge.InDate = DateTime.UtcNow.AddHours(8);
                    charge.InUserID = saveSellerCharge.InUserID;
                    charge.UpdateDate = DateTime.UtcNow.AddHours(8);
                    charge.UpdateUserID = saveSellerCharge.InUserID;
                    spdb.Seller_Charge.Add(charge);


                    //apiResult.Code = (int)ResponseCode.Success;
                    //apiResult.IsSuccess = true;
                    //apiResult.Msg = "寫入成功";
                    //apiResult.Body = "Type:" + charge.ChargeType;
                }
                try
                {
                    spdb.SaveChanges();
                    apiResult.Code = (int)ResponseCode.Success;
                    apiResult.IsSuccess = true;
                    apiResult.Msg = "寫入成功";
                    //apiResult.Body = "Type:" + charge.ChargeType;
                }
                catch (Exception errorException)
                {
                    logger.Error("SellerInvitationService/SaveSellerCharge error: 新增失敗，請聯絡新蛋客服。 " + errorException.Message);
                    apiResult.Code = (int)ResponseCode.Error;
                    apiResult.IsSuccess = false;
                    apiResult.Msg = "新增失敗，請聯絡新蛋客服";
                }
            }

            

            return apiResult;
        }

        /// <summary>
        /// 取得區域列表
        /// </summary>
        /// <returns></returns>
        public API.Models.ActionResponse<List<API.Models.GetRegionListResult>> GetRegionList()
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            API.Models.ActionResponse<List<API.Models.GetRegionListResult>> apiResult = new Models.ActionResponse<List<API.Models.GetRegionListResult>>();

            List<DB.TWSELLERPORTALDB.Models.Seller_Country> countryList = new List<DB.TWSELLERPORTALDB.Models.Seller_Country>();

            countryList = spdb.Seller_Country.Where(x => !(string.IsNullOrEmpty(x.CountryCode))).ToList<DB.TWSELLERPORTALDB.Models.Seller_Country>();



            if (countryList.Count == 0)
            {
                apiResult.Code = (int)ResponseCode.Error;
                apiResult.IsSuccess = false;
                apiResult.Msg = "查無資料";
            }
            else
            {
                API.Models.GetRegionListResult country;
                List<API.Models.GetRegionListResult> result = new List<API.Models.GetRegionListResult>();
                foreach (var item in countryList)
                {
                    country = new API.Models.GetRegionListResult();
                    country.CountryCode = item.CountryCode;
                    country.Name = item.Name;
                    country.NameTW = item.NameTW;
                    result.Add(country);
                }

                apiResult.Code = (int)ResponseCode.Success;
                apiResult.IsSuccess = true;
                apiResult.Msg = "List資料傳回";
                apiResult.Body = result;
            }

            return apiResult;
        }

        /// <summary>
        /// 取得幣別列表
        /// </summary>
        /// <returns></returns>
        public API.Models.ActionResponse<List<API.Models.GetCurrencyListResult>> GetCurrencyList()
        {
            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();

            API.Models.ActionResponse<List<API.Models.GetCurrencyListResult>> apiResult = new Models.ActionResponse<List<API.Models.GetCurrencyListResult>>();

            List<DB.TWSELLERPORTALDB.Models.Seller_Currency> currencyList = new List<DB.TWSELLERPORTALDB.Models.Seller_Currency>();

            currencyList = spdb.Seller_Currency.Where(x => !(string.IsNullOrEmpty(x.CurrencyCode))).ToList<DB.TWSELLERPORTALDB.Models.Seller_Currency>();



            if (currencyList.Count == 0)
            {
                apiResult.Code = (int)ResponseCode.Error;
                apiResult.IsSuccess = false;
                apiResult.Msg = "查無資料";
            }
            else
            {
                API.Models.GetCurrencyListResult currency;
                List<API.Models.GetCurrencyListResult> result = new List<API.Models.GetCurrencyListResult>();
                foreach (var item in currencyList)
                {
                    currency = new API.Models.GetCurrencyListResult();
                    currency.CurrencyCode = item.CurrencyCode;
                    currency.Name = item.Name;
                    result.Add(currency);
                }

                apiResult.Code = (int)ResponseCode.Success;
                apiResult.IsSuccess = true;
                apiResult.Msg = "List資料傳回";
                apiResult.Body = result;
            }

            return apiResult;
        }

        /// <summary>
        /// 寄送邀請信
        /// </summary>
        /// <returns></returns>

        //public API.Models.ActionResponse<string> SendEmail(int sellerID, string email)//(DB.TWSELLERPORTALDB.Models.Seller_BasicInfo basicInfo)
        public API.Models.ActionResponse<Models.SendInvitationEmailResult> SendInvitationEmail(Models.SendInvitationEmail sendInvitationEmail)
        {
            API.Models.ActionResponse<Models.SendInvitationEmailResult> result = new Models.ActionResponse<Models.SendInvitationEmailResult>();

            DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
            DB.TWSELLERPORTALDB.Models.Seller_User user = spdb.Seller_User.Where(x => x.UserEmail == sendInvitationEmail.UserEmail).FirstOrDefault();

            if (user == null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "找不到此商家";
                result.Body = new Models.SendInvitationEmailResult();
                return result;
            }


            //改成建立user時生成
            /*//+ 生成亂數
            int rand;
            char word;
            string ranNum = String.Empty;
            // 生成啟用連結路徑中的亂數
            System.Random random = new Random();
            for (int i = 0; i < 30; i++)
            {
                rand = random.Next();
                if (rand % 3 == 0)
                {
                    word = (char)('A' + (char)(rand % 26));
                }
                else if (rand % 3 == 1)
                {
                    word = (char)('a' + (char)(rand % 26));
                }
                else
                {
                    word = (char)('0' + (char)(rand % 10));
                }

                ranNum += word.ToString();
            }

            //2013.11.13 寫入亂數至資料庫 by Jack Lin
            user.RanNum = ranNum;
            spdb.Entry(user).State = EntityState.Modified;
            spdb.SaveChanges();
            */



            string MailMessage = "<!DOCTYPE html><html><body><h1>你好!</h1><p>您已被邀請使用台灣新蛋超級商城，</p><p>您的帳號為： <strong>" + user.UserEmail + "</strong></p><p>請點選<a href='https://marketingplace.newegg.com.qa/Portal/'>此處</a>以完成帳號啟用程序。亂數：" + user.RanNum + "</p></body></html>"; //信件訊息
            string Mysubject = "台灣新蛋 Seller Portal 邀請信"; //信件主旨
            string Recipient = sendInvitationEmail.UserEmail; //收件者
            string RecipientBcc = ""; //密件收件人

            try
            {
                MailMessage msg = new MailMessage();
                // 收件者，以逗號分隔不同收件者
                msg.To.Add(Recipient);
                // msg.CC.Add("c@msn.com"); // 副本
                if (RecipientBcc != "")
                {
                    msg.Bcc.Add(RecipientBcc); // 密件副本
                }
                // 3個參數分別是發件人地址(可以隨便寫)，發件人姓名，編碼
                msg.From = new MailAddress("service@newegg.com.tw", "台灣新蛋", System.Text.Encoding.UTF8);
                msg.Subject = Mysubject; // 郵件主旨
                msg.SubjectEncoding = System.Text.Encoding.UTF8; // 郵件主旨編碼
                msg.Body = MailMessage; // 郵件內容
                msg.BodyEncoding = System.Text.Encoding.UTF8; // 郵件內容編碼
                msg.IsBodyHtml = true; // 是否為HTML郵件
                msg.Priority = MailPriority.Normal; // 郵件優先等級
                // 建立 SmtpClient 物件 並設定 Gmail的smtp主機及Port
                SmtpClient MySmtp = new SmtpClient("st01smtp01.buyabs.corp", 25);
                //SmtpClient MySmtp = new SmtpClient("172.22.5.55", 25);
                //MySmtp.Credentials = new System.Net.NetworkCredential("帳號", "密碼");
                // Gmail的smtp使用SSL
                //MySmtp.EnableSsl = true;
                // 發送Email
                MySmtp.Send(msg);

                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);

                result.Code = (int)ResponseCode.Success;
                result.IsSuccess = true;
                result.Msg = "寄送成功";
                result.Body = new Models.SendInvitationEmailResult();

                result.Body.UserID = user.UserID;
                result.Body.SendTime = dt;
                result.Body.MailContent = MailMessage;

                return result;
            }
            catch
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "寄送失敗";
                result.Body = new Models.SendInvitationEmailResult();

                return result;
            }
        }
    }
}