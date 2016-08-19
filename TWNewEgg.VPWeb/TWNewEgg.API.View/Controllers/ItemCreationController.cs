using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.View.Attributes;
using TWNewEgg.API.Models;
using System.IO;
using System.Drawing;

namespace TWNewEgg.API.View.Controllers
{
    public class ItemCreationController : Controller
    {
        //
        // GET: /ItemCreation/

        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        Connector conn = new Connector();
        private string website = System.Configuration.ConfigurationManager.AppSettings["TWSPHost"];

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.ItemCreation)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("建立商品")]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ItemCreationIndex()
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            TWNewEgg.API.View.ItemCreationVM creationVM = new ItemCreationVM();

            creationVM.AesInventoryQtyReg = aes.AesEncrypt(creationVM.ItemStock.InventoryQtyReg.ToString());
            creationVM.AesItemQtyReg = aes.AesEncrypt(creationVM.Item.ItemQtyReg.ToString());

            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
            ViewBag.userType = sellerInfo.IsAdmin;

            return PartialView(creationVM);
        }

        [HttpPost]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult CreateNewItem(TWNewEgg.API.View.ItemCreationVM newItemData)
        {
            // 加解密
            TWNewEgg.API.View.Service.AES aes = new Service.AES();
            TWNewEgg.API.Models.ActionResponse<List<string>> createNewItemResult = new ActionResponse<List<string>>();
            List<ItemSketch> createItemList = new List<ItemSketch>();
            newItemData.Item.SellerID = sellerInfo.currentSellerID;
            newItemData.CreateAndUpdate.CreateUser = sellerInfo.UserID;
            int getItemQtyReg = 0;
            int getInventoryQtyReg = 0;
            int.TryParse(aes.AesDecrypt(newItemData.AesItemQtyReg), out getItemQtyReg);
            newItemData.Item.ItemQtyReg = getItemQtyReg;
            int.TryParse(aes.AesDecrypt(newItemData.AesInventoryQtyReg), out getInventoryQtyReg);
            newItemData.ItemStock.InventoryQtyReg = getInventoryQtyReg;
            newItemData.CreateAndUpdate.CreateUser = sellerInfo.UserID;
            createItemList.Add(newItemData);
            try
            {
                createNewItemResult = conn.CreateItemSketch(createItemList);
                if (createNewItemResult.IsSuccess == true)
                {
                    return Json(new { IsSuccess = createNewItemResult.IsSuccess }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //return Json(new { IsSuccess = createNewItemResult.IsSuccess , ErrorMessage = createNewItemResult.Body.First() }, JsonRequestBehavior.AllowGet);
                    if (createNewItemResult.Code == (int)ResponseCode.AccessError)
                    {
                        logger.Error("商品儲存失敗 editItemResult.Code == (int)ResponseCode.AccessError [ErrorMessage] " + createNewItemResult.Body[0]);
                        return Json(new { IsSuccess = createNewItemResult.IsSuccess, ErrorMessage = createNewItemResult.Body[0] }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        logger.Error("商品儲存失敗 editItemResult.Code != (int)ResponseCode.AccessError [ErrorMessage] " + createNewItemResult.Msg);
                        return Json(new { IsSuccess = createNewItemResult.IsSuccess, ErrorMessage = createNewItemResult.Msg }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("商品建立失敗 [ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                return Json(new { IsSuccess = false, ErrorMessage = "商品建立失敗請重新確認資料是否填寫正確或與客服聯繫" }, JsonRequestBehavior.AllowGet);
            }

            return View();
        }

        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult Category(int? categoryID)
        {
            List<DB.TWSQLDB.Models.Category> CategoryList = new List<DB.TWSQLDB.Models.Category>();
            //ViewBag.categoryName = GetCategoryList(categoryID.Value).Where(x => x.ID == categoryID.Value).Select(r => r.Description).FirstOrDefault();
            try
            {
                if (categoryID == null)
                {

                }
                else
                {

                }


            }
            catch (Exception)
            {

                throw;
            }

            return PartialView();
        }

        [HttpGet]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public JsonResult QueryCategory(int? categoryID, int? Layer, int? parentID)
        {
            List<DB.TWSQLDB.Models.Category> CategoryList = new List<DB.TWSQLDB.Models.Category>();

            CategoryList = GetCategoryList(categoryID, Layer, parentID);

            return Json(CategoryList.Select(c => new { Description = c.Description, CategoryID = c.ID }), JsonRequestBehavior.AllowGet);
        }

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

        //public ActionResult Submit(IEnumerable<HttpPostedFileBase> files)
        //{
        //    if (files != null)
        //    {
        //        TempData["UploadedFiles"] = GetFileInfo(files);
        //    }
        //    //return View();
        //    return RedirectToAction("ItemMaintainDetail");
        //}

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult ProductProperty(string msg)
        {
            ViewBag.test = msg;
            return PartialView();
        }

        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public JsonResult QueryProductProperty(int? CategoryID)
        {
            List<PropertyResult> propertyResult = new List<PropertyResult>();

            try
            {
                if (CategoryID.HasValue)
                {
                    propertyResult = conn.GetProperty(null, null, CategoryID.Value).Body;
                }

            }
            catch (Exception ex)
            {


            }
            ViewBag.propertyDataList = propertyResult;
            string productPropertyUpdate = RenderView("ProductProperty");
            if (propertyResult.Count == 0)
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

        //public ActionResult Result()
        //{
        //    return View();
        //}

        //private IEnumerable<string> GetFileInfo(IEnumerable<HttpPostedFileBase> files)
        //{
        //    return
        //        from a in files
        //        where a != null
        //        select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.ContentLength);
        //}


        //public ActionResult DraftIndex()
        //{
        //    //TWNewEgg.Website.IPP.Models.ItemMaintain.ViewModelMaster master=new Models.ItemMaintain.ViewModelMaster\
        //    return View();
        //}

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

                physicalPath = Path.Combine(this.Server.MapPath(@"~/Upload//ProductContent"), "VendorPortal_" + sellerInfo.currentSellerID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + RandomCode() + ".jpg");

                if (!System.IO.Directory.Exists(this.Server.MapPath(@"~/Upload//ProductContent")))
                {
                    System.IO.Directory.CreateDirectory(this.Server.MapPath(@"~/Upload//ProductContent"));
                }
                // The files are not actually saved in this demo
                upload.SaveAs(physicalPath);

                // 回傳到前面顯示
                int RegUrl_index = physicalPath.IndexOf("Upload");
                physicalPath = physicalPath.Replace("\\", "/");
                physicalPath = website + physicalPath.Substring(RegUrl_index - 1);
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
        public ActionResult SortUploadImg()
        {
            return View();
        }
    }
}
