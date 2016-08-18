using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.API.View.Attributes;

namespace TWNewEgg.API.View.Controllers
{
    public class ProductBatchController : Controller
    {
        Connector conn = new Connector();
        TWNewEgg.API.View.Service.SellerInfoService _sellerinfoService = new Service.SellerInfoService();
        //TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        //
        // GET: /ProductBatch/
        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchItemCreationUpdate)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("商品批次建立")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadExcel(IEnumerable<HttpPostedFileBase> files, string ActionValue)
        {
            int _ActionValue = int.Parse(ActionValue);
            string _strErrorMsg = string.Empty;
            string _strSavePath = Server.MapPath("~/Upload/BatchItemSketchCreation");
            List<TWNewEgg.API.Models.ItemSketch> _listItemSketch = new List<API.Models.ItemSketch>();
            
            DateTime dateTimeMillisecond = new DateTime(0001, 01, 01, 01, 01, 01, 01);
            string timeString = dateTimeMillisecond.ToString("yyyy-MM-dd hh:mm:ss fff");
            dateTimeMillisecond = DateTime.Now;
            bool isSaveSuccess = true;
            #region 判斷是否有上傳檔案
            if (files == null)
            {
                return Content("請選擇上傳檔案");
            }
            #endregion
            #region save excel data check
            //判斷要存在Excel的資料夾是否存在，沒有則建立資料夾
            if (Directory.Exists(_strSavePath) == false)
            {
                Directory.CreateDirectory(_strSavePath);
            }
            foreach (var item in files)
            {
                string extension = System.IO.Path.GetExtension(item.FileName);
                if (extension != ".xls")
                {
                    _strErrorMsg = "檔案格式錯誤";
                    return Content(_strErrorMsg);
                }
                string fileName = System.IO.Path.GetFileName(item.FileName);

                fileName = "batchItem" + "_" + _sellerinfoService.currentSellerID + "_" + dateTimeMillisecond.ToString("yyyyMMddHHmmss") + ".xls";
                _strSavePath = _strSavePath + "\\" + fileName;
                try
                {
                    item.SaveAs(_strSavePath);
                }
                catch (Exception error)
                {
                    logger.Error("/ProductBatch/UploadExcel SaveAs error: " + error.Message);
                    isSaveSuccess = false;
                    break;
                }
            }
            //檔案存儲有誤
            if (isSaveSuccess == false)
            {
                return Content("資料錯誤，請洽詢客服人員");
            }
            #endregion
            #region linq to excel process
            var excelfile = new LinqToExcel.ExcelQueryFactory(_strSavePath);
            var excelData = excelfile.Worksheet<TWNewEgg.API.Models.BatchExcelCreate>("Datafeed");
            //把Excel資料轉換成對應的model格式
            var result = ExcelToModel(excelData);
            #endregion
            ActionResponse<string> checkResult = new ActionResponse<string>();
            ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> insertToModel = new ActionResponse<List<ItemSketch>>();
            //model 轉換成功
            if (result.Code == (int)TWNewEgg.API.Models.ResponseCode.Success && result.IsSuccess == true)
            {
                if (_ActionValue == 0)
                {
                    //檢查資料的完整性
                    checkResult = checkModel(result.Body);
                    //資料格式有無誤
                    if (checkResult.IsSuccess == false && checkResult.Code == (int)ResponseCode.Error)
                    {
                        return Content(checkResult.Msg);
                    }
                    else
                    {//資料格式無誤
                        insertToModel = createItemSketchModel(result.Body);
                        if (insertToModel.Code == (int)TWNewEgg.API.Models.ResponseCode.Success && insertToModel.IsSuccess == true)
                        {
                            ActionResponse<List<string>> CreateItemSketchResult = new ActionResponse<List<string>>();
                            bool createSuccess = true;
                            try
                            {
                                //連接API並建立商品
                                CreateItemSketchResult = conn.CreateItemSketch(insertToModel.Body);
                            }
                            catch (Exception error)
                            {
                                logger.Error("/ProductBatch/UploadExcel connect to api error: " + error.Message);
                                createSuccess = false;
                            }
                            if (createSuccess == false)
                            {
                                return Content(CreateItemSketchResult.Msg);
                            }
                            else
                            {
                                //建立成功
                                if (CreateItemSketchResult.IsSuccess == true)
                                {
                                    return Json(new { Msg = CreateItemSketchResult.Msg }, JsonRequestBehavior.AllowGet);
                                }
                                else//建立失敗
                                {
                                    return Content(CreateItemSketchResult.Msg);
                                }
                            }
                        }
                        else
                        {
                            logger.Error("/ProductBatch/UploadExcel \"checkResult\" even error");
                            return Content(insertToModel.Msg);
                        }
                    }
                }
                else if (_ActionValue == 1)
                {
                    //檢查規則:修改                            
                    //檢查資料的完整性
                    checkResult = checkBatchEditModel(result.Body);
                    //資料格式有無誤
                    if (checkResult.IsSuccess == false && checkResult.Code == (int)ResponseCode.Error)
                    {
                        return Content(checkResult.Msg);
                    }
                    else
                    {//資料格式無誤               
                        insertToModel = createItemBatchEditSketchModel(result.Body);
                        if (insertToModel.Code == (int)TWNewEgg.API.Models.ResponseCode.Success && insertToModel.IsSuccess == true)
                        {
                            TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
                            ActionResponse<List<string>> BatchEditItemSketchResult = new ActionResponse<List<string>>();
                            bool createSuccess = true;
                            try
                            {
                                //連接API並修改商品
                                BatchEditItemSketchResult = conn.BatchEditDetailTemp(insertToModel.Body, sellerInfo.UserID, sellerInfo.currentSellerID);
                            }
                            catch (Exception error)
                            {
                                logger.Error("/ProductBatch/UploadExcel connect to api error: " + error.Message);
                                BatchEditItemSketchResult.Msg = "發生意外";
                                createSuccess = false;
                            }
                            if (createSuccess == false)
                            {
                                return Content(BatchEditItemSketchResult.Msg);
                            }
                            else
                            {
                                //建立成功
                                if (BatchEditItemSketchResult.IsSuccess == true)
                                {
                                    return Json(new { Msg = BatchEditItemSketchResult.Msg }, JsonRequestBehavior.AllowGet);
                                }
                                else//建立失敗
                                {
                                    return Content(BatchEditItemSketchResult.Msg);
                                }
                            }
                        }
                        else
                        {
                            logger.Error("/ProductBatch/UploadExcel \"checkResult\" even error");
                            return Content(insertToModel.Msg);
                        }
                    }
                }
                else
                {
                    logger.Error("dropdownlist value even error");
                    return Content("意外發生，請重新整理畫面");
                }

            }
            else if (result.Code == (int)TWNewEgg.API.Models.ResponseCode.Error && result.IsSuccess == false)
            {
                return Content(result.Msg);
            }
            else
            {
                logger.Error("/ProductBatch/UploadExcel: 發生沒有 hand 到的錯誤");
                return Content(result.Msg);
            }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.BatchExcelCreate>> ExcelToModel(LinqToExcel.Query.ExcelQueryable<TWNewEgg.API.Models.BatchExcelCreate> model)
        {
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.BatchExcelCreate>> result = new API.Models.ActionResponse<List<API.Models.BatchExcelCreate>>();
            List<TWNewEgg.API.Models.BatchExcelCreate> CreateBatchInfo = new List<TWNewEgg.API.Models.BatchExcelCreate>();
            try
            {
                foreach (var item in model)
                {
                    CreateBatchInfo.Add(item);
                }
                result.Body = CreateBatchInfo;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
                result.IsSuccess = true;
            }
            catch (Exception error)
            {
                logger.Error("/ProductBatch/ExcelToModel 錯誤 error: " + error.Message + "[StackTrace]" + error.StackTrace);
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            return result;
        }
        public TWNewEgg.API.Models.ActionResponse<string> checkModel(List<TWNewEgg.API.Models.BatchExcelCreate> _listcheck)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> _listManufacturerList = new ActionResponse<List<Manufacturer>>();
            int i = 1;
            decimal _decimalTemp;
            Regex reg = new Regex(@"^http[s]?://[\w-_.%/:?=&#]+$");
            _listcheck.RemoveAt(0);//刪除Model memeber name
            _listcheck.RemoveAt(0);
            List<int> checkdata = new List<int>();
            ActionResponse<bool> checkresult = new ActionResponse<bool>();
            #region 初始化
            int tempMainCategoryID_Layer2 = 0;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            result.IsSuccess = true;
            #endregion
            if (_listcheck.Count == 0 || _listcheck == null)
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "請填選正確的資料";
                return result;
            }
            else
            {
                //檢查Excel上傳資料是否超過限制
                if (_listcheck.Count > 100)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "資料數量超過限制，最多 100 筆資料";
                    return result;
                }
                
            }
            #region 檢查製造商
            TWNewEgg.API.Models.SearchDataModel searchDataModel = new SearchDataModel();
            searchDataModel.SearchType = TWNewEgg.API.Models.SearchType.SearchofficialALLInfo;
            
            #endregion
            foreach (var item in _listcheck)
            {
                #region 檢查 商品類別 ID 資料完整性
                //檢查是否有填寫 商品類別 ID
                if (string.IsNullOrEmpty(item.MainCategoryID_Layer2) == true)
                {
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆: 請填寫 商品類別 ID";
                    break;
                }
                else
                {
                    if (int.TryParse(item.MainCategoryID_Layer2, out tempMainCategoryID_Layer2) == false)
                    {
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆: 請填寫正確的 商品類別 ID";
                        break;
                    }
                }
                #endregion
                #region 檢查跨分類是在同一類別底下，並檢查跨分類1、跨分類2 的資料型態
                try
                {
                    if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == true && string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == true)
                    {
                        //no SubCategoryID_1_Layer2 and SubCategoryID_2_Layer2, it not need to connect the api CheckCategoryParentId to check the parents are in the same categoly
                        checkresult.IsSuccess = true;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == false)
                        {
                            checkdata.Add(Convert.ToInt32(item.SubCategoryID_1_Layer2));
                        }
                        if (string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == false)
                        {
                            checkdata.Add(Convert.ToInt32(item.SubCategoryID_2_Layer2));
                        }
                        checkresult = conn.CheckCategoryParentId(tempMainCategoryID_Layer2, checkdata);
                    }
                    
                }
                catch (Exception error)
                {
                    logger.Error("/ProductBatch/checkModelList error:" + error.Message);
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆: 商品類別 ID or 跨分類 1 or 跨分類 2 資料型態有誤";
                    break;
                }
                if (checkresult.IsSuccess == false)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆跨分類需要在同一個類別底下";
                    break;
                }
                #endregion
                #region 商家料號(為字串)
                if (string.IsNullOrEmpty(item.SellerProductID) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 商家商品編號 為必填";
                    break;
                }
                //商家料號
                //判斷是否有填寫資料
                //if (string.IsNullOrEmpty(item.SellerProductID) == true)
                //{
                //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                //    result.IsSuccess = false;
                //    result.Msg = "第 " + i + " 筆資料 \"商家料號\" 為必填";
                //    break;
                //}
                #endregion
                #region 製造商 URL
                //製造商 URL(必填) 判斷是否有填入資料
                //沒有填寫資料
                if (string.IsNullOrEmpty(item.ManufactureID) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 製造商URL/統編 為必填";
                    break;
                }
                else
                {
                    try
                    {
                        //先把通過的製造商搜索出來，後面要進行查詢是否製造商的URL是存在的
                        _listManufacturerList = conn.SearchManufacturerInfo(searchDataModel);
                    }
                    catch (Exception error)
                    {
                        logger.Error("/ProductBatch/createItemSketchModel error, api error: " + error.Message);
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "資料錯誤";
                        break;
                    }
                    var SN_search = _listManufacturerList.Body.Where(p => p.ManufactureURL == item.ManufactureID).FirstOrDefault();
                    if (SN_search == null)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第" + i + " 筆資料 製造商 不存在";
                        break;
                    }
                    ////有輸入值，判斷URL格式是否正確
                    //if (reg.IsMatch(item.ManufactureID) == false)
                    //{
                    //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    //    result.IsSuccess = false;
                    //    result.Msg = "第 " + i + " 筆資料 製造商 URL 錯誤";
                    //    break;
                    //}
                }
                #endregion
                #region 製造商料號
                ////製造商料號
                //if (string.IsNullOrEmpty(item.MenufacturePartNum) == false)
                //{

                //}
                #endregion
                #region 通用產品代碼
                //通用產品代碼
                #endregion
                #region 商品名稱(品名)
                //商品名稱(品名)(必填)
                //判斷是否有填寫資料
                if (string.IsNullOrEmpty(item.Name) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 商品名稱(品名) 為必填";
                    break;
                }
                #endregion
                #region 商品描述(內文)
                //商品描述(內文)
                //判斷是否有填寫資料
                if (string.IsNullOrEmpty(item.Description) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 商品中文說明 為必填";
                    break;
                }
                #endregion
                #region 注意事項
                //注意事項
                #endregion
                #region 簡要描述(主賣點1)
                //簡要描述(主賣點1)
                //檢查資料是否為空
                if (string.IsNullOrEmpty(item.Sdesc) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 商品特色標題 為必填";
                    break;
                }
                else
                {
                    string _checkresult = liTagCHeck(item.Sdesc);
                    string TorF = _checkresult.Split(';')[0];
                    if (TorF == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 商品特色標題" + _checkresult.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 簡要條列式描述(主賣點2)
                //簡要條列式描述(主賣點2)(必填)
                //檢查資料是否有填寫
                if (string.IsNullOrEmpty(item.Spechead) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 簡要簡要描述 為必填";
                    break;
                }
                else
                {
                    if (item.Spechead.Length > 30)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 簡要簡要描述 限制為30個字";
                        break;
                    }
                }
                #endregion
                #region 商品型號
                #endregion
                #region 商品條碼
                #endregion
                #region 包裝-長(cm)
                //包裝-長(cm)、必填
                //檢查資料是否填寫
                if (string.IsNullOrEmpty(item.Length) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 包裝-長(cm) 為必填";
                    break;
                }
                else
                {
                    _decimalTemp = 0;
                    //型別錯誤
                    if (decimal.TryParse(item.Length, out _decimalTemp) == false)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 包裝-長(cm) 錯誤";
                        break;
                    }
                    else
                    {
                        if (_decimalTemp <= 0)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆資料 包裝-長(cm) 錯誤，不可小於 0";
                            break;
                        }
                    }
                }
                #endregion
                #region 包裝-寬(cm)
                //包裝-寬(cm)(必填)
                //檢查資料是否填寫
                if (string.IsNullOrEmpty(item.Width) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 包裝-寬(cm) 為必填";
                    break;
                }
                else
                {
                    _decimalTemp = 0;
                    //型別錯誤
                    if (decimal.TryParse(item.Width, out _decimalTemp) == false)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 包裝-寬(cm) 錯誤";
                        break;
                    }
                    else
                    {
                        if (_decimalTemp <= 0)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆資料 包裝-寬(cm) 錯誤，不可小於 0";
                            break;
                        }
                    }

                    
                }
                #endregion
                #region 包裝-高(cm)
                //包裝-高(cm)(必填)
                if (string.IsNullOrEmpty(item.Height) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 包裝-高(cm) 為必填";
                    break;
                }
                else
                {
                    _decimalTemp = 0;
                    //型別錯誤
                    if (decimal.TryParse(item.Height, out _decimalTemp) == false)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 包裝-高(cm) 錯誤";
                        break;
                    }
                    else
                    {
                        if (_decimalTemp <= 0)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆資料 包裝-高(cm) 錯誤，不可小於 0";
                            break;
                        }
                    }
                }
                #endregion
                #region 重量(kg)
                //重量(kg)(必填)
                //判斷資料是否填寫
                if (string.IsNullOrEmpty(item.Weight) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 重量(kg) 為必填";
                    break;
                }
                else
                {
                    _decimalTemp = 0;
                    //型別錯誤
                    if (decimal.TryParse(item.Weight, out _decimalTemp) == false)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 重量(kg) 錯誤";
                        break;
                    }
                    else
                    {
                        if (_decimalTemp <= 0)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆資料 重量 錯誤，不可小於 0";
                            break;
                        }
                    }
                }
                #endregion
                #region 新品/舊品
                //新品舊品
                //檢查是否選填
                if (string.IsNullOrEmpty(item.IsNew) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 商品類別(新品/舊品) 為必填";
                    break;
                }
                else
                {
                    //選填錯誤
                    if (item.IsNew != "New" && item.IsNew != "Refurbished")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 商品類別(新品/舊品) 資料錯誤，必須為 New or Refurbished";
                        break;
                    }
                }
                #endregion
                #region 完整包裝裸裝(無外盒)
                //完整包裝裸裝(無外盒)(必填)
                //檢查是否填選
                if (string.IsNullOrEmpty(item.ItemPackage) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 商品包裝 為必填";
                    break;
                }
                else
                {
                    if (item.ItemPackage != "OEM" && item.ItemPackage != "Retail")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 商品包裝 資料錯誤，必須為 Retail or OEM";
                        break;
                    }
                }
                #endregion
                #region 成本(seller非必填、vendor必填)
                //成本(seller非必填、vendor必填)
                if (_sellerinfoService.AccountTypeCode == "V")
                {
                    //檢查是否有填寫資料
                    if (string.IsNullOrEmpty(item.Cost) == true)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆 成本(seller非必填、vendor必填) 為必填";
                        break;
                    }
                    else
                    {
                        string resultCheckType = numberVerity(item.Cost);
                        if (resultCheckType.Split(';')[0] == "F")
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆 成本(seller非必填、vendor必填) 資料錯誤，" + resultCheckType.Split(';')[1];
                            break;
                        }
                    }
                }
                else
                {
                    //有填值
                    if (string.IsNullOrEmpty(item.Cost) == false)
                    {
                        string resultCheckType = numberVerity(item.Cost);
                        if (resultCheckType.Split(';')[0] == "F")
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆 成本(seller非必填、vendor必填) 資料錯誤，" + resultCheckType.Split(';')[1];
                            break;
                        }
                    }
                }
                #endregion
                #region 賣價(user價)
                //賣價(user價)(必填)
                //判斷資料是否有填寫
                if (string.IsNullOrEmpty(item.priceCash) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 售價(user價) 為必填";
                    break;
                }
                else
                {
                    string resultCheckType = numberVerity(item.priceCash);
                    if (resultCheckType.Split(';')[0] == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 售價(user價) 資料錯誤，" + resultCheckType.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 毛利率

                if (_sellerinfoService.AccountTypeCode == "V" && !string.IsNullOrEmpty(item.Cost) && !string.IsNullOrEmpty(item.priceCash))
                {
                    string chekPriceCash = numberVerity(item.priceCash);
                    string checkCost = numberVerity(item.Cost);

                    if (chekPriceCash.Split(';')[0] == "T" && checkCost.Split(';')[0] == "T")
                    {
                        decimal cost = 0m;
                        decimal priceCatch = 0m;

                        cost = Convert.ToDecimal(item.Cost);
                        priceCatch = Convert.ToDecimal(item.priceCash);

                        if ((cost > 0 && priceCatch > 0) && cost > priceCatch)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆資料 毛利率 經計算後為負數，請重新設定售價或成本";
                            break;
                        }

                    }
                }

                #endregion 毛利率
                #region 市場建議售價
                //市場建議售價
                if (string.IsNullOrEmpty(item.MarketPrice) == false)
                {
                    string resultCheckType = numberVerity(item.MarketPrice);
                    if (resultCheckType.Split(';')[0] == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 建議售價 資料錯誤，" + resultCheckType.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 保固(月)
                //保固(月)(非必填)
                //判斷是否有填值
                if (string.IsNullOrEmpty(item.Warranty) == false)
                {
                    string resultCheckType = numberVerity(item.Warranty);
                    if (resultCheckType.Split(';')[0] == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 保固(月) 資料錯誤，" + resultCheckType.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 限量數量
                //限量數量
                if (string.IsNullOrEmpty(item.ItemQty) == false)
                {
                    string resultCheckType = numberVerity(item.ItemQty);
                    if (resultCheckType.Split(';')[0] == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 限量數量 資料錯誤，" + resultCheckType.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 限購數量
                //限購數量
                if (string.IsNullOrEmpty(item.QtyLimit) == false)
                {
                    string resultCheckType = numberVerity(item.QtyLimit);
                    if (resultCheckType.Split(';')[0] == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 限購數量 資料錯誤，" + resultCheckType.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 安全庫存量
                if (string.IsNullOrEmpty(item.SaveQty) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 安全庫存量 為必填";
                    break;
                }
                else
                {
                    string resultCheckType = numberVerity(item.SaveQty);
                    if (resultCheckType.Split(';')[0] == "F")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 安全庫存量 資料錯誤，" + resultCheckType.Split(';')[1];
                        break;
                    }
                }
                #endregion
                #region 庫存量
                //庫存量
                if (string.IsNullOrEmpty(item.InventoryQty) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 可售數量 為必填";
                    break;
                }
                else
                {
                    if (string.IsNullOrEmpty(item.InventoryQty) == false)
                    {
                        string resultCheckType = numberVerity(item.InventoryQty);
                        if (resultCheckType.Split(';')[0] == "F")
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "第 " + i + " 筆資料 可售數量 資料錯誤，" + resultCheckType.Split(';')[1];
                            break;
                        }
                    }
                }
                #endregion
                #region 自行出貨/台蛋出貨
                //自行出貨/台蛋出貨
                if (string.IsNullOrEmpty(item.ShipType) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 自行出貨/新蛋出貨 為必填";
                    break;
                }
                else
                {
                    if (item.ShipType != "Newegg" && item.ShipType != "Seller/Vendor")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 自行出貨/新蛋出貨 資料錯誤，必須為 Newegg or Seller/Vendor";
                        break;
                    }
                }
                #endregion
                #region 到貨天數
                //到貨天數
                if (string.IsNullOrEmpty(item.DelvDate) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 到貨天數 為必填";
                    break;
                }
                #endregion
                #region 開賣日期
                DateTime checkStartDate = new DateTime();
                //開賣日期
                if (string.IsNullOrEmpty(item.DateStart) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 開始銷售日期 為必填";
                    break;
                }
                else
                {
                    try
                    {
                        checkStartDate = Convert.ToDateTime(item.DateStart);
                    }
                    catch (Exception error)
                    {
                        logger.Error("/ProductBatch/checkModel 開始銷售日期 error:" + error.Message);
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 開始銷售日期 資料錯誤";
                        break;
                    }
                }
                #endregion
                #region 商品圖檔
                if (string.IsNullOrEmpty(item.PicPatch_Edit) == false)
                {
                    string[] picUrlArray = item.PicPatch_Edit.Replace(" ", "").Split(';');
                    if (picUrlArray.Length > 7)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料商品圖檔 資料錯誤，最多7筆資料";
                        break;
                    }
                }
                #endregion
                #region 結束日期
                //DateTime checkendDate = new DateTime();
                ////結束日期
                //if (string.IsNullOrEmpty(item.DateEnd) == true)
                //{
                //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                //    result.IsSuccess = false;
                //    result.Msg = "第 " + i + " 筆資料 結束日期 為必填";
                //    break;
                //}
                //else
                //{
                //    try
                //    {
                //        checkendDate = Convert.ToDateTime(item.DateEnd);
                //    }
                //    catch (Exception error)
                //    {
                //        logger.Error("/ProductBatch/checkModel 結束日期 error:" + error.Message);
                //        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                //        result.IsSuccess = false;
                //        result.Msg = "第 " + i + " 筆資料 結束日期 資料錯誤";
                //        break;
                //    }
                //}
                //開賣日期不可大於結束日期
                //if (checkStartDate > checkendDate)
                //{
                //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                //    result.IsSuccess = false;
                //    result.Msg = "第 " + i + " 筆資料 開賣日期不可大於結束日期";
                //    break;
                //}
                #endregion
                #region 危險物料
                //危險物料
                if (string.IsNullOrEmpty(item.IsShipDanger) == true)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第 " + i + " 筆資料 危險物料 為必填";
                    break;
                }
                else
                {
                    if (item.IsShipDanger != "Yes" && item.IsShipDanger != "No")
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第 " + i + " 筆資料 危險物料 資料錯誤，必須為 Yes or No";
                        break;
                    }
                }
                #endregion
                i++;
            }
            return result;
        }
        public TWNewEgg.API.Models.ActionResponse<string> checkBatchEditModel(List<TWNewEgg.API.Models.BatchExcelCreate> _listcheck)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> _listManufacturerList = new ActionResponse<List<Manufacturer>>();
            int i = 1;
            Regex reg = new Regex(@"^http[s]?://[\w-_.%/:?=&#]+$");
            _listcheck.RemoveAt(0);//刪除Model memeber name
            _listcheck.RemoveAt(0);
            List<int> checkdata = new List<int>();
            ActionResponse<bool> checkresult = new ActionResponse<bool>();
            #region 初始化
            int tempItemID = 0;
            int tempMainCategoryID_Layer2 = 0;
            int tempSubCategoryID_1_Layer2 = 0;
            int tempSubCategoryID_2_Layer2 = 0;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            result.IsSuccess = true;
            #endregion
            if (_listcheck.Count == 0 || _listcheck == null)
            {
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "請填選正確的資料";
                return result;
            }
            else
            {
                //檢查Excel上傳資料是否超過限制
                if (_listcheck.Count > 100)
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "資料數量超過限制，最多 100 筆資料";
                    return result;
                }

            }
            #region 檢查製造商
            TWNewEgg.API.Models.SearchDataModel searchDataModel = new SearchDataModel();
            searchDataModel.SearchType = TWNewEgg.API.Models.SearchType.SearchofficialALLInfo;

            #endregion
            foreach (var item in _listcheck)
            {
                ItemSketchSearchCondition condition = new ItemSketchSearchCondition();
                ActionResponse<List<AdditionalPurchase>> AdditionalPurchaseList = new ActionResponse<List<AdditionalPurchase>>();
                ActionResponse<List<ItemSketch>> itemSketchList = new ActionResponse<List<ItemSketch>>();
                Service.SellerInfoService SellerInfoService = new Service.SellerInfoService();
                if (item.ItemID == null || SellerInfoService.currentSellerID == null || int.TryParse(item.ItemID, out tempItemID) == false)
                {
                    result.IsSuccess = false;
                    result.Msg = result.Msg + "第 " + i + " 筆: 請填寫 新蛋賣場編號" + "<br>";
                }
                else
                {
                    condition.KeyWord = item.ItemID.ToString();
                    condition.SellerID = SellerInfoService.currentSellerID;
                    condition.KeyWordScarchTarget = ItemSketchKeyWordSearchTarget.ItemID;
                    try
                    {
                        itemSketchList = conn.GetItemTempList(condition, true);

                        AdditionalPurchaseList = conn.GetAdditionalPurchaseItem(condition, true);
                        if (AdditionalPurchaseList.IsSuccess)
                        {
                            if (AdditionalPurchaseList.Body.Count != 0)
                            {
                                if (AdditionalPurchaseList.Body.FirstOrDefault().ShowOrder == (int)TWNewEgg.DB.TWSQLDB.Models.Item.ShowOrderStatus.AddtionalItemForCart)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆  " + "為加價購 不可批次修改" + "<br>";
                                }
                            }
                        }
                        else
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = result.Msg + "第 " + i + " 筆資料  " + "檢查加價購時失敗" + "<br>";
                        }
                    }
                    catch (Exception error)
                    {
                        logger.Error("checkBatchEditModel 事前檢查 第 " + i + " 筆資料 於 連接API.GetItemTempList  例外發生 :" + error.Message);
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = result.Msg + "第 " + i + " 筆資料  " + "發生例外錯誤" + "<br>";
                    }
                }
                string checkitem = "";
                if (itemSketchList.IsSuccess == true)
                {
                    try
                    {
                        checkitem = "檢查 商品名稱";
                        #region 商品名稱(品名)
                        //商品名稱(品名)(必填)
                        //判斷是否有填寫資料
                        //if (string.IsNullOrEmpty(item.Name) == true)
                        //{
                        //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //result.IsSuccess = false;
                        //result.Msg = "第 " + i + " 筆資料 商品名稱(品名) 為必填";
                        //    break;
                        //}
                        #endregion
                        checkitem = "檢查 跨分類是否同一類別";
                        #region 檢查跨分類是在同一類別底下，並檢查跨分類1、跨分類2 的資料型態

                        tempMainCategoryID_Layer2 = int.Parse(itemSketchList.Body[0].ItemCategory.MainCategoryID_Layer2.ToString());

                        if (itemSketchList.Body[0].ItemCategory.SubCategoryID_1_Layer2 != null)
                        {
                            tempSubCategoryID_1_Layer2 = int.Parse(itemSketchList.Body[0].ItemCategory.SubCategoryID_1_Layer2.ToString());
                        }
                        if (itemSketchList.Body[0].ItemCategory.SubCategoryID_2_Layer2 != null)
                        {
                            tempSubCategoryID_2_Layer2 = int.Parse(itemSketchList.Body[0].ItemCategory.SubCategoryID_2_Layer2.ToString());
                        }
                        if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == true && string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == true)
                        {
                            //no SubCategoryID_1_Layer2 and SubCategoryID_2_Layer2, it not need to connect the api CheckCategoryParentId to check the parents are in the same categoly
                            checkresult.IsSuccess = true;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == false)
                            {
                                checkdata.Add(Convert.ToInt32(item.SubCategoryID_1_Layer2));
                            }
                            if (string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == false)
                            {
                                checkdata.Add(Convert.ToInt32(item.SubCategoryID_2_Layer2));
                            }
                            checkresult = conn.CheckCategoryParentId(tempMainCategoryID_Layer2, checkdata);
                        }
                        if (checkresult.IsSuccess == false)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = result.Msg + "第 " + i + " 筆跨分類需要在同一個類別底下" + "<br>";
                        }
                        #endregion
                        checkitem = "檢查 跨分類是否重複";
                        #region 檢查跨分類是否重複
                        if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == false)
                        {
                            if (tempMainCategoryID_Layer2 == int.Parse(item.SubCategoryID_1_Layer2))
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆:  跨分類1 與 主分類 相同" + "<br>";
                            }
                            if (tempSubCategoryID_2_Layer2 == int.Parse(item.SubCategoryID_1_Layer2))
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆:  跨分類 1 與 跨分類 2(原賣場) 相同" + "<br>";
                            }

                            if (string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == false)
                            {
                                if (int.Parse(item.SubCategoryID_2_Layer2) == int.Parse(item.SubCategoryID_1_Layer2))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆:  跨分類 1 與 跨分類 2  相同" + "<br>";
                                }
                                else if (tempSubCategoryID_1_Layer2 == int.Parse(item.SubCategoryID_2_Layer2))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆:  跨分類 2 與 跨分類 1(原賣場) 相同" + "<br>";
                                }
                                else if (int.Parse(item.SubCategoryID_2_Layer2) == tempMainCategoryID_Layer2)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆:  跨分類 2 與 主分類 相同" + "<br>";
                                }
                            }
                        }
                        else if (string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == false)
                        {
                            if (int.Parse(item.SubCategoryID_2_Layer2) == tempMainCategoryID_Layer2)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆:  跨分類 2 與 主分類 相同" + "<br>";
                            }
                            else if (tempSubCategoryID_1_Layer2 == int.Parse(item.SubCategoryID_2_Layer2))
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆:  跨分類 2 與 跨分類 1(原賣場) 相同" + "<br>";
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品描述";
                        #region 商品描述(內文)
                        //商品描述(內文)
                        //判斷是否有填寫資料
                        //if (string.IsNullOrEmpty(item.Description) == true)
                        //{
                        //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //result.IsSuccess = false;
                        //result.Msg = "第 " + i + " 筆資料 商品中文說明 為必填";
                        //    break;
                        //}
                        #endregion
                        checkitem = "檢查 商品特色標籤";
                        #region 商品特色標籤
                        //商品特色標籤
                        //檢查資料是否為空
                        if (string.IsNullOrEmpty(item.Sdesc) == true)
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料  商品特色標籤 為必填";
                            //break;
                        }
                        else
                        {
                            string _checkresult = liTagCHeck(item.Sdesc);
                            string TorF = _checkresult.Split(';')[0];
                            if (TorF == "F")
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆資料  商品特色標籤" + _checkresult.Split(';')[1] + "<br>";
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品簡要描述";
                        #region 商品簡要描述
                        //商品簡要描述
                        //檢查資料是否有填寫
                        if (string.IsNullOrEmpty(item.Spechead) == true)
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料 商品簡要描述 為必填";
                            //break;
                        }
                        else
                        {
                            if (item.Spechead.Length > 30)
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆資料 商品簡要描述 限制為30個字" + "<br>";
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品成本";
                        #region 成本(seller非必填、vendor必填)
                        //成本(seller非必填、vendor必填)
                        if (_sellerinfoService.AccountTypeCode == "V")
                        {
                            //檢查是否有填寫資料
                            if (string.IsNullOrEmpty(item.Cost) == true)
                            {
                                //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                //result.IsSuccess = false;
                                //result.Msg = "第 " + i + " 筆 成本(seller非必填、vendor必填) 為必填";
                                //break;
                            }
                            else
                            {
                                string resultCheckType = numberVerity(item.Cost);
                                if (resultCheckType.Split(';')[0] == "F")
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆 成本 資料錯誤，" + resultCheckType.Split(';')[1] + "<br>";
                                }
                            }
                        }
                        else
                        {
                            //有填值
                            if (string.IsNullOrEmpty(item.Cost) == false)
                            {
                                string resultCheckType = CostnumberVerity(item.Cost);
                                if (resultCheckType.Split(';')[0] == "F")
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆 成本(seller非必填、vendor必填) 資料錯誤，" + resultCheckType.Split(';')[1] + "<br>";
                                }
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品賣價";
                        #region 賣價(user價)
                        //賣價(user價)(必填)
                        //判斷資料是否有填寫
                        if (string.IsNullOrEmpty(item.priceCash) == true)
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料 售價(user價) 為必填";
                            //break;
                        }
                        else
                        {
                            string resultCheckType = numberVerity(item.priceCash);
                            if (resultCheckType.Split(';')[0] == "F")
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆資料 售價(user價) 資料錯誤，" + resultCheckType.Split(';')[1] + "<br>";
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品市場建議售價";
                        #region 市場建議售價
                        //市場建議售價
                        if (string.IsNullOrEmpty(item.MarketPrice) == false)
                        {
                            string resultCheckType = numberVerity(item.MarketPrice);
                            if (resultCheckType.Split(';')[0] == "F")
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆資料 建議售價 資料錯誤，" + resultCheckType.Split(';')[1] + "<br>";
                            }
                        }
                        else
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料 建議售價(user價) 為必填";
                            //break;
                        }
                        #endregion
                        checkitem = "檢查 商品保固";
                        #region 保固(月)
                        //保固(月)(非必填)
                        //判斷是否有填值
                        if (string.IsNullOrEmpty(item.Warranty) == false)
                        {
                            string resultCheckType = numberVerity(item.Warranty);
                            if (resultCheckType.Split(';')[0] == "F")
                            {
                                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                result.IsSuccess = false;
                                result.Msg = result.Msg + "第 " + i + " 筆資料 保固(月) 資料錯誤，" + resultCheckType.Split(';')[1] + "<br>";
                            }
                        }
                        else
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料 保固(月) 為必填";
                            //break;
                        }
                        #endregion
                        checkitem = "檢查 商品安全庫存量";
                        #region 安全庫存量
                        if (string.IsNullOrEmpty(item.SaveQty) == true)
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料 安全庫存量 為必填";
                            //break;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.SaveQty) == false)
                            {
                                Regex numberReg = new Regex(@"^-?\d+$");

                                if (numberReg.IsMatch(item.SaveQty) == false)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 安全庫存量 請填寫數字" + "<br>";
                                }
                                else
                                {
                                    int _intCost = Convert.ToInt32(item.SaveQty);
                                    if (_intCost < 0)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = result.Msg + "第 " + i + " 筆資料 安全庫存量 不可為負數" + "<br>";
                                    }
                                }
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品可售數量";
                        #region 可售數量
                        //庫存量
                        if (string.IsNullOrEmpty(item.InventoryQty) == true)
                        {
                            //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            //result.IsSuccess = false;
                            //result.Msg = "第 " + i + " 筆資料 可售數量 為必填";
                            //break;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.InventoryQty) == false)
                            {
                                Regex numberReg = new Regex(@"^-?\d+$");

                                if (numberReg.IsMatch(item.InventoryQty) == false)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 可售數量 請填寫數字" + "<br>";
                                }
                                else
                                {
                                    int _intCost = Convert.ToInt32(item.InventoryQty);
                                    if (_intCost < 0)
                                    {
                                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                        result.IsSuccess = false;
                                        result.Msg = result.Msg + "第 " + i + " 筆資料 可售數量 不可為負數" + "<br>";
                                    }
                                }
                            }
                        }
                        #endregion
                        checkitem = "檢查 可售數量與安全庫存量";
                        #region 可售數量與安全庫存量判斷
                        if (string.IsNullOrEmpty(item.SaveQty) == true)
                        {
                            if (string.IsNullOrEmpty(item.InventoryQty) == false)
                            {
                                if (int.Parse(item.InventoryQty) == 0)
                                {
                                    item.SaveQty = "0";
                                }
                                else if (int.Parse(item.InventoryQty) < itemSketchList.Body[0].ItemStock.InventorySafeQty)
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 可售數量 必須大於 安全庫存量(原賣場)" + "<br>";
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.InventoryQty) == false)
                            {
                                if (int.Parse(item.InventoryQty) == 0 && int.Parse(item.SaveQty) == 0)
                                {

                                }
                                else if (int.Parse(item.InventoryQty) < int.Parse(item.SaveQty))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 可售數量 必須大於 安全庫存量" + "<br>";
                                }
                                //else if (itemSketchList.Body[0].ItemStock.InventoryQty < int.Parse(item.SaveQty))
                                //{
                                //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                //    result.IsSuccess = false;
                                //    result.Msg = "第 " + i + " 筆資料 可售數量(原賣場) 必須大於 安全庫存量(Excel)";
                                //    break;
                                //}
                                //else if (int.Parse(item.InventoryQty) < itemSketchList.Body[0].ItemStock.InventorySafeQty)
                                //{
                                //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                //    result.IsSuccess = false;
                                //    result.Msg = "第 " + i + " 筆資料 可售數量(Excel) 必須大於 安全庫存量(原賣場)";
                                //    break;
                                //}

                            }
                            else
                            {
                                if (itemSketchList.Body[0].ItemStock.InventoryQty < int.Parse(item.SaveQty))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 可售數量(原賣場) 必須大於 安全庫存量" + "<br>";
                                }
                            }
                        }
                        #endregion
                        checkitem = "檢查 商品毛利率";
                        #region 毛利率判斷
                        if (string.IsNullOrEmpty(item.Cost) == true)
                        {
                            if (string.IsNullOrEmpty(item.priceCash) == false)
                            {

                                if (double.Parse(item.priceCash) < double.Parse(itemSketchList.Body[0].Product.Cost.ToString()))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 成本(原賣場) 必須小於 售價" + "<br>";
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(item.priceCash) == false)
                            {
                                if (double.Parse(item.Cost) > double.Parse(item.priceCash))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 毛利率 經計算後為負數，請重新設定售價或成本" + "<br>";
                                }
                            }
                            else
                            {
                                if (double.Parse(itemSketchList.Body[0].Item.PriceCash.ToString()) < double.Parse(item.Cost))
                                {
                                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                                    result.IsSuccess = false;
                                    result.Msg = result.Msg + "第 " + i + " 筆資料 成本 必須小於 售價(原賣場)" + "<br>";
                                }
                            }
                        }
                        #endregion
                    }
                    catch (Exception error)
                    {
                        logger.Error("批次修改 checkBatchEditModel 事前檢查 第 " + i + " 筆資料 於 " + checkitem + "例外發生 :" + error.Message);
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = result.Msg + "第 " + i + " 筆資料 於 " + checkitem + "發生例外錯誤" + "<br>";
                    }
                }
                else
                {
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    if (itemSketchList.Msg == "")
                    {

                    }
                    else
                    {
                        result.Msg = result.Msg + "第 " + i + " 筆 ，" + itemSketchList.Msg + "<br>";
                    }
                }
                i++;
            }
            return result;
        }
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> createItemBatchEditSketchModel(List<TWNewEgg.API.Models.BatchExcelCreate> model)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            List<TWNewEgg.API.Models.ItemSketch> _listSketch = new List<ItemSketch>();
            TWNewEgg.API.Models.SearchDataModel searchDataModel = new SearchDataModel();
            searchDataModel.SearchType = TWNewEgg.API.Models.SearchType.SearchofficialALLInfo;
            TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> _listManufacturerList = new ActionResponse<List<Manufacturer>>();
            #region 初始化
            result.IsSuccess = true;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            #endregion
            int count = 1;
            foreach (var item in model)
            {
                TWNewEgg.API.Models.ItemSketch s_itemSketch = new ItemSketch();
                #region insert data to model
                try
                {
                    #region 新蛋賣場編號判斷

                    if (string.IsNullOrEmpty(item.ItemID) == true)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第" + count + "新蛋賣場編號 為必填欄位";
                        break;
                    }
                    else
                    {
                        s_itemSketch.Item.ItemID = Convert.ToInt32(item.ItemID);
                    }
                    #endregion
                    #region 跨分類1 判斷
                    //跨分類1沒有值，則判斷第一筆資料的跨分類1是否有填寫資料，有: 把預設值回填，沒有: 就給null
                    if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == true)
                    {
                        s_itemSketch.ItemCategory.SubCategoryID_1_Layer2 = null;
                    }
                    else
                    {
                        s_itemSketch.ItemCategory.SubCategoryID_1_Layer2 = Convert.ToInt32(item.SubCategoryID_1_Layer2);
                    }
                    #endregion
                    #region 跨分類2 判斷
                    //跨分類2沒有值，則判斷第一筆資料的跨分類2是否有填寫資料，有: 把預設值回填，沒有: 就給null
                    if (string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == true)
                    {

                        s_itemSketch.ItemCategory.SubCategoryID_2_Layer2 = null;

                    }
                    else
                    {
                        s_itemSketch.ItemCategory.SubCategoryID_2_Layer2 = Convert.ToInt32(item.SubCategoryID_2_Layer2);
                    }
                    #endregion
                    s_itemSketch.Product.Name = item.Name;
                    s_itemSketch.Product.Description = item.Description;
                    s_itemSketch.Item.Sdesc = item.Sdesc;
                    s_itemSketch.Item.Spechead = item.Spechead;


                    if (string.IsNullOrEmpty(item.SaveQty) == true)
                    {
                        s_itemSketch.ItemStock.InventorySafeQty = null;
                    }
                    else
                    {
                        s_itemSketch.ItemStock.InventorySafeQty = Convert.ToInt32(item.SaveQty);
                    }
                    if (string.IsNullOrEmpty(item.Cost) == true)
                    {
                        //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //result.IsSuccess = false;
                        //result.Msg = "第" + count + "成本 為必填欄位";
                        s_itemSketch.Product.Cost = null;
                    }
                    else
                    {
                        s_itemSketch.Product.Cost = decimal.Parse(item.Cost);
                    }
                    if (string.IsNullOrEmpty(item.priceCash) == true)
                    {
                        s_itemSketch.Item.PriceCash = null;
                    }
                    else
                    {
                        s_itemSketch.Item.PriceCash = decimal.Parse(item.priceCash);
                    }

                    if (string.IsNullOrEmpty(item.MarketPrice) == true)
                    {
                        //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //result.IsSuccess = false;
                        //result.Msg = "第" + count + "市場建議售價 為必填欄位";
                        s_itemSketch.Item.MarketPrice = null;
                    }
                    else
                    {
                        s_itemSketch.Item.MarketPrice = decimal.Parse(item.MarketPrice);
                    }
                    if (string.IsNullOrEmpty(item.Warranty) == true)
                    {
                        //result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        //result.IsSuccess = false;
                        //result.Msg = "第" + count + "保固 為必填欄位";
                        s_itemSketch.Product.Warranty = null;
                    }
                    else
                    {
                        s_itemSketch.Product.Warranty = Convert.ToInt32(item.Warranty);
                    }
                    if (string.IsNullOrEmpty(item.InventoryQty) == true)
                    {
                        s_itemSketch.ItemStock.InventoryQty = null;
                    }
                    else
                    {
                        s_itemSketch.ItemStock.InventoryQty = Convert.ToInt32(item.InventoryQty);
                    }

                    if (string.IsNullOrEmpty(item.SellerProductID) == true)
                    {
                        s_itemSketch.Product.SellerProductID = null;
                    }
                    else
                    {
                        s_itemSketch.Product.SellerProductID = item.SellerProductID;
                    }

                    _listSketch.Add(s_itemSketch);
                    count++;
                }
                catch (Exception error)
                {
                    logger.Error("/ProductBatch/createItemBatchEditSketchModel insert to model error: " + error.Message);
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第" + count + "筆資料錯誤";
                    break;
                }
                #endregion
            }
            if (result.IsSuccess == true)
            {
                result.Body = _listSketch;
            }
            return result;
        }
        public string liTagCHeck(string _strCheck)
        {
            Dictionary<bool, string> result = new Dictionary<bool, string>();
            if (_strCheck.Length <= 500)
            {
                string splitDesctext = _strCheck.Replace(" ", "");
                List<string> checkDesctexts = new List<string>();
                do
                {
                    // 尋找 <li> 的位置
                    int startTag = splitDesctext.IndexOf("<li>");
                    // 尋找 </li> 的位置
                    int endTag = splitDesctext.IndexOf("</li>");
                    // 切割類型
                    string splitType = string.Empty;
                    // 商品簡要描述內容斷點長度
                    int splitLength = 0;

                    // 是否找到第2個 <li>
                    bool isSecondStartTag = false;
                    // 當 <li> 於開頭位置時，判斷 <li> 後面是否還有第2個 <li>
                    if (startTag == 0)
                    {
                        // 先隱藏第一個 <li>
                        string splitFirstStartTag = splitDesctext.Substring(4);
                        // 更新隱藏第一個 <li> 後，下一個 <li> 位置
                        startTag = splitFirstStartTag.IndexOf("<li>");
                        // 更新隱藏 <li> 後的 </li> 位置
                        endTag = splitFirstStartTag.IndexOf("</li>");
                        // 當 <li> 位置小於 </li> 位置時，將是否找到第2個 <li> 設為 true
                        if (startTag < endTag)
                        {
                            isSecondStartTag = true;
                        }
                    }
                    // 判斷是否有找到 <li> 或 </li>
                    if (startTag != -1 || endTag != -1)
                    {
                        // 如果只有找到 </li> 或先找到的是 </li>
                        if ((startTag == -1 && endTag != -1)
                         || (endTag < startTag))
                        {
                            // 使用 </li> 做為切割條件
                            splitType = "EndTag";
                        }
                        else if ((endTag == -1 && startTag != -1)
                              || (startTag < endTag))
                        {
                            // 如果只有找到 <li> 或先找到的是 <li>
                            // 使用 <li> 做為切割條件
                            splitType = "StartTag";
                        }
                    }
                    else
                    {
                        // 都沒找到，則全部內容視為一個斷點
                        splitType = "All";
                    }
                    switch (splitType)
                    {
                        case "StartTag":
                            {
                                // 如果有找到第2個 <li> 
                                if (isSecondStartTag)
                                {
                                    // 先隱藏第一個 <li> ，並在找下一個 <li> 位置後，將商品簡要描述內容斷點長度 + 4 (第一個隱藏的 <li> 字串長度)
                                    splitLength = splitDesctext.Substring(4).IndexOf("<li>") + 4;
                                }
                                else
                                {
                                    // 尋找 <li> 的斷點位置
                                    splitLength = splitDesctext.IndexOf("<li>");
                                }

                                break;
                            }
                        case "EndTag":
                            {
                                // 找到 </li> 位置，並將斷點設在 </li> 之後
                                splitLength = splitDesctext.IndexOf("</li>") + 5;

                                break;
                            }
                        default:
                        case "All":
                            {
                                // 將全部內容視為一個斷點
                                splitLength = splitDesctext.Length;
                                break;
                            }
                    }
                    // 將商品簡要描述內容斷點之前的內容，寫入 List 中
                    checkDesctexts.Add(splitDesctext.Substring(0, splitLength));

                    // 刪除已寫入 List 中的商品簡要描述內容
                    splitDesctext = splitDesctext.Remove(0, splitLength);
                    // 將商品簡要描述內容斷點之前的內容，寫入 List 中
                } while (!string.IsNullOrEmpty(splitDesctext));
                if (string.IsNullOrEmpty(_strCheck) == false)
                {
                    // 判斷商品簡要描述內容是否使用 <li> 及 </li> 包覆
                    bool iscolsebyli = true;
                    // 商品簡要描述內容計數 (商品簡要描述內容最多只允許3項)
                    int descCount = 0;
                    // 判斷商品簡要描述內容是否有空白行
                    bool isEmptyLine = false;
                    // 逐一檢查商品簡要描述內容 List
                    foreach (var text in checkDesctexts)
                    {
                        // 若只有單一的換行，則跳過內容檢查
                        if (text != "\n" && text != "\r" && text != "\r\n" && text != string.Empty)
                        {
                            if (text.IndexOf("<li>") == 0 && text.IndexOf("</li>") != -1)
                            {
                                // 有使用 <li> 及 </li> 包覆，將商品簡要描述內容計數 + 1
                                descCount++;
                            }
                            else if (text.IndexOf("\r\r") != -1 || text.IndexOf("\n\n") != -1)
                            {
                                // 輸入2行以上的換行，則顯示空白行提示
                                isEmptyLine = true;
                            }
                            else
                            {
                                // 未使用 <li> 及 </li> 包覆
                                iscolsebyli = false;

                                // 若只使用 <li> 或只使用 </li>，則將商品簡要描述內容計數 + 1
                                if ((text.IndexOf("<li>") != -1 && text.IndexOf("</li>") == -1)
                                 || (text.IndexOf("<li>") == -1 && text.IndexOf("</li>") != -1))
                                {
                                    descCount++;
                                }
                            }
                        }
                    }
                    // 判斷是否符合商品簡要描述內容
                    // 1.每一點斷行以<li></li>做首尾
                    // 2.最多以三點為上限
                    // 3.不可以有空白行
                    if (iscolsebyli && descCount <= 3 && !isEmptyLine)
                    {
                        //Item.Sdesc = ItemsInfoListDatafeed[40 + (Sequence * 32)];
                    }
                    else
                    {
                        string errorMessage = string.Format("上傳不成功：上傳檔案的Datafeed工作表的商品特色標題內容{0}{1}{2}請檢查修改。",
                            (!iscolsebyli) ? "每一點斷行需要以&ltli&gt&lt/li&gt做首尾，" : string.Empty,
                            (descCount > 3) ? "最多以三點為上限，" : string.Empty,
                            (isEmptyLine) ? "不可以有空白行，" : string.Empty);
                        
                        // 回傳的狀態


                        //ResultCookie("【第" + Column + "行，第" + Row + "列】" + errorMessage);

                        return "F;" + errorMessage;
                    }
                }
            }
            else
            {
                return "F;資料錯誤";      
            }
            return "T;";
        }
        public string numberVerity(string _strCheckTemp)
        {
            Regex numberReg = new Regex(@"^-?\d+$");
            string result = string.Empty;
            if (_strCheckTemp == "0")
            {
                result = "F;不可為0";
                return result;
            }
            if (numberReg.IsMatch(_strCheckTemp) == false)
            {
                result = "F;必須為整數";
                return result;
            }
            int _intCost = Convert.ToInt32(_strCheckTemp);
            if (_intCost <= 0)
            {
                result = "F;不可為負數";
                return result;
            }
            result = "T; ";
            return result;
        }
        public string CostnumberVerity(string _strCheckTemp)
        {
            Regex numberReg = new Regex(@"^-?\d+$");
            string result = string.Empty;
            if (_strCheckTemp == "0")
            {
                result = "F;不可為0";
                return result;
            }
            decimal _intCost = decimal.Parse(_strCheckTemp);
            if (_intCost <= 0)
            {
                result = "F;不可為負數";
                return result;
            }
            result = "T; ";
            return result;
        }
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> createItemSketchModel(List<TWNewEgg.API.Models.BatchExcelCreate> model)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> result = new ActionResponse<List<ItemSketch>>();
            List<TWNewEgg.API.Models.ItemSketch> _listSketch = new List<ItemSketch>();
            TWNewEgg.API.Models.SearchDataModel searchDataModel = new SearchDataModel();
            searchDataModel.SearchType = TWNewEgg.API.Models.SearchType.SearchofficialALLInfo;
            TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> _listManufacturerList = new ActionResponse<List<Manufacturer>>();
            #region 初始化
            result.IsSuccess = true;
            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Success;
            #endregion
            int SubCategoryID_1_Layer2 = 0, SubCategoryID_2_Layer2 = 0;
            try
            {
                if (string.IsNullOrEmpty(model[0].SubCategoryID_1_Layer2) == false)
                {
                    SubCategoryID_1_Layer2 = Convert.ToInt32(model[0].SubCategoryID_1_Layer2);
                }
                if (string.IsNullOrEmpty(model[0].SubCategoryID_2_Layer2) == false)
                {
                    SubCategoryID_2_Layer2 = Convert.ToInt32(model[0].SubCategoryID_2_Layer2);
                }
            }
            catch (Exception error)
            {
                logger.Error("/ProductBatch/createItemSketchModel convert.int32 error: " + error.Message);
                result.IsSuccess = false;
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.Msg = "資料錯誤";
            }
            #region connect to api SearchManufacturerInfo
            try
            {
                //先把通過的製造商搜索出來，後面要進行查詢是否製造商的URL是存在的
                _listManufacturerList = conn.SearchManufacturerInfo(searchDataModel);
            }
            catch(Exception error)
            {
                logger.Error("/ProductBatch/createItemSketchModel error, api error: " + error.Message);
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
            }
            if (result.IsSuccess == false && result.Code == (int)TWNewEgg.API.Models.ResponseCode.Success)
            {
                return result;
            }
            #endregion
            int count = 1;
            foreach (var item in model)
            {
                TWNewEgg.API.Models.ItemSketch s_itemSketch = new ItemSketch();
                #region insert data to model
                try
                {
                    #region MainCategoryID_Layer2判斷
                    //MainCategoryID_Layer2 則回傳錯誤，MainCategoryID_Layer2為必填欄位
                    if (string.IsNullOrEmpty(item.MainCategoryID_Layer2) == true)
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "第" + count + "商品類別 ID 為必填欄位";
                        break;
                    }
                    else
                    {
                        s_itemSketch.ItemCategory.MainCategoryID_Layer2 = Convert.ToInt32(item.MainCategoryID_Layer2);
                    }
                    #endregion
                    #region 跨分類1 判斷
                    //跨分類1沒有值，則判斷第一筆資料的跨分類1是否有填寫資料，有: 把預設值回填，沒有: 就給null
                    if (string.IsNullOrEmpty(item.SubCategoryID_1_Layer2) == true)
                    {
                        if (SubCategoryID_1_Layer2 != 0)
                        {
                            s_itemSketch.ItemCategory.SubCategoryID_1_Layer2 = SubCategoryID_1_Layer2;
                        }
                        else
                        {
                            s_itemSketch.ItemCategory.SubCategoryID_1_Layer2 = null;
                        }
                    }
                    else
                    {
                        s_itemSketch.ItemCategory.SubCategoryID_1_Layer2 = Convert.ToInt32(item.SubCategoryID_1_Layer2);
                    }
                    #endregion
                    #region 跨分類2 判斷
                    //跨分類2沒有值，則判斷第一筆資料的跨分類2是否有填寫資料，有: 把預設值回填，沒有: 就給null
                    if (string.IsNullOrEmpty(item.SubCategoryID_2_Layer2) == true)
                    {
                        if (SubCategoryID_2_Layer2 != 0)
                        {
                            s_itemSketch.ItemCategory.SubCategoryID_2_Layer2 = SubCategoryID_2_Layer2;
                        }
                        else
                        {
                            s_itemSketch.ItemCategory.SubCategoryID_2_Layer2 = null;
                        }
                    }
                    else
                    {
                        s_itemSketch.ItemCategory.SubCategoryID_2_Layer2 = Convert.ToInt32(item.SubCategoryID_2_Layer2);
                    }
                    #endregion
                    s_itemSketch.Product.SellerProductID = item.SellerProductID;
                    var SN_search = _listManufacturerList.Body.Where(p => p.ManufactureURL == item.ManufactureID).FirstOrDefault();
                    s_itemSketch.Product.ManufactureID = SN_search.SN;
                    //if (SN_search == null)
                    //{
                    //    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    //    result.IsSuccess = false;
                    //    result.Msg = "製造商不存在";
                    //    break;
                    //}
                    //else
                    //{
                    //    s_itemSketch.Product.ManufactureID = SN_search.SN;
                    //}
                    s_itemSketch.Product.MenufacturePartNum = item.MenufacturePartNum;
                    s_itemSketch.Product.UPC = item.UPC;
                    s_itemSketch.Product.Name = item.Name;
                    s_itemSketch.Product.Description = item.Description;
                    s_itemSketch.Item.Note = item.Note;
                    s_itemSketch.Item.Sdesc = item.Sdesc;
                    s_itemSketch.Item.Spechead = item.Spechead;
                    s_itemSketch.Product.Model = item.Model;
                    s_itemSketch.Product.BarCode = item.BarCode;
                    s_itemSketch.Product.Length = Convert.ToDecimal(item.Length);
                    s_itemSketch.Product.Width = Convert.ToDecimal(item.Width);
                    s_itemSketch.Product.Height = Convert.ToDecimal(item.Height);
                    s_itemSketch.Product.Weight = Convert.ToDecimal(item.Weight);
                    s_itemSketch.ItemStock.InventorySafeQty = Convert.ToInt32(item.SaveQty);
                    if (item.IsNew == "New")
                    {
                        item.IsNew = "Y";
                    }
                    else if (item.IsNew == "Refurbished")
                    {
                        item.IsNew = "N";
                    }
                    else
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "新品/舊品 資料錯誤";
                        break;
                    }
                    if (item.ItemPackage == "Retail")
                    {
                        s_itemSketch.Item.ItemPackage = "0";
                    }
                    else if (item.ItemPackage == "OEM")
                    {
                        s_itemSketch.Item.ItemPackage = "1";
                    }
                    else
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "完整包裝/裸裝(無外盒) 資料錯誤";
                        break;
                    }

                    if (string.IsNullOrEmpty(item.Cost) == true)
                    {
                        s_itemSketch.Product.Cost = null;
                    }
                    else
                    {
                        s_itemSketch.Product.Cost = Convert.ToInt32(item.Cost);
                    }
                    s_itemSketch.Item.PriceCash = Convert.ToInt32(item.priceCash);
                    if (string.IsNullOrEmpty(item.MarketPrice) == true)
                    {
                        s_itemSketch.Item.MarketPrice = null;
                    }
                    else
                    {
                        s_itemSketch.Item.MarketPrice = Convert.ToInt32(item.MarketPrice);
                    }
                    if (string.IsNullOrEmpty(item.Warranty) == true)
                    {
                        s_itemSketch.Product.Warranty = null;
                    }
                    else
                    {
                        s_itemSketch.Product.Warranty = Convert.ToInt32(item.Warranty);
                    }
                    if (string.IsNullOrEmpty(item.ItemQty) == true)
                    {
                        s_itemSketch.Item.ItemQty = null;
                    }
                    else
                    {
                        s_itemSketch.Item.ItemQty = Convert.ToInt32(item.ItemQty);
                    }
                    if (string.IsNullOrEmpty(item.QtyLimit) == true)
                    {
                        s_itemSketch.Item.QtyLimit = null;
                    }
                    else
                    {
                        s_itemSketch.Item.QtyLimit = Convert.ToInt32(item.ItemQty);
                    }
                    
                    s_itemSketch.ItemStock.CanSaleQty = Convert.ToInt32(item.InventoryQty);
                    if (item.ShipType == "Seller/Vendor")
                    {
                        s_itemSketch.Item.ShipType = "S";
                    }
                    else if (item.ShipType == "Newegg")
                    {
                        s_itemSketch.Item.ShipType = "N";
                    }
                    else
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "自行出貨/台蛋出貨 資料錯誤";
                        break;
                    }
                    s_itemSketch.Item.DelvDate = item.DelvDate;
                    s_itemSketch.Item.DateStart = Convert.ToDateTime(item.DateStart);
                    s_itemSketch.Item.DateEnd = Convert.ToDateTime("2099-12-31");
                    #region 圖片URL 切割，最多7筆資料
                    if (string.IsNullOrEmpty(item.PicPatch_Edit) == false)
                    {
                        string[] picUrlArray = item.PicPatch_Edit.Replace(" ", "").Split(';');
                        if (picUrlArray.Length > 7)
                        {
                            result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                            result.IsSuccess = false;
                            result.Msg = "商品圖檔 資料錯誤，最多7筆資料";
                            break;
                        }
                        else
                        {
                            for (int j = 0; j < picUrlArray.Length; j++)
                            {
                                s_itemSketch.Product.PicPatch_Edit.Add(picUrlArray[j]);
                            }
                        }
                    }
                    #endregion
                    if (item.IsShipDanger == "Yes")
                    {
                        s_itemSketch.Product.IsShipDanger = "Y";
                    }
                    else if (item.IsShipDanger == "No")
                    {
                        s_itemSketch.Product.IsShipDanger = "N";
                    }
                    else
                    {
                        result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                        result.IsSuccess = false;
                        result.Msg = "危險物料 資料錯誤";
                        break;
                    }
                    
                    s_itemSketch.Item.SellerID = _sellerinfoService.currentSellerID;
                    s_itemSketch.CreateAndUpdate.CreateUser = _sellerinfoService.UserID;
                    _listSketch.Add(s_itemSketch);
                    count++;
                }
                catch (Exception error)
                {
                    logger.Error("/ProductBatch/createItemSketchModel insert to model error: " + error.Message);
                    result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Msg = "第" + count +  "筆資料錯誤";
                    break;
                }
                #endregion
            }
            if (result.IsSuccess == true)
            {
                result.Body = _listSketch;
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MainCategoryID_Layer2"></param>
        /// <param name="SubCategoryID_1_Layer2"></param>
        /// <param name="SubCategoryID_2_Layer2"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExcelTemplate(string MainCategoryID_Layer2, string SubCategoryID_1_Layer2, string SubCategoryID_2_Layer2 )
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            if (string.IsNullOrEmpty(MainCategoryID_Layer2) == true)
            {
                return Json("[F]: 請選擇商品類別", JsonRequestBehavior.AllowGet);
            }
            //跨分類1跟2都有選擇下的檢查
            if (string.IsNullOrEmpty(SubCategoryID_1_Layer2) == false && string.IsNullOrEmpty(SubCategoryID_2_Layer2) == false)
            {
                if (MainCategoryID_Layer2 == SubCategoryID_1_Layer2 || MainCategoryID_Layer2 == SubCategoryID_2_Layer2 || SubCategoryID_1_Layer2 == SubCategoryID_2_Layer2)
                {
                    return Json("[F]: 跨分類不可重複選擇", JsonRequestBehavior.AllowGet);
                }
            }
            if (string.IsNullOrEmpty(SubCategoryID_1_Layer2) == false && string.IsNullOrEmpty(SubCategoryID_2_Layer2) == true)
            {
                if (SubCategoryID_1_Layer2 == MainCategoryID_Layer2)
                {
                    return Json("[F]: 跨分類不可重複選擇", JsonRequestBehavior.AllowGet);
                }
            }
            if (string.IsNullOrEmpty(SubCategoryID_1_Layer2) == true && string.IsNullOrEmpty(SubCategoryID_2_Layer2) == false)
            {
                if (SubCategoryID_2_Layer2 == MainCategoryID_Layer2)
                {
                    return Json("[F]: 跨分類不可重複選擇", JsonRequestBehavior.AllowGet);
                }
            }


            //if (MainCategoryID_Layer2 == SubCategoryID_1_Layer2 || MainCategoryID_Layer2 == SubCategoryID_2_Layer2 || SubCategoryID_1_Layer2 == SubCategoryID_2_Layer2)
            //{
            //    return Json("[F]: 跨分類不可重複選擇", JsonRequestBehavior.AllowGet);
            //}

            string _strFileName = "batchItem.xls";
            string _strExcelServerMap = Server.MapPath(@"~/Download/ItemBatch/" + _strFileName);
            
            if (System.IO.File.Exists(_strExcelServerMap) == false)
            {
                logger.Error("/ProductBatch/ExcelTemplate error: Excel 範本檔案不存在");
                return Json("[F]: 下載錯誤，請洽詢客服", JsonRequestBehavior.AllowGet);
            }

            var excelfile = new LinqToExcel.ExcelQueryFactory(_strExcelServerMap);
            var excelData = excelfile.Worksheet<TWNewEgg.API.Models.BatchExcelCreate>("Datafeed");
            
            ActionResponse<string> resultToExcel = createExcel(MainCategoryID_Layer2, SubCategoryID_1_Layer2, SubCategoryID_2_Layer2);
            
            if (resultToExcel.IsSuccess == true)
            {
                return Json("[T]: ", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("[F]: " + resultToExcel.Msg, JsonRequestBehavior.AllowGet);
            }
            
        }
        public JsonResult BatchEditExcelTemplate()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            string _strFileName = "batcheditItem.xls";
            string _strExcelServerMap = Server.MapPath(@"~/Download/ItemBatch/" + _strFileName);

            if (System.IO.File.Exists(_strExcelServerMap) == false)
            {
                logger.Error("/ProductBatch/BatchEditExcelTemplate error: Excel 範本檔案不存在");
                return Json("[F]: 下載錯誤，請洽詢客服", JsonRequestBehavior.AllowGet);
            }

            var excelfile = new LinqToExcel.ExcelQueryFactory(_strExcelServerMap);
            var excelData = excelfile.Worksheet<TWNewEgg.API.Models.BatchExcelCreate>("Datafeed");

            ActionResponse<string> resultToExcel = createBatchEditExcel();

            if (resultToExcel.IsSuccess == true)
            {
                return Json("[T]: ", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("[F]: " + resultToExcel.Msg, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResponse<string> createBatchEditExcel()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            ActionResponse<string> result = new ActionResponse<string>();
            NPOI.SS.UserModel.IWorkbook wb = new NPOI.XSSF.UserModel.XSSFWorkbook();
            NPOI.HSSF.UserModel.HSSFWorkbook hssfwb;
            string _strExcelServerMap = Server.MapPath(@"../Download/ItemBatch/batcheditItem.xls");
            if (System.IO.File.Exists(_strExcelServerMap) == false)
            {
                result.IsSuccess = false;
                result.Msg = "檔案不存在";
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
            }
            try
            {
                using (FileStream file = new FileStream(_strExcelServerMap, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new NPOI.HSSF.UserModel.HSSFWorkbook(file);
                    file.Close();
                }
                using (FileStream file = new FileStream(_strExcelServerMap, FileMode.Open, FileAccess.Write))
                {
                    hssfwb.Write(file);
                    file.Close();
                }
                result.IsSuccess = true;

            }
            catch (Exception error)
            {
                logger.Error("/ProductBatch/createExcel error: " + error.Message);
                result.IsSuccess = false;
                result.Msg = "下載失敗，請洽詢客服。";
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MainCategoryID_Layer2"></param>
        /// <param name="SubCategoryID_1_Layer2"></param>
        /// <param name="SubCategoryID_2_Layer2"></param>
        /// <returns></returns>
        public ActionResponse<string>createExcel(string MainCategoryID_Layer2, string SubCategoryID_1_Layer2, string SubCategoryID_2_Layer2)
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
            ActionResponse<string> result = new ActionResponse<string>();
            NPOI.SS.UserModel.IWorkbook wb = new NPOI.XSSF.UserModel.XSSFWorkbook();
            NPOI.HSSF.UserModel.HSSFWorkbook hssfwb;
            string _strExcelServerMap = Server.MapPath(@"../Download/ItemBatch/batchItem.xls");
            if (System.IO.File.Exists(_strExcelServerMap) == false)
            {
                result.IsSuccess = false;
                result.Msg = "檔案不存在";
                result.Code = (int)TWNewEgg.API.Models.ResponseCode.Error;
            }
            try
            {
                using (FileStream file = new FileStream(_strExcelServerMap, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new NPOI.HSSF.UserModel.HSSFWorkbook(file);
                    file.Close();
                }
                
                NPOI.SS.UserModel.ISheet sheet = hssfwb.GetSheetAt(3);
                sheet.CreateRow(3);
                sheet.GetRow(3).CreateCell(0).SetCellValue(MainCategoryID_Layer2);
                
                if (string.IsNullOrEmpty(SubCategoryID_1_Layer2) == false)
                {
                    sheet.GetRow(3).CreateCell(1).SetCellValue(SubCategoryID_1_Layer2.ToString());                    
                }
                else
                {
                    sheet.GetRow(3).CreateCell(1).SetCellValue("");
                }
                if (string.IsNullOrEmpty(SubCategoryID_2_Layer2) == false)
                {
                    sheet.GetRow(3).CreateCell(2).SetCellValue(SubCategoryID_2_Layer2.ToString());
                }
                else
                {
                    sheet.GetRow(3).CreateCell(2).SetCellValue("");
                }

                using (FileStream file = new FileStream(_strExcelServerMap, FileMode.Open, FileAccess.Write))
                {
                    hssfwb.Write(file);
                    file.Close();
                }
                result.IsSuccess = true;

            }
            catch (Exception error)
            {
                logger.Error("/ProductBatch/createExcel error: " + error.Message);
                result.IsSuccess = false;
                result.Msg = "下載失敗，請洽詢客服。";
            }
            return result;
        }
    }
}
