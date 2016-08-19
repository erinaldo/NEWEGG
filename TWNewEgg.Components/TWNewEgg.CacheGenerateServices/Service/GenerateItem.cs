using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Configuration;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.ItemService.Service;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ManufactureServices;
using TWNewEgg.ManufactureServices.Interface;
using TWNewEgg.ManufactureRepoAdapters;
using TWNewEgg.ManufactureRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels;
using TWNewEgg.DAL;
using TWNewEgg.CacheGenerateServices.Model.CSVtoFtpday;
using log4net;

namespace TWNewEgg.CacheGenerateServices.Service
{
    public class GenerateItem
    {
        private TWNewEgg.CategoryService.Service.CategoryApiService categoryService;
        private string priceXMLFolderName = "XMLFindPrice";
        private List<Models.DomainModels.Manufacture.Manufacture> _listManufacture = null;
        private IManufactureService _ManufService = null;
        private string ImageDomain = System.Configuration.ConfigurationManager.AppSettings["ECWebHttpsImgDomain"];
        private ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        public GenerateItem()
        {
            this._ManufService = AutofacConfig.Container.Resolve<IManufactureService>();
            _listManufacture = this._ManufService.GetAll();
        }

        public GenerateItem(IManufactureService argMenufService)
        {
            this._ManufService = argMenufService;
            _listManufacture = this._ManufService.GetAll();
        }

        /// <summary>
        /// 從DB中撈出資料並且產生XML
        /// </summary>
        /// <param name="companyName">預計根據不同的第三方公司，產生不同的結果</param>
        /// <returns></returns>
        public List<string> XMLGenerate(string companyName)
        {
            int takeNumber = 5000;
            List<string> results = new List<string>();
            int totalItemNumber = new int();
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IItemRepoAdapter itemDBService = scope.Resolve<IItemRepoAdapter>();
                var currentItems = itemDBService.GetAll().Where(x => x.Status == 0 && x.ShowOrder >= 0);
                totalItemNumber = currentItems.Count();
            }
            this.categoryService = new TWNewEgg.CategoryService.Service.CategoryApiService();
            DateTime timeNow = DateTime.UtcNow.AddHours(8);
            for (int i = 0; i < Convert.ToInt16(Math.Ceiling(((double)totalItemNumber / takeNumber))); i++)
            {
                var totalItems = GetAllItemFromDB(i * takeNumber, takeNumber, null, null);
                var totalXMLData = ConvertToFindPriceXML(totalItems);

                string serverPath = AppDomain.CurrentDomain.BaseDirectory;
                string fileName = timeNow.Date.Year.ToString() + timeNow.Date.Month.ToString() + timeNow.Date.Day.ToString() + "_" + timeNow.Hour.ToString("D2") + timeNow.Minute.ToString("D2") + "_" + (i + 1).ToString("D3") + ".xml";
                string fullPath = @"\" + priceXMLFolderName + @"\" + fileName;

                if (totalXMLData != null && totalXMLData.products.Count > 0)
                {
                    string xmlString = TWNewEgg.Framework.Common.XMLSerialization.Serializer(totalXMLData);
                    TWNewEgg.Framework.Common.FileUtility.WriteFileByPath("", "XMLFindPrice", fileName, xmlString, true);
                    results.Add(fullPath);
                }
            }


            return results;
        }

        /// <summary>
        /// 產生CSV檔
        /// </summary>
        /// <param name="argNumPerCount">每個檔案的數量</param>
        /// <returns></returns>
        public string CSVGenerate(int argNumPerCount)
        {
            int takeNumber = 5000;
            List<string> results = new List<string>();
            int totalItemNumber = new int();
            List<string> listFileName = null;
            string strFileName = "";
            StreamWriter sw = null;
            string strGap = "\t";
            string strTemp = "";
            bool boolCompress = false;
            bool boolUpload = false;
            bool boolDelete = false;
            DateTime timeNow = DateTime.UtcNow.AddHours(8);
            string strServerPath = AppDomain.CurrentDomain.BaseDirectory;
            string strFolderPath = strServerPath + "\\" + "CSVGenerate\\";
            int numRecordCount = 0;
            int numFileCount = 0;

            /* //FtP連線參數
            string strFtpIp = ConfigurationSettings.AppSettings.Get("DataFeedFtp_Ip");
            string strFtpUserName = ConfigurationSettings.AppSettings.Get("DataFeedFtp_UserName");
            string strFtpPwd = ConfigurationSettings.AppSettings.Get("DataFeedFtp_Pwd");
             */

            string strMessage = "";

            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IItemRepoAdapter itemDBService = scope.Resolve<IItemRepoAdapter>();
                var currentItems = itemDBService.GetAll().Where(x => x.Status == 0 && x.ShowOrder >= 0);
                totalItemNumber = currentItems.Count();
            }
            this.categoryService = new TWNewEgg.CategoryService.Service.CategoryApiService();

            //設定檔名
            listFileName = new List<string>();
            //Change file name's rule. 2015/12/11
            //strFileName = "Newegg_DataFeed_" + timeNow.Year.ToString("D2") + timeNow.Month.ToString("D2") + timeNow.Day.ToString("D2") + "_" + timeNow.Hour.ToString("D2") + timeNow.Minute.ToString("D2") + timeNow.Second.ToString("D2");
            //strFileName = "Newegg_DataFeed_" + timeNow.Year.ToString("D2") + timeNow.Month.ToString("D2") + timeNow.Day.ToString("D2");
            strFileName = timeNow.Year.ToString("D2") + timeNow.Month.ToString("D2") + timeNow.Day.ToString("D2");

            //建立暫存目錄
            if (!Directory.Exists(strFolderPath + "\\" + strFileName))
            {
                Directory.CreateDirectory(strFolderPath + "\\" + strFileName);
            }

            strMessage += String.Format("{0:u}", DateTime.Now) + " : 從DB抓取資料並產生CSV檔<br />";
            for (int i = 0; i < Convert.ToInt16(Math.Ceiling(((double)totalItemNumber / takeNumber))); i++)
            {
                //取得資料
                TotalItemData totalItems = GetAllItemFromDB(i * takeNumber, takeNumber, null, null);
                Model.FindPrice.ProductLists listProducts = ConvertToFindPriceXML(totalItems);

                #region 設定檔案
                if (argNumPerCount <= 0)
                {
                    //不分頁檔案
                    listFileName.Add(strFileName + ".csv");
                    sw = new StreamWriter(strFolderPath + "\\" + strFileName + "\\" + strFileName + ".csv", true, Encoding.UTF8);
                    //行首
                    if (i == 0)
                    {
                        //sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                        //    + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                        //    + strGap + "google_product_category" + strGap + "condition"
                        //    + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                        //http://jira/browse/BSATW-159 Change head column, add category column.
                        sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                            + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                            + strGap + "category1" + strGap + "category2" + strGap + "category3" + strGap + "condition"
                            + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                    }
                }
                else
                {
                    //分頁檔案
                    if (numRecordCount == 0)
                    {
                        if (sw != null)
                        {
                            sw.Flush();
                            sw.Dispose();
                            //sw = null;
                        }
                        //開檔
                        listFileName.Add(strFileName + "_" + numFileCount.ToString("D3") + ".csv");
                        sw = new StreamWriter(strFolderPath + "\\" + strFileName + "\\" + strFileName + "_" + numFileCount.ToString("D3") + ".csv", false, Encoding.UTF8);
                        //行首
                        //sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                        //    + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                        //    + strGap + "google_product_category" + strGap + "condition"
                        //    + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                        //http://jira/browse/BSATW-159 Change head column, add category column.
                        sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                            + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                            + strGap + "category1" + strGap + "category2" + strGap + "category3" + strGap + "condition"
                            + strGap + "availability" + strGap + "price" + strGap + "sale_price");

                        numFileCount++;
                    }
                }
                #endregion

                foreach (Model.FindPrice.Products objProduct in listProducts.products)
                {
                    strTemp = "";

                    #region 逐行寫入檔案
                    //ID
                    strTemp += objProduct.itemID.Trim() + strGap;
                    //title
                    strTemp += objProduct.itemName.Replace("\r\n", "").Replace("\n\r", "").Replace("\n", "").Trim() + strGap;
                    
                    //description
                    if (objProduct.itemDescription!=null)
                    {
                        strTemp += objProduct.itemDescription.Replace("\r\n", "").Replace("\n\r", "").Replace("\n", "") + strGap;
                    }
                    else
                    {
                        strTemp += objProduct.itemDescription + strGap;
                    }

                    //link
                    strTemp += objProduct.itemUrl + strGap;
                    //image_link
                    strTemp += objProduct.itemImage1 + strGap;
                    //product_type
                    strTemp += objProduct.itemCategoryPath + strGap;
                    //brand
                    strTemp += objProduct.itemManufacture + strGap;
                    //google_product_category
                    //strTemp += objProduct.itemGooglePath + strGap;
                    //http://jira/browse/BSATW-159 Change head column, add category column.
                    //category1
                    strTemp += objProduct.itemCategoryPath1 + strGap;
                    //category2
                    strTemp += objProduct.itemCategoryPath2 + strGap;
                    //category3
                    strTemp += objProduct.itemCategoryPath3 + strGap;

                    //condition
                    strTemp += objProduct.itemCondition + strGap;
                    //availability
                    strTemp += objProduct.itemAvailability + strGap;
                    //price
                    strTemp += objProduct.itemMarketPrice.Replace(",", "") + " TWD" + strGap;
                    //sale_price
                    strTemp += objProduct.itemSalePrice.Replace(",", "") + " TWD";

                    //寫入檔案
                    sw.WriteLine(strTemp);
                    #endregion

                    numRecordCount++;

                    //關閉檔案, 並重置計數器
                    if (numRecordCount == argNumPerCount)
                    {
                        //關閉檔案, 釋放記憶體
                        sw.Flush();
                        //sw.Close();
                        sw.Dispose();
                        //sw = null;

                        //重置計數器
                        numRecordCount = 0;

                        //開新檔
                        listFileName.Add(strFileName + "_" + numFileCount.ToString("D3") + ".csv");
                        sw = new StreamWriter(strFolderPath + "\\" + strFileName + "\\" + strFileName + "_" + numFileCount.ToString("D3") + ".csv", false, Encoding.UTF8);
                        //行首
                        //sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                        //    + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                        //    + strGap + "google_product_category" + strGap + "condition"
                        //    + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                        //http://jira/browse/BSATW-159 Change head column, add category column.
                        sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                            + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                            + strGap + "category1" + strGap + "category2" + strGap + "category3" + strGap + "condition"
                            + strGap + "availability" + strGap + "price" + strGap + "sale_price");

                        numFileCount++;
                    }
                }////end foreach - product

                if (argNumPerCount <= 0)
                {
                    //不分頁檔案
                    //關閉檔案, 釋放記憶體
                    sw.Flush();
                    //sw.Close();
                    sw.Dispose();
                    //sw = null;
                }

                if ((i + 1) >= Convert.ToInt16(Math.Ceiling(((double)totalItemNumber / takeNumber))))
                {
                    //關閉檔案, 釋放記憶體
                    sw.Flush();
                    //sw.Close();
                    sw.Dispose();
                    //sw = null;

                    numRecordCount = 0;
                }


            }////end for i

            if (argNumPerCount <= 0)
            {
                //不分頁檔案
                strMessage += "檔案產生成功<br />";
            }
            else
            {
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //將檔案送至壓縮
                strMessage += String.Format("{0:u}", DateTime.Now) + " : 將檔案送至壓縮<br />";
                boolCompress = this.CompressFolder(strFolderPath + "\\" + strFileName, strFolderPath + "\\" + strFileName + ".zip");
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //壓縮成功, 上傳至FTP並刪除舊檔
                if (boolCompress)
                {
                    strMessage += "檔案壓縮成功<br />";
                    //刪除暫存檔
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : 刪除暫存檔<br />";
                    boolDelete = this.DeleteFolder(strFolderPath + "\\" + strFileName);
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                    /*
                    //上傳至FTP
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : 將壓縮檔上傳至FTP<br />";
                    dictFtpLog = this.UploadToFtp(strFolderPath + "\\" + strFileName + ".zip", strFtpIp, strFtpUserName, strFtpPwd);
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                    boolUpload = (bool)dictFtpLog[0];
                    strMessage += (string)dictFtpLog[1];
                    if (boolUpload)
                    {
                        strMessage += "檔案上傳FTP成功<br />";
                    
                    }
                    else
                    {
                        strMessage += "<span style='color:red'>檔案上傳FTP失敗</span><br />";
                    }
                     * */

                }
                else
                {
                    strMessage += "<span style='color:red'>檔案壓縮失敗</span><br />";
                }
            }

            string strResult = "{\"Result\":{$Result}, \"Message\":\"{$Message}\", \"FileName\":\"{$FileName}\"}";
            //替換執行結果
            strResult = strResult.Replace("{$Message}", strMessage);
            if (argNumPerCount <= 0)
            {
                //不分頁檔案
                strResult = strResult.Replace("{$FileName}", strFileName + ".csv");
            }
            else
            {
                strResult = strResult.Replace("{$FileName}", strFileName + ".zip");
            }
            if (boolCompress)
            {
                strResult = strResult.Replace("{$Result}", "true");
            }
            else
            {
                strResult = strResult.Replace("{$Result}", "false");
            }

            return strResult;
        }

        /// <summary>
        /// 使用ftp上傳檔案
        /// </summary>
        /// <param name="argStrSourceFile">上傳檔案的路徑及檔名</param>
        /// <param name="argStrFtp">FTP Url, 格式為ftp://127.0.0.1</param>
        /// <param name="argStrUserName">User Name</param>
        /// <param name="argStrPwd">Password</param>
        /// <returns>dict[0]=上傳結果, dict[1]=歷程及錯誤訊息</returns>
        public Dictionary<int, object> UploadToFtp(string argStrSourceFile, string argStrFtp, string argStrUserName, string argStrPwd)
        {
            Dictionary<int, object> dictResult = null;

            if (String.IsNullOrEmpty(argStrSourceFile) || !File.Exists(argStrSourceFile))
            {
                dictResult = new Dictionary<int, object>();
                dictResult.Add(0, false);
                dictResult.Add(1, "<span style='color:red'>上傳檔案不存在</span><br />");
                return dictResult;
            }

            bool boolExec = false;
            byte[] byteBuffer = null;
            int numByteRead = 0;
            string strFileName = "";
            Stream streamRequest = null;
            FileStream fsFile = null;
            FtpWebResponse responseFtp = null;
            FtpWebRequest requestFtp = null;
            string strMessage = "";

            //取得單純檔名
            strFileName = argStrSourceFile.Substring(argStrSourceFile.LastIndexOf('\\')).TrimStart('\\');

            //檢查ftp的格式
            if (argStrFtp.ToLower().IndexOf("ftp://") < 0)
            {
                argStrFtp = "ftp://" + argStrFtp;
            }
            argStrFtp = argStrFtp.TrimEnd('/');

            try
            {
                //設定FTP基本連線資訊
                strMessage += String.Format("{0:u}", DateTime.Now) + " : 設定FTP基本連線資訊<br />";
                requestFtp = (FtpWebRequest)WebRequest.Create(argStrFtp + "/" + strFileName);
                requestFtp.Method = WebRequestMethods.Ftp.UploadFile;
                requestFtp.Proxy = null;
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //設定FTP連線帳號密碼
                strMessage += String.Format("{0:u}", DateTime.Now) + " : 設定FTP連線帳號密碼<br />";
                if (!String.IsNullOrEmpty(argStrUserName))
                {
                    requestFtp.Credentials = new NetworkCredential(argStrUserName, argStrPwd);
                }
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //上傳資料流
                strMessage += String.Format("{0:u}", DateTime.Now) + " : 上傳資料流<br />";
                streamRequest = requestFtp.GetRequestStream();
                fsFile = File.Open(argStrSourceFile, FileMode.Open);
                byteBuffer = new byte[1024];

                while (true)
                {
                    numByteRead = fsFile.Read(byteBuffer, 0, byteBuffer.Length);
                    if (numByteRead == 0)
                    {
                        break;
                    }
                    streamRequest.Write(byteBuffer, 0, numByteRead);
                }////end while
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //關閉資料流
                strMessage += String.Format("{0:u}", DateTime.Now) + " : 關閉資料流<br />";
                streamRequest.Close();
                responseFtp = (FtpWebResponse)requestFtp.GetResponse();
                boolExec = true;
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

            }////end for try
            catch (Exception ex)
            {
                strMessage += String.Format("{0:u}", DateTime.Now) + " : " + ex.Message + "<br />";
                if (!String.IsNullOrEmpty(ex.StackTrace))
                {
                    strMessage += ex.StackTrace.Replace("\\", "/") + "<br /><br /><br />";
                    strMessage = strMessage.Replace("\r\n", "<br />");
                }
            }

            //關閉資料流及釋放記憶體
            if (responseFtp != null)
            {
                responseFtp.Dispose();
            }
            if (requestFtp != null)
            {
                requestFtp.Abort();
            }
            if (fsFile != null)
            {
                fsFile.Dispose();
            }
            if (streamRequest != null)
            {
                streamRequest.Dispose();
            }

            //回傳執行結果
            dictResult = new Dictionary<int, object>();
            dictResult.Add(0, boolExec);
            dictResult.Add(1, strMessage);
            return dictResult;
        }

        /// <summary>
        /// 檔案壓縮
        /// </summary>
        /// <param name="argListFile"></param>
        /// <param name="argStrTargetFile">輸出檔案</param>
        /// <returns></returns>
        public bool CompressFolder(string argStrSourceFolder, string argStrTargetFile)
        {
            bool boolExec = false;
            if (!Directory.Exists(argStrSourceFolder) || String.IsNullOrEmpty(argStrTargetFile))
            {
                return false;
            }

            try
            {
                ZipFile.CreateFromDirectory(argStrSourceFolder, argStrTargetFile);
                boolExec = true;
            }
            catch (Exception ex)
            {
            }
            return boolExec;
        }

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="argListFile"></param>
        private bool DeleteFolder(string argStrFolder)
        {
            if (String.IsNullOrEmpty(argStrFolder) || !Directory.Exists(argStrFolder))
            {
                return false;
            }

            bool boolExec = false;

            try
            {
                Directory.Delete(argStrFolder, true);
                boolExec = true;
            }
            catch (Exception ex)
            {
            }

            return boolExec;

        }

        /// <summary>
        /// 將DB中Item的資料這個Model轉成FindPrice規定的格式
        /// </summary>
        /// <param name="totalItems"></param>
        /// <returns></returns>
        public Model.FindPrice.ProductLists ConvertToFindPriceXML(TotalItemData totalItems)
        {
            Model.FindPrice.ProductLists results = new Model.FindPrice.ProductLists();

            results.products = new List<Model.FindPrice.Products>();

            for (int i = 0; i < totalItems.itemResults.Count; i++)
            {
                TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice itemDisplayPrice;
                totalItems.itemPriceResults.TryGetValue(totalItems.itemResults[i].ID, out itemDisplayPrice);
                List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM> itemUrls;
                totalItems.itemImgResults.TryGetValue(totalItems.itemResults[i].ID, out itemUrls);
                var singleResult = ConvertItem2FindPriceXML(totalItems.itemResults[i],
                    itemDisplayPrice,
                    itemUrls);
                results.products.Add(singleResult);
            }

            return results;
        }

        /// <summary>
        /// 將DB Data轉成FindPrice所需要的Product Model
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemDisplayPrice"></param>
        /// <param name="itemUrls"></param>
        /// <returns></returns>
        private Model.FindPrice.Products ConvertItem2FindPriceXML(TWNewEgg.Models.DBModels.TWSQLDB.Item item, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice itemDisplayPrice, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM> itemUrls)
        {
            Models.DomainModels.Manufacture.Manufacture objManufacture = this._listManufacture.Where(x => x.ID == item.ManufactureID).FirstOrDefault();
            Model.FindPrice.Products results = new Model.FindPrice.Products();

            var categoryParents = categoryService.GetParents(item.CategoryID, 2, null, null, null);

            if (categoryParents.Count() > 1)
            {
                int ii = 1;
            }

            decimal numMarketPrice = 0;

            //分類, product_category & google_category
            results.itemCategoryPath = String.Join(" > ", categoryParents.OrderBy(x => x.category_layer).Select(x => x.category_description).ToArray());
            string categoryID = "0";
            string storeID = "0";
            if (String.IsNullOrEmpty(results.itemCategoryPath))
            {
                results.itemCategoryPath = "";
                results.itemGooglePath = "";
            }
            else
            {
                var firstCategory = categoryParents.Where(x => x.category_layer == "0").FirstOrDefault();
                var secondCategory = categoryParents.Where(x => x.category_layer == "1").FirstOrDefault();
                var thirdCategory = categoryParents.Where(x => x.category_layer == "2").FirstOrDefault();
                results.itemCategoryPath1 = (firstCategory == null) ? "" : firstCategory.category_description;
                storeID = (firstCategory == null) ? "0" : firstCategory.category_id;
                results.itemCategoryPath2 = (secondCategory == null) ? "" : secondCategory.category_description;
                results.itemCategoryPath3 = (thirdCategory == null) ? "" : thirdCategory.category_description;
                categoryID = (thirdCategory == null) ? "0" : thirdCategory.category_id;
                results.itemGooglePath = results.itemCategoryPath;
            }
            //分類頁Url
            results.itemCategoryUrl = string.Format("http://www.newegg.com.tw/Category?CategoryID={0}", item.CategoryID);
            //產品Id
            results.itemID = item.ID.ToString();

            //產品圖檔, product_image & image2
            if (itemUrls != null)
            {
                if (itemUrls.Where(x => x.Size == 300).Count() > 1)
                {
                    var firstUrl = itemUrls.Where(x => x.Size == 300).ToList()[0];
                    var secondUrl = itemUrls.Where(x => x.Size == 300).ToList()[1];
                    if (firstUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    {
                        results.itemImage1 = firstUrl.ImageUrl;
                    }
                    else
                    {
                        results.itemImage1 = string.Format("{0}{1}", ImageDomain, firstUrl.ImageUrl);
                    }
                    if (secondUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    {
                        results.itemImage2 = secondUrl.ImageUrl;
                    }
                    else
                    {
                        results.itemImage2 = string.Format("{0}{1}", ImageDomain, secondUrl.ImageUrl);
                    }
                }
                else if (itemUrls.Where(x => x.Size == 300).Count() == 1)
                {
                    var firstUrl = itemUrls.Where(x => x.Size == 300).ToList()[0];
                    if (firstUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    {
                        results.itemImage1 = firstUrl.ImageUrl;
                    }
                    else
                    {
                        results.itemImage1 = string.Format("{0}{1}", ImageDomain, firstUrl.ImageUrl);
                    }
                }
                else
                {
                    results.itemImage1 = "";
                    results.itemImage2 = "";
                }
            }
            else
            {
                results.itemImage1 = "";
                results.itemImage2 = "";
            }

            //產品名稱 product_name
            results.itemName = item.Name;

            //產品頁 product_url
            results.itemUrl = string.Format("http://www.newegg.com.tw/item?itemid={0}&categoryid={1}&StoreID={2}", item.ID, categoryID, storeID);

            //產品實際售價, product_price,
            numMarketPrice = item.MarketPrice ?? 0;
            if (itemDisplayPrice != null)
            {
                results.itemSalePrice = String.Format("{0:N2}", itemDisplayPrice.DisplayPrice);
                if (numMarketPrice < itemDisplayPrice.DisplayPrice)
                {
                    numMarketPrice = itemDisplayPrice.DisplayPrice;
                }
            }
            else
            {
                results.itemSalePrice = String.Format("{0:N2}", item.PriceCash);
                if (numMarketPrice < item.PriceCash)
                {
                    numMarketPrice = item.PriceCash;
                }
            }

            //建議售價, product_retail_price
            results.itemMarketPrice = String.Format("{0:N2}", numMarketPrice);

            //廠牌/製造商,product_brand
            if (objManufacture != null && !String.IsNullOrEmpty(objManufacture.Name))
            {
                results.itemManufacture = objManufacture.Name;
            }
            else
            {
                results.itemManufacture = "";
            }

            //固定值的資料:product_condition, product_availability
            results.itemCondition = "new";
            // 判斷此 Item 是否有庫存
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IItemStockService objStockService = scope.Resolve<IItemStockService>();

                var itemStock = objStockService.GetItemSellingQtyByItemId(item.ID);

                results.itemAvailability = (itemStock != null)
                                           && (itemStock.StockQtyAmount.HasValue)
                                           && (itemStock.StockQtyAmount.Value > 0) ? "in stock" : "out of stock";
            }

            return results;
        }

        /// <summary>
        /// 從DB中撈出資料，Item的所有資料，並且可以分頁
        /// </summary>
        /// <param name="startNumber"></param>
        /// <param name="takeNumber"></param>
        /// <returns></returns>
        private TotalItemData GetAllItemFromDBforday(int startNumber, int takeNumber, DateTime? getItemDateStart, DateTime? getItemDateEnd)
        {
            TotalItemData results = new TotalItemData();
            results.itemResults = new List<TWNewEgg.Models.DBModels.TWSQLDB.Item>();
            results.itemImgResults = new Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>();
            results.itemPriceResults = new Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>();
            int numItemQty = 0;
            Dictionary<int, int> dictSellingQty = null;
            int i = 0;
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.Item> currentItems;
            List<TWNewEgg.Models.DBModels.TWSQLDB.Item> items = new List<Models.DBModels.TWSQLDB.Item>();

            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IItemRepoAdapter itemDBService = scope.Resolve<IItemRepoAdapter>();
                IItemImageUrlService itemImgUrlService = scope.Resolve<IItemImageUrlService>();
                IItemPrice itemPriceService = scope.Resolve<IItemPrice>();
                IItemStockService objStockService = scope.Resolve<IItemStockService>();

                if (getItemDateStart.HasValue && getItemDateEnd.HasValue)
                {
                    // 如果取得的時間是一致的，抓取每日特定時間點修改後的商品
                    if (DateTime.Compare(getItemDateStart.Value, getItemDateEnd.Value) == 0)
                    {
                        currentItems = itemDBService.GetAll().Where(x =>
                                                                 x.ShowOrder >= 0
                                                                 && DateTime.Compare(x.UpdateDate.Value, getItemDateStart.Value) >= 0);

                    }
                    else
                    {
                        // 如果取得的時間是不一致的，抓取每日特定時間點前特定小時前修改的商品
                        currentItems = itemDBService.GetAll().Where(x =>
                                                                x.ShowOrder >= 0
                                                                && DateTime.Compare(x.UpdateDate.Value, getItemDateStart.Value) >= 0
                                                                && DateTime.Compare(x.UpdateDate.Value, getItemDateEnd.Value) < 0);

                    }
                }
                else
                {
                    currentItems = itemDBService.GetAll().Where(x => x.Status == 0 && x.ShowOrder >= 0);
                }

                items = currentItems.OrderByDescending(x => x.ID).Skip(startNumber).Take(takeNumber).ToList();

                var itemIDs = items.Select(x => x.ID).ToList();
                results.itemResults.AddRange(items);

                var itemUrlData = itemImgUrlService.GetItemImagePath(itemIDs);
                foreach (var singleImgUrl in itemUrlData)
                {
                    results.itemImgResults.Add(singleImgUrl.Key, singleImgUrl.Value);
                }

                var itemPriceData = itemPriceService.GetItemDisplayPriceByIDs(itemIDs);
                results.itemPriceResults = ModelConverter.ConvertTo<Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>>(itemPriceData);
            }
            return results;
        }

        /// <summary>
        /// 從DB中撈出資料，Item的所有資料，並且可以分頁
        /// </summary>
        /// <param name="startNumber"></param>
        /// <param name="takeNumber"></param>
        /// <returns></returns>
        private TotalItemData GetAllItemFromDB(int startNumber, int takeNumber, DateTime? getItemDateStart, DateTime? getItemDateEnd)
        {
            TotalItemData results = new TotalItemData();
            results.itemResults = new List<TWNewEgg.Models.DBModels.TWSQLDB.Item>();
            results.itemImgResults = new Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>>();
            results.itemPriceResults = new Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>();
            int numItemQty = 0;
            Dictionary<int, int> dictSellingQty = null;
            int i = 0;
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.Item> currentItems;
            List<TWNewEgg.Models.DBModels.TWSQLDB.Item> items = new List<Models.DBModels.TWSQLDB.Item>();

            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IItemRepoAdapter itemDBService = scope.Resolve<IItemRepoAdapter>();
                IItemImageUrlService itemImgUrlService = scope.Resolve<IItemImageUrlService>();
                IItemPrice itemPriceService = scope.Resolve<IItemPrice>();
                IItemStockService objStockService = scope.Resolve<IItemStockService>();

                if (getItemDateStart.HasValue && getItemDateEnd.HasValue)
                {
                    // 如果取得的時間是一致的，抓取每日特定時間點修改後的商品
                    if (DateTime.Compare(getItemDateStart.Value, getItemDateEnd.Value) == 0)
                    {
                        currentItems = itemDBService.GetAll().Where(x =>
                                                                 x.ShowOrder >= 0
                                                                 && DateTime.Compare(x.UpdateDate.Value, getItemDateStart.Value) >= 0);

                    }
                    else
                    {
                        // 如果取得的時間是不一致的，抓取每日特定時間點前特定小時前修改的商品
                        currentItems = itemDBService.GetAll().Where(x =>
                                                                    x.ShowOrder >= 0
                                                                 && DateTime.Compare(x.UpdateDate.Value, getItemDateStart.Value) < 0
                                                                 && DateTime.Compare(x.UpdateDate.Value, getItemDateEnd.Value) > 0);

                    }
                }
                else
                {
                    currentItems = itemDBService.GetAll().Where(x => x.Status == 0 && x.ShowOrder >= 0);
                }

                items = currentItems.OrderByDescending(x => x.ID).Skip(startNumber).Take(takeNumber).ToList();

                //檢查庫存, 無庫存就去除
                dictSellingQty = objStockService.GetSellingQtyByItemList(items.Select(x => x.ID).ToList());
                for (i = 0; i < items.Count; i++)
                {
                    numItemQty = dictSellingQty[items[i].ID];
                    if (numItemQty <= 0)
                    {
                        items.Remove(items[i]);
                        i--;
                    }
                }//end for i


                var itemIDs = items.Select(x => x.ID).ToList();
                results.itemResults.AddRange(items);

                var itemUrlData = itemImgUrlService.GetItemImagePath(itemIDs);
                foreach (var singleImgUrl in itemUrlData)
                {
                    results.itemImgResults.Add(singleImgUrl.Key, singleImgUrl.Value);
                }

                var itemPriceData = itemPriceService.GetItemDisplayPriceByIDs(itemIDs);
                results.itemPriceResults = ModelConverter.ConvertTo<Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice>>(itemPriceData);
            }
            return results;
        }

        /// <summary>
        /// 產生每日的 CSV 檔
        /// </summary>
        /// <param name="jsonValue">CSVtoFtpday Model</param>
        /// <returns></returns>
        public string CSVGenerateDaily(string jsonValue)
        {
            CSVtoFtpday itemModel = new CSVtoFtpday();

            itemModel = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<CSVtoFtpday>(jsonValue);

            string default_Time = System.Configuration.ConfigurationManager.AppSettings["GetItemTimeDefault"];

            int argNumPerCount, cutPage;
            string time_start = string.Empty;
            string time_end = string.Empty;
            double before_Hours = new double();
            // 若傳入的參數無法序列化則給預設值
            if (itemModel == null)
            {
                argNumPerCount = 12000;
                time_start = default_Time;
                before_Hours = 0;
                cutPage = 0;
            }
            else
            {
                argNumPerCount = string.IsNullOrEmpty(itemModel.NumPerCount) ? 12000 : Convert.ToInt32(itemModel.NumPerCount);
                time_start = string.IsNullOrEmpty(itemModel.GetItemTime) ? default_Time : itemModel.GetItemTime;
                before_Hours = string.IsNullOrEmpty(itemModel.getItemHours) ? 0 : Convert.ToDouble(itemModel.getItemHours);
                cutPage = string.IsNullOrEmpty(itemModel.cutPage) ? 0 : ((itemModel.cutPage == "YES") ? 1 : 0);
            }
            int takeNumber = 12000;
            List<string> results = new List<string>();
            int totalItemNumber = new int();
            List<string> listFileName = null;
            string strFileName = "";
            StreamWriter sw = null;
            string strGap = "\t";
            string strTemp = "";
            bool boolCompress = false;
            //bool boolUpload = false;
            bool boolDelete = false;
            DateTime timeNow = DateTime.UtcNow.AddHours(8);
            string strServerPath = AppDomain.CurrentDomain.BaseDirectory;
            string strFolderPath = strServerPath + "\\" + "CSVGenerate\\";
            int numRecordCount = 0;
            int numFileCount = 0;

            /* //FtP連線參數
            string strFtpIp = ConfigurationSettings.AppSettings.Get("DataFeedFtp_Ip");
            string strFtpUserName = ConfigurationSettings.AppSettings.Get("DataFeedFtp_UserName");
            string strFtpPwd = ConfigurationSettings.AppSettings.Get("DataFeedFtp_Pwd");
             */
            logger.Info(string.Format("Daily CSV Generate Start"));
            string strMessage = "";

            #region 時間區間設定



            DateTime start_time = new DateTime();
            DateTime end_time = new DateTime();

            time_start = string.IsNullOrEmpty(time_start) ? default_Time : time_start;
            //before_Hours = string.IsNullOrEmpty(time_end) ? 0 : Convert.ToDouble(time_end);

            // 每日特定時間點
            DateTime.TryParse(DateTime.Now.Date.ToString("yyyy-MM-dd " + time_start), out start_time);

            // 抓今天 x 小時前的時間點
            end_time = start_time;
            start_time = start_time.AddHours((-before_Hours));

            #endregion
            logger.Info(string.Format("Daily CSV Generate StartDate: {0}, EndDate: {1}", start_time.ToString(), end_time.ToString()));
            var ItemList = this.GetAllItemFromDBforday(0, argNumPerCount, start_time, end_time);

            var currentItems = ItemList.itemResults;
            totalItemNumber = currentItems.Count();
            logger.Info(string.Format("Daily CSV Generate TotalItemNumber: {0}", totalItemNumber));
            this.categoryService = new TWNewEgg.CategoryService.Service.CategoryApiService();

            //設定檔名
            listFileName = new List<string>();
            string DaliyFileName = "Newegg_DataFeed_Daily_";
            //Change file name's rule. 2015/12/11
            //strFileName = DaliyFileName + timeNow.Year.ToString("D2") + timeNow.Month.ToString("D2") + timeNow.Day.ToString("D2") + "_" + timeNow.Hour.ToString("D2") + timeNow.Minute.ToString("D2") + timeNow.Second.ToString("D2");
            //strFileName = "Newegg_DataFeed_" + timeNow.Year.ToString("D2") + timeNow.Month.ToString("D2") + timeNow.Day.ToString("D2");
            strFileName = timeNow.Year.ToString("D2") + timeNow.Month.ToString("D2") + timeNow.Day.ToString("D2");

            //建立暫存目錄
            if (!Directory.Exists(strFolderPath + "\\" + strFileName))
            {
                Directory.CreateDirectory(strFolderPath + "\\" + strFileName);
            }

            strMessage += String.Format("{0:u}", DateTime.Now) + " : 從DB抓取資料並產生CSV檔<br />";
            for (int i = 0; i < Convert.ToInt16(Math.Ceiling(((double)totalItemNumber / takeNumber))); i++)
            {
                //取得資料
                TotalItemData totalItems = ItemList;
                Model.FindPrice.ProductLists listProducts = ConvertToFindPriceXML(totalItems);

                #region 設定檔案
                if (cutPage == 0)
                {
                    //不分頁檔案
                    listFileName.Add(strFileName + ".csv");
                    sw = new StreamWriter(strFolderPath + "\\" + strFileName + "\\" + strFileName + ".csv", true, Encoding.UTF8);
                    if (i == 0)
                    {
                        //行首
                        //sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                        //    + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                        //    + strGap + "google_product_category" + strGap + "condition"
                        //    + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                        //http://jira/browse/BSATW-159 Change head column, add category column.
                        sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                            + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                            + strGap + "category1" + strGap + "category2" + strGap + "category3" + strGap + "condition"
                            + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                    }
                }
                else
                {
                    //分頁檔案
                    if (numRecordCount == 0)
                    {
                        if (sw != null)
                        {
                            sw.Flush();
                            sw.Dispose();
                            //sw = null;
                        }
                        //開檔
                        listFileName.Add(strFileName + "_" + numFileCount.ToString("D3") + ".csv");
                        sw = new StreamWriter(strFolderPath + "\\" + strFileName + "\\" + strFileName + "_" + numFileCount.ToString("D3") + ".csv", false, Encoding.UTF8);
                        //行首
                        //sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                        //    + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                        //    + strGap + "google_product_category" + strGap + "condition"
                        //    + strGap + "availability" + strGap + "price" + strGap + "sale_price");
                        //http://jira/browse/BSATW-159 Change head column, add category column.
                        sw.WriteLine("ID" + strGap + "title" + strGap + "description" + strGap + "link"
                            + strGap + "image_link" + strGap + "product_type" + strGap + "brand"
                            + strGap + "category1" + strGap + "category2" + strGap + "category3" + strGap + "condition"
                            + strGap + "availability" + strGap + "price" + strGap + "sale_price");

                        numFileCount++;
                    }
                }
                #endregion

                foreach (Model.FindPrice.Products objProduct in listProducts.products)
                {
                    strTemp = "";

                    #region 逐行寫入檔案
                    //ID
                    strTemp += objProduct.itemID.Trim() + strGap;
                    //title
                    strTemp += objProduct.itemName.Replace("\r\n", "").Replace("\n\r", "").Replace("\n", "").Trim() + strGap;

                    //description
                    if (objProduct.itemDescription!=null)
                    {
                        strTemp += objProduct.itemDescription.Replace("\r\n", "").Replace("\n\r", "").Replace("\n", "") + strGap;
                    }
                    else
                    {
                        strTemp += objProduct.itemDescription + strGap;
                    }
                    //link
                    strTemp += objProduct.itemUrl + strGap;
                    //image_link
                    strTemp += objProduct.itemImage1 + strGap;
                    //product_type
                    strTemp += objProduct.itemCategoryPath + strGap;
                    //brand
                    strTemp += objProduct.itemManufacture + strGap;
                    //google_product_category
                    //strTemp += objProduct.itemGooglePath + strGap;
                    //http://jira/browse/BSATW-159 Change head column, add category column.
                    //category1
                    strTemp += objProduct.itemCategoryPath1 + strGap;
                    //category2
                    strTemp += objProduct.itemCategoryPath2 + strGap;
                    //category3
                    strTemp += objProduct.itemCategoryPath3 + strGap;

                    //condition
                    strTemp += objProduct.itemCondition + strGap;
                    //availability
                    strTemp += objProduct.itemAvailability + strGap;
                    //price
                    strTemp += objProduct.itemMarketPrice.Replace(",", "") + " TWD" + strGap;
                    //sale_price
                    strTemp += objProduct.itemSalePrice.Replace(",", "") + " TWD";

                    //寫入檔案
                    sw.WriteLine(strTemp);
                    #endregion

                    numRecordCount++;
                }////end foreach - product
                logger.Info(string.Format("Daily CSV Generate lines: {0}", numRecordCount));
                if (cutPage == 0)
                {
                    //關閉檔案, 釋放記憶體
                    sw.Flush();
                    //sw.Close();
                    sw.Dispose();
                    //sw = null;
                }

                if ((i + 1) >= Convert.ToInt16(Math.Ceiling(((double)totalItemNumber / takeNumber))))
                {
                    if (sw == null)
                    {
                        break;
                    }
                    else
                    {
                        //關閉檔案, 釋放記憶體
                        sw.Flush();
                        //sw.Close();
                        sw.Dispose();
                        //sw = null;
                    }
                    numRecordCount = 0;
                }


            }////end for i

            if (cutPage == 0)
            {
                //不分頁
                strMessage += "檔案產生成功<br />";
            }
            else
            {

                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //將檔案送至壓縮
                strMessage += String.Format("{0:u}", DateTime.Now) + " : 將檔案送至壓縮<br />";
                boolCompress = this.CompressFolder(strFolderPath + "\\" + strFileName, strFolderPath + "\\" + strFileName + ".zip");
                strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                //壓縮成功, 上傳至FTP並刪除舊檔
                if (boolCompress)
                {
                    strMessage += "檔案壓縮成功<br />";
                    //刪除暫存檔
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : 刪除暫存檔<br />";
                    boolDelete = this.DeleteFolder(strFolderPath + "\\" + strFileName);
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                    /*
                    //上傳至FTP
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : 將壓縮檔上傳至FTP<br />";
                    dictFtpLog = this.UploadToFtp(strFolderPath + "\\" + strFileName + ".zip", strFtpIp, strFtpUserName, strFtpPwd);
                    strMessage += String.Format("{0:u}", DateTime.Now) + " : ..................................................................................... DONE<br /><br />";

                    boolUpload = (bool)dictFtpLog[0];
                    strMessage += (string)dictFtpLog[1];
                    if (boolUpload)
                    {
                        strMessage += "檔案上傳FTP成功<br />";
                    
                    }
                    else
                    {
                        strMessage += "<span style='color:red'>檔案上傳FTP失敗</span><br />";
                    }
                     * */

                }
                else
                {
                    strMessage += "<span style='color:red'>檔案壓縮失敗</span><br />";
                }
            }

            string strResult = "{\"Result\":{$Result}, \"Message\":\"{$Message}\", \"FileName\":\"{$FileName}\"}";
            //替換執行結果
            strResult = strResult.Replace("{$Message}", strMessage);
            if (cutPage == 0)
            {
                //不分頁
                strResult = strResult.Replace("{$FileName}", strFileName + ".csv");
            }
            else
            {
                strResult = strResult.Replace("{$FileName}", strFileName + ".zip");
            }

            if (boolCompress)
            {
                strResult = strResult.Replace("{$Result}", "true");
            }
            else
            {
                strResult = strResult.Replace("{$Result}", "false");
            }

            return strResult;
        }

    }


    /// <summary>
    /// 全部資訊的MODEL，暫存用的
    /// </summary>
    public class TotalItemData
    {
        public List<TWNewEgg.Models.DBModels.TWSQLDB.Item> itemResults { get; set; }
        public Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>> itemImgResults { get; set; }
        public Dictionary<int, TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> itemPriceResults { get; set; }
    }
}
