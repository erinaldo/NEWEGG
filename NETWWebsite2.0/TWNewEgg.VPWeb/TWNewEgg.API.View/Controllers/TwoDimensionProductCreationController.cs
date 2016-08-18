using Newtonsoft.Json;
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
    public class TwoDimensionProductCreationController : Controller
    {
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region View操作畫面

        #region 主要草稿畫面

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.TwoDimensionProductCreation)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("建立規格商品")]
        //[Filter.LoginAuthorize]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            TWNewEgg.API.View.ItemCreationVM viewModel = new ItemCreationVM();
            viewModel.Item.DateEnd = new DateTime(2099, 12, 31);
            viewModel.SaveActionUrl = "/TwoDimensionProductCreation/CreateNewItem";

            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
            ActionResponse<string> UserInfo = new ActionResponse<string>();
            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
            ViewBag.userType = sellerInfo.IsAdmin;

            ViewBag.From = "NewItem";
            return View(viewModel);
        }
        //[Filter.LoginAuthorize]
        //[Filter.PermissionFilter]
        //public ActionResult TwoDimensionProductCreation()
        //{
        //    TWNewEgg.API.View.ItemCreationVM viewModel = new ItemCreationVM();
        //    viewModel.Item.DateEnd = new DateTime(2099, 12, 31);

        //    ViewBag.Type = "Create";

        //    return PartialView(viewModel);
        //}

        /// <summary>
        /// 取得製造商清單
        /// </summary>
        /// <returns>製造商清單</returns>
        public JsonResult GetManufacturerList()
        {
            // API 製造商清單
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.Manufacturer>> apiResult = new API.Models.ActionResponse<List<API.Models.Manufacturer>>();

            // 顯示在下拉式選單的製造商清單
            List<TWNewEgg.API.View.ItemListManufacturer> result = new List<ItemListManufacturer>();

            // 指定搜尋類型
            TWNewEgg.API.Models.SearchDataModel searchData = new API.Models.SearchDataModel();
            searchData.SearchType = API.Models.SearchType.SearchofficialALLInfo;

            // 連接至 API
            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();

            try
            {
                // 讀取 API 製造商清單
                apiResult = connector.SearchManufacturerInfo(searchData);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("取得製造商清單失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            if (apiResult.IsSuccess)
            {
                if (apiResult.Body.Count > 0)
                {
                    // 將 API 製造商清單轉成下拉式選單的製造商清單 Model
                    result = apiResult.Body.Select(x => new TWNewEgg.API.View.ItemListManufacturer
                    {
                        ManufactureName = x.ManufactureName,
                        SN = x.SN
                    }).ToList();

                    // 將製造商清單依製造商名稱排序
                    result = result.OrderBy(x => x.ManufactureName).ToList();
                }
                else
                {
                    log.Info("查無製造商資料。");
                }
            }
            else
            {
                log.Info(string.Format("取得製造商清單失敗; APIMessage = {0}.", apiResult.Msg));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 各層分類選單

        /// <summary>
        /// 產生各層分類下拉式選單UI
        /// </summary>
        /// <returns>各層分類下拉式選單UI</returns>
        [HttpGet]
        public ActionResult Category(TWNewEgg.API.Models.ItemSketch_ItemCategory itemCategory)
        {
            return PartialView(itemCategory);
        }

        /// <summary>
        /// 讀取各層分類清單
        /// </summary>
        /// <param name="Layer">分類的層數</param>
        /// <param name="parentID">父分類ID</param>
        /// <returns>分類清單</returns>
        [HttpGet]
        public JsonResult QueryCategory(int? Layer, int? parentID)
        {
            List<DB.TWSQLDB.Models.Category> result = new List<DB.TWSQLDB.Models.Category>();

            result = GetCategoryList(Layer, parentID);

            return Json(result.Select(c => new { Description = c.Description, CategoryID = c.ID }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 讀取 DB 的各層分類清單
        /// </summary>
        /// <param name="layer">分類的層數</param>
        /// <param name="parentID">父分類ID</param>
        /// <returns>分類清單</returns>
        private List<DB.TWSQLDB.Models.Category> GetCategoryList(int? layer, int? parentID)
        {
            TWNewEgg.API.Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> apiResult = new API.Models.ActionResponse<List<DB.TWSQLDB.Models.Category>>();
            apiResult.Body = new List<DB.TWSQLDB.Models.Category>();

            if ((layer != null && parentID != null) && (layer.Value >= 0 && parentID >= 0))
            {
                TWNewEgg.API.Models.Connector connector = new API.Models.Connector();

                try
                {
                    apiResult = connector.APIQueryCategory(null, null, layer.Value, parentID.Value);
                }
                catch (Exception ex)
                {
                    log.Info(string.Format("取得商品分類清單失敗(exception); Layer = {0}; parentID = {1}; ExceptionMessage = {2}; StackTrace = {3}.", layer, parentID, GetExceptionMessage(ex), ex.StackTrace));
                }

                if (apiResult.IsSuccess)
                {
                    if (apiResult.Body.Count > 0)
                    {
                        apiResult.Body = apiResult.Body.Where(x => x.ShowAll == 1).ToList();
                    }
                    else
                    {
                        log.Info(string.Format("查無商品分類資料; Layer = {0}; parentID = {1}.", layer, parentID));
                    }
                }
                else
                {
                    log.Info(string.Format("取得商品分類清單失敗; Layer = {0}; parentID = {1}; API Message = {2}.", layer, parentID, apiResult.Msg));
                }
            }
            else
            {
                log.Info(string.Format("輸入值錯誤，取得商品分類清單失敗; Layer = {0}; parentID = {1}.", layer, parentID));
            }

            return apiResult.Body;
        }

        #endregion 各層分類選單

        #endregion 主要草稿畫面

        #region 二維屬性畫面

        #region 產生屬性選擇

        /// <summary>
        /// 讀取商品屬性
        /// </summary>
        /// <param name="categoryID">第2層分類ID</param>
        /// <returns>商品屬性選擇操作畫面</returns>
        [HttpPost]
        public JsonResult QueryProductProperty(int? categoryID, string aesItemProperty)
        {
            string propertySeletionUI = string.Empty;

            if (categoryID != null && categoryID > 0)
            {
                TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.PropertyResult>> apiResult = GetProductProperty(categoryID.Value);

                if (apiResult.IsSuccess)
                {
                    if (apiResult.Body.Count > 0)
                    {
                        TWNewEgg.API.View.PropertySelection viewModel = new PropertySelection();

                        // 主項目和次項目的商品屬性集合
                        List<TWNewEgg.API.Models.PropertyResult> propertyCell = new List<API.Models.PropertyResult>();

                        foreach (TWNewEgg.API.Models.PropertyResult property in apiResult.Body)
                        {
                            switch (property.PropertyName.ToLower())
                            {
                                case "color":
                                case "顏色":
                                    {
                                        viewModel.MainSetectionCell.Add(new KendoSelectData
                                        {
                                            Text = property.PropertyName,
                                            Value = property.PropertyID
                                        });
                                        propertyCell.Add(property);
                                        break;
                                    }
                                case "size":
                                case "尺寸":
                                    {
                                        viewModel.SecondSetectionCell.Add(new KendoSelectData
                                        {
                                            Text = property.PropertyName,
                                            Value = property.PropertyID
                                        });
                                        propertyCell.Add(property);
                                        break;
                                    }
                                default:
                                    {
                                        if (property.PropertyName.ToLower().Contains("尺寸"))
                                        {
                                            viewModel.SecondSetectionCell.Add(new KendoSelectData
                                            {
                                                Text = property.PropertyName,
                                                Value = property.PropertyID
                                            });
                                            propertyCell.Add(property);
                                        }
                                        break;
                                    }
                            }
                        }

                        // 將屬性內容轉成 Jason 並加密
                        TWNewEgg.API.View.Service.AES aes = new Service.AES();
                        viewModel.AesPropertyCell = aes.AesEncrypt(JsonConvert.SerializeObject(propertyCell));

                        #region 編輯時，設定已選擇的主項目、次項目 ID

                        if (!string.IsNullOrEmpty(aesItemProperty))
                        {
                            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> itemProperty = GetSketchPropertyList(aesItemProperty);

                            if (itemProperty != null && itemProperty.Count > 0)
                            {
                                int dimension = itemProperty.Select(x => x.PropertyID).Distinct().ToList().Count();

                                // 讀取主項目 ID (判斷依據為 GroupValueID == ValueID)
                                int mainPropertyID = 0;
                                mainPropertyID = itemProperty.Where(x => x.GroupValueID == x.ValueID).Select(x => x.PropertyID).FirstOrDefault();

                                if (mainPropertyID > 0)
                                {
                                    viewModel.MainSelectedID = mainPropertyID.ToString();
                                }
                                else
                                {
                                    log.Info(string.Format("設定主項目 ID 失敗;  PropertyID = {0}.", mainPropertyID));
                                }

                                if (dimension == 2)
                                {
                                    // 讀取次項目 ID (判斷依據為 主項目 ID 之外，清單中另一個 PropertyID)
                                    int secondPropertyID = 0;
                                    if (mainPropertyID > 0)
                                    {
                                        secondPropertyID = itemProperty.Where(x => x.PropertyID != mainPropertyID).Select(x => x.PropertyID).FirstOrDefault();
                                    }

                                    if (secondPropertyID > 0)
                                    {
                                        viewModel.SecondSelectedID = secondPropertyID.ToString();
                                    }
                                    else
                                    {
                                        log.Info(string.Format("設定次項目 ID 失敗;  PropertyID = {0}.", secondPropertyID));
                                    }
                                }
                            }
                        }

                        #endregion 編輯時，設定選擇主項目、次項目的 ID

                        ViewBag.viewModel = viewModel;
                        propertySeletionUI = RenderView("PropertySeletionUI");
                    }
                    else
                    {
                        log.Info(string.Format("查無商品屬性資料; CategoryID = {0}.", categoryID));
                    }
                }
                else
                {
                    log.Info(string.Format("取得商品屬性失敗; CategoryID = {0}; API Message = {1}.", categoryID, apiResult.Msg));
                }
            }

            return Json(new { IsSuccess = true, ViewHtml = propertySeletionUI }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得商品屬性
        /// </summary>
        /// <param name="categoryID">第2層分類ID</param>
        /// <returns>商品屬性清單</returns>
        private TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.PropertyResult>> GetProductProperty(int categoryID)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.PropertyResult>> result = new API.Models.ActionResponse<List<API.Models.PropertyResult>>();
            result.Body = new List<API.Models.PropertyResult>();

            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();

            try
            {
                result = connector.GetProperty(null, null, categoryID);
            }
            catch (Exception ex)
            {
                log.Info(string.Format("取得商品屬性失敗(exception); CategoryID = {0}; ExceptionMessage = {1}; StackTrace = {2}.", categoryID, GetExceptionMessage(ex), ex.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 產生商品屬性選擇操作介面
        /// </summary>
        /// <returns>商品屬性選擇操作介面</returns>
        [HttpGet]
        public ActionResult PropertySeletionUI()
        {
            return PartialView();
        }

        #endregion 產生屬性選擇

        #region 設定下拉式勾選操作介面

        /// <summary>
        /// 設定選擇屬性
        /// </summary>
        /// <param name="aesPropertyCell">已加密的商品屬性清單</param>
        /// <param name="mainPropertyID">主項目ID</param>
        /// <param name="secondPropertyID">次項目ID</param>
        /// <returns>商品屬性下拉式勾選操作介面</returns>
        [HttpPost]
        public JsonResult SetProperty(string aesPropertyCell, int mainPropertyID, int? secondPropertyID, string aesItemProperty, string pictureUrlCell)
        {
            bool isSuccess = true;

            // Partial view
            string propertyValueSeletionUI = string.Empty;

            if (!string.IsNullOrEmpty(aesPropertyCell) && mainPropertyID > 0 && (mainPropertyID != secondPropertyID))
            {
                // 解密並取得商品屬性清單
                List<TWNewEgg.API.Models.PropertyResult> propertyList = GetPropertyList(aesPropertyCell);

                if (propertyList != null && propertyList.Count > 0)
                {
                    // View Model
                    TWNewEgg.API.View.PropertyValueSelection viewModel = new TWNewEgg.API.View.PropertyValueSelection();

                    #region 讀取主項目內容

                    if (propertyList.Any(x => x.PropertyID == mainPropertyID))
                    {
                        // 展開維度
                        viewModel.ExpandDimension = 1;

                        // 主項目ID
                        viewModel.TwoDimensionProductProperty.MainPropertyID = mainPropertyID;

                        // 主項目名稱
                        viewModel.TwoDimensionProductProperty.MainPropertyName = propertyList.Where(x => x.PropertyID == mainPropertyID).FirstOrDefault().PropertyName;

                        // 產生主項目選項
                        foreach (TWNewEgg.API.Models.PropertyValue propertyValue in propertyList.Where(x => x.PropertyID == mainPropertyID).FirstOrDefault().ValueInfo)
                        {
                            viewModel.MainSetectionCell.Add(new TWNewEgg.API.View.KendoSelectData
                            {
                                Text = propertyValue.Value,
                                Value = propertyValue.ValueID
                            });
                        }
                    }
                    else
                    {
                        isSuccess = false;
                        log.Info(string.Format("讀取主項目內容失敗; mainPropertyID = {0}.", mainPropertyID));
                    }

                    #endregion 讀取主項目內容

                    #region 讀取次項目內容

                    if (isSuccess && secondPropertyID.HasValue)
                    {
                        if (secondPropertyID.Value > 0 && propertyList.Any(x => x.PropertyID == secondPropertyID))
                        {
                            // 展開維度
                            viewModel.ExpandDimension = 2;

                            // 次項目ID
                            viewModel.TwoDimensionProductProperty.SecondPropertyID = secondPropertyID;

                            // 次項目名稱
                            viewModel.TwoDimensionProductProperty.SecondPropertyName = propertyList.Where(x => x.PropertyID == secondPropertyID).FirstOrDefault().PropertyName;

                            // 產生次項目選項
                            foreach (TWNewEgg.API.Models.PropertyValue propertyValue in propertyList.Where(x => x.PropertyID == secondPropertyID).FirstOrDefault().ValueInfo)
                            {
                                viewModel.SecondSetectionCell.Add(new TWNewEgg.API.View.KendoSelectData
                                {
                                    Text = propertyValue.Value,
                                    Value = propertyValue.ValueID
                                });
                            }
                        }
                        else
                        {
                            isSuccess = false;
                            log.Info(string.Format("讀取次項目內容失敗; secondPropertyID = {0}.", secondPropertyID));
                        }
                    }

                    #endregion 讀取次項目內容

                    // 設定編輯時，自動設定的項目
                    if (isSuccess && !string.IsNullOrEmpty(aesItemProperty))
                    {
                        // 解密二維屬性資料
                        List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> itemProperty = GetSketchPropertyList(aesItemProperty);

                        if (itemProperty != null && itemProperty.Count > 0)
                        {
                            TWNewEgg.API.Models.ActionResponse<List<MainPropertyValue>> getMainPropertyValueCell = MakeMainPropertyValueCell(aesItemProperty, viewModel.ExpandDimension, pictureUrlCell);
                            if (getMainPropertyValueCell.IsSuccess)
                            {
                                viewModel.TwoDimensionProductProperty.MainPropertyValueCell = getMainPropertyValueCell.Body;
                            }
                        }
                    }

                    ViewBag.viewModel = viewModel;
                    propertyValueSeletionUI = RenderView("PropertyValueSeletionUI");
                }
            }

            return Json(new { IsSuccess = isSuccess, ViewHtml = propertyValueSeletionUI }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 產生商品屬性下拉式勾選操作介面
        /// </summary>
        /// <returns>商品屬性下拉式勾選操作介面</returns>
        public ActionResult PropertyValueSeletionUI()
        {
            return PartialView();
        }

        #endregion 設定下拉式勾選操作介面

        private TWNewEgg.API.Models.ActionResponse<List<MainPropertyValue>> MakeMainPropertyValueCell(string aesItemProperty, int expandDimension, string aesPictureUrlCell)
        {
            TWNewEgg.API.Models.ActionResponse<List<MainPropertyValue>> result = new TWNewEgg.API.Models.ActionResponse<List<MainPropertyValue>>();
            result.Body = new List<MainPropertyValue>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // 解密二維屬性資料
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> itemPropertyCell = GetSketchPropertyList(aesItemProperty);

            // 解密圖片路徑
            List<string> pictureUrlCell = GetPictureUrlCell(aesPictureUrlCell);

            // 成功取得二維屬性資料
            if (itemPropertyCell != null && pictureUrlCell != null)
            {
                List<int> groupValueIDCell = itemPropertyCell.Select(x => x.GroupValueID).Distinct().ToList();

                if (groupValueIDCell.Count > 0)
                {
                    foreach (int groupValueID in groupValueIDCell)
                    {
                        TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty mainData = itemPropertyCell.Where(x => x.ValueID == groupValueID).FirstOrDefault();

                        if (mainData != null)
                        {
                            MainPropertyValue mainPropertyValue = new MainPropertyValue();
                            mainPropertyValue.InputValue = mainData.InputValue;
                            mainPropertyValue.MainPropertyValueID = mainData.ValueID;
                            mainPropertyValue.MainPropertyValueName = mainData.ValueName;
                            mainPropertyValue.PictureURL = pictureUrlCell[groupValueIDCell.IndexOf(groupValueID)] + "?" + RandomCode();

                            #region 讀取次項目內容

                            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> secondData = new List<DB.TWSQLDB.Models.ItemSketchProperty>();
                            if (expandDimension == 1)
                            {
                                secondData.Add(mainData);
                            }
                            if (expandDimension == 2)
                            {
                                secondData = itemPropertyCell.Where(x => x.GroupValueID == groupValueID && x.GroupValueID != x.ValueID).ToList();
                            }

                            TWNewEgg.API.Models.ActionResponse<List<SecondPropertyValue>> getSecondPropertyValueCell = MakeSecondPropertyValueCell(secondData, expandDimension);
                            if (getSecondPropertyValueCell.IsSuccess)
                            {
                                mainPropertyValue.SecondPropertyValueCell = getSecondPropertyValueCell.Body;
                            }

                            #endregion 讀取次項目內容

                            result.Body.Add(mainPropertyValue);
                        }
                        else
                        {
                            result.IsSuccess = false;
                            result.Msg = string.Format("讀取主項目失敗，ValueID = {0}", groupValueID);
                        }
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Msg += "讀取主項目資料筆數失敗。";
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            if (result.IsSuccess == false)
            {
                result.Msg = "讀取主項目內容失敗。ErrorMessage = " + result.Msg;
                log.Info(result.Msg);
                result.Msg = string.Empty;
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        private TWNewEgg.API.Models.ActionResponse<List<SecondPropertyValue>> MakeSecondPropertyValueCell(List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> itemPropertyCell, int expandDimension)
        {
            TWNewEgg.API.Models.ActionResponse<List<SecondPropertyValue>> result = new TWNewEgg.API.Models.ActionResponse<List<SecondPropertyValue>>();
            result.Body = new List<SecondPropertyValue>();
            result.IsSuccess = true;
            result.Msg = string.Empty;

            if (itemPropertyCell.Count > 0 && (expandDimension == 1 || expandDimension == 2))
            {
                if (expandDimension == 1)
                {
                    SecondPropertyValue secondPropertyValue = new SecondPropertyValue();
                    secondPropertyValue.CanSaleQty = itemPropertyCell[0].Qty;

                    result.Body.Add(secondPropertyValue);
                }

                if (expandDimension == 2)
                {
                    foreach (TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty itemSketchProperty in itemPropertyCell)
                    {
                        SecondPropertyValue secondPropertyValue = new SecondPropertyValue();
                        secondPropertyValue.CanSaleQty = itemSketchProperty.Qty;
                        secondPropertyValue.SecondPropertyValueID = itemSketchProperty.ValueID;
                        secondPropertyValue.SecondPropertyValueName = itemSketchProperty.ValueName;

                        result.Body.Add(secondPropertyValue);
                    }
                }
            }
            else
            {
                result.IsSuccess = false;
                log.Info(string.Format("讀取次項目內容失敗。"));

                if (itemPropertyCell.Count == 0)
                {
                    log.Info(string.Format("ErrorMessage = 未傳入次項目資料."));
                }

                if (expandDimension != 1 && expandDimension != 2)
                {
                    log.Info(string.Format("ErrorMessage = 展開維度值錯誤; expandDimension = {0}.", expandDimension));
                }
            }

            result.Code = SetResponseCode(result.IsSuccess);

            return result;
        }

        #region 上傳圖片

        /// <summary>
        /// 產生圖片預覽區塊
        /// </summary>
        /// <returns>圖片預覽區塊</returns>
        //public ActionResult PreviewPicture()
        //{
        //    return PartialView();
        //}

        /// <summary>
        ///  上傳圖片
        /// </summary>
        /// <param name="files">要上傳的圖片</param>
        /// <returns>已上傳的圖片圖徑</returns>
        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase files)
        {
            if (files != null)
            {
                if (files.ContentType.ToString() == "image/jpeg" || files.ContentType.ToString() == "image/png")
                {
                    string physicalPath = string.Empty;

                    // 檢查目的資料夾是否存在
                    if (!System.IO.Directory.Exists(this.Server.MapPath(@"~/Upload/GroupProduct")))
                    {
                        System.IO.Directory.CreateDirectory(this.Server.MapPath(@"~/Upload/GroupProduct"));
                    }

                    try
                    {
                        byte[] imageBuffer = new byte[files.ContentLength];

                        files.InputStream.Read(imageBuffer, 0, files.ContentLength);

                        // 將使用者上傳的圖片塞進 MemoryStream
                        MemoryStream imageMS = new MemoryStream(imageBuffer);

                        // 建立 Image 物件
                        Image inputImage = Image.FromStream(imageMS);

                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        var fileName = Path.GetFileName(files.FileName);

                        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
                        physicalPath = Path.Combine(Server.MapPath(@"~/Upload/GroupProduct"), "SellerPortal_" + sellerInfo.currentSellerID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + RandomCode() + ".jpg");

                        // The files are not actually saved in this demo
                        files.SaveAs(physicalPath);

                        // 回傳到前面顯示
                        int RegUrl_index = physicalPath.IndexOf("Upload");
                        physicalPath = physicalPath.Replace("\\", "/");
                        physicalPath = "http://" + HttpContext.Request.Url.Authority + physicalPath.Substring(RegUrl_index - 1);

                        // 計算預覽圖片的寬高
                        var style = string.Empty;

                        // Div 格子大小寬高比
                        var divAspectRatio = (float)150 / (float)102;

                        // 上傳圖片寬高比
                        var imageAspectRatio = (float)inputImage.Width / (float)inputImage.Height;

                        // 若圖片寬高比大於 Div 寬高比，則縮圖以寬為主計算高的值，否則以高為主計算寬的值
                        if (imageAspectRatio > divAspectRatio)
                        {
                            style = string.Format(";150;{0}", (inputImage.Height * 150) / inputImage.Width);
                        }
                        else
                        {
                            style = string.Format(";{0};102", (inputImage.Width * 102) / inputImage.Height);
                        }

                        return Content("上傳成功。;" + physicalPath + style);
                    }
                    catch (Exception ex)
                    {
                        log.Info(string.Format("上傳圖片失敗(expection); ExceptionMessage= {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                        return Content("上傳失敗，請稍後再試！");
                    }
                }
                else
                {
                    return Content("圖片類型僅支援 jpeg 及 png 格式。");
                }
            }

            return Content("");
        }

        #endregion 上傳圖片及圖片預覽區塊

        /// <summary>
        /// 產生亂碼
        /// </summary>
        /// <returns>15碼英數亂碼</returns>
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

        /// <summary>
        /// 取得商品屬性清單
        /// </summary>
        /// <remarks>解密屬性資料，並轉成 model 格式</remarks>
        /// <param name="orderDetails">屬性資訊(json 格式、已加密)</param>
        /// <returns>商品屬性清單(model 格式)</returns>
        private List<TWNewEgg.API.Models.PropertyResult> GetPropertyList(string propertyCell)
        {
            // 訂單商品資訊
            List<TWNewEgg.API.Models.PropertyResult> result = new List<TWNewEgg.API.Models.PropertyResult>();

            try
            {
                // 加解密
                TWNewEgg.API.View.Service.AES aes = new Service.AES();

                // 1.將屬性資訊，由解密成 JSON 格式
                // 2.將 JSON 格式 的屬性資訊，轉為 model
                result = JsonConvert.DeserializeObject<List<TWNewEgg.API.Models.PropertyResult>>(aes.AesDecrypt(propertyCell));

            }
            catch (Exception ex)
            {
                log.Info(string.Format("屬性解密失敗(exception); ExceptinMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 解密二維屬性資料
        /// </summary>
        /// <param name="itemPorperty">已加密的二維屬性資料</param>
        /// <returns>二維屬性資料</returns>
        private List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> GetSketchPropertyList(string itemPorperty)
        {
            List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty> result = new List<DB.TWSQLDB.Models.ItemSketchProperty>();

            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            try
            {
                result = JsonConvert.DeserializeObject<List<TWNewEgg.DB.TWSQLDB.Models.ItemSketchProperty>>(aes.AesDecrypt(itemPorperty));
            }
            catch (Exception ex)
            {
                result = null;
                log.Info(string.Format("解密二維屬性失敗; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            return result;
        }

        /// <summary>
        /// 解密圖片路徑
        /// </summary>
        /// <param name="pictureUrlCell">已加密的圖片路徑</param>
        /// <returns>圖片路徑</returns>
        private List<string> GetPictureUrlCell(string aesPictureUrlCell)
        {
            List<string> result = new List<string>();

            TWNewEgg.API.View.Service.AES aes = new Service.AES();

            try
            {
                result = JsonConvert.DeserializeObject<List<string>>(aes.AesDecrypt(aesPictureUrlCell));
            }
            catch (Exception ex)
            {
                result = null;
                log.Info(string.Format("解密圖片路徑失敗; ErrorMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
            }

            return result;
        }

        #endregion 二維屬性畫面

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
                try
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialView);
                    ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    result = sw.GetStringBuilder().ToString();
                }
                catch (Exception ex)
                {
                    log.Info(string.Format("產生 {0} PartialView 失敗; ErrorMessage = {1}; StackTrace = {2}.", partialView, GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            return result;
        }

        #endregion View操作畫面

        #region 收集畫面資訊，組合 model 傳至 API

        /// <summary>
        /// 建立規格品草稿
        /// </summary>
        /// <param name="newItemData">草稿資料</param>
        /// <param name="twoDimenstionProperty">二維屬性資料</param>
        /// <returns>成功、失敗訊息</returns>
        [HttpPost]
        //[Filter.LoginAuthorize]
        //[Filter.PermissionFilter]
        public ActionResult CreateNewItem(TWNewEgg.API.View.ItemCreationVM newItemData, TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty)
        {
            log.Info("規格品建立開始");

            
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            result.IsSuccess = true;
            result.Body = string.Empty;
            result.Msg = string.Empty;

           

            TWNewEgg.API.View.Service.TwoDimensionProductService service = new TWNewEgg.API.View.Service.TwoDimensionProductService();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> makeCreateStandardProductSketch = service.MakeCreateStandardProductSketch(newItemData, twoDimenstionProperty);
            if (makeCreateStandardProductSketch.IsSuccess)
            {
                TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
                try
                {
                    result = connector.TwoDimensionProductCreate(makeCreateStandardProductSketch.Body);                    
                    log.Info(result.Msg);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    log.Info(string.Format("規格品建立失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }
            else
            {
                result.IsSuccess = false;
            }

            if (result.IsSuccess)
            {
                result.Body = "新增成功。";
            }
            else
            {
                result.Body = "新增失敗。";
            }

            log.Info("規格品建立結束");

            return Json(new { isSuccess = result.IsSuccess, message = result.Body }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 規格品送審
        /// </summary>
        /// <param name="newItemData">草稿資料</param>
        /// <param name="twoDimenstionProperty">二維屬性資料</param>
        /// <returns>成功、失敗訊息</returns>
        public JsonResult ExamineNewItem(TWNewEgg.API.View.ItemCreationVM newItemData, TWNewEgg.API.View.TwoDimensionProductProperty twoDimenstionProperty, string FromWhere = "")
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            TWNewEgg.API.View.Service.SellerInfoService sellerinfoSer = new Service.SellerInfoService();
            TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
            TWNewEgg.API.View.Service.TwoDimensionProductService service = new Service.TwoDimensionProductService();
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> makeCreateStandardProductSketch = service.MakeCreateStandardProductSketch(newItemData, twoDimenstionProperty);
            List<string> returnMSG = new List<string>();
            if (string.IsNullOrEmpty(FromWhere) == true)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                returnMSG.Add("F");
                returnMSG.Add("資料錯誤");
                return Json(returnMSG);
            }
            bool isNewItem = true;
            if (FromWhere == "NewItem")
            {
                isNewItem = true;
            }
            else
            {
                isNewItem = false;
            }

            List<SecondPropertyValue> qtyArray = new List<SecondPropertyValue>();
            foreach (MainPropertyValue mainPropertyValue in twoDimenstionProperty.MainPropertyValueCell)
            {
                if (mainPropertyValue.SecondPropertyValueCell.Count > 0)
                {
                    foreach (SecondPropertyValue secondPropertyValue in mainPropertyValue.SecondPropertyValueCell)
                    {
                        if (secondPropertyValue.CanSaleQty > 0)
                        {
                            qtyArray.Add(secondPropertyValue);
                        }
                    }
                }
            }




            //用來判斷是否有 exception
            bool isNoException = true;
            //先組合要存儲的 MODEL 型態: 成功
            if (makeCreateStandardProductSketch.IsSuccess == true && qtyArray.Count > 0)
            {
                try
                {
                    int sellerid = sellerinfoSer.currentSellerID;
                    int userid = sellerinfoSer.UserID;
                    result = connector.TwoDimensionProductCreateExamine(makeCreateStandardProductSketch.Body, userid, sellerid, isNewItem);
                }
                catch (Exception error)
                {
                    isNoException = false;
                    log.Error("[MSG]: " + error.Message + " ;[StackTrace]: " + error.StackTrace);
                }
                //發生 exception error
                if (isNoException == false)
                {
                    result.IsSuccess = false;
                    result.Msg = "資料發生錯誤";
                    returnMSG.Add("F");
                    returnMSG.Add("資料發生錯誤");
                    return Json(returnMSG);
                }
                else
                {
                    //沒有發生 exception error
                    if (result.IsSuccess == true)
                    {
                        returnMSG.Add("T");
                        returnMSG.Add(result.Msg);
                        return Json(returnMSG);
                    }
                    else
                    {
                        returnMSG.Add("F");
                        returnMSG.Add(result.Msg);
                        return Json(returnMSG);
                    }

                }
            }
            else
            {
                //組合 MODEL 失敗
                result.IsSuccess = false;
                result.Msg = "資料新增送審失敗，請檢查資料是否填寫正確及數量至少一筆大於0!";
                returnMSG.Add("F");
                returnMSG.Add("資料新增送審失敗，請檢查資料是否填寫正確及數量至少一筆大於0!");
                return Json(returnMSG);
            }
        }

        #endregion 收集畫面資訊，組合 model 傳至 API

        #region 取得 Exception 訊息、填寫 Response Code

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

        /// <summary>
        /// 填寫 Response Code
        /// </summary>
        /// <param name="isSuccess">成功、失敗資訊</param>
        /// <returns>Response Code</returns>
        private int SetResponseCode(bool isSuccess)
        {
            if (isSuccess)
            {
                return (int)TWNewEgg.API.Models.ResponseCode.Success;
            }
            else
            {
                return (int)TWNewEgg.API.Models.ResponseCode.Error;
            }
        }

        #endregion 取得 Exception 訊息、填寫 Response Code
    }
}
