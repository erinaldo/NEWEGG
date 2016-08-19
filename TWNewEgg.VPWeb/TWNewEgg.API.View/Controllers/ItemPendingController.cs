using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.View.Controllers
{
    public class ItemPendingController : Controller
    {
        //
        // GET: /ItemSketchPending/
        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        Connector conn = new Connector();

        /// <summary>
        /// ItemSketch送審
        /// </summary>
        /// <param name="pendingData">送審資訊</param>
        /// <param name="actionType">執行送審的類型</param>
        /// <returns>返回送審API執行結果</returns>
        public ActionResult ItemSketchPending(TWNewEgg.API.View.ItemCreationVM pendingData, int actionType)
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            ActionResponse<string> pendingServiceResult = new ActionResponse<string>();
            List<TWNewEgg.API.Models.ItemSketch> saveItemSketchList = new List<ItemSketch>();
            TWNewEgg.API.Models.ItemSketch getItemSketch = new ItemSketch();

            pendingData.Item.SellerID = sellerInfo.currentSellerID;
            pendingData.CreateAndUpdate.UpdateUser = sellerInfo.UserID;

            int getItemQtyReg = 0;
            int getInventoryQtyReg = 0;

            int.TryParse(aes.AesDecrypt(pendingData.AesItemQtyReg), out getItemQtyReg);
            pendingData.Item.ItemQtyReg = getItemQtyReg;

            int.TryParse(aes.AesDecrypt(pendingData.AesInventoryQtyReg), out getInventoryQtyReg);
            pendingData.ItemStock.InventoryQtyReg = getInventoryQtyReg;

            pendingData.CreateAndUpdate.CreateUser = sellerInfo.UserID;

            Mapper.CreateMap<ItemCreationVM, TWNewEgg.API.Models.ItemSketch>();
            getItemSketch = Mapper.Map<TWNewEgg.API.Models.ItemSketch>(pendingData);
            saveItemSketchList.Add(getItemSketch);
            try
            {
                pendingServiceResult = conn.SendItemSketchToPending(saveItemSketchList, sellerInfo.UserID.ToString(), actionType);
                if (pendingServiceResult.IsSuccess == true)
                {
                    return Json(new { IsSuccess = true, SuccessMsg = "資料送審成功" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, ErrorMsg = pendingServiceResult.Msg }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                logger.Error("商品送審失敗 [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                return Json(new { IsSuccess = false, ErrorMsg = "商品送審失敗請重新確認資料是否填寫正確或與客服聯繫" }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }
    }
}
