using AutoMapper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.View.Attributes;

namespace TWNewEgg.API.View.Controllers
{
    public class ItemDetailEditController : Controller
    {
        //
        // GET: /ItemDetailEdit/

        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        Connector conn = new Connector();

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemCreation)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("編輯商品")]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult Index(int? actionType, int? itemSketchID)
        {
            ViewBag.ActionType = actionType ?? 2;
            ViewBag.ItemSketchID = itemSketchID ?? 0;
            return PartialView();
        }

        /// <summary>
        /// 草稿修改
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="itemSketchID"></param>
        /// <returns></returns>
        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ModifyItemIndex(int actionType, int itemSketchID)
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            TWNewEgg.API.View.ItemCreationVM getItemVM = new ItemCreationVM();
            ItemSketchSearchCondition condition = new ItemSketchSearchCondition();
            ActionResponse<List<ItemSketch>> itemSketchList = new ActionResponse<List<ItemSketch>>();
            condition.KeyWord = itemSketchID.ToString();
            condition.SellerID = sellerInfo.currentSellerID;
            condition.pageInfo.PageIndex = 0;
            condition.pageInfo.PageSize = 10;
            ViewBag.userType = sellerInfo.IsAdmin;

            try
            {
                if (actionType != (int)TWNewEgg.API.View.ItemCreationVM.GetActionType.TempEdit && actionType != (int)TWNewEgg.API.View.ItemCreationVM.GetActionType.TempCopy)
                {
                    condition.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemSketchID;
                    itemSketchList = conn.GetItemSketchList(condition);
                }
                else
                {
                    condition.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemTempID;
                    itemSketchList = conn.GetItemTempList(condition, true);
                }
            }
            catch (Exception e)
            {
                logger.Error("資料查詢存失敗 [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                ViewBag.ErrorMsg = "資料修改查詢錯誤，請重新查詢或與客服聯絡";
                //return PartialView("~/Views/ItemDetailEdit/ErrorMessageView.cshtml");
                return PartialView("ErrorMessageView");
                //return Json(new { ErrorMsg = "資料修改查詢錯誤，請重新查詢或與客服聯絡" }, JsonRequestBehavior.AllowGet);
            }

            if (itemSketchList.Body != null && itemSketchList.Body.Count > 0)
            {
                Mapper.CreateMap<ItemSketch, ItemCreationVM>();
                if (actionType != (int)TWNewEgg.API.View.ItemCreationVM.GetActionType.TempEdit && actionType != (int)TWNewEgg.API.View.ItemCreationVM.GetActionType.TempCopy)
                {
                    getItemVM = Mapper.Map<ItemCreationVM>(itemSketchList.Body.Where(x => x.ID == itemSketchID).FirstOrDefault());
                }
                else
                {
                    getItemVM = Mapper.Map<ItemCreationVM>(itemSketchList.Body.Where(x => x.Item.ID == itemSketchID).FirstOrDefault());
                }

                getItemVM.ActionType = actionType;
                getItemVM.AesInventoryQtyReg = aes.AesEncrypt(getItemVM.ItemStock.InventoryQtyReg.ToString());
                getItemVM.AesItemQtyReg = aes.AesEncrypt(getItemVM.Item.ItemQtyReg.ToString());

                return PartialView(getItemVM);
            }
            else
            {
                //return Json(new { ErrorMsg = "資料修改查詢錯誤，請重新查詢或與客服聯絡!" }, JsonRequestBehavior.AllowGet);
                ViewBag.ErrorMsg = "資料修改查詢錯誤，請重新查詢或與客服聯絡!";
                //return PartialView("~/Views/ItemDetailEdit/ErrorMessageView.cshtml");
                return PartialView("ErrorMessageView");
            }
        }

        //public ActionResult ModifyItemSketch(TWNewEgg.API.View.ItemCreationVM ModifyItemData)
        //{
        //    // 加解密
        //    TWNewEgg.API.View.Service.AES aes = new Service.AES();
        //    ModifyItemData.Item.SellerID = sellerInfo.currentSellerID;
        //    int getItemQtyReg = 0;
        //    int getInventoryQtyReg = 0;
        //    int.TryParse(aes.AesDecrypt(ModifyItemData.AesItemQtyReg), out getItemQtyReg);
        //    ModifyItemData.Item.ItemQtyReg = getItemQtyReg;
        //    int.TryParse(aes.AesDecrypt(ModifyItemData.AesInventoryQtyReg), out getInventoryQtyReg);
        //    ModifyItemData.ItemStock.InventoryQtyReg = getInventoryQtyReg;

        //    return View();
        //}

        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ModifyCategory(int? categoryID, int actionType, ItemSketch itemSketch, ItemSketch_ItemCategory modifyCategory)
        {
            ViewBag.ActionType = actionType;
            //List<DB.TWSQLDB.Models.Category> CategoryList = new List<DB.TWSQLDB.Models.Category>();

            //ViewBag.categoryName = GetCategoryList(categoryID.Value).Where(x => x.ID == categoryID.Value).Select(r => r.Description).FirstOrDefault();
            //try
            //{
            //    if (categoryID == null)
            //    {

            //    }
            //    else
            //    {

            //    }
            //}
            //catch (Exception)
            //{
            //    throw;
            //}

            return PartialView(modifyCategory);
        }

        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public JsonResult ModifyQueryCategory(int? categoryID, int? Layer, int? parentID)
        {
            List<DB.TWSQLDB.Models.Category> CategoryList = new List<DB.TWSQLDB.Models.Category>();

            CategoryList = GetCategoryList(categoryID, Layer, parentID);

            return Json(CategoryList.Select(c => new { Description = c.Description, CategoryID = c.ID }), JsonRequestBehavior.AllowGet);
        }

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        private List<DB.TWSQLDB.Models.Category> GetCategoryList(int? queryCategoryID, int? Layer, int? parentID)
        {
            List<DB.TWSQLDB.Models.Category> CategoryList = new List<DB.TWSQLDB.Models.Category>();

            try
            {
                CategoryList = conn.APIQueryCategory(null, null, Layer.Value, parentID.Value).Body;

                CategoryList = CategoryList.Where(x => x.ShowAll == 1).ToList();
                //if (queryCategoryID != null || queryCategoryID != 0)
                //{
                //    CategoryList = CategoryList.Where(x => x.ID == queryCategoryID.Value).ToList();
                //}

            }
            catch (Exception ex)
            {

                throw;
            }

            return CategoryList;
        }

        TWNewEgg.API.View.ServiceAPI.APIConnector connAPI = new ServiceAPI.APIConnector();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editItemData"></param>
        /// <returns></returns>
        [HttpPost]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult EditItemSketch(TWNewEgg.API.View.ItemCreationVM editItemData)
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            TWNewEgg.API.Models.ActionResponse<List<string>> editItemResult = new ActionResponse<List<string>>();
            List<ItemSketch> editItemSketchList = new List<ItemSketch>();
            ItemSketch editItemSketch = new ItemSketch();

            editItemData.Item.SellerID = sellerInfo.currentSellerID;
            editItemData.CreateAndUpdate.UpdateUser = sellerInfo.UserID;
            int getItemQtyReg = 0;
            int getInventoryQtyReg = 0;
            int.TryParse(aes.AesDecrypt(editItemData.AesItemQtyReg), out getItemQtyReg);
            editItemData.Item.ItemQtyReg = getItemQtyReg;
            int.TryParse(aes.AesDecrypt(editItemData.AesInventoryQtyReg), out getInventoryQtyReg);
            editItemData.ItemStock.InventoryQtyReg = getInventoryQtyReg;
            editItemData.CreateAndUpdate.CreateUser = sellerInfo.UserID;

            Mapper.CreateMap<ItemCreationVM, ItemSketch>();
            editItemSketch = Mapper.Map<ItemSketch>(editItemData);

            editItemSketchList.Add(editItemSketch);
            try
            {
                if (editItemData.ActionType != (int)TWNewEgg.API.View.ItemCreationVM.GetActionType.TempEdit)
                {
                    editItemResult = conn.EditItemSketch(ItemSketchEditType.DetailEdit, editItemSketchList);
                }
                else
                {
                    //editItemResult = connAPI.EditDetailTemp(editItemSketch);
                    editItemResult = conn.EditDetailTemp(editItemSketch);
                }

                if (editItemResult.IsSuccess == true)
                {
                    return Json(new { IsSuccess = editItemResult.IsSuccess }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (editItemResult.Code == (int)ResponseCode.AccessError)
                    {
                        logger.Error("商品儲存失敗 editItemResult.Code == (int)ResponseCode.AccessError [ErrorMessage] " + editItemResult.Body[0]);
                        return Json(new { IsSuccess = editItemResult.IsSuccess, ErrorMessage = editItemResult.Body[0] }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        logger.Error("商品儲存失敗 editItemResult.Code != (int)ResponseCode.AccessError [ErrorMessage] " + editItemResult.Msg);
                        return Json(new { IsSuccess = editItemResult.IsSuccess, ErrorMessage = editItemResult.Msg }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("商品儲存失敗 [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                return Json(new { IsSuccess = false, ErrorMessage = "商品儲存失敗請重新確認資料是否填寫正確或與客服聯繫" }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ModifyProductProperty()
        {
            return PartialView();
        }

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public JsonResult QueryProductProperty(int? CategoryID, int? ItemSketchID, int? ProductTempID)
        {
            List<PropertyResult> propertyResult = new List<PropertyResult>();
            List<SaveProductProperty> sppList = new List<SaveProductProperty>();
            try
            {
                if (CategoryID.HasValue)
                {
                    propertyResult = conn.GetProperty(null, null, CategoryID.Value).Body;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QueryProductProperty CategoryID.HasValue [ErrorMessage] " + ex.Message + " [StackTrace] " + ex.StackTrace);
            }

            try
            {
                if (ItemSketchID.HasValue)
                {
                    sppList = conn.GetProductPropertySketch(null, null, ItemSketchID.Value).Body;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QueryProductProperty ItemSketchID.HasValue [ErrorMessage] " + ex.Message + " [StackTrace] " + ex.StackTrace);
            }

            try
            {
                if (ProductTempID.HasValue)
                {
                    sppList = conn.GetProductPropertyTemp(ProductTempID.Value).Body;
                }
            }
            catch (Exception ex)
            {
                logger.Error("QueryProductProperty ProductTempID.HasValue [ErrorMessage] " + ex.Message + " [StackTrace] " + ex.StackTrace);
            }

            List<ProductPropertyEdit> result = new List<ProductPropertyEdit>();
            if (propertyResult != null && sppList != null)
            {
                result = (from pr in propertyResult.ToList()
                          join spp in sppList.ToList() on pr.PropertyID equals spp.PropertyID into ps
                          from spp in ps.DefaultIfEmpty()
                          select new ProductPropertyEdit
                          {
                              CategoryID = pr.CategoryID,
                              GroupID = pr.GroupID,
                              PropertyID = pr.PropertyID,
                              PropertyName = pr.PropertyName,
                              ValueInfo = pr.ValueInfo,
                              ValueOption = ((spp == null || string.IsNullOrWhiteSpace(spp.InputValue)) ? 0 : 1),
                              ValueID = ((spp == null || !string.IsNullOrWhiteSpace(spp.InputValue)) ? 0 : spp.ValueID),
                              InputValue = (spp == null ? "" : spp.InputValue),
                          }
                 ).ToList();
            }

            ViewBag.propertyDataList = result;
            string productPropertyUpdate = RenderView("ModifyProductProperty");
            if (result.Count == 0)
            {
                productPropertyUpdate = "<div id=\"withoutPropertyList\">" +
                                            "<br />" +
                                            "<span>查無所選類別屬性資料，請洽管理員或PM建立相關類別屬性。</span>" +
                                        "</div>";
            }

            return Json(new { IsSuccess = true, ViewHtml = productPropertyUpdate }, JsonRequestBehavior.AllowGet);
        }

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

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public JsonResult ImageUpload(IEnumerable<HttpPostedFileBase> files)
        {
            string physicalPath = string.Empty;
            if (!System.IO.Directory.Exists(this.Server.MapPath(@"~/Upload/ItemSketch")))
            {
                System.IO.Directory.CreateDirectory(this.Server.MapPath(@"~/Upload/ItemSketch"));
            }
            // The Name of the Upload component is "files"
            if (files != null)
            {
                if (files.Count() > 7)
                {
                    return Json(new { img = "", Msg = "圖片儲存數量最大值僅限於7張，請重新上傳!" }, JsonRequestBehavior.AllowGet);
                }

                foreach (var file in files)
                {
                    try
                    {
                        byte[] imageBuffer = new byte[file.ContentLength];

                        file.InputStream.Read(imageBuffer, 0, file.ContentLength);
                        //將使用者上傳的圖片塞進 MemoryStream
                        MemoryStream imageMS = new MemoryStream(imageBuffer);

                        //建立 Image 物件
                        Image inputImage = Image.FromStream(imageMS);

                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        var fileName = Path.GetFileName(file.FileName);

                        physicalPath = Path.Combine(this.Server.MapPath(@"~/Upload/ItemSketch"), "VendorPortal_" + sellerInfo.currentSellerID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + RandomCode() + ".jpg");

                        // The files are not actually saved in this demo
                        file.SaveAs(physicalPath);

                        // 回傳到前面顯示
                        int RegUrl_index = physicalPath.IndexOf("Upload");
                        physicalPath = physicalPath.Replace("\\", "/");
                        physicalPath = "http://" + HttpContext.Request.Url.Authority + physicalPath.Substring(RegUrl_index - 1);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        return Json(new { IsSuccess = false, img = physicalPath, Msg = "發生意外錯誤，請稍後再試!" }, JsonRequestBehavior.AllowGet);
                    }
                }
                // Return an empty string to signify success
                return Json(new { IsSuccess = false, img = physicalPath, Msg = "圖片上傳成功!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, img = physicalPath, Msg = "請選擇上傳圖片!" }, JsonRequestBehavior.AllowGet);
            }
        }

        private string RandomCode()
        {
            string reRandomCode = string.Empty;
            int rand;
            char pd;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < 15; i++)
            {
                rand = random.Next();
                if (rand % 2 == 0)
                {
                    pd = (char)('A' + (char)(rand % 26));
                }
                else
                {
                    pd = (char)('0' + (char)(rand % 10));
                }
                reRandomCode += pd.ToString();
            }

            return reRandomCode;
        }

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public void PictureUpload(HttpPostedFileBase upload, int CKEditorFuncNum)
        {
            string physicalPath = string.Empty;

            try
            {
                byte[] imageBuffer = new byte[upload.ContentLength];

                upload.InputStream.Read(imageBuffer, 0, upload.ContentLength);
                //將使用者上傳的圖片塞進 MemoryStream
                MemoryStream imageMS = new MemoryStream(imageBuffer);

                //建立 Image 物件
                Image inputImage = Image.FromStream(imageMS);

                // Some browsers send file names with full path.
                // We are only interested in the file name.
                var fileName = Path.GetFileName(upload.FileName);

                physicalPath = Path.Combine(this.Server.MapPath(@"~/Upload//ProductContent"), "VendorPortal_" + sellerInfo.currentSellerID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg");

                if (!System.IO.Directory.Exists(this.Server.MapPath(@"~/Upload//ProductContent")))
                {
                    System.IO.Directory.CreateDirectory(this.Server.MapPath(@"~/Upload//ProductContent"));
                }
                // The files are not actually saved in this demo
                upload.SaveAs(physicalPath);

                // 回傳到前面顯示
                int RegUrl_index = physicalPath.IndexOf("Upload");
                physicalPath = physicalPath.Replace("\\", "/");
                physicalPath = "http://" + HttpContext.Request.Url.Authority + physicalPath.Substring(RegUrl_index - 1);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            Response.Write("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum.ToString() + ",'" + physicalPath + "');</script>");
            Response.Flush();
        }

        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ModifySortUploadImg(List<string> imgSrcList)
        {
            List<string> srcList = new List<string>();
            for (int indexI = 0; indexI < 7; indexI++)
            {
                if (imgSrcList.Count > indexI)
                {
                    srcList.Add(imgSrcList[indexI]);
                }
                else
                {
                    srcList.Add("");
                }
            }

            return View(srcList);
        }

        #region -- ItemMaintainProperty --
        //public ActionResult ItemMaintainProperty()
        //{
        //    return View();
        //}

        //public JsonResult GetCategoryProperty(string CategoryID, string ProductID)
        //{
        //    //if (string.IsNullOrWhiteSpace(CategoryID))
        //    //    CategoryID = "60";
        //    //    //CategoryID = "50"; //"60";

        //    //if (string.IsNullOrWhiteSpace(ProductID))
        //    //    ProductID = "6881";
        //    //    //ProductID = "60227"; //"6881";

        //    if (string.IsNullOrWhiteSpace(CategoryID) || string.IsNullOrWhiteSpace(ProductID))
        //        return null;

        //    ////檢查是否登入
        //    //try
        //    //{
        //    //    if (string.IsNullOrWhiteSpace(Service.Verification.UserName))
        //    //        throw new Exception();
        //    //}
        //    //catch
        //    //{
        //    //    return null;
        //    //}

        //    //BackendService.Interface.IProductProperty BService = new BackendService.Service.CategoryPropertyService();

        //    Models.ActionResponse<List<Models.PropertyResult>> prList = ProductProperty.GetPropertybyCategoryID(int.Parse(CategoryID));
        //    Models.ActionResponse<List<Models.SaveProductProperty>> sppList = ProductProperty.GetProductPropertybyProductID(int.Parse(ProductID));

        //    //new TWNewEgg.Website.IPP.Models.ItemMaintain.ItemMaintainProperty
        //    var result = from pr in prList.Body.ToList()
        //                 join spp in sppList.Body.ToList()
        //                 on pr.PropertyID equals spp.PropertyID into ps
        //                 from spp in ps.DefaultIfEmpty()
        //                 select new
        //                 {
        //                     CategoryID = pr.CategoryID,
        //                     GroupID = pr.GroupID,
        //                     PropertyID = pr.PropertyID,
        //                     PropertyName = pr.PropertyName,
        //                     ValueInfo = pr.ValueInfo,
        //                     ValueOption = ((spp == null || string.IsNullOrWhiteSpace(spp.InputValue)) ? "0" : "1"),
        //                     ValueID = ((spp == null || !string.IsNullOrWhiteSpace(spp.InputValue)) ? 0 : spp.ValueID),
        //                     InputValue = (spp == null ? "" : spp.InputValue),
        //                     UpdateUser = sellerInfo.UserID
        //                 };
        //    //(spp == null ? "" : spp.UpdateUser)
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult SaveCategoryProductProperty(List<Models.SaveProductProperty> proInfo, string ProductID)
        //{
        //    //object result = new { IsSuccess = false };
        //    Models.ActionResponse<string> message = new Models.ActionResponse<string>();

        //    message.IsSuccess = false;
        //    //message.Msg = "存檔失敗!!";

        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(ProductID)) throw new Exception("無ProductID，存檔失敗!");

        //        //只取得屬性有設定的資料
        //        var result = proInfo
        //                    .Where(o => o.ValueID > 0 || !string.IsNullOrWhiteSpace(o.InputValue))
        //                    .Select(o => o);

        //        //BackendService.Interface.IProductProperty BService = new BackendService.Service.CategoryPropertyService();
        //        message = ProductProperty.SaveProductPropertyClick(result.ToList<Models.SaveProductProperty>(), int.Parse(ProductID));

        //        ////List<BackendService.Models.SaveProductProperty> result = new List<BackendService.Models.SaveProductProperty>();
        //        ////foreach (BackendService.Models.SaveProductProperty info in proInfo.Where(o => o.ValueID > 0 || !string.IsNullOrWhiteSpace(o.InputValue)))
        //        ////{
        //        ////    result.Add(info);
        //        ////}
        //        ////message = ProductProperty.SaveProductPropertyClick(result, int.Parse(ProductID));       

        //        #region -- 列出原先有，但修改後被移除的資料 --
        //        ////取得原資料
        //        //BackendService.Models.ActionResponse<List<BackendService.Models.SaveProductProperty>> sppList = ProductProperty.GetProductPropertybyProductID(int.Parse(ProductID));

        //        ////取得被刪除的資料
        //        //var delResult = from spp in sppList.Body.ToList()
        //        //                join pro in result
        //        //                on spp.PropertyID equals pro.PropertyID into ps
        //        //                from pro in ps.DefaultIfEmpty()
        //        //                where pro == null
        //        //                select spp;

        //        //int intDel = delResult.Count();
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        message.Msg = ex.Message;
        //    }
        //    return Json(message, JsonRequestBehavior.AllowGet);
        //}
        #endregion

    }
}
