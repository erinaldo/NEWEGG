using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Attributes;
using TWNewEgg.API.Models;
using TWNewEgg.API.Models.Models;

namespace TWNewEgg.API.Controllers
{
    /*---------- add by Ian and Thisway ----------*/
    /// <summary>
    /// Add Products Info (APIController)
    /// <para>Website Page:Create Products(all step)</para>
    /// </summary>
    public class AddProductsInfoController : Controller
    {
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.View)]
        [ActionDescription("商品創建/選擇商品類別")]
        public ActionResult GetProductCategory(CategoryResult Category)
        {
            Models.ActionResponse<List<CategoryResult>> result = null;

            Service.GetCategoryService AMIS = new Service.GetCategoryService();
            result = AMIS.GetProductCategory(Category);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("傳 Products Info 到 DB")]
        public ActionResult PostProductsInfoToAPIService(List<ProductsInfoResult> ProductsInfo)
        {
            ActionResponse<bool> result = null;
            if (ProductsInfo != null)
            {
                Service.AddProductsInfoService ProductsService = new Service.AddProductsInfoService();
                result = ProductsService.PostProductsInfoToDB(ProductsInfo);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("傳 Products Detail Info 到 DB")]
        public ActionResult PostDetailInfoToAPIService(List<DetailInfoResult> DetailInfo)
        {
            ActionResponse<bool> result = null;
            if (DetailInfo != null)
            {
                Service.AddProductsInfoService ProductsService = new Service.AddProductsInfoService();
                result = ProductsService.PostDetailInfoToDB(DetailInfo);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("傳 Item Info 到 DB")]
        public ActionResult PostItemInfoToAPIService(List<ItemInfoResult> ItemInfo)
        {
            ActionResponse<bool> result = null;
            if (ItemInfo != null)
            {
                Service.AddProductsInfoService ItemService = new Service.AddProductsInfoService();
                result = ItemService.PostItemInfoToDB(ItemInfo);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("傳 Image Info 到 DB")]
        public ActionResult PostImageToAPIService(List<string> PictureURL, string ProductID, string ItemID)
        {
            ActionResponse<bool> result = null;
            if (PictureURL != null)
            {
                Service.AddProductsInfoService ImageService = new Service.AddProductsInfoService();
                result = ImageService.PostImageToDB(PictureURL, ProductID, ItemID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("取得傭金費率")]
        public ActionResult GetCommisionSevice(int SellerID, int CategoryID)
        {
            ActionResponse<decimal> result = null;
            if (CategoryID != 0)
            {
                Service.AddProductsInfoService GetCommisionService = new Service.AddProductsInfoService();
                result = GetCommisionService.GetCommision(SellerID, CategoryID);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BathCreateItem(API.Models.BathItemCreateInfo bathItemCreateInfo)
        {
            API.Models.BathItemCreateInfoResult bathCreateResult = new Models.BathItemCreateInfoResult();

            Service.AddProductsInfoService productinfoservice = new Service.AddProductsInfoService();
            bathCreateResult = productinfoservice.ExcelAnalyze(bathItemCreateInfo);

            return Json(bathCreateResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult PostItemCreation(SPItemCreation spCreateitemInfos)
        {           
            API.Service.AddProductsInfoService addItemInfosService = new Service.AddProductsInfoService();

            var result = addItemInfosService.SPItemCreation(spCreateitemInfos);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Property
        /// </summary>
        /// <param name="Property">Category Property Info</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult GetProperty(int CategoryID)
        {
            Models.ActionResponse<List<PropertyResult>> result = new Models.ActionResponse<List<PropertyResult>>();
            API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();

            result = PropertyService.GetProperty(CategoryID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Property
        /// </summary>
        /// <param name="SaveProductProperty">Save Product Property Info</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult SaveProductPropertyClick(List<SaveProductProperty> proInfo, int proID)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();

            result = PropertyService.SaveProductPropertyClick(proInfo, proID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Property
        /// </summary>
        /// <param name="GetProductProperty">Get Product Property data</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult GetProductProperty(int ProductID)
        {
            //Models.ActionResponse<List<GetProductProperty>> result = new Models.ActionResponse<List<GetProductProperty>>();
            Models.ActionResponse<List<SaveProductProperty>> result = new Models.ActionResponse<List<SaveProductProperty>>();
            API.Service.CategoryPropertyService PropertyService = new Service.CategoryPropertyService();

            result = PropertyService.GetProductProperty(ProductID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
    /*---------- end by Ian and Thisway ----------*/
}
 