using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.Models.ViewModel;
using Newtonsoft.Json;

namespace TWNewEgg.API.View.Controllers
{
    public class ItemBatchCreationController : Controller
    {
        //
        // GET: /ItemBatchCreationController/

        Connector conn = new Connector();
        TWNewEgg.API.View.ServiceAPI.APIConnector apiConnvetor = new ServiceAPI.APIConnector();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public ActionResult Index()
        {
            List<DB.TWSQLDB.Models.Category> CategoryList = new List<DB.TWSQLDB.Models.Category>();

            try
            {
                CategoryList = conn.APIQueryCategory(null, null, null, null).Body;

                CategoryList = CategoryList.Where(x => x.ShowAll == 1).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
            ViewBag.categoryList = CategoryList;
            return View(CategoryList);
        }


        #region 批次送審

        [HttpPost]
        public JsonResult BatchCreationTest(List<TWNewEgg.API.Models.ItemSketch> _itemSketch, int times, string userEmail, string passWord)
        {
            List<TWNewEgg.API.Models.ItemSketch> _listSketchModel = new List<ItemSketch>();
            if (_itemSketch == null || _itemSketch.Count == 0)
            {
                return Json("請傳入正確的資料");
            }
            for (int i = 0; i < times; i++)
            {
                _listSketchModel.Add(_itemSketch[0]);
            }
            var result = this.ItemTempBatchCreation(_listSketchModel, userEmail, passWord);
            //ActionResponse<string> result = new ActionResponse<string>();
            return Json(result.Data.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_itemSketch"></param>
        /// <param name="userEmail"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ItemTempBatchCreation(List<TWNewEgg.API.Models.ItemSketch> _itemSketch, string userEmail, string passWord)
        {
            TWNewEgg.API.Models.ActionResponse<string> checkExamineModel = new ActionResponse<string>();
            string _strListMsg = string.Empty;
            TWNewEgg.API.Models.ActionResponse<string> resultFinal = new ActionResponse<string>();
            #region 檢查是否有傳入 model 資料、Email、Password
            if (_itemSketch == null)
            {
                return Json("資料錯誤，請檢查資料");
            }
            else
            {
                if (_itemSketch.Count == 0)
                {
                    return Json("資料錯誤，請檢查資料");
                }
            }
            if (string.IsNullOrEmpty(userEmail) == true)
            {
                return Json("Email 錯誤");
            }
            if (string.IsNullOrEmpty(passWord) == true)
            {
                return Json("passWord 錯誤");
            }
            #endregion
            #region connect version
            ActionResponse<List<string>> BatchItemCreation_JsonResult = new ActionResponse<List<string>>();
            try
            {
                BatchItemCreation_JsonResult = conn.ItemTempBatchExamine(_itemSketch, userEmail, passWord);
                foreach (var itemMsg in BatchItemCreation_JsonResult.Body)
                {
                    _strListMsg = _strListMsg + itemMsg + "。";
                }
                BatchItemCreation_JsonResult.IsSuccess = true;
            }
            catch (Exception error)
            {
                BatchItemCreation_JsonResult.IsSuccess = false;
                logger.Error("ErrorMsg: " + error.Message + "; [StackTrace]: " + error.StackTrace);
            }
            if (BatchItemCreation_JsonResult.IsSuccess == true)
            {
                return Json(_strListMsg);
            }
            else
            {
                return Json("系統錯誤");
            }

            #endregion
            #region Bill's architecture version
            //ActionResponse<List<string>> BatchItemCreation_result = new ActionResponse<List<string>>();
            //ActionResponse<bool> connectToApiResult = new ActionResponse<bool>();
            //try
            //{
            //    BatchItemCreation_result = apiConnvetor.BatchItemCreation(_itemSketch, userEmail, passWord);
            //    connectToApiResult.Body = true;
            //    connectToApiResult.IsSuccess = true;
            //    //var BatchItemCreation_result = apiConnvetor.BatchItemCreation(_itemSketch, userEmail, passWord);
            //}
            //catch (Exception error)
            //{
            //    logger.Error("Message: " + error.Message + "; [StackTrace]" + error.StackTrace);
            //    connectToApiResult.Body = false;
            //    connectToApiResult.IsSuccess = false;
            //}
            ////連接 api 失敗，Exception
            //if (connectToApiResult.Body == false)
            //{
            //    logger.Error("連接 api 失敗，Exception。Line: 111");
            //    return Json("系統錯誤");
            //}
            ////因為連接 api 失敗，回傳 null 給 BatchItemCreation_result，必須做判斷，不然程式會死掉 
            //if (BatchItemCreation_result == null)
            //{
            //    logger.Error("因為連接 api 失敗，回傳 null 給 BatchItemCreation_result，必須做判斷，不然程式會死掉。Line: 116");
            //    return Json("伺服器錯誤");
            //}
            ////成功連接 api 並請有回傳結果
            //foreach (var listErrorMsg in BatchItemCreation_result.Body)
            //{
            //    _strListMsg = _strListMsg + listErrorMsg + "。";
            //}
            //return Json(_strListMsg);
            //if (BatchItemCreation_result.IsSuccess == false)
            //{
            //    foreach (var listErrorMsg in BatchItemCreation_result.Body)
            //    {
            //        _strListMsg = _strListMsg + listErrorMsg + "。";
            //    }
            //    //_strListMsg = BatchItemCreation_result.Msg + ": " + _strListMsg;
            //    return Json(_strListMsg);
            //}
            //else
            //{
            //    return Json(BatchItemCreation_result.Msg);
            //}
            #endregion
        }
        #endregion
        #region 屬性商品批次送審
        public JsonResult propertyExamine(List<TWNewEgg.API.Models.BatchExamineModel> batchExamineModel, string userEmail, string passWord, int time = 1)
        {
            TWNewEgg.API.Models.Connector connector = new Connector();
            string returnMsg = string.Empty;
            if (batchExamineModel == null)
            {
                return Json("請傳入資料");
            }
            else
            {

            }
            if (string.IsNullOrEmpty(userEmail) == true || string.IsNullOrEmpty(passWord) == true)
            {
                return Json("請輸入信箱或密碼");
            }
            ActionResponse<string> propertyExamineresult = new ActionResponse<string>();
            List<TWNewEgg.API.Models.BatchExamineModel> ListBatchModel = new List<BatchExamineModel>();
            for (int i = 0; i < time; i++)
            {
                ListBatchModel.Add(batchExamineModel[0]);
            }
            try
            {
                propertyExamineresult = connector.propertyExamine(ListBatchModel, userEmail, passWord);
                returnMsg = propertyExamineresult.Body;
            }
            catch (Exception error)
            {
                returnMsg = "系統錯誤或 time out";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            return Json(returnMsg);
        }
        #endregion

        #region 取得Property Value ID

        /// <summary>
        /// 提供 Vendor 取得規格品的商品屬性 Value ID
        /// </summary>
        /// <param name="userEmail">登入帳號</param>
        /// <param name="passWord">登入密碼</param>
        /// <param name="accountType">登入身份別</param>
        /// <param name="categoryID">主分類_第 2 層分類 ID</param>
        /// <returns>商品屬性 Value ID</returns>
        [HttpPost]
        public JsonResult GetCategoryPropertyValueID(string userEmail, string passWord, string accountType, int categoryID)
        {
            // API Connector
            TWNewEgg.API.Models.Connector connector = new Connector();

            // 登入資訊
            TWNewEgg.API.Models.UserLogin userloginInfo = new UserLogin();

            // 登入結果
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> loginResult = new ActionResponse<UserLoginResult>();

            #region 輸入檢查

            if (categoryID < 0)
            {
                return Json("商品分類 ID 錯誤。");
            }

            if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(passWord))
            {
                return Json("請輸入信箱及密碼。");
            }

            #endregion 輸入檢查

            #region 填寫登入資訊

            if (string.IsNullOrEmpty(accountType))
            {
                accountType = "V";
            }

            userloginInfo.UserEmail = userEmail;
            userloginInfo.Password = passWord;
            userloginInfo.VendorSeller = accountType;

            #endregion 填寫登入資訊

            try
            {
                loginResult = connector.Login(null, null, userloginInfo);
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("登入失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                return Json("系統錯誤或操作逾時。", JsonRequestBehavior.AllowGet);
            }

            switch (loginResult.Code)
            {
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Success:
                    {
                        TWNewEgg.API.Models.ActionResponse<List<twoDimPropertyValue>> getPropertyValue = GetPropertyValue(categoryID);

                        if (getPropertyValue.IsSuccess)
                        {
                            return Json(getPropertyValue.Body, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(getPropertyValue.Msg, JsonRequestBehavior.AllowGet);
                        }
                    }
                default:
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Error:
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.PasswordFaild:
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.AccountTypeError:
                    {
                        logger.Info(string.Format("登入失敗; ErrorMessage = 登入帳號、密碼或身份別錯誤; UserEmail = {0}; PassWord = {1}; AccountType = {2}.", userEmail, passWord, accountType));
                        return Json("登入帳號、密碼或身份別錯誤。", JsonRequestBehavior.AllowGet);
                    }
                case (int)TWNewEgg.API.Models.UserLoginingResponseCode.Accountalreadystop:
                    {
                        logger.Info(string.Format("登入失敗; ErrorMessage = 此商家帳號已經停止使用; UserEmail = {0}; PassWord = {1}; AccountType = {2}.", userEmail, passWord, accountType));
                        return Json("此商家帳號已經停止使用，請聯絡新蛋客服。", JsonRequestBehavior.AllowGet);
                    }
            }
        }

        /// <summary>
        /// 查詢規格品屬性編號
        /// </summary>
        /// <param name="categoryID">主分類_第 2 層分類 ID</param>
        /// <returns>規格品屬性編號</returns>
        private TWNewEgg.API.Models.ActionResponse<List<twoDimPropertyValue>> GetPropertyValue(int categoryID)
        {
            // API Connector
            TWNewEgg.API.Models.Connector connector = new Connector();

            // 規格品屬性編號
            TWNewEgg.API.Models.ActionResponse<List<twoDimPropertyValue>> result = new TWNewEgg.API.Models.ActionResponse<List<twoDimPropertyValue>>();
            result.Body = new List<twoDimPropertyValue>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            try
            {
                TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.PropertyResult>> getPropertyResult = connector.GetProperty(null, null, categoryID);

                if (getPropertyResult.Body != null)
                {
                    // 依可使用的規格屬性篩選
                    foreach (TWNewEgg.API.Models.PropertyResult property in getPropertyResult.Body)
                    {
                        twoDimPropertyValue tempdimProperty = new twoDimPropertyValue();
                        switch (property.PropertyName.ToLower())
                        {
                            case "color":
                            case "顏色":
                                {
                                    tempdimProperty.PropertyName = property.PropertyName;
                                    tempdimProperty.PropertyValues.AddRange(property.ValueInfo.Where(x => !string.IsNullOrEmpty(x.Value)).Select(r => new TWNewEgg.API.Models.ViewModel.twoDimPropertyValue.StandPropertyValue { Value = r.Value, ValueID = r.ValueID }));

                                    result.Body.Add(tempdimProperty);
                                    break;
                                }
                            case "size":
                            case "尺寸":
                                {
                                    tempdimProperty.PropertyName = property.PropertyName;
                                    tempdimProperty.PropertyValues.AddRange(property.ValueInfo.Where(x => !string.IsNullOrEmpty(x.Value)).Select(r => new TWNewEgg.API.Models.ViewModel.twoDimPropertyValue.StandPropertyValue { Value = r.Value, ValueID = r.ValueID }));

                                    result.Body.Add(tempdimProperty);
                                    break;
                                }
                            default:
                                {
                                    // 包含 尺寸 字樣的
                                    if (property.PropertyName.ToLower().Contains("尺寸"))
                                    {
                                        tempdimProperty.PropertyName = property.PropertyName;
                                        tempdimProperty.PropertyValues.AddRange(property.ValueInfo.Where(x => !string.IsNullOrEmpty(x.Value)).Select(r => new TWNewEgg.API.Models.ViewModel.twoDimPropertyValue.StandPropertyValue { Value = r.Value, ValueID = r.ValueID }));

                                        result.Body.Add(tempdimProperty);
                                    }
                                    break;
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "發生意外錯誤，請稍後再試。";
                logger.Info(string.Format("查詢規格品屬性編號失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            if (result.Body.Count == 0)
            {
                result.IsSuccess = false;
                result.Msg = "此類別無顏色、尺寸屬性。";
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        /// <summary>
        /// 設定 Reponse Code
        /// </summary>
        /// <param name="isSuccess">成功失敗資訊</param>
        /// <returns>Reponse Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)ResponseCode.Success;
            }
            else
            {
                return (int)ResponseCode.Error;
            }
        }

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }

        #endregion
    }
}
