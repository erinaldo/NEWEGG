using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.View.Controllers
{
    public class TwoDimensionPropertyController : Controller
    {
        //
        // GET: /TwoDimensionProperty/
        TWNewEgg.API.Models.Connector conn = new API.Models.Connector(); 
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        TWNewEgg.API.View.Service.SellerInfoService sellerinfoService = new Service.SellerInfoService();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult MakePendingEditData(TWNewEgg.API.View.ItemCreationVM newItemData, string pictureURL)
        {
            JsonResult result = new JsonResult();

            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
            var UserType = sellerInfo.IsAdmin;
            if (newItemData.Item.ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart
                && UserType == false)
            {
                List<string> errorMessage = new List<string>();
                errorMessage.Add("F");
                errorMessage.Add("商品儲存失敗此商品為加價購商品，請聯繫相關 PM 編輯!");
                result = Json(errorMessage);
                return result;
            }
            //if (!UserType)
            //{
            //    List<string> errorMessage = new List<string>();
            //    errorMessage.Add("F");
            //    errorMessage.Add("商品儲存失敗請重新確認資料是否填寫正確或與客服聯繫");
            //    result = Json(errorMessage);
            //    return result;
            //}

            #region 解密已售值

            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            int itemQtyReg = 0;
            int inventoryQtyReg = 0;
            
            // 判斷解密是否成功
            bool parseItemQtyReg = false;
            bool parseInventoryQtyReg = false;
            try
            {
                parseItemQtyReg = int.TryParse(aes.AesDecrypt(newItemData.AesItemQtyReg), out itemQtyReg);
                parseInventoryQtyReg = int.TryParse(aes.AesDecrypt(newItemData.AesInventoryQtyReg), out inventoryQtyReg);
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("已售值解密失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            #endregion 解密已售值

            if (parseInventoryQtyReg && parseItemQtyReg)
            {
                // 回填已售值
                newItemData.Item.ItemQtyReg = itemQtyReg;
                newItemData.ItemStock.InventoryQtyReg = inventoryQtyReg;

                // 加入圖片
                newItemData.Product.PicPatch_Edit.Add(pictureURL);

                // 填寫登入資訊
                newItemData.CreateAndUpdate.UpdateUser = sellerinfoService.UserID;

                result = PropertyEdit(newItemData, null);
            }
            else
            {
                List<string> errorMessage = new List<string>();
                errorMessage.Add("F");
                errorMessage.Add("儲存失敗。");
                result = Json(errorMessage);
            }
            return result;
        }

        [HttpPost]
        public JsonResult PropertyEdit(TWNewEgg.API.View.ItemCreationVM newItemData, TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            bool isNoException = true;
            int sellerid = sellerinfoService.currentSellerID;
            ActionResponse<TWNewEgg.API.Models.ItemSketch> autoMapModelResult = new ActionResponse<API.Models.ItemSketch>();
            autoMapModelResult = this.ItemCreationVM_To_Itemsketch(newItemData);
            autoMapModelResult.Body.Item.SellerID = sellerid;
            List<string> returnresult = new List<string>();
            if (autoMapModelResult.IsSuccess == false)
            {
                returnresult.Add("F");
                returnresult.Add("資料處理失敗");
                return Json(returnresult);
            }
            try
            {

                result = conn.ItemPropertyOpenViewEdit(autoMapModelResult.Body);
                isNoException = true;
            }
            catch (Exception error)
            {
                isNoException = false;
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
            }
            //發生exception
            if (isNoException == false)
            {
                returnresult.Add("F");
                returnresult.Add("資料處理失敗");
                return Json(returnresult);
            }
            if (result.IsSuccess == false)
            {
                returnresult.Add("F");
                returnresult.Add("資料處理失敗");
                return Json(returnresult);
            }
            else
            {
                returnresult.Add("T");
                returnresult.Add(result.Msg);
                return Json(returnresult);
            }

        }
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketch> ItemCreationVM_To_Itemsketch(TWNewEgg.API.View.ItemCreationVM newItemData)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ItemSketch> result = new API.Models.ActionResponse<API.Models.ItemSketch>();
            try
            {
                TWNewEgg.API.Models.ItemSketch itemsketchModel = new API.Models.ItemSketch();
                AutoMapper.Mapper.CreateMap<TWNewEgg.API.View.ItemCreationVM, TWNewEgg.API.Models.ItemSketch>();
                AutoMapper.Mapper.Map(newItemData, itemsketchModel);
                result.IsSuccess = true;
                result.Body = itemsketchModel;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料處理失敗";
                logger.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
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

    }
}
