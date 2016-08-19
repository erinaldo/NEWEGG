using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API = TWNewEgg.API.Models;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// 庫存警示 Controller
    /// </summary>
    public class ItemInventoryAlertController : Controller
    {      
        /// <summary>
        /// 搜尋庫存警示資料，依據KeyWord 及 SellerID 依據條件找出屬於此 SellerID 的庫存資料
        /// </summary>
        /// <param name="searchItem">輸入keyword、SellerID、PageInfo</param>
        /// <returns>Return search iteminventoryinfo results </returns>
        [HttpPost]
        [Attributes.ActionDescription("搜尋庫存警示資料")]
        public JsonResult SearchItemInventoryAlert(API.Models.InventoryAlertSearchModel searchItem)
        {
            API.Models.ActionResponse<List<API.Models.VM_ItemInventoryAlertInfo>> result = new Models.ActionResponse<List<API.Models.VM_ItemInventoryAlertInfo>>();

            Service.ItemInventoryAlertService inventoryService = new Service.ItemInventoryAlertService();

            result = inventoryService.SearchItemInventoryInfo(searchItem);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
             
        /// <summary>
        /// 編輯 ItemInventoryAlert
        /// </summary>
        /// <param name="saveInventoryInfo">傳入要新增或修改的ItemInventoryInfoList</param>
        /// <returns>Success or faild to save iteminventoryinfolist</returns>
        [HttpPost]
        [Attributes.ActionDescription("儲存庫存警示更新資料")]
        public JsonResult SaveAllUpdateInventoryAlert(List<API.Models.VM_ItemInventoryAlertInfo> saveInventoryInfo)
        {
            API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>> result = new Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>>();

            Service.ItemInventoryAlertService inventoryService = new Service.ItemInventoryAlertService();

            result = inventoryService.SaveAllUpdateInventoryAlert(saveInventoryInfo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
               
        /// <summary>
        /// 刪除 ItemInventoryAlert
        /// </summary>
        /// <param name="deleteInventoryList">Delete InventoryInfo List</param>
        /// <returns>Delete success or faild</returns>
        [HttpPost]
        [Attributes.ActionDescription("刪除庫存警示資料")]
        public JsonResult DeleteItemInventoryAlert(List<API.Models.DeleteItemInventory> deleteInventoryList)
        {
            API.Models.ActionResponse<List<string>> result = new Models.ActionResponse<List<string>>();

            Service.ItemInventoryAlertService inventoryService = new Service.ItemInventoryAlertService();

            result = inventoryService.DeleteInventoryAlert(deleteInventoryList);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
     
        /// <summary>
        ///  寄出 InventoryAlert Mail
        /// </summary>
        /// <param name="inventoryMailInfo">Send iteminventory mail by seller email</param>
        /// <returns>Success or fail send mail </returns>
        [HttpPost]
        [Attributes.ActionDescription("寄送庫存警示Email")]
        public JsonResult SendInventoryAlertEmail(API.Models.ItemInventoryMailInfo inventoryMailInfo)
        {
            API.Models.ActionResponse<API.Models.MailResult> result = new Models.ActionResponse<API.Models.MailResult>();

            Service.ItemInventoryAlertService inventoryService = new Service.ItemInventoryAlertService();

            result = inventoryService.SendInventoryAlertEmail(inventoryMailInfo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
