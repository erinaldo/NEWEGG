using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.API.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using TWNewEgg.API.View.Attributes;
using AutoMapper;
using System.Text.RegularExpressions;
using LinqToExcel;
using Remotion;
using TWNewEgg.API.View;

namespace TWNewEgg.API.View.Controllers
{
    public class ManufacturerBatchController : Controller
    {
        //
        // GET: /ManufacturerBatch/

        TWNewEgg.API.View.Service.SellerInfoService sellerInfo = new Service.SellerInfoService();
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        [HttpGet]
        [FunctionCategoryName(FunctionCategoryNameAttributeValues.ManageItems)]
        [FunctionName(FunctionNameAttributeValues.BatchCreateManufacturer)]
        [ActiveKey(ActiveKeyAttributeValues.Menu)]
        [ActionDescription("製造商批次建立")]
        [Filter.PermissionFilter]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManufacturerBatchCreate()
        {

            return View();
        }

        /// <summary>
        /// 下載excel檔案
        /// </summary>
        /// <returns>回傳檔案</returns>
        public ActionResult DownloadFile()
        {
            string DownloadPath = Server.MapPath("~/Downlaod/Manufacturer/ManufacturerTemplate.xls");
            string filename = System.IO.Path.GetFileName(DownloadPath);
            Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            /* xls mime type:application/vnd.ms-excel */
            return File(iStream, "application/vnd.ms-excel", filename);
        }

        /// <summary>
        /// 上傳Excel檔案
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public JsonResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            Connector Connector = new API.Models.Connector();
            ActionResponse<List<Seller_ManufactureInfo_Edit>> createResult = new ActionResponse<List<Seller_ManufactureInfo_Edit>>();
            string physicalPath = string.Empty;
            string batchResult = string.Empty;
            string IsSuccess = string.Empty;
            
            if (!System.IO.Directory.Exists(this.Server.MapPath(@"~/Upload/Manufacturer")))
            {
                System.IO.Directory.CreateDirectory(this.Server.MapPath(@"~/Upload/Manufacturer"));
            }
            
            // The Name of the Upload component is "files"
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(file.FileName);
                    physicalPath = Path.Combine(Server.MapPath("~/Upload/Manufacturer"), sellerInfo.currentSellerID.ToString() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

                    // The files are not actually saved in this demo
                    file.SaveAs(physicalPath);
                }
            }

            if (System.IO.File.Exists(physicalPath))
            {
                createResult = this.LinqToExcel(physicalPath);
                if (createResult.IsSuccess)
                {
                    IsSuccess = "true";
                    batchResult = createResult.Msg;
                }
                else
                {
                    IsSuccess = "false";
                    batchResult = createResult.Msg;
                }
            }

            // Return an empty string to signify success
            return Json(new { IsSuccess = IsSuccess, batchResult = batchResult });
            //return Json(batchResult,IsSuccess);
        }

        /// <summary>
        /// 讀取Excel
        /// </summary>
        /// <param name="filePath">Excel位置</param>
        /// <returns>成功/錯誤訊息</returns>
        public ActionResponse<List<Seller_ManufactureInfo_Edit>> LinqToExcel(string filePath)
        {
            Connector Connect = new Connector();
            List<TWNewEgg.API.Models.Manufacturer> manuFaturerBatchInfo = new List<Manufacturer>();
            ActionResponse<List<Seller_ManufactureInfo_Edit>> queryResult = new ActionResponse<List<Seller_ManufactureInfo_Edit>>();
            string readExceleMenufaturerInfoResult = string.Empty;
            DateTime nowTime = DateTime.Now;
            bool IsEmpty = true;
            
            try
            {
                var excelFile = new ExcelQueryFactory(filePath);
                var excelData = excelFile.Worksheet<TWNewEgg.API.Models.Manufacturer>("MenufaturerInfo");

                if (excelData != null)
                {
                    readExceleMenufaturerInfoResult = ReadExcelDetailInfo(excelData);

                    if (readExceleMenufaturerInfoResult == string.Empty)
                    {
                        foreach (var manuDetail in excelData)
                        {
                            // 過濾表格中空白行
                            IsEmpty = this.IsEmpty(manuDetail);
                            if (IsEmpty)
                            {
                                continue;
                            }
                            else
                            {
                                manuDetail.UpdateUserID = sellerInfo.currentSellerID;
                                manuDetail.SellerID = sellerInfo.currentSellerID;
                                manuDetail.UpdateDate = nowTime;
                                manuDetail.UserID = sellerInfo.UserID;
                                manuDetail.InDate = nowTime;
                                manuDetail.InUserID = sellerInfo.UserID;
                                manuDetail.ManufactureStatus = "P";
                                manuFaturerBatchInfo.Add(manuDetail);
                            }
                        }
                        manuFaturerBatchInfo = manuFaturerBatchInfo.Skip(1).ToList();
                        queryResult = Connect.CreateManufacturerInfo(manuFaturerBatchInfo);
                        return queryResult;
                    }
                    else
                    {
                        queryResult.IsSuccess = false;
                        queryResult.Msg = readExceleMenufaturerInfoResult;
                    }
                }
                else
                {
                    queryResult.IsSuccess = false;
                    queryResult.Msg = "上傳不成功：上傳檔案(excel)的工作表名稱有誤，請檢查修改，或重新下載檔案進行填寫商家資訊。";
                }

            }
            catch (Exception)
            {
                queryResult.IsSuccess = false;
                queryResult.Msg = "上傳不成功：上傳檔案(excel)的工作表名稱有誤，請檢查修改，或重新下載檔案進行填寫商家資訊。";
            }
            return queryResult;
        }

        /// <summary>
        /// 讀取Excel MenufaturerInfo工作表
        /// </summary>
        /// <param name="manufacturerBatchInfo">要建立的manufacturerBatchInfo</param>
        /// <returns>回傳讀取及檢查的訊息，Empty代表讀取成功</returns>
        private string ReadExcelDetailInfo(LinqToExcel.Query.ExcelQueryable<TWNewEgg.API.Models.Manufacturer> manufacturerBatchInfo)
        {
            //string checkManufacturerResultMessage = string.Empty;
            string checkExcelMenufaturerInfoResult = string.Empty;
            List<TWNewEgg.API.Models.Manufacturer> CreateBatchInfo = new List<TWNewEgg.API.Models.Manufacturer>();

            /* 檢查Excel表格內的資料是否正確 */
            foreach (var Item in manufacturerBatchInfo)
            {
                CreateBatchInfo.Add(Item);
            }

            CreateBatchInfo = CreateBatchInfo.Skip(1).ToList();

            if (CreateBatchInfo.Count == 0)
            {
                checkExcelMenufaturerInfoResult = "上傳不成功：上傳檔案的MenufaturerInfo工作表的 '內容' 有誤，請檢查修改，建議重新下載檔案進行填寫。";
                return checkExcelMenufaturerInfoResult;
            }

            checkExcelMenufaturerInfoResult = CheckDetailManufacturerInfoList(CreateBatchInfo);

            return checkExcelMenufaturerInfoResult;
        }

        /// <summary>
        /// 檢查 Excel MenufaturerInfo 工作表內容
        /// </summary>
        /// <param name="manufacturerInfo">要檢查的ManufactureInfo</param>
        /// <returns>回傳檢查到的錯誤訊息，Empty代表無錯誤</returns>
        private string CheckDetailManufacturerInfoList(List<TWNewEgg.API.Models.Manufacturer> manufacturerInfo)
        {
            string checkDetailResult = string.Empty;
            string emailFormat = @"^.+@.+\..+$";
            Regex emailRegex = new Regex(emailFormat);
            bool IsEmpty = true;
            //bool IsNumeric = false;
            
            foreach (var detailInfo in manufacturerInfo)
            {
                IsEmpty = this.IsEmpty(detailInfo);
                if (IsEmpty)
                {
                    continue;
                }
                else
                {
                    int manufacturerInfoIndex = manufacturerInfo.IndexOf(detailInfo);
                    manufacturerInfoIndex = manufacturerInfoIndex + 1;

                    // 1.ManufactureName 製造商名稱，必填
                    if (!string.IsNullOrEmpty(detailInfo.ManufactureName))
                        detailInfo.ManufactureName = detailInfo.ManufactureName.Replace(" ", "");
                    if (string.IsNullOrEmpty(detailInfo.ManufactureName))
                    {
                        checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 ManufactureName 欄位為必填，請檢查修改。</br>");
                    }
                    else
                    {
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.ManufactureName).Length >= 40)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 ManufactureName 欄位字數限制為40(中文2、英數1)，請檢查修改。</br>");
                        }
                    }

                    // 2.ManufactureURL 網址，必填
                    if (!string.IsNullOrEmpty(detailInfo.ManufactureURL))
                        detailInfo.ManufactureURL = detailInfo.ManufactureURL.Replace(" ", "");
                    if (string.IsNullOrEmpty(detailInfo.ManufactureURL))
                    {
                        checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 ManufactureURL 欄位為必填，請檢查修改。</br>");
                    }
                    else
                    {
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.ManufactureURL).Length >= 256)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 ManufactureURL 欄位字數限制為256，請檢查修改。</br>");
                        }
                    }

                    // 3.SupportEmail 電子信箱
                    if (!string.IsNullOrEmpty(detailInfo.SupportEmail))
                        detailInfo.SupportEmail = detailInfo.SupportEmail.Replace(" ", "");
                    if (!string.IsNullOrEmpty(detailInfo.SupportEmail))
                    {
                        if (!emailRegex.IsMatch(detailInfo.SupportEmail))
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 SupportEmail 格式有誤，請檢查修改。</br>");
                        }
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.SupportEmail).Length >= 256)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 SupportEmail 欄位字數限制為256，請檢查修改。</br>");
                        }
                    }

                    // 4.PhoneRegion 電話(區碼)
                    if (!string.IsNullOrEmpty(detailInfo.PhoneRegion))
                        detailInfo.PhoneRegion = detailInfo.PhoneRegion.Replace(" ", "");
                    if (!string.IsNullOrEmpty(detailInfo.PhoneRegion))
                    {
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.PhoneRegion).Length >= 10)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 PhoneRegion 欄位字數限制為10，請檢查修改。</br>");
                        }
                    }

                    // 5.Phone 電話
                    if (!string.IsNullOrEmpty(detailInfo.Phone))
                        detailInfo.Phone = detailInfo.Phone.Replace(" ", "");
                    if (!string.IsNullOrEmpty(detailInfo.Phone))
                    {
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.Phone).Length >= 30)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 Phone 欄位字數限制為30，請檢查修改。</br>");
                        }

                    }

                    // 6.PhoneExt 電話(分機)
                    if (!string.IsNullOrEmpty(detailInfo.PhoneExt))
                        detailInfo.PhoneExt = detailInfo.PhoneExt.Replace(" ", "");
                    if (!string.IsNullOrEmpty(detailInfo.PhoneExt))
                    {
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.PhoneExt).Length >= 10)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 PhoneExt 欄位字數限制為10，請檢查修改。</br>");
                        }
                    }

                    // 7.supportURL 備援的網址
                    if (!string.IsNullOrEmpty(detailInfo.supportURL))
                        detailInfo.supportURL = detailInfo.supportURL.Replace(" ", "");
                    if (!string.IsNullOrEmpty(detailInfo.supportURL))
                    {
                        if (System.Text.Encoding.Default.GetBytes(detailInfo.supportURL).Length >= 256)
                        {
                            checkDetailResult += string.Format("【第" + manufacturerInfoIndex + "筆】" + "上傳失敗：MenufaturerInfo工作表的 supportURL 欄位字數限制為256，請檢查修改。</br>");
                        }
                    }
                }
            }
            return checkDetailResult;
        }

        /// <summary>
        /// 檢查 Excel MenufaturerInfo 工作表內是否空行
        /// </summary>
        /// <param name="manufacturerInfo">要檢查的ManufactureInfo</param>
        /// <returns>回傳True代表有空行/false代表無空行</returns>
        public bool IsEmpty(TWNewEgg.API.Models.Manufacturer manufacturerInfo)
        {
            bool IsEmpty = true;

            if (!string.IsNullOrEmpty(manufacturerInfo.ManufactureName))
                IsEmpty = false;
            if (!string.IsNullOrEmpty(manufacturerInfo.ManufactureURL))
                IsEmpty = false;
            if (!string.IsNullOrEmpty(manufacturerInfo.SupportEmail))
                IsEmpty = false;
            if (!string.IsNullOrEmpty(manufacturerInfo.PhoneRegion))
                IsEmpty = false;
            if (!string.IsNullOrEmpty(manufacturerInfo.Phone))
                IsEmpty = false;
            if (!string.IsNullOrEmpty(manufacturerInfo.PhoneExt))
                IsEmpty = false;
            if (!string.IsNullOrEmpty(manufacturerInfo.supportURL))
                IsEmpty = false;

            return IsEmpty;
        }
    }
}
