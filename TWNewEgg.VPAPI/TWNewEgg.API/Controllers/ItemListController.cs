using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Service;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.API.Attributes;
using System.Transactions;

namespace TWNewEgg.API.Controllers
{
    /// <summary>
    /// ItemList
    /// </summary>
    public class ItemListController : Controller
    {
        //// GET: /ItemList/

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns>ItemList 主頁</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 所有商品列表
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>Call ItemList</returns>
        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemList)]
        [ActiveKey(ActiveKeyAttributeValues.View)]
        [ActionDescription("所有商品列表")]
        public JsonResult GetItemList(TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();

            Models.ActionResponse<List<Models.ItemInfoList>> result = new Models.ActionResponse<List<Models.ItemInfoList>>();

            result = its.ItemList(itemSearch);

            return Json(result);
        }

        /// <summary>
        /// 所有由新蛋運送商品
        /// </summary>
        /// <param name="itemSearch">itemSearch</param>
        /// <returns>Call ItemFBNList</returns>
        public JsonResult ItemFBNList(TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();

            Models.ActionResponse<List<Models.ItemInfoList>> result = new Models.ActionResponse<List<Models.ItemInfoList>>();

            result = its.ItemFBNList(itemSearch);

            return Json(result);
        }

        /// <summary>
        /// 修改查詢後商品
        /// </summary>
        /// <param name="itemInfoList">itemInfoList</param>
        /// <returns>Call ItemModify</returns>
        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemList)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("修改查詢後商品")]
        public JsonResult ItemModify(List<Models.ItemInfoList> itemInfoList)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();

            Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            //using (TransactionScope scope = new TransactionScope())
            //{
                result = its.ItemModify(itemInfoList);

            //    if (result.IsSuccess == true)
            //    {
            //        scope.Complete();
            //    }
            //}

            return Json(result);
        }

        /// <summary>
        /// 修改新蛋運送之商品資訊
        /// </summary>
        /// <param name="itemInfoList">itemInfoList</param>
        /// <returns>Call ShippedByNeweggModify</returns>
        public JsonResult ShippedByNeweggModify(List<Models.ItemInfoList> itemInfoList)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();

            Models.ActionResponse<string> result = new Models.ActionResponse<string>();

            //using (TransactionScope scope = new TransactionScope())
            //{
                result = its.ShippedByNeweggModify(itemInfoList);
                //if (result.IsSuccess == true)
                //{
                //    scope.Complete();
                //}
            //}

            return Json(result);
        }

        /// <summary>
        /// 查詢分類目錄
        /// </summary>
        /// <param name="layer">layer</param>
        /// <param name="parentID">parentID</param>
        /// <returns>Call QueryCategory</returns>
        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemCreation)]
        [ActiveKey(ActiveKeyAttributeValues.View)]
        [ActionDescription("QueryCategory")]
        public JsonResult QueryCategory(int? layer, int? parentID)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> result = new Models.ActionResponse<List<DB.TWSQLDB.Models.Category>>();
            result = its.QueryCategory(layer, parentID);
            return Json(result);
        }

        /// <summary>
        /// 計算主類別子類別總量
        /// </summary>
        /// <returns>Call CountCategory</returns>
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemCreation)]
        [ActiveKey(ActiveKeyAttributeValues.View)]
        [ActionDescription("CountCategory")]
        public JsonResult CountCategory()
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Dictionary<string, int> result = its.CountCategory();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 測試用API
        /// </summary>
        /// <param name="loc">loc</param>
        /// <returns>測試 邏輯直接寫在這</returns>
        public JsonResult TestAPI(string loc)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(loc);
            string line;
            line = file.ReadLine();
            file.Close();

            return Json(line, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增ProductDetail資料
        /// </summary>
        /// <param name="addProducts">addProducts</param>
        /// <returns>Call AddProductDetail</returns>
        public JsonResult AddProductDetail(List<Models.ProductDetail> addProducts)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            result = its.AddProductDetail(addProducts);
            return Json(result);
        }

        /// <summary>
        /// 刪除賣場，賣場狀態改為99
        /// </summary>
        /// <param name="deleteItem">刪除商品</param>
        /// <returns>回傳成功失敗</returns>
        public JsonResult DeleteItem(Models.ItemInfoList deleteItem)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            result = its.DeleteItem(deleteItem);
            return Json(result);
        }

        public JsonResult UpdateItemStatus(Models.ItemInfoList updateStatusItem)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            result = its.UpdateItemStatus(updateStatusItem);
            return Json(result);
        }

        /// <summary>
        /// 搜尋創建商品
        /// </summary>
        /// <param name="sellerID">賣家</param>
        /// <param name="itemID">商品編號</param>
        /// <returns>回傳結果</returns>
        public JsonResult SearchCreatedItem(int sellerID, int itemID)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<Models.APIItemModel> result = new Models.ActionResponse<Models.APIItemModel>();
            result = its.SearchCreatedItem(sellerID, itemID);
            return Json(result);
        }

        /// <summary>
        /// 修改創建商品
        /// </summary>
        /// <param name="sellerID">賣家</param>
        /// <param name="itemID">商品編號</param>
        /// <returns>回傳結果</returns>
        public JsonResult EditCreatedItem(Models.APIItemModel editItem)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            //using (TransactionScope scope = new TransactionScope())
            //{
                result = its.EditCreatedItem(editItem);
                
            //    if (result.IsSuccess == true)
            //    {
            //        scope.Complete();
            //    }
            //}
            return Json(result);
        }

        /// <summary>
        /// 上傳商品圖
        /// </summary>
        /// <param name="pictureURL"></param>
        /// <param name="productID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public JsonResult PostImageToDB(List<string> pictureURL, string productID, string itemID)
        {
            TWNewEgg.API.Service.AddProductsInfoService add = new TWNewEgg.API.Service.AddProductsInfoService();
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            result = add.PostImageToDB(pictureURL, productID, itemID);
            return Json(result);
        }
        
        [HttpGet]
        public JsonResult APICountActiveItem(string QueryDate)
        {
            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();
            Models.ActionResponse<Dictionary<string, int>> result = new Models.ActionResponse<Dictionary<string, int>>();
            result = its.CountActiveItem(QueryDate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SendProductContentTOPM()
        {
            API.Models.ActionResponse<List<API.Models.ItemInfoList>> result = new Models.ActionResponse<List<API.Models.ItemInfoList>>();

            TWNewEgg.API.Service.ItemService its = new TWNewEgg.API.Service.ItemService();

            result = its.SendPMMail();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 匯出 Excel 商品列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult DownloadItemList(API.Models.DownloadItemListModel dataInfo)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            Service.ItemService serviceIS = new Service.ItemService();

            result = serviceIS.DownloadItemList(dataInfo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 匯出 Excel 商品列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult DownloadItemListToExcel(API.Models.DownloadItemListModel dataInfo)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            Service.ItemService serviceIS = new Service.ItemService();

            result = serviceIS.DownloadItemListToExcel(dataInfo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 匯入 Excel 商品列表
        /// </summary>
        /// <param name="UpdateItemInfo">匯入Item資訊</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult BatchItemUpdate(string fileName, string sheetName, string UserID)
        {
            Models.ActionResponse<List<Models.ActionResponse<string>>> result = new Models.ActionResponse<List<Models.ActionResponse<string>>>();
            Service.BatchItemUpdate updService = new Service.BatchItemUpdate();

            result = updService.LinqFromExcel(fileName, sheetName, UserID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemBatchCreate<T>(IEnumerable<T> dataList)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            
            return Json(result);
        }
        [HttpPost]
        public JsonResult CheckCategoryParentId(int main, List<int> checkcategoryid)
        {
            Models.ActionResponse<bool> result = new Models.ActionResponse<bool>();
            Service.ItemSketchService checkSketch = new ItemSketchService();
            result = checkSketch.CheckCategoryParentId(main, checkcategoryid);
            return Json(result);
        }
    }
}
