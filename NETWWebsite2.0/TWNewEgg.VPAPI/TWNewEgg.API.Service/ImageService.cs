using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWNewEgg.API.Models;

namespace TWNewEgg.API.Service
{
    public class ImageService
    {
        // 宣告 Log4net 寫入 Log 
        private static ILog log = LogManager.GetLogger(typeof(ImageService));
        private DB.TWSqlDBContext db = new DB.TWSqlDBContext();

        #region 圖片處理

        public ActionResponse<bool> ImageProcessItemAndProduct(List<string> picturesURL, string FullSizefilePath, string filePath, int ItemID, int ProductID, string ProductfilePath, bool Isreduction = false)
        {
            ActionResponse<bool> queryResult = new ActionResponse<bool>();
            System.Net.WebClient client = new System.Net.WebClient();

            string str_ID = ItemID.ToString();
            
            try
            {
                if (picturesURL != null)
                {
                    var json_picturesURL = Newtonsoft.Json.JsonConvert.SerializeObject(picturesURL);
                    log.Info("ID: " + ItemID + ", FilePath: " + filePath);
                    log.Info("PICURLs: " + json_picturesURL);

                    //ID = ("75403");
                    //int Size = 0;
                    int picCount = 1;

                    List<string> tempImageURL = new List<string>();

                    bool downResult = true;

                    // 第一步 儲存原始圖片到/pic/item及Product
                    foreach (string picURL in picturesURL)
                    {
                        // WebClient 加入 Header 讓Server 認為是瀏覽器訪問
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                        str_ID = str_ID.PadLeft(8, '0');
                        string FilePathformat = AppDomain.CurrentDomain.BaseDirectory + FullSizefilePath + "\\{0}\\{1}_{2}_{3}.jpg";
                        log.Info("[FilePathformat] " + FilePathformat);
                        string path = string.Format(FilePathformat, str_ID.Substring(0, 4), str_ID.Substring(4, 4), picCount, "640");
                        log.Info("[path] " + path);
                        //略過代理驗證 
                        IWebProxy myProxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
                        System.Net.GlobalProxySelection.Select = myProxy;
                        log.Info("[Directory.Exists?] " + path.Substring(0, path.LastIndexOf('\\')));
                        //檢查資料夾路徑
                        if (Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
                        }

                        #region 圖片機 URL 處理

                        string imagesServer = System.Configuration.ConfigurationManager.AppSettings["NeweggImages"];

                        string imagesServerIP = System.Configuration.ConfigurationManager.AppSettings["NeweggImagesIP"];

                        // 判斷是否有圖片機 URL
                        Regex regex = new Regex(imagesServer, RegexOptions.IgnoreCase);

                        string downLoadURL = string.Empty;
                        // 若有則 Replace 成 IP 進行下載圖片處理
                        if (regex.IsMatch(picURL))
                        {
                            downLoadURL = picURL.Replace(imagesServer, imagesServerIP);
                        }
                        else
                        {
                            downLoadURL = picURL;
                        }

                        #endregion


                        //從x的位置下載圖片，若圖片網址回應404，跳過儲存
                        try
                        {
                            log.Info("[picURL] " + downLoadURL + " [path] " + AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");
                            client.DownloadFile(downLoadURL, AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");
                        }
                        catch (Exception ex)
                        {
                            log.Info(string.Format("下載原始大小圖片失敗(expection); Errormessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                            downResult = false;
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        string srcImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";
                        log.Info("[srcImagePath] " + srcImagePath);
                        //使用product的原始圖片做縮圖，丟往第二步
                        tempImageURL.Add(srcImagePath);
                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    if (downResult == true)
                    {
                        // 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                        if (Isreduction)
                        {
                            int saveCount = 1;
                            string str_ProductID = ProductID.ToString();
                            str_ProductID = str_ProductID.PadLeft(8, '0');
                            foreach (var url in tempImageURL)
                            {
                                SaveThumbPicWidth(url, 640, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 300, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 125, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 60, str_ID, saveCount, filePath);
                                // 同時處理，避免下載兩次降低速度

                                SaveThumbPicWidth(url, 640, str_ProductID, saveCount, ProductfilePath);
                                SaveThumbPicWidth(url, 300, str_ProductID, saveCount, ProductfilePath);
                                SaveThumbPicWidth(url, 125, str_ProductID, saveCount, ProductfilePath);
                                SaveThumbPicWidth(url, 60, str_ProductID, saveCount, ProductfilePath);

                                saveCount++;
                            }
                        }
                        else
                        {
                            int saveCount = 1;
                            foreach (var url in tempImageURL)
                            {          
                                SaveThumbPicWidth(url, 640, str_ID, saveCount, filePath);
                                //SaveThumbPicWidth(url, 300, str_ID, saveCount, filePath);
                                //SaveThumbPicWidth(url, 125, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 60, str_ID, saveCount, filePath);
                                saveCount++;
                            }
                        }

                        queryResult.Body = true;
                        queryResult.Msg = "成功存入圖片！";
                        queryResult.Code = (int)ResponseCode.Success;
                        queryResult.IsSuccess = true;

                        // 圖片處理完成，刪除原始大小圖片
                        foreach (var file_index in tempImageURL)
                        {
                            File.Delete(file_index);
                        }
                    }
                    else
                    {
                        queryResult.Body = false;
                        queryResult.Msg = "圖片存入失敗！";
                        queryResult.Code = (int)ResponseCode.Error;
                        queryResult.IsSuccess = false;
                    }
                }
                else
                {
                    //顯示DB寫入狀態(Detail Info)
                    queryResult.Body = false;
                    queryResult.Msg = "部分資料無內容(Item_圖片URL)。";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                    log.Error("Error: ID: " + ItemID + ", filePath: " + filePath + ", Msg: " + queryResult.Msg);
                }
            }
            catch (Exception ex)
            {
                log.Info(string.Format("圖片處理錯誤(expection); Errormessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }

            return queryResult;
        }


        /// <summary>
        /// 圖片處理Service
        /// </summary>
        /// <param name="picturesURL">要download的圖片URL</param>
        /// <param name="FullSizefilePath">原始圖片要儲存的位置，EX: Pic\\Img</param>
        /// <param name="filePath">要縮尺吋的filePath，Ex: Pic\\Pic\\Img</param>
        /// <param name="ID">要儲存的ID，會補滿8碼，Ex: ID:111 Pic\\0000\\0111_index_.jpg</param>
        /// <param name="Isreduction">是否需要進行 300、125的縮圖</param>
        /// <returns></returns>
        public ActionResponse<bool> ImageProcess(List<string> picturesURL, string FullSizefilePath, string filePath, int ID, bool Isreduction = false)
        {
            ActionResponse<bool> queryResult = new ActionResponse<bool>();
            System.Net.WebClient client = new System.Net.WebClient();

            string str_ID = ID.ToString();

            try
            {
                if (picturesURL != null)
                {
                    var json_picturesURL = Newtonsoft.Json.JsonConvert.SerializeObject(picturesURL);
                    log.Info("ID: " + ID + ", FilePath: " + filePath);
                    log.Info("PICURLs: " + json_picturesURL);

                    //ID = ("75403");
                    //int Size = 0;
                    int picCount = 1;

                    List<string> tempImageURL = new List<string>();

                    bool downResult = true;

                    // 第一步 儲存原始圖片到/pic/item及Product
                    foreach (string picURL in picturesURL)
                    {
                        // WebClient 加入 Header 讓Server 認為是瀏覽器訪問
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                        str_ID = str_ID.PadLeft(8, '0');
                        string FilePathformat = AppDomain.CurrentDomain.BaseDirectory + FullSizefilePath + "\\{0}\\{1}_{2}_{3}.jpg";
                        log.Info("[FilePathformat] " + FilePathformat);
                        string path = string.Format(FilePathformat, str_ID.Substring(0, 4), str_ID.Substring(4, 4), picCount, "640");
                        log.Info("[path] " + path);
                        //略過代理驗證 
                        IWebProxy myProxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
                        System.Net.GlobalProxySelection.Select = myProxy;
                        log.Info("[Directory.Exists?] " + path.Substring(0, path.LastIndexOf('\\')));
                        //檢查資料夾路徑
                        if (Directory.Exists(path.Substring(0, path.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(path.Substring(0, path.LastIndexOf('\\')));
                        }

                        #region 圖片機 URL 處理

                        string imagesServer = System.Configuration.ConfigurationManager.AppSettings["NeweggImages"];

                        string imagesServerIP = System.Configuration.ConfigurationManager.AppSettings["NeweggImagesIP"];

                        // 判斷是否有圖片機 URL
                        Regex regex = new Regex(imagesServer, RegexOptions.IgnoreCase);

                        string downLoadURL = string.Empty;
                        // 若有則 Replace 成 IP 進行下載圖片處理
                        if (regex.IsMatch(picURL))
                        {
                            downLoadURL = picURL.Replace(imagesServer, imagesServerIP);
                        }
                        else
                        {
                            downLoadURL = picURL;
                        }

                        #endregion

                        //從x的位置下載圖片，若圖片網址回應404，跳過儲存
                        try
                        {
                            log.Info("[picURL] " + downLoadURL + " [path] " + AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");
                            client.DownloadFile(downLoadURL, AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");

                        }
                        catch (WebException we)
                        {
                            HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                            if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                            {
                                // 將下載失敗的圖片，改成 no picture
                                client.DownloadFile("http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/Content/defautlNoImage.png", AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Info(string.Format("下載原始大小圖片失敗(expection); Errormessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                            continue;
                        }

                        //使用以下位置及檔名儲存圖片
                        string srcImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + FullSizefilePath + "\\" + str_ID.Substring(0, 4) + "\\" + str_ID.Substring(4, 4) + "_" + picCount + "_" + "原始大小.jpg";
                        log.Info("[srcImagePath] " + srcImagePath);
                        //使用product的原始圖片做縮圖，丟往第二步
                        tempImageURL.Add(srcImagePath);
                        //client.DownloadFile(x , @"D:\pic\");
                        picCount++;
                    }

                    if (downResult == true)
                    {
                        // 第二步 使用/pic/item和Product 的圖做縮圖存到 Pic/pic/item&Product Jack.W.Wu mod 0530
                        if (Isreduction)
                        {
                            int saveCount = 1;
                            foreach (var url in tempImageURL)
                            {
                                log.Info("SaveThumbPicWidth(" + url + "," + 640 + "," + str_ID + "," + saveCount + "," + filePath + ")");
                                log.Info("SaveThumbPicWidth(" + url + "," + 300 + "," + str_ID + "," + saveCount + "," + filePath + ")");
                                log.Info("SaveThumbPicWidth(" + url + "," + 125 + "," + str_ID + "," + saveCount + "," + filePath + ")");
                                log.Info("SaveThumbPicWidth(" + url + "," + 60 + "," + str_ID + "," + saveCount + "," + filePath + ")");
                                SaveThumbPicWidth(url, 640, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 300, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 125, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 60, str_ID, saveCount, filePath);
                                saveCount++;
                            }
                        }
                        else
                        {
                            int saveCount = 1;
                            foreach (var url in tempImageURL)
                            {
                                log.Info("SaveThumbPicWidth(" + url + "," + 640 + "," + str_ID + "," + saveCount + "," + filePath + ")");
                                log.Info("SaveThumbPicWidth(" + url + "," + 60 + "," + str_ID + "," + saveCount + "," + filePath + ")");
                                SaveThumbPicWidth(url, 640, str_ID, saveCount, filePath);
                                //SaveThumbPicWidth(url, 300, str_ID, saveCount, filePath);
                                //SaveThumbPicWidth(url, 125, str_ID, saveCount, filePath);
                                SaveThumbPicWidth(url, 60, str_ID, saveCount, filePath);
                                saveCount++;
                            }
                        }

                        queryResult.Body = true;
                        queryResult.Msg = "成功存入圖片！";
                        queryResult.Code = (int)ResponseCode.Success;
                        queryResult.IsSuccess = true;
                    }
                    else
                    {
                        queryResult.Body = false;
                        queryResult.Msg = "圖片存入失敗！";
                        queryResult.Code = (int)ResponseCode.Error;
                        queryResult.IsSuccess = false;
                    }
                }
                else
                {
                    //顯示DB寫入狀態(Detail Info)
                    queryResult.Body = false;
                    queryResult.Msg = "部分資料無內容(Item_圖片URL)。";
                    queryResult.Code = (int)ResponseCode.Success;
                    queryResult.IsSuccess = true;
                    log.Error("Error: ID: " + ID + ", filePath: " + filePath + ", Msg: " + queryResult.Msg);
                }
            }
            catch (Exception ex)
            {
                log.Info(string.Format("圖片處理錯誤(expection); Errormessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                //顯示DB寫入狀態(Detail Info)
                queryResult.Body = false;
                queryResult.Msg = "例外錯誤發生，" + ex.Message;
                queryResult.Code = (int)ResponseCode.Error;
                queryResult.IsSuccess = false;
            }

            return queryResult;
        }

        /// <summary>
        /// 儲存圖片並進行縮圖
        /// </summary>
        /// <param name="srcImagePath">圖片來源URL</param>
        /// <param name="widthMaxPix">最大寬度Pix</param>
        /// <param name="productID">ProductID</param>
        /// <param name="itemID">ItemID</param>
        /// <param name="picCount">紀錄第幾張圖</param>
        public static void SaveThumbPicWidth(string srcImagePath, int widthMaxPix, string ID, int picCount, string filePath)
        {
            using (FileStream fs = new FileStream(srcImagePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                //取得原始圖片
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(fs);

                //圖片寬高
                int imgWidth = bitmap.Width;
                int imgHeight = bitmap.Height;
                // 計算維持比例的縮圖大小
                int[] thumbnailScaleWidth = GetThumbPic_WidthAndHeight(bitmap, widthMaxPix);
                int afterImgWidth = thumbnailScaleWidth[0];
                int afterImgHeight = thumbnailScaleWidth[1];

                // 產生縮圖
                using (var bmp = new Bitmap(afterImgWidth, afterImgHeight))
                {
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        gr.CompositingQuality = CompositingQuality.HighSpeed;
                        gr.SmoothingMode = SmoothingMode.HighSpeed;
                        gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gr.DrawImage(bitmap, new Rectangle(0, 0, afterImgWidth, afterImgHeight), 0, 0, imgWidth, imgHeight, GraphicsUnit.Pixel);

                        string ImagePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + filePath + "\\" + ID.Substring(0, 4) + "\\" + ID.Substring(4, 4) + "_" + picCount + "_" + widthMaxPix + ".jpg";
                        //log.Info("[ImagePath] " + ImagePath);
                        if (Directory.Exists(ImagePath.Substring(0, ImagePath.LastIndexOf('\\'))) == false)
                        {
                            Directory.CreateDirectory(ImagePath.Substring(0, ImagePath.LastIndexOf('\\')));
                        }

                        try
                        {
                            bmp.Save(ImagePath);
                        }
                        catch (Exception ex)
                        {
                            ImageService imageService = new Service.ImageService();
                            log.Info(string.Format("縮圖({2})儲存敗(expection); Errormessage = {0}; StackTrace = {1}.", imageService.GetExceptionMessage(ex), ex.StackTrace, widthMaxPix));
                        }


                }
            }
        }
        }

        /// <summary>
        /// 執行圖片縮圖判斷
        /// </summary>
        /// <param name="image"> 要縮圖的圖片 </param>
        /// <param name="maxPx"> 縮圖的最大限制 </param>
        /// <returns>得到維持比例的縮圖大小比例</returns>
        public static int[] GetThumbPic_WidthAndHeight(System.Drawing.Image image, int maxPx)
        {
            int fixWidth = 0;
            int fixHeight = 0;

            //如果圖片的寬大於最大值或高大於最大值就往下執行 
            if (image.Width != maxPx || image.Height != maxPx)
            {
                //圖片的寬大於圖片的高 
                if (image.Width > image.Height)
                {
                    fixHeight = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(image.Width)) * Convert.ToDouble(image.Height));
                    //設定修改後的圖高 
                    fixWidth = maxPx;
                }
                else
                {
                    fixWidth = Convert.ToInt32((Convert.ToDouble(maxPx) / Convert.ToDouble(image.Height)) * Convert.ToDouble(image.Width));
                    //設定修改後的圖寬 
                    fixHeight = maxPx;
                }
            }
            else
            {
                //圖片沒有超過設定值，不執行縮圖 
                fixHeight = image.Height;

                fixWidth = image.Width;
            }

            int[] fixWidthAndfixHeight = { fixWidth, fixHeight };

            return fixWidthAndfixHeight;
        }

        #endregion

        #region 草稿送審圖片處理
        string images = System.Configuration.ConfigurationManager.AppSettings["Images"];

        public ActionResponse<string> ItemSketchListImgToTemp(List<int> ItemSketchIDs)
        {
            var json_ItemSketchIDs = Newtonsoft.Json.JsonConvert.SerializeObject(ItemSketchIDs);
            log.Info("送審ID: " + json_ItemSketchIDs);
            
            ActionResponse<string> result = new ActionResponse<string>();

            var sketchs = db.ItemSketch.Where(x => ItemSketchIDs.Contains(x.ID)).ToList();            
            foreach (var index in sketchs)
            {
                List<string> picUrls = new List<string>();
                if (index.PicStart != 0 && index.PicEnd != 0)
                {
                    log.Info("ID: " + index.ID);

                    for (int i = 1; i <= index.PicEnd; i++)
                    {
                        // 將 ID 補足 8 碼
                        string itemSketchID = index.ID.ToString("00000000");
                        picUrls.Add(images + "/pic/itemsketch/" + itemSketchID.Substring(0, 4) + "/" + itemSketchID.Substring(4, 4) + "_" + i + "_640.jpg");
                    }

                    var json_PicUrls = Newtonsoft.Json.JsonConvert.SerializeObject(picUrls);

                    log.Info("送審 Img Urls: " + json_PicUrls);

                    // productTemp 圖片儲存
                    this.ImageProcess(picUrls, "pic\\producttemp", "pic\\pic\\producttemp", index.ProducttempID.Value);
                    // ItemTemp 圖片儲存
                    this.ImageProcess(picUrls, "pic\\itemtemp", "pic\\pic\\itemtemp", index.itemtempID.Value);
                }
            }

            return result;
        }

        public ActionResponse<string> ItemSketchDetailImgToTemp(List<string> PicUrls, int ItemSketchID)
        {
            ActionResponse<string> result = new ActionResponse<string>();

            var sketch = db.ItemSketch.Where(x => x.ID == ItemSketchID && x.itemtempID.HasValue && x.ProducttempID.HasValue).FirstOrDefault();
            log.Info("[db.ItemSketch.Where] " + sketch);
            if (sketch != null)
            {
                //log.Info("[PicUrls] " + PicUrls + " [sketch.ProducttempID.Value] " + sketch.ProducttempID.Value);
                log.Info("[PicUrls] " + PicUrls + " [sketch.itemtempID.Value] " + sketch.itemtempID.Value);
                // productTemp 圖片儲存
                //this.ImageProcess(PicUrls, "pic\\producttemp", "pic\\pic\\producttemp", sketch.ProducttempID.Value);
                // ItemTemp 圖片儲存
                this.ImageProcess(PicUrls, "pic\\itemtemp", "pic\\pic\\itemtemp", sketch.itemtempID.Value);
            }
            else
            {
                log.Info("ItemSketch 查無 ItemTempID and ProductTempID");

            }

            return result;
        }

        public ActionResponse<string> ItemSketchBatchImgToTemp(List<string> PicUrls, int itemTempId = 0)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            if (itemTempId == 0)
            {
                result.IsSuccess = false;
                result.Msg = "圖片處理錯誤";
                log.Error("itemTempId 未傳入值");
                return result;
            }
            else
            {
                log.Info("[PicUrls]: " + PicUrls + " [itemTempId] " + itemTempId);
                var ImgResult = this.ImageProcess(PicUrls, "pic\\itemtemp", "pic\\pic\\itemtemp", itemTempId);
                if (ImgResult.IsSuccess == true)
                {
                    log.Info("圖片處理成功: " + ImgResult.Msg);
                    result.IsSuccess = true;
                    result.Msg = "圖片處理成功";
                }
                else
                {
                    log.Info("圖片處理失敗: " + ImgResult.Msg);
                    result.IsSuccess = false;
                    result.Msg = "圖片處理失敗";
                }
            }
            return result;
        }
        #endregion

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
    }
}
