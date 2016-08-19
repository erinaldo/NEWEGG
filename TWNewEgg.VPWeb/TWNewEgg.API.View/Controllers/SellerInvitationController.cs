using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;

namespace TWNewEgg.API.View.Controllers
{
    public class SellerInvitationController : Controller
    {
        //
        // GET: /SellerInvitation/

        TWNewEgg.API.Models.Connector conn = new TWNewEgg.API.Models.Connector();
        API.View.Service.SellerInfoService sellerInfo_fromCookies = new Service.SellerInfoService();
        
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        /// <summary>
        /// 商家邀請主畫面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.UserManage)]
        [FunctionName(FunctionNameAttributeValues.SellerInvitation)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("商家邀請")]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            List<TWNewEgg.API.Models.GetCurrencyListResult> currSelectItemList = conn.GetCurrencyList().Body;
            List<TWNewEgg.API.Models.GetRegionListResult> countrySelectItemList = conn.GetRegionList().Body;
            ViewBag.currSelectItemList = currSelectItemList;
            ViewBag.countrySelectItemList = countrySelectItemList;

            return View();
        }

        /// <summary>
        /// 取得 commission rate (佣金率) 的 partial view
        /// </summary>
        /// <param name="sellerChargeModel"></param>
        /// <param name="accountTypeCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult pvSellerCommissionSetting(TWNewEgg.API.Models.GetSellerCharge sellerChargeModel, string accountTypeCode)
        {
            if (accountTypeCode == "S")
            {
                sellerChargeModel.SellerID = this.sellerInfo_fromCookies.SellerID;
                List<TWNewEgg.API.Models.GetSellerChargeResult> getSellerChargeResult = conn.GetSellerCharge(null, null, sellerChargeModel).Body;

                if (accountTypeCode == "S")
                {
                    ViewBag.sellerCharge_itemCollec = null;
                }
                else if (accountTypeCode == "V")
                {
                    ViewBag.sellerCharge_itemCollec = null;
                }
                else
                {
                    ViewBag.sellerCharge_itemCollec = null;
                }

                ViewBag.accountTypeCode = accountTypeCode;

                // 進行partial view的處理/取得
                ViewEngineResult viewEngineResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "CommissionRate");
                StringWriter sw = new StringWriter();
                ViewContext vc = new ViewContext(this.ControllerContext, viewEngineResult.View, this.ViewData, this.TempData, sw);
                viewEngineResult.View.Render(vc, sw);

                // 回傳pratial view
                return Json(new { pvCommissionRate = sw.ToString(), showTable = true });
            }
            else
            {
                return Json(new { pvCommissionRate = string.Empty, showTable = false });
            }
        }

        /// <summary>
        /// 將該 View 轉成 string
        /// </summary>
        /// <param name="partialView">View 的名稱</param>
        /// <returns>返回 string</returns>
        public string RenderView(string partialView)
        {
            string result = string.Empty;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                result = sw.GetStringBuilder().ToString();
            }

            return result;
        }

        /// <summary>
        /// 寄送商家邀請email
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="sellerInfo">被邀請者(seller/vendor)資訊</param>
        /// <param name="sellerCharge">傭金設定(table)</param>
        /// <returns></returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult SendMail(string auth, string token, TWNewEgg.API.Models.SellerCreation sellerInfo, TWNewEgg.API.Models.SaveSellerCharge sellerCharge)
        {
            //bool stepSuccess = false;
            //bool resultSuccess = false;
            string resultMsg = string.Empty;
            #region 呼叫 API 前先檢查資料格式
            TWNewEgg.API.Models.ActionResponse<string> checkResult = new API.Models.ActionResponse<string>();
            bool isNoException = true;
            try
            {
                //呼叫檢查資料完整性的, 國內廠商: 統一編號; 個人戶: 身分證字號
                checkResult = this.Identy_BillingCycle_TaxID_Check(sellerInfo.Identy.GetValueOrDefault(), sellerInfo.Currency, sellerInfo.CompanyTaxId_Identity);
                isNoException = true;
            }
            catch (Exception error)
            {
                isNoException = false;
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //發生 Exception
            if (isNoException == false)
            {
                return Json("系統錯誤");
            }
            //檢查資料完整性有錯誤
            if (checkResult.IsSuccess == false)
            {
                return Json(checkResult.Msg);
            }
            #endregion
            TWNewEgg.API.Models.ActionResponse<string> createSellerVendorResult = new API.Models.ActionResponse<string>();
            #region 呼叫 Api 建立製造商
            try
            {
                createSellerVendorResult = conn.createSellerVendor(sellerInfo, sellerCharge, sellerInfo_fromCookies.UserID);
            }
            catch (Exception error)
            {
                createSellerVendorResult.IsSuccess = false;
                createSellerVendorResult.Msg = "建立自製商錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //建立製造商發生錯誤
            if (createSellerVendorResult.IsSuccess == false)
            {
                return Json(createSellerVendorResult.Msg);
            }
            #endregion
            #region 寄信給製造商
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.MailResult> sendMailResult = new API.Models.ActionResponse<API.Models.MailResult>();
            try
            {
                sendMailResult = conn.SendMail(auth, token, new TWNewEgg.API.Models.Mail() { MailType = TWNewEgg.API.Models.Mail.MailTypeEnum.SallerInvitationEmail, UserEmail = sellerInfo.SellerEmail, UserName = sellerInfo.SellerName });
            }
            catch (Exception error)
            {
                sendMailResult.IsSuccess = false;
                sendMailResult.Msg = "寄信失敗";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            if (sendMailResult.IsSuccess == false)
            {
                return Json("寄信失敗");
            }
            else
            {
                return Json("寄信成功");
            }
            #endregion
        }

        /// <summary>
        /// 取得 標準/自訂 傭金率的資料
        /// </summary>
        /// <param name="sellerChargeModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult GetCommissionRateData(TWNewEgg.API.Models.GetSellerCharge sellerChargeModel)
        {
            //取得傭金率資料
            List<TWNewEgg.API.Models.GetSellerChargeResult> getSellerChargeResult = conn.GetSellerCharge(null, null, sellerChargeModel).Body;

            int counts = 0;
            if (getSellerChargeResult != null)
            {
                counts = getSellerChargeResult.Count();
            }

            ViewBag.sellerCharge_itemCollec = getSellerChargeResult;

            //取得partial view
            string result = string.Empty;
            ViewEngineResult viewEngineResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "CommissionRate");
            StringWriter sw = new StringWriter();
            ViewContext vc = new ViewContext(this.ControllerContext, viewEngineResult.View, this.ViewData, this.TempData, sw);
            viewEngineResult.View.Render(vc, sw);
            result = sw.GetStringBuilder().ToString();


            return Json(new { pvCommissionRate = result, counts = counts });
        }

        /// <summary>
        /// 佣金率若有小數，四捨五入至小數點兩位數(畫面上為"%"百分比)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Filter.PermissionFilter]
        [Filter.LoginAuthorize]
        public JsonResult ValidateCommRate(string CategoryID, string Commission, string rightDigit)
        {
            TWNewEgg.API.Models.SaveSellerCharge.CommissionRateInfo commRateModel = new TWNewEgg.API.Models.SaveSellerCharge.CommissionRateInfo();
            decimal inputRate = 0m;
            decimal roundedRateToRtn = 0m;

            // 若非正確的數字格式，回傳0
            if (!decimal.TryParse(Commission, out inputRate))
            {
                return Json(new { CategoryID = CategoryID, Commission = "0", msg = "請輸入正確數字格式!" });
            }

            // 四捨五入至小數點第二位
            roundedRateToRtn = Math.Round(inputRate, 2, MidpointRounding.AwayFromZero);

            return Json(new { CategoryID = CategoryID, Commission = string.Format("{0:0.00}", roundedRateToRtn), msg = "(完成)" });
        }


        #region 公佈欄 area
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.UserManage)]
        [FunctionName(FunctionNameAttributeValues.BulletinsMessage)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [Filter.PermissionFilter]
        [ActionDescription("公佈欄")]
        public ActionResult BulletinsMessage()
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.View.XmlModel xmlModel = new XmlModel();
            try
            {
                result = conn.xmlBulletinsRead();
                if (result.IsSuccess == false)
                {
                    xmlModel.userid = 0;
                    xmlModel.updateNumber = "0";
                    xmlModel.isSuccess = false;
                    xmlModel.xmlcontent = "";
                }
                else
                {
                    xmlModel.userid = sellerInfo_fromCookies.UserID;
                    xmlModel.updateNumber = result.Msg;
                    xmlModel.isSuccess = result.IsSuccess;
                    xmlModel.xmlcontent = result.Body;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("/SellerInvitation/BulletinsMessage connect to api error: " + error.Message);
            }
            return View(xmlModel);
        }
        public JsonResult WriteXML(string xmlContent, int UserID, int updateNumber)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();

            try
            {
                result = conn.xmlBulletinsWrite(xmlContent, UserID, updateNumber);
            }
            catch (Exception error)
            {
                logger.Error("/SellerInvitation/WriteXML api connect error: " + error.Message);
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            if (result.IsSuccess == false)
            {
                return Json(new { isSucess = "F", msg = result.Msg });
            }
            else
            {
                return Json(new { isSucess = "T", msg = result.Msg });
            }
        }
        #endregion

        #region 商家邀請-> 廠商身分別, 付款方式, 統編/身分證字號檢查
        /// <summary>
        /// 商家邀請-> 廠商身分別, 份款方式, 統編/身分證字號檢查
        /// </summary>
        /// <param name="Identy">廠商身分別</param>
        /// <param name="PayMoneyType">幣別</param>
        /// <param name="TaxId_PeopleId">統編/身分證字號</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> Identy_BillingCycle_TaxID_Check(int Identy, string PayMoneyType, string TaxId_PeopleId)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            TWNewEgg.API.View.Service.TaxID_IdentityCard_CheckService TaxId_IdCardCheck = new Service.TaxID_IdentityCard_CheckService();
            #region 初始化回傳的 TWNewEgg.API.Models.ActionResponse<string> result
            result.IsSuccess = true;
            result.Msg = "檢查成功, 無錯誤";
            #endregion
            #region 國內廠商或個人戶只能用台幣
            //Identy == 1: 國內廠商, Identy == 3: 個人戶
            if (Identy == 1 || Identy == 3)
            {
                if (PayMoneyType != "TWD")
                {
                    result.IsSuccess = false;
                    result.Msg = "國內廠商幣別只能選擇 TWD (New Taiwan dollar)";
                    return result;
                }
            }
            #endregion
            #region 國內廠商檢查統一編號
            //Identy == 1: 國內廠商
            if (Identy == 1)
            {
                //國內廠商檢查統一編號
                TWNewEgg.API.Models.ActionResponse<string> checkresult = TaxId_IdCardCheck.TaxIDCheck(TaxId_PeopleId);
                if (checkresult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = checkresult.Msg;
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    //result.Msg = checkresult.Msg;
                    return result;
                }
            }
            #endregion
            #region 個人戶檢查身分證號碼
            //Identy == 3: 個人戶
            if (Identy == 3)
            {
                //個人戶檢查身分證字號
                TWNewEgg.API.Models.ActionResponse<string> checkresult = TaxId_IdCardCheck.IdentityCardCheck(TaxId_PeopleId);
                if (checkresult.IsSuccess == false)
                {
                    result.IsSuccess = false;
                    result.Msg = checkresult.Msg;
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    //result.Msg = checkresult.Msg;
                    return result;
                }
            }
            #endregion
            return result;
        }
        #endregion
        #region 抓取 InnerException error msg
        public string ExceptionInnerMsg(Exception error)
        {
            string returnMsg = string.Empty;
            returnMsg = error.InnerException == null ? "" : error.InnerException.Message;

            return returnMsg;
        }
        #endregion
        #region 轉換統編字串為數字陣列
        public TWNewEgg.API.Models.ActionResponse<List<int>> ListIntTaxId_PeopleId(string TaxId_PeopleId)
        {
            TWNewEgg.API.Models.ActionResponse<List<int>> result = new API.Models.ActionResponse<List<int>>();
            if (string.IsNullOrEmpty(TaxId_PeopleId) == true)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            List<int> tempListInt = new List<int>();
            var charArray = TaxId_PeopleId.ToCharArray();
            try
            {
                for (int i = 0; i < charArray.Length; i++)
                {
                    tempListInt.Add(Convert.ToInt16(charArray[i].ToString()));
                }
                result.IsSuccess = true;
                result.Body = tempListInt;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerExceptionMsg]: " + this.ExceptionInnerMsg(error));
            }
            return result;
        }
        #endregion
    }
}
