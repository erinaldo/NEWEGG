using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// Manufacturer Controller
    /// </summary>
    public class ManufacturerController : Controller
    {
        private Service.ManufacturerService connect_ManufacturerService = new Service.ManufacturerService();

        /// <summary>
        /// 新增製造商
        /// </summary>
        /// <param name="manufacturer">要寫入資料庫的新增清單</param>
        /// <returns>成功、失敗資訊</returns>
        [Attributes.ActionDescriptionAttribute("建立製造商資料")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult CreateManufacturerInfo(List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit> manufacturer)
        {
            Service.ManufacturerService serviceMS = new Service.ManufacturerService();
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>> result = new ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>>();

            result = serviceMS.CreateListManufacturerInfo(manufacturer);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  建立新Email
        /// </summary>
        /// <param name="intSellerID">商家ID</param>
        /// <param name="strUserEmail">新增的使用者信箱</param>
        /// <param name="intInUserID">建立人UserID</param>
        /// <returns>新建立的使用者ID</returns>
        [Attributes.ActionDescription("新增使用者電子郵件")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult CreateUserEmail(int intSellerID = 0, string strUserEmail = null, int intInUserID = 0)
        {
            Service.ManufacturerService serviceMS = new Service.ManufacturerService();
            API.Models.ActionResponse<int> result = new ActionResponse<int>();

            result = serviceMS.CreateUserEmail(intSellerID, strUserEmail, intInUserID);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 編輯製造商
        /// </summary>
        /// <param name="updateinfo">要寫入資料庫的編輯清單</param>
        /// <returns>編輯成功、失敗資訊</returns>
        [Attributes.ActionDescription("編輯製造商資料")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult EditManufacturerInfo(List<Models.Manufacturer> updateinfo)
        {
            Service.ManufacturerService serviceMS = new Service.ManufacturerService();
            Models.ActionResponse<string> result = new ActionResponse<string>();

            result = serviceMS.EditManufacturerInfo(updateinfo);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 讀取審核結果通知對象清單
        /// </summary>
        /// <param name="intSellerID">商家 ID</param>
        /// <returns>審核結果通知對象清單</returns>
        [Attributes.ActionDescription("取得審核結果通知對象電子郵件清單")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult GetEmailToList(int intSellerID = -1)
        {
            API.Models.ActionResponse<List<Models.ManufacturerEmailToListResultModel>> result = connect_ManufacturerService.GetEmailToList(intSellerID);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得製造商名稱
        /// </summary>
        /// <param name="intSellerID">商家 ID</param>
        /// <returns>製造商名稱</returns>
        [Attributes.ActionDescription("取得製造商名稱")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult GetSellerName(int intSellerID = -1)
        {
            Service.ManufacturerService service_MS = new Service.ManufacturerService();
            API.Models.ActionResponse<string> result = service_MS.GetSellerName(intSellerID);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 權限查詢
        /// </summary>
        /// <param name="intUserID">使用者 ID</param>
        /// <returns>ture：有審核權限，false：無審核權限</returns>
        [Attributes.ActionDescriptionAttribute("權限查詢")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult IsRatifyPermission(int intUserID)
        {
            Service.ManufacturerService serviceMS = new Service.ManufacturerService();
            API.Models.ActionResponse<bool> jsonResult = serviceMS.IsRatifyPermission(intUserID);

            return this.Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 搜尋製造商
        /// </summary>
        /// <param name="searchdata">搜尋條件</param>
        /// <returns>製造商列表</returns>
        [Attributes.ActionDescription("搜尋製造商資料")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult SearchManufacturerInfo(SearchDataModel searchdata)
        {
            Service.ManufacturerService serviceMS = new Service.ManufacturerService();
            API.Models.ActionResponse<List<Models.ManufacturerListResultModel>> result = serviceMS.SearchManufacturerInfo(searchdata);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 審核製造商
        /// </summary>
        /// <param name="updateinfo">審核資訊</param>
        /// <returns>審核成功及失敗訊息</returns>
        [Attributes.ActionDescription("審核製造商資料")]
        //[Filters.PermissionFilter]
        [HttpPost]
        public JsonResult UpdateStatus(Models.ManufacturerUpdateStatusInfo updateinfo)
        {
            Service.ManufacturerService serviceMS = new Service.ManufacturerService();
            API.Models.ActionResponse<string> result = serviceMS.UpdateStatus(updateinfo);

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
