using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Attributes;
using TWNewEgg.API.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace TWNewEgg.API.Controllers
{
    public class TwoDimProductPropertyController : Controller
    {

        /// <summary>
        /// 建立二維屬性商品草稿
        /// </summary>
        /// <param name="twodimItemSketch">商品資訊</param>
        /// <returns>成功、失敗訊息</returns>
        public JsonResult TwoDimensionProductCreate(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch twodimItemSketch)
        {
            ActionResponse<string> result = new ActionResponse<string>();

            TWNewEgg.API.Service.StandardProductsService service = new Service.StandardProductsService();
            result = service.twoDimPropertyCreate(twodimItemSketch);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 二維商品建立送審
        /// </summary>
        /// <param name="twodimItemSketch"></param>
        /// <returns></returns>
        public JsonResult TwoDimensionProductCreateExamine(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch twodimItemSketch, int userid, int sellerid, bool isNewItem = true)
        {
            TWNewEgg.API.Service.StandardProductsService service = new Service.StandardProductsService();
            var result = service.TwoDimensionProductCreateExamine(twodimItemSketch, userid, sellerid, isNewItem);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 二維屬性商品草稿編輯
        /// </summary>
        /// <param name="twodimItemSketch">商品資訊</param>
        /// <returns>成功、失敗訊息</returns>
        public JsonResult TwoDimensionProductDetailEdit(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch twodimItemSketch)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            TWNewEgg.API.Service.StandardProductsService service = new Service.StandardProductsService();
            result = service.UpdateTwoDimensionProductDetailEdit(twodimItemSketch);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查詢規格商品草稿
        /// </summary>
        /// <param name="condition">草稿查詢條件</param>
        /// <returns>規格商品草稿</returns>
        public JsonResult GetTwoDimensionProduct(ItemSketchSearchCondition condition, bool isTempCopy = false)
        {
            ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> result = new ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch>();

            TWNewEgg.API.Service.StandardProductsService service = new Service.StandardProductsService();
            result = service.GetTwoDimensionProduct(condition, isTempCopy);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
