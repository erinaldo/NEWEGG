using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.View.Controllers
{
    public class TwoDimensionProductDetailEditController : Controller
    {
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region 產生 View 畫面

        /// <summary>
        /// 規格商品草稿編輯
        /// </summary>
        /// <param name="itemSketchID">草稿ID</param>
        /// <param name="dataSendType">判斷是修改(DetailEdit)或是複製(Copy)</param>
        /// <returns>編輯畫面</returns>
        public ActionResult TwoDimensionProductDetailEdit(int itemSketchID, string dataSendType, string from)
        {
            bool isGetSketchDataSuccess = true;
            TWNewEgg.API.View.ItemCreationVM viewModel = new ItemCreationVM();

            if (itemSketchID > 0 && (dataSendType == "DetailEdit" || dataSendType == "Copy"))
            {
                if (dataSendType == "DetailEdit")
                {
                    ViewBag.From = "EditItem";
                }
                else
                {
                    ViewBag.From = "NewItem";
                }

                // 讀取草稿資料
                TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> getSketchData = GetSketchData(itemSketchID, from);

                if (getSketchData.IsSuccess)
                {
                    // 轉換 View model
                    AutoMapper.Mapper.CreateMap<TWNewEgg.API.Models.ItemSketch, TWNewEgg.API.View.ItemCreationVM>();
                    try
                    {
                        viewModel = AutoMapper.Mapper.Map<TWNewEgg.API.View.ItemCreationVM>(getSketchData.Body.basicItemInfo[0]);
                    }
                    catch (Exception ex)
                    {
                        isGetSketchDataSuccess = false;
                        log.Info(string.Format("轉換 View model 失敗(exception); ErrorMessage = {0}; StackTrack = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                    }

                    #region 設定儲存事件的 Controller 路徑

                    if (isGetSketchDataSuccess)
                    {
                        switch (dataSendType)
                        {
                            case "DetailEdit":
                                {
                                    viewModel.SaveActionUrl = "/TwoDimensionProductDetailEdit/EditSketch";
                                    break;
                                }
                            case "Copy":
                                {
                                    viewModel.SaveActionUrl = "/TwoDimensionProductCreation/CreateNewItem";
                                    break;
                                }
                            default:
                                {
                                    isGetSketchDataSuccess = false;
                                    log.Info(string.Format("設定儲存事件的 Controller 路徑發生錯誤; dataSendType = {0}.", dataSendType));
                                    break;
                                }
                        }
                    }

                    #endregion 設定儲存事件的 Controller 路徑

                    #region 加密二維屬性資料

                    if (isGetSketchDataSuccess)
                    {
                        TWNewEgg.API.View.Service.AES aes = new Service.AES();
                        try
                        {
                            viewModel.AesItemProperty = aes.AesEncrypt(JsonConvert.SerializeObject(getSketchData.Body.twodimProperty.ItemProperty));
                        }
                        catch (Exception ex)
                        {
                            isGetSketchDataSuccess = false;
                            log.Info(string.Format("加密二維屬性失敗; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                        }
                    }

                    #endregion 加密二維屬性資料

                    #region 加密圖片路徑

                    if (isGetSketchDataSuccess && getSketchData.Body.basicItemInfo[0].Product.PicPatch_Edit.Count > 0)
                    {
                        TWNewEgg.API.View.Service.AES aes = new Service.AES();
                        try
                        {
                            viewModel.AesPictureUrlCell = aes.AesEncrypt(JsonConvert.SerializeObject(getSketchData.Body.basicItemInfo[0].Product.PicPatch_Edit));
                        }
                        catch (Exception ex)
                        {
                            isGetSketchDataSuccess = false;
                            log.Info(string.Format("加密圖片路徑失敗; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                        }
                    }

                    #endregion 加密圖片路徑
                }
                else
                {
                    isGetSketchDataSuccess = false;
                }
            }
            else
            {
                isGetSketchDataSuccess = false;
                log.Info(string.Format("傳入資料錯誤; ItemSketchID = {0}; DataSendType = {1}.", itemSketchID, dataSendType));
            }

            ViewBag.IsGetSketchDataSuccess = isGetSketchDataSuccess;

            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
            ActionResponse<string> UserInfo = new ActionResponse<string>();
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
            ViewBag.userType = sellerInfo.IsAdmin;

            return PartialView(viewModel);
        }

        /// <summary>
        /// 產生各層分類下拉式選單UI
        /// </summary>
        /// <returns>各層分類下拉式選單UI</returns>
        [HttpGet]
        public ActionResult Category(TWNewEgg.API.Models.ItemSketch_ItemCategory itemCategory)
        {
            return PartialView("~/Views/TwoDimensionProductCreation/Category.cshtml", itemCategory);
        }

        /// <summary>
        /// 讀取草稿資料
        /// </summary>
        /// <param name="itemSketchID">草稿 ID</param>
        /// <returns>草稿資料</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> GetSketchData(int itemSketchID, string from)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> result = new API.Models.ActionResponse<API.Models.DomainModel.CreateStandardProductSketch>();

            // 使用者資訊
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();

            // 組合查詢條件
            TWNewEgg.API.Models.ItemSketchSearchCondition itemSketchSearchCondition = new API.Models.ItemSketchSearchCondition();
            itemSketchSearchCondition.SellerID = sellerInfo.currentSellerID;
            itemSketchSearchCondition.KeyWordScarchTarget = API.Models.ItemSketchKeyWordSearchTarget.ItemSketchID;
            itemSketchSearchCondition.KeyWord = itemSketchID.ToString();

            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
            bool isTempCopy = false;
            try
            {
                if (from == "sketch")
                {
                    isTempCopy = false;
                }
                else
                {
                    isTempCopy = true;
                }
                result = connector.GetTwoDimensionProductDetailData(itemSketchSearchCondition, isTempCopy);
            }
            catch (Exception ex)
            {
                result.Finish(false, (int)TWNewEgg.API.Models.ResponseCode.Error, string.Empty, null);
                log.Info(string.Format("讀取草稿資料失敗(exception); ItemSketchID = {0}; ErrorMessage = {1}; StackTrace = {2}.", itemSketchID, GetExceptionMessage(ex), ex.StackTrace));
            }

            return result;
        }

        #endregion 產生 View 畫面

        #region 儲存編輯
        /// <summary>
        /// 儲存規格商品草稿編輯
        /// </summary>
        /// <param name="newItemData">草稿資料</param>
        /// <param name="twoDimenstionProperty">二維屬性資料</param>
        /// <returns>成功、失敗訊息</returns>
        public ActionResult EditSketch(TWNewEgg.API.View.ItemCreationVM newItemData, TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty)
        {
            log.Info("規格品編輯開始");
            
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();           

            TWNewEgg.API.View.Service.TwoDimensionProductService service = new TWNewEgg.API.View.Service.TwoDimensionProductService();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> makeCreateStandardProductSketch = service.MakeCreateStandardProductSketch(newItemData, twoDimenstionProperty);
            if (makeCreateStandardProductSketch.IsSuccess)
            {
                TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
                try
                {
                    result = connector.TwoDimensionProductDetailEdit(makeCreateStandardProductSketch.Body);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("規格品編輯失敗(exception); ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            if (result.IsSuccess)
            {
                result.Body = "儲存成功。";
            }
            else
            {
                result.Body = "儲存失敗。";
            }

            log.Info("規格品編輯結束");

            return Json(new { isSuccess = result.IsSuccess, message = result.Body }, JsonRequestBehavior.AllowGet);
        }

        #endregion 儲存編輯

        #region View 轉 string、取得 Exception 訊息

        /// <summary>
        /// 將該View轉成string
        /// </summary>
        /// <param name="partialView">View的名稱</param>
        /// <returns>返回string</returns>
        private string RenderView(string partialView)
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

        #endregion View 轉 string、取得 Exception 訊息
    }
}