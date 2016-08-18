using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Attributes;
using TWNewEgg.API.Models;
using TWNewEgg.BackendService.Interface;

namespace TWNewEgg.API.Controllers
{
    public class ItemSketchController : Controller
    {
        TWNewEgg.BackendService.Interface.ICategoryAssociated CategoryAssociated = new TWNewEgg.BackendService.Service.CategoryAssociatedService();
        
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        TWNewEgg.API.Service.ImageService imgService = new Service.ImageService();

        /// <summary>
        /// 建立草稿
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <returns>成功、失敗資訊</returns>
        [Attributes.ActionDescriptionAttribute("建立草稿資料")]
        [HttpPost]
        public JsonResult CreateItemSketch(List<ItemSketch> itemSketchCell)
        {
            Service.ItemSketchService connecter = new Service.ItemSketchService();
            API.Models.ActionResponse<List<string>> result = connecter.CreateItemSketch(itemSketchCell);
            Models.ActionResponse<string> userresult = new ActionResponse<string>();
            ActionResponse<string> AdditionalPurchaseResult = new ActionResponse<string>();
            API.Models.AdditionalPurchase AdditionalPurchaseItem = new AdditionalPurchase();

            if (result.IsSuccess)
            {
                //判斷是否為vender才進行加價購處理
                Service.SellerUserService SellerUserService = new Service.SellerUserService();
                userresult = SellerUserService.GetVenderOrSeller(itemSketchCell.FirstOrDefault().CreateAndUpdate.UpdateUser.ToString(), 0);              
                                       
                //若為PM
                if (userresult.Body.Equals("S"))
                {
                    int NewID = 0;
                    if (result.Body.Count != 0)
                    {
                        if (String.IsNullOrEmpty(result.Body.FirstOrDefault()) || int.TryParse(result.Body.FirstOrDefault(), out NewID) == false)
                        {
                            result.IsSuccess = false;
                            result.Msg = "草稿建立成功 ，但加價購處理失敗。";
                            logger.Error("傳入新建草稿ID錯誤 : " + result.Body.FirstOrDefault());
                        }
                        else
                        {
                            //更新加價購資料
                            Service.AdditionalPurchaseService AdditionalPurchaseService = new Service.AdditionalPurchaseService();
                            AdditionalPurchaseItem.ItemSketchID = NewID;
                            AdditionalPurchaseItem.SearchTarget = API.Models.AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemSketchID;
                            AdditionalPurchaseItem.SellerID = itemSketchCell.FirstOrDefault().Item.SellerID ?? default(int);
                            AdditionalPurchaseItem.ShowOrder = itemSketchCell.FirstOrDefault().Item.ShowOrder;
                            AdditionalPurchaseItem.CreateAndUpdate.UpdateUser = itemSketchCell.FirstOrDefault().CreateAndUpdate.UpdateUser;

                            AdditionalPurchaseResult = AdditionalPurchaseService.AdditionalPurchaseItemEdit(AdditionalPurchaseItem);
                            if (!AdditionalPurchaseResult.IsSuccess)
                            {
                                result.IsSuccess = false;
                                result.Msg = "草稿建立成功 ，但加價購處理失敗。";
                            }
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "草稿建立成功 ，但加價購處理失敗。";
                        logger.Error("傳入新建草稿ID錯誤為空");
                    }
                }
                                   
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 編輯草稿
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <returns>成功、失敗資訊</returns>
        [Attributes.ActionDescriptionAttribute("編輯草稿資料")]
        [HttpPost]
        public JsonResult EditItemSketch(ItemSketchEditType editType, List<ItemSketch> itemSketchCell)
        {
            Service.ItemSketchService connecter = new Service.ItemSketchService();
            API.Models.ActionResponse<List<string>> result = connecter.EditItemSketch(editType, itemSketchCell);
            Models.ActionResponse<string> userresult = new ActionResponse<string>();
            ActionResponse<string> AdditionalPurchaseResult = new ActionResponse<string>();
            API.Models.AdditionalPurchase AdditionalPurchaseItem = new AdditionalPurchase();

            if (result.IsSuccess)
            {
                //判斷是否為vender才進行加價購處理
                Service.SellerUserService SellerUserService = new Service.SellerUserService();
                userresult = SellerUserService.GetVenderOrSeller(itemSketchCell.FirstOrDefault().CreateAndUpdate.UpdateUser.ToString(), 0);

                //若為PM
                if (userresult.Body.Equals("S"))
                {
                    //更新加價購資料
                    Service.AdditionalPurchaseService AdditionalPurchaseService = new Service.AdditionalPurchaseService();
                    AdditionalPurchaseItem.ItemSketchID = itemSketchCell.FirstOrDefault().ID;
                    AdditionalPurchaseItem.SearchTarget = API.Models.AdditionalPurchase.AdditionalPurchaseSearchTarget.ItemSketchID;
                    AdditionalPurchaseItem.SellerID = itemSketchCell.FirstOrDefault().Item.SellerID ?? default(int);
                    AdditionalPurchaseItem.ShowOrder = itemSketchCell.FirstOrDefault().Item.ShowOrder;
                    AdditionalPurchaseItem.CreateAndUpdate.UpdateUser = itemSketchCell.FirstOrDefault().CreateAndUpdate.UpdateUser;

                    AdditionalPurchaseResult = AdditionalPurchaseService.AdditionalPurchaseItemEdit(AdditionalPurchaseItem);
                    if (!AdditionalPurchaseResult.IsSuccess)
                    {
                        result.IsSuccess = false;
                        result.Msg = "草稿修改成功 ，但加價購處理失敗。";
                    }
                }
            }
            
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 搜尋草稿
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns>草稿清單</returns>
        [Attributes.ActionDescriptionAttribute("搜尋草稿資料")]
        [HttpPost]
        public JsonResult GetItemSketchList(ItemSketchSearchCondition condition)
        {
            Service.ItemSketchService connecter = new Service.ItemSketchService();
            API.Models.ActionResponse<List<ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            result = connecter.GetItemSketchList(condition);
            // 若有規格的草稿資料，排除
            if (result.Body != null)
            {
                result.Body = connecter.DistinctItemSketch(result.Body);
            }
            return this.Json(result, JsonRequestBehavior.AllowGet); 
        }
        [HttpPost]
        public JsonResult GetItemSketchListRemoveDes(ItemSketchSearchCondition condition)
        {
            Service.ItemSketchService connecter = new Service.ItemSketchService();
            API.Models.ActionResponse<List<ItemSketch>> resultTemp = new ActionResponse<List<ItemSketch>>();
            API.Models.ActionResponse<List<ItemSketch>> result = new ActionResponse<List<ItemSketch>>();

            resultTemp = connecter.GetItemSketchList(condition);
            result.Body = new List<ItemSketch>();
            if (resultTemp.IsSuccess == false)
            {
                return this.Json(resultTemp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // 若有規格的草稿資料，排除
                if (resultTemp.Body != null)
                {
                    resultTemp.Body = connecter.DistinctItemSketch(resultTemp.Body);                 
                }
                result = desEmpty(resultTemp.Body);
                return this.Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResponse<List<ItemSketch>> desEmpty(List<ItemSketch> modelTemp)
        {
            ActionResponse<List<ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            foreach (var index in modelTemp)
            {
                index.Item.Sdesc = "";
                index.Product.Description = "";
                index.Item.Spechead = "";
            }
            result.Body = modelTemp;
            result.IsSuccess = true;
            return result;
        }
        [HttpPost]
        //[FunctionCategoryName(FunctionCategoryNameAttributeValues.)]
        //[FunctionName(FunctionNameAttributeValues.ItemList)]
        [ActiveKey(ActiveKeyAttributeValues.Edit)]
        [ActionDescription("ItemSketch delete")]
        public JsonResult DeleteStetch(int StetchID)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            TWNewEgg.API.Service.ItemSketchService apiItemSketchService = new Service.ItemSketchService();
            result = apiItemSketchService.DeleteItemSketch(StetchID);
            return Json(result);
        }
       
        /// <summary>
        /// 草稿儲存與送審
        /// </summary>
        /// <param name="itemSketckList">送審資訊</param>
        /// <param name="userId">使用者ID</param>
        /// <param name="actionType">執行送審的類型</param>
        /// <returns>返回草稿儲存與送審結果</returns>
        [HttpPost]
        public JsonResult SendItemSketchToPending(List<TWNewEgg.API.Models.ItemSketch> itemSketckList, string userId, int actionType)
        {
            Service.ItemSketchService connecter = new Service.ItemSketchService();
            Models.ActionResponse<string> pendingItemSketchResult = connecter.SendItemSketchToPending(itemSketckList, userId, actionType);

            return Json(pendingItemSketchResult);
        }

        [HttpPost]
        [ActionDescription("ItemSketch verify")]
        public JsonResult VerifyStetch(List<int> ProductID, string Userid)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            TWNewEgg.API.Service.ItemSketchService apiItemSketchService = new Service.ItemSketchService();
            TWNewEgg.API.Service.PMInfoService pmService = new Service.PMInfoService();
            
            result = apiItemSketchService.VerifyStetch(ProductID, Userid);

            if (result.IsSuccess == true)
            {
                // 圖片處理
                imgService.ItemSketchListImgToTemp(ProductID);

                // 館價處理
                //Dictionary<int, int> checkResult = pmService.CheckGrossMargin();
                // 改以送審的 ID 判斷
                Dictionary<int, int> checkResult = pmService.CheckGrossMargin(ProductID);
                if (checkResult != null)
                {   
                    foreach (var index in checkResult)
                    {
                        // Call BackendService 寄送 Email
                        List<string> PmMails = CategoryAssociated.SendPMWithGrossMargin(index.Value);
                        pmService.SendPMGrossMarginMial(index.Key, PmMails);
                    }                   
                }               
            }

            return Json(result);
        }
        #region VerifyStetchByModel
        //[HttpPost]
        //[ActionDescription("ItemSketch verify by model")]
        //public JsonResult VerifyStetchByModel(TWNewEgg.API.Models.ItemSketch _ItemSketck, string Userid)
        //{
        //    ActionResponse<string> result = new ActionResponse<string>();
        //    TWNewEgg.API.Service.ItemSketchService apiItemSketchService = new Service.ItemSketchService();
        //    TWNewEgg.API.Service.PMInfoService pmService = new Service.PMInfoService();
            
        //    if (string.IsNullOrEmpty(Userid) == true)
        //    {
        //        result.IsSuccess = false;
        //        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
        //        result.Msg = "資料錯誤";
        //        logger.Error("/ItemSketch/VerifyStetchByModel error: 沒有 Userid");
        //    }
        //    result = apiItemSketchService.VerifyStetchByModel(_ItemSketck, Userid);
            
        //    if (result.IsSuccess == true)
        //    {
        //        int ID = 0;
        //        int.TryParse(result.Body, out ID);
        //        // 送審成功進行圖片處理
        //        imgService.ItemSketchDetailImgToTemp(_ItemSketck.Product.PicPatch_Edit, ID);

        //        Dictionary<int, int> checkResult = pmService.CheckGrossMargin();
                
        //        if (checkResult != null)
        //        {
        //            foreach (var index in checkResult)
        //            {
        //                // Call BackendService 寄送 Email
        //                List<string> PmMails = CategoryAssociated.SendPMWithGrossMargin(index.Value);
        //                pmService.SendPMGrossMarginMial(index.Key,PmMails);

        //            }
        //        }
        //    }

        //    return Json(result);
        //}
        #endregion
         /// <summary>
        /// Property
        /// </summary>
        /// <param name="Property">Category Property Info</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult GetProperty(int CategoryID)
        {
            Models.ActionResponse<List<PropertyResult>> result = new Models.ActionResponse<List<PropertyResult>>();
            API.Service.ProductPorpertySketchService PropertyService = new Service.ProductPorpertySketchService();

            result = PropertyService.GetProperty(CategoryID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Property
        /// </summary>
        /// <param name="SaveProductProperty">Save Product Property Info</param>
        /// <returns>成功及失敗訊息</returns>
        [HttpPost]
        public JsonResult SaveProductPropertyClick(List<SaveProductProperty> proInfo, int proID, int UpdateUserID)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            API.Service.ProductPorpertySketchService PropertyService = new Service.ProductPorpertySketchService();

            result = PropertyService.SaveProductPropertyClick(proInfo, proID, UpdateUserID);

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
            API.Service.ProductPorpertySketchService PropertyService = new Service.ProductPorpertySketchService();

            result = PropertyService.GetProductProperty(ProductID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public List<ItemSketch> DistinctItemSketch(List<ItemSketch> itemSketchlist)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            List<int> groupItemSketch = db.ItemSketchGroup.Select(r => r.ItemSketchID).ToList();
            // 排除 Group草稿
            return itemSketchlist.Where(x => !groupItemSketch.Contains(x.ID)).ToList();
        }
    }
}
