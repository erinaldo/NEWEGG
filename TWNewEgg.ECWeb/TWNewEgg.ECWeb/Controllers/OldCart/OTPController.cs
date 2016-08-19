using Newegg.Mobile.MvcApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.Website.ECWeb.Service;
using TWNewEgg.Redeem.Service;
using TWNewEgg.InternalSendMail.Service;
using System.Data;
using TWNewEgg.ECWeb.PrivilegeFilters;
using System.Data.SqlClient;
using System.Web.Util;
using TWNewEgg.GetConfigData.Service;
using TWNewEgg.DB;
using TWNewEgg.ECWeb.Auth;


namespace TWNewEgg.ECWeb.Controllers
{
    public class OTPController : Controller
    {
        //[AllowNonSecures]
        //[AllowAnonymous]
        //public ActionResult Index() 
        //{
        //    TWNewEgg.Models.DomainModels.OTP.PostOTPItem PostOTPItem = new TWNewEgg.Models.DomainModels.OTP.PostOTPItem();
        //    PostOTPItem.PriceSum =2000;
        //    PostOTPItem.PayType = (int)PayType.nPayType.信用卡一次付清;
        //    PostOTPItem.Moblie = "0970000200000";
        //    PostOTPItem.ItemID = 5;
            
        //    return SendSMS(PostOTPItem,8);
        //}

        //[AllowNonSecures]
        //[AllowAnonymous]
        //public ActionResult SendSMS(TWNewEgg.Models.DomainModels.OTP.PostOTPItem PostOTPItem, int UserID)
        //{
        //    try
        //    {
        //        //UserID = NEUser.ID;
        //        OTPService optService = new OTPService();
        //        TWSqlDBContext db_before = new TWSqlDBContext();

        //        if (optService.IsSMSBlock(UserID, PostOTPItem.PriceSum))
        //        {
        //            return RedirectToAction("Shoppingcart", "Marketablecache", new { message = "親愛的" + NEUser.Name + "顧客，您已錯誤輸入超過三次認證碼，您將於24小時內無法進行1萬元以上的購物，若還需購買一萬元以上商品，歡迎來電洽詢客服中心。" });
        //        }
        //        //判斷購物金額是否大於10000及是否選擇信用卡付款方式
        //        if (PostOTPItem.PriceSum >= 10000 && checkCredit(PostOTPItem.PayType))
        //        {
        //            OTPRecord DBRecord = null;
        //            DBRecord = new OTPRecord();
        //            DBRecord = db_before.OTPRecord.Where(x => x.UserID == UserID).FirstOrDefault();
        //            if (DBRecord != null && DBRecord.Phone.Trim() != PostOTPItem.Moblie.Trim())
        //            {
        //                string SMSpaasword = optService.GenerateSMSCode(UserID, PostOTPItem.PriceSum, PostOTPItem.Moblie, PostOTPItem.ItemID.ToString());
        //                if (SMSpaasword.Trim().Count() != 0)
        //                {
        //                    optService.SendSMS(PostOTPItem.Moblie, SMSpaasword);
        //                }
        //                else
        //                {
        //                    return RedirectToAction("shoppingcart", "MarketableCache", new { message = "SendSMS錯誤密碼" });

        //                }
        //            }
        //            else
        //            {
        //                if (optService.IsSMSSuccess(UserID) != true)
        //                {
        //                    if ((DBRecord != null && DateTime.Now >= DBRecord.StatusDate.AddMinutes(10)) || DBRecord == null)
        //                    {
        //                        string SMSpaasword = optService.GenerateSMSCode(UserID, PostOTPItem.PriceSum, PostOTPItem.Moblie, PostOTPItem.ItemID.ToString());
        //                        if (SMSpaasword.Trim().Count() != 0)
        //                        {
        //                            optService.SendSMS(PostOTPItem.Moblie, SMSpaasword);
        //                        }
        //                        else
        //                        {
        //                            return RedirectToAction("shoppingcart", "MarketableCache", new { message = "SendSMS錯誤密碼" });

        //                        }
        //                    }
        //                }
        //            }
        //            DBRecord = db_before.OTPRecord.Where(x => x.UserID == UserID).FirstOrDefault();
        //            ViewBag.opttimeout = DBRecord.StatusDate.AddMinutes(10).Hour.ToString("00") + ":" + DBRecord.StatusDate.AddMinutes(10).Minute.ToString("00");//otp到期時間
        //            TimeSpan timeout = new TimeSpan(DBRecord.StatusDate.AddMinutes(10).Ticks - DateTime.UtcNow.AddHours(8).Ticks);

        //            ViewBag.timeOut = timeout.TotalMinutes;
        //            ViewBag.timeOutsecond = timeout.TotalMinutes * 60;
        //        }
        //    }
        //    catch 
        //    {
            
            
        //    }
        //    return View();
        //}
        /// <summary>
        /// 判斷消費者選擇的付費方式是否為信用卡付款
        /// </summary>
        /// <param name="paytype">付費方式代號</param>
        /// <returns>如果是信用卡付費:回傳true   如果不是信用卡付費:回傳false</returns>
        //public bool checkCredit(int paytype)
        //{
        //    bool exec = false;
        //    if (paytype == (int)PayType.nPayType.信用卡一次付清
        //        || paytype == (int)PayType.nPayType.三期零利率
        //        || paytype == (int)PayType.nPayType.六期零利率
        //        || paytype == (int)PayType.nPayType.十期零利率
        //        || paytype == (int)PayType.nPayType.十二期零利率
        //        || paytype == (int)PayType.nPayType.十八期零利率
        //        || paytype == (int)PayType.nPayType.二十四期零利率
        //        || paytype == (int)PayType.nPayType.十期分期
        //        || paytype == (int)PayType.nPayType.十二期分期
        //        || paytype == (int)PayType.nPayType.十八期分期
        //        || paytype == (int)PayType.nPayType.二十四期分期)
        //    {
        //        exec = true;
        //    }
        //    return exec;
        //}

        //
        // GET: /OTP/
        private ICarts repository = new CartsRepository();
        /// <summary>
        /// 取得該UID的資料並回傳
        /// </summary>
        /// <param name="UID">會員帳號</param>
        /// <param name="flag">判斷是否需將會員帳號轉碼</param>
        /// <returns></returns>
        public JsonResult Status(string UID, string flag = "notUse")
        {
            TWNewEgg.DB.TWSqlDBContext sDB = null;
            OTPClientModel checkStatus = null;
            OTPRecord otr = null;
            DateTime datetimeNow = new DateTime();
            int Uid;
            if (flag == "notUse")
            {
                string[] plainText = repository.Decoder(UID, false);
                Uid = Int32.Parse(plainText[0]);
            }
            else
            {
                Uid = Convert.ToInt32(UID);
            }

            sDB = new DB.TWSqlDBContext();
            otr = sDB.OTPRecord.Where(x => x.UserID == Uid).FirstOrDefault();
            checkStatus = new OTPClientModel();
            if (otr != null)
            {
                datetimeNow = DateTime.Now;
                //判斷是否jo6BLOCK且已經超過Block時間，若已超過把Status改成Null，並把FailCount歸零
                if (datetimeNow > otr.StatusDate.AddDays(1) && otr.Status == (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block)
                {
                    otr.Status = (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Null;
                    otr.FailCount = 0;
                    otr.StatusDate = datetimeNow;
                    sDB.SaveChanges();
                }

                checkStatus.UID = otr.UserID;
                //var aa = TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block;
                switch (otr.Status)
                {
                    case (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Null: checkStatus.Status = "Null";
                        break;
                    case (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Success: checkStatus.Status = "Success";
                        break;
                    case (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Block: checkStatus.Status = "Block";
                        break;
                }
                checkStatus.Phone = otr.Phone;
                checkStatus.FailCount = otr.FailCount ?? 0;
            }
            else
            {
                checkStatus.UID = Uid;
                checkStatus.Status = "Null";
                checkStatus.Phone = "";
                checkStatus.FailCount = 0;
            }
            return Json(checkStatus, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 檢查驗證碼是否正確
        ///     0:null 1:success 2:block
        ///     驗證三個條件：
        ///         status=null
        ///         密碼有效(10mins)內輸入
        ///         密碼驗證成功
        /// </summary>
        /// <param name="password">簡訊驗證密碼</param>
        /// <param name="UserID">會員帳號</param>
        /// <returns></returns>
        public ActionResult CheckPassword(string password, int UserID)
        {
            //bool boolPwd = false;
            OTPClientModel CheckPwd = null;
            OTPRecord DBRecord = null;
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            objDb = new DB.TWSqlDBContext();
            DBRecord = new OTPRecord();
            CheckPwd = new OTPClientModel();
            password = password.ToUpper();
            /* 從資料庫取出資料 */
            DBRecord = objDb.OTPRecord.Where(x => x.UserID == UserID).FirstOrDefault();

            if (DBRecord == null)
            {
                CheckPwd.UID = UserID;
                CheckPwd.Status = "ERROR";
                CheckPwd.Phone = "";
                CheckPwd.FailCount = 0;
                return Json(CheckPwd, JsonRequestBehavior.AllowGet);
            }

            /* 檢查status是否為null */
            if (DBRecord.Status != 0)
            {
                CheckPwd.UID = DBRecord.UserID;
                CheckPwd.Status = "status" + DBRecord.Status;
                CheckPwd.Phone = DBRecord.Phone.Substring(0, 4) + "****" + DBRecord.Phone.Substring(8, 2);
                CheckPwd.FailCount = DBRecord.FailCount ?? 0;

                return Json(CheckPwd, JsonRequestBehavior.AllowGet);
            }

            /* 驗證 */
            if (DBRecord != null && DateTime.Now <= DBRecord.StatusDate.AddMinutes(10) && DBRecord.Password.ToUpper() == password)
            {
                DBRecord.Status = 1;
                DBRecord.FailCount = CheckPwd.FailCount;

                CheckPwd.UID = DBRecord.UserID;
                CheckPwd.Status = "Success";
                CheckPwd.Phone = DBRecord.Phone.Substring(0, 4) + "****" + DBRecord.Phone.Substring(8, 2);
            }
            else
            {
                CheckPwd.UID = UserID;
                CheckPwd.Status = ((OTPRecord.OPTStatus)DBRecord.Status).ToString();
                CheckPwd.Phone = DBRecord.Phone.Substring(0, 4) + "****" + DBRecord.Phone.Substring(8, 2);
                DBRecord.FailCount += 1;
                CheckPwd.FailCount = DBRecord.FailCount ?? 0;
            }
            objDb.SaveChanges();

            /* 驗證失敗超過三次 */
            if (DBRecord.FailCount >= 3)
            {
                DBRecord.Status = 2;
                DBRecord.StatusDate = DateTime.Now;
                DBRecord.FailCount = CheckPwd.FailCount;
                CheckPwd.Status = "Block";

                objDb.SaveChanges();
            }

            return Json(CheckPwd, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重送簡訊
        ///     10mins內只能重送一次
        /// </summary>
        /// <param name="UserID">會員帳號</param>
        /// <returns></returns>
        public ActionResult Resend(int UserID)
        {
            OTPRecord DBRecord = null;
            TWNewEgg.DB.TWSqlDBContext objDb = null;
            OTPService runOTP = null;

            DateTime timeNow = DateTime.Now;
            DBRecord = new OTPRecord();
            objDb = new DB.TWSqlDBContext();
            runOTP = new OTPService();
            string SMSCode = string.Empty;

            DBRecord = objDb.OTPRecord.Where(x => x.UserID == UserID).FirstOrDefault();

            if (DBRecord == null)
            {
                return Json(new { status = "ERROR" });
            }

            if (timeNow >= DBRecord.StatusDate.AddMinutes(10))
            {
                SMSCode = runOTP.ReGenerateSMSCode(DBRecord.UserID);
                runOTP.SendSMS(DBRecord.Phone, SMSCode);
            }
            else
            {
                SMSCode = DBRecord.Password;
                runOTP.SendSMS(DBRecord.Phone, SMSCode);
            }

            return Json(new { status = "RESEND" });
        }
        /// <summary>
        /// 重送簡訊
        ///     10mins內只能重送一次
        /// </summary>
        /// <param name="Phone">獲得驗證碼的手機號碼</param>
        /// <returns></returns>
        public ActionResult ResendforActCheck(string Phone)
        {

            Accountactcheck Accountactcheck = new Accountactcheck();
            TWNewEgg.DB.TWSqlDBContext objDb = new DB.TWSqlDBContext();
            OTPService runOTP = null;
            NewMemModel NewMemModel = null;
            DateTime timeNow = DateTime.Now;
            runOTP = new OTPService();
            string SMSCode = string.Empty;
            TWNewEgg.Website.ECWeb.Service.BlackList.BlackList _blackList = new TWNewEgg.Website.ECWeb.Service.BlackList.BlackList();

            Accountactcheck = objDb.Accountactcheck.Where(x => x.Phone == Phone).FirstOrDefault();
            if (Accountactcheck == null)
            {
                return Json(new { status = "ERROR" });
            }
            bool isPayMoneyPhoneNumber = _blackList.MobileBlackList(Phone);
            if (isPayMoneyPhoneNumber)
            {
                return Json(new { status = "illegal" });
            }

            if (timeNow >= ((DateTime)Accountactcheck.StatusDate).AddMinutes(10))
            {
                SMSCode = runOTP.ReGenerateSMSCodeForRegister(Accountactcheck.Phone);

            }
            else
            {
                SMSCode = Accountactcheck.Authenticate;

            }
            if (string.IsNullOrWhiteSpace(SMSCode) == false)
            {
                runOTP.SendSMSForRegister(Accountactcheck.Phone, SMSCode);
            }
            return Json(new { status = "RESEND" });
        }
        /// <summary>
        /// 取得該Phone的資料並回傳
        /// </summary>
        /// <param name="Phone">獲得驗證碼的手機號碼</param>
        /// <param name="EmailAccount">登入得會員帳號</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult StatusforActCheck(string Phone, string EmailAccount)
        {
            TWNewEgg.DB.TWSqlDBContext sDB = new DB.TWSqlDBContext();
            Accountactcheck Accountactcheck = new Accountactcheck();
            NewMemModel NewMemModel = new NewMemModel();
            TWNewEgg.Website.ECWeb.Service.OTPService otpservice = new OTPService();
            DateTime datetimeNow = new DateTime();
            //sDB = new DB.TWSqlDBContext();
            /*Writing by LittleMe for checking the EmailAccount exits in account table or not*/
            // 為修改非會員購物轉會員，將判斷條件改為只找會員註冊的EMAIL GuestLogin == 0
            var ExitAccount = sDB.Account.Where(p => p.Email.Trim() == EmailAccount.Trim() & p.GuestLogin == 0).FirstOrDefault();
            Accountactcheck Accountactcheckremov = sDB.Accountactcheck.Where(x => x.Phone == Phone).FirstOrDefault();
            TWNewEgg.Website.ECWeb.Service.BlackList.BlackList _blackList = new TWNewEgg.Website.ECWeb.Service.BlackList.BlackList();
            bool isPayMoneyPhoneNumber = _blackList.MobileBlackList(Phone);

            if (ExitAccount != null)
            {
                NewMemModel.Status = "AccountExit";
                return Json(NewMemModel, JsonRequestBehavior.AllowGet);
            }
            if (isPayMoneyPhoneNumber)
            {
                NewMemModel.Status = "BlackList";
                return Json(NewMemModel, JsonRequestBehavior.AllowGet);
            }
            //假設無userid代表他沒完成註冊步驟,所以將他資料刪除重新加一筆資料
            if (Accountactcheckremov != null && string.IsNullOrWhiteSpace(Accountactcheckremov.User_id.ToString()) == true && Accountactcheckremov.Status == (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success)
            {

                sDB.Accountactcheck.Remove(Accountactcheckremov);
                sDB.SaveChanges();
            }


            Accountactcheck = sDB.Accountactcheck.Where(x => x.Phone == Phone).FirstOrDefault();
            if (Accountactcheck != null)
            {

                datetimeNow = DateTime.Now;
                //判斷是否BLOCK且已經超過Block時間，若已超過把Status改成Null，並把FailCount歸零
                if (datetimeNow > ((DateTime)Accountactcheck.StatusDate).AddDays(1) && Accountactcheck.Status == (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Block)
                {
                    Accountactcheck.Status = (int)TWNewEgg.DB.TWSQLDB.Models.OTPRecord.OPTStatus.Null;
                    Accountactcheck.FailCount = 0;
                    Accountactcheck.StatusDate = datetimeNow;
                    sDB.SaveChanges();
                }
                NewMemModel.Phone = Accountactcheck.Phone;
                switch (Accountactcheck.Status)
                {
                    //判斷超過10分鐘是否需要產生新的密碼
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Null:
                        string SMSCode = "";
                        if (((DateTime)Accountactcheck.StatusDate).AddMinutes(10) <= DateTime.Now)
                        {
                            SMSCode = otpservice.ReGenerateSMSCodeForRegister(Phone);
                        }
                        else
                        {
                            SMSCode = Accountactcheck.Authenticate;
                        }
                        if (string.IsNullOrWhiteSpace(SMSCode) == false)
                        {
                            otpservice.SendSMSForRegister(Phone, SMSCode);
                        }
                        NewMemModel.Status = "Null";
                        NewMemModel.StatusDate = Accountactcheck.StatusDate;//時鐘時間(要抓Server時間)
                        break;
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success:

                        NewMemModel.Status = "Success";
                        break;
                    case (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Block:
                        NewMemModel.Status = "Block";
                        break;
                }
                NewMemModel.Phone = Accountactcheck.Phone;
                NewMemModel.FailCount = Accountactcheck.FailCount ?? 0;
            }
            else
            {
                string SMSCode = "";

                NewMemModel.Status = "Null";
                NewMemModel.Phone = "";
                NewMemModel.FailCount = 0;
                NewMemModel.StatusDate = DateTime.Now.AddMinutes(10);



                SMSCode = otpservice.GenerateSMSCodeForRegister(Phone);
                otpservice.SendSMSForRegister(Phone, SMSCode);

                /*-------------------------------*/
            }

            return Json(NewMemModel, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 檢查驗證碼是否正確
        ///     0:null 1:success 2:block
        ///     驗證三個條件：
        ///         status=null
        ///         密碼有效(10mins)內輸入
        ///         密碼驗證成功
        /// </summary>
        /// <param name="password">簡訊驗證密碼</param>
        /// <param name="phone">獲得驗證碼的手機號碼</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CheckPasswordforActCheck(string password, string phone)
        {
            NewMemModel NewMemModel = new NewMemModel();
            Accountactcheck Accountactcheck = new Accountactcheck();
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            objDb = new DB.TWSqlDBContext();
            Accountactcheck = objDb.Accountactcheck.Where(x => x.Phone == phone).FirstOrDefault();
            if (Accountactcheck == null)
            {
                NewMemModel.Phone = phone;
                NewMemModel.Status = "ERROR";
                NewMemModel.FailCount = 0;

                return Json(NewMemModel, JsonRequestBehavior.AllowGet);

            }
            /* 檢查status是否為null */
            if (Accountactcheck.Status != (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Null)
            {

                NewMemModel.Phone = Accountactcheck.Phone;
                NewMemModel.Status = Accountactcheck.Status.ToString();

                NewMemModel.FailCount = Accountactcheck.FailCount ?? 0;

                return Json(NewMemModel, JsonRequestBehavior.AllowGet);
            }
            /* 驗證 */
            if (Accountactcheck != null && DateTime.Now <= ((DateTime)Accountactcheck.StatusDate).AddMinutes(10) && Accountactcheck.Authenticate.ToUpper() == password.ToUpper())
            {
                Accountactcheck.Status = (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success;
                Accountactcheck.FailCount = NewMemModel.FailCount;
                NewMemModel.Status = "Success";
                NewMemModel.Phone = Accountactcheck.Phone;

            }
            else
            {
                NewMemModel.Phone = phone;
                NewMemModel.Status = ((OTPRecord.OPTStatus)Accountactcheck.Status).ToString();

                Accountactcheck.FailCount += 1;
                NewMemModel.FailCount = Accountactcheck.FailCount ?? 0;
            }
            objDb.SaveChanges();

            /* 驗證失敗超過三次 */
            if (Accountactcheck.FailCount >= 3)
            {
                Accountactcheck.Status = (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Block;
                Accountactcheck.StatusDate = DateTime.Now;
                Accountactcheck.FailCount = NewMemModel.FailCount;
                NewMemModel.Status = "Block";

                objDb.SaveChanges();
            }

            return Json(NewMemModel, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CheckPasswordforActCheckV2(string password, string phone)
        {
            NewMemModel NewMemModel = new NewMemModel();
            Accountactcheck Accountactcheck = new Accountactcheck();
            TWNewEgg.DB.TWSqlDBContext objDb = null;

            objDb = new DB.TWSqlDBContext();
            Accountactcheck = objDb.Accountactcheck.Where(x => x.Phone == phone).FirstOrDefault();
            if (Accountactcheck == null)
            {
                NewMemModel.Phone = phone;
                NewMemModel.Status = "ERROR";
                NewMemModel.FailCount = 0;

                return Json(NewMemModel, JsonRequestBehavior.AllowGet);

            }
            /* 檢查status是否為null */
            if (Accountactcheck.Status != (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Null)
            {

                NewMemModel.Phone = Accountactcheck.Phone;
                NewMemModel.Status = Accountactcheck.Status.ToString();

                NewMemModel.FailCount = Accountactcheck.FailCount ?? 0;

                return Json(NewMemModel, JsonRequestBehavior.AllowGet);
            }
            /* 驗證 */
            if (Accountactcheck != null && DateTime.Now <= ((DateTime)Accountactcheck.StatusDate).AddMinutes(10) && Accountactcheck.Authenticate.ToUpper() == password.ToUpper())
            {
                Accountactcheck.Status = (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Success;
                Accountactcheck.FailCount = NewMemModel.FailCount;
                NewMemModel.Status = "Success";
                NewMemModel.Phone = Accountactcheck.Phone;
                NewMemModel.SubmitButton = "<div id=\"AgreeCheck\"><div class=\"agreeBox\"><input type=\"checkbox\" name=\"AgreePaper\" id=\"account_agreepaper\" tabindex=\"5\" /><label for=\"account_agreepaper\">我已閱讀完畢，並同意會員條款<span class=\"red\">(需勾選才能加入會員)</span><br /></label><input type=\"checkbox\" name=\"ReceiveEDM\" id=\"account_receiveedm\" tabindex=\"6\" checked=\"checked\" /><label for=\"account_receiveedm\">訂閱eDM，取得更多優惠訊息</label><br /><input type=\"checkbox\" name=\"MessagePaper\" id=\"account_messagepaper\" tabindex=\"7\" checked=\"checked\" /><label for=\"account_messagepaper\">訂閱電子報，取得最新活動資訊</label></div><!-- 按鈕 --><div class=\"bottomBtn\"><input type=\"submit\" id=\"submitdata\" name=\"submitdata\" disabled=\"disabled\" value=\" @Language.Determine \"class=\"iconB btnSub gray\"  onclick=\"getcoupon();\" /></div></div>";

            }
            else
            {
                NewMemModel.Phone = phone;
                NewMemModel.Status = ((OTPRecord.OPTStatus)Accountactcheck.Status).ToString();

                Accountactcheck.FailCount += 1;
                NewMemModel.FailCount = Accountactcheck.FailCount ?? 0;
            }
            objDb.SaveChanges();

            /* 驗證失敗超過三次 */
            if (Accountactcheck.FailCount >= 3)
            {
                Accountactcheck.Status = (int)TWNewEgg.DB.TWSQLDB.Models.Accountactcheck.AccountactcheckStatus.Block;
                Accountactcheck.StatusDate = DateTime.Now;
                Accountactcheck.FailCount = NewMemModel.FailCount;
                NewMemModel.Status = "Block";

                objDb.SaveChanges();
            }

            return Json(NewMemModel, JsonRequestBehavior.AllowGet);
        }
    }
   
}

