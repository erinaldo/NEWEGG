using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;
using System.Data;

using System.Net;
using System.Web;

using log4net;
using log4net.Config;
using System.Threading;
using TWNewEgg.API.Models;
using LinqToExcel;
using Remotion;
using System.IO;
using System.Reflection;
using System.Transactions;

namespace TWNewEgg.API.Service
{
    public class BatchItemUpdate
    {
        private DB.TWSqlDBContext db = new DB.TWSqlDBContext();
        private DB.TWSqlDBContext itemdb = new DB.TWSqlDBContext();
        private DB.TWSellerPortalDBContext spdb = new DB.TWSellerPortalDBContext();
        //private DB.TWSqlDBContext prodb = new DB.TWSqlDBContext();
        //private DB.TWSqlDBContext stockdb = new DB.TWSqlDBContext();

        private static ILog log = LogManager.GetLogger(typeof(BatchItemUpdate));

        Models.ActionResponse<List<Models.ActionResponse<string>>> massage = new Models.ActionResponse<List<Models.ActionResponse<string>>>();
        List<Models.ActionResponse<string>> result = new List<Models.ActionResponse<string>>();        
                
        public Models.ActionResponse<List<Models.ActionResponse<string>>> LinqFromExcel(string fileName, string sheetName, string UserID)
        {
            if ((string.IsNullOrWhiteSpace(fileName)) || (string.IsNullOrWhiteSpace(sheetName)) || (string.IsNullOrWhiteSpace(UserID)))
            {
                massage.IsSuccess = false;
                massage.Code = (int)ResponseCode.Error;
                massage.Msg = "excel路徑、sheet name、UserID不可為空白，請洽系統工程師！";
                massage.Body = null;

                return massage;
            }

            DateTime dt = DateTime.UtcNow.AddHours(8);
            dt.GetDateTimeFormats('r')[0].ToString();
            Convert.ToDateTime(dt);

            var excelFile = new ExcelQueryFactory(@fileName);

            var excel = from item in excelFile.Worksheet(sheetName) select item;


            //var query = from a in excelFile.Worksheet<ExportToExcelModel>(sheetName) select a;
            //int Cnt = 0;
            //foreach (var item in query)
            //{
            //    Cnt++;
            //    if (Cnt > 1)
            //    {
            //        //string ii = item["ItemID"].ToString();
            //        string jj = item.ItemID.ToString();
            //    }

            //}//for item

            int checkCnt = 0;
            foreach (Row row in excel)
            {
                checkCnt++;
                if (checkCnt > 1)
                {
                    if (string.IsNullOrWhiteSpace(row["ItemID"].ToString()))
                    {
                        //因為list.add是call by reference，所以需要重新new
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：不可為空白！!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }
                    else
                    {
                        int intItemID = Convert.ToInt32(row["ItemID"].ToString());
                        if (db.Item.Where(x => x.ID == intItemID).Count() == 0)
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + row["ItemID"].ToString() + "，沒有此筆資料!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                        else
                        {
                            DB.TWSQLDB.Models.Item ItemInfo = new DB.TWSQLDB.Models.Item();
                            DB.TWSQLDB.Models.Product ProInfo = new DB.TWSQLDB.Models.Product();
                            DB.TWSQLDB.Models.ItemStock StockInfo = new DB.TWSQLDB.Models.ItemStock();

                            ItemInfo = itemdb.Item.Where(x => x.ID == intItemID).SingleOrDefault();
                            int intProductID = db.Item.Where(x => x.ID == intItemID).Select(x => x.ProductID).FirstOrDefault();
                            ProInfo = itemdb.Product.Where(x => x.ID == intProductID).SingleOrDefault();
                            StockInfo = itemdb.ItemStock.Where(x => x.ProductID == intProductID).SingleOrDefault();

                            DataValidation(row, ProInfo, ItemInfo);
                        }
                    }
                }
            }


            if (result.Count == 0)
            {
                try
                {
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                        int saveCnt = 0;
                        TWService IPPPrice = new TWService();
                        foreach (Row row in excel)
                        {
                            saveCnt++;
                            if (saveCnt > 1)
                            {
                                DB.TWSQLDB.Models.Item ItemInfo = new DB.TWSQLDB.Models.Item();
                                DB.TWSQLDB.Models.Product ProInfo = new DB.TWSQLDB.Models.Product();
                                DB.TWSQLDB.Models.ItemStock StockInfo = new DB.TWSQLDB.Models.ItemStock();

                                int intItemID = Convert.ToInt32(row["ItemID"].ToString());
                                ItemInfo = itemdb.Item.Where(x => x.ID == intItemID).SingleOrDefault();
                                int intProductID = db.Item.Where(x => x.ID == intItemID).Select(x => x.ProductID).FirstOrDefault();
                                ProInfo = itemdb.Product.Where(x => x.ID == intProductID).SingleOrDefault();
                                StockInfo = itemdb.ItemStock.Where(x => x.ProductID == intProductID).SingleOrDefault();

                                if (ItemInfo != null)
                                {
                                    ItemInfo.Name = string.IsNullOrWhiteSpace(row["WebsiteShortTitle"].ToString()) ? ItemInfo.Name : row["WebsiteShortTitle"].ToString();
                                    ItemInfo.DescTW = ItemInfo.ItemDesc = string.IsNullOrWhiteSpace(row["ProductDescription"].ToString()) ? ItemInfo.DescTW : row["ProductDescription"].ToString();
                                    ItemInfo.Note = string.IsNullOrWhiteSpace(row["ProductNote"].ToString()) ? ItemInfo.Note : row["ProductNote"].ToString();
                                    ItemInfo.Sdesc = string.IsNullOrWhiteSpace(row["ProductShortDesc"].ToString()) ? ItemInfo.Sdesc : row["ProductShortDesc"].ToString();
                                    ItemInfo.Spechead = string.IsNullOrWhiteSpace(row["ProductFeatureTitle"].ToString()) ? ItemInfo.Spechead : row["ProductFeatureTitle"].ToString();
                                    ItemInfo.PriceCash = ItemInfo.PriceCard = string.IsNullOrWhiteSpace(row["SellingPrice"].ToString()) ? ItemInfo.PriceCash : Convert.ToDecimal(row["SellingPrice"].ToString());
                                    ItemInfo.MarketPrice = string.IsNullOrWhiteSpace(row["MarketPrice"].ToString()) ? ItemInfo.MarketPrice : Convert.ToDecimal(row["MarketPrice"].ToString());
                                    ItemInfo.DelvDate = string.IsNullOrWhiteSpace(row["DelvDay"].ToString()) ? ItemInfo.DelvDate : row["DelvDay"].ToString();
                                    ItemInfo.UpdateUser = UserID;
                                    ItemInfo.UpdateDate = dt;
                                    //itemdb.Entry(ItemInfo).State = EntityState.Modified;
                                }

                                if (ProInfo != null)
                                {
                                    ProInfo.Name = ProInfo.NameTW = string.IsNullOrWhiteSpace(row["WebsiteShortTitle"].ToString()) ? ProInfo.NameTW : row["WebsiteShortTitle"].ToString();
                                    ProInfo.DescriptionTW = ProInfo.Description = string.IsNullOrWhiteSpace(row["ProductDescription"].ToString()) ? ProInfo.DescriptionTW : row["ProductDescription"].ToString();
                                    ProInfo.Note = string.IsNullOrWhiteSpace(row["ProductNote"].ToString()) ? ProInfo.Note : row["ProductNote"].ToString();
                                    ProInfo.Cost = string.IsNullOrWhiteSpace(row["Cost"].ToString()) ? ProInfo.Cost : Convert.ToDecimal(row["Cost"].ToString());
                                    ProInfo.Warranty = string.IsNullOrWhiteSpace(row["Warranty"].ToString()) ? ProInfo.Warranty : Convert.ToInt32(row["Warranty"].ToString());
                                    ProInfo.UpdateUser = UserID;
                                    ProInfo.UpdateDate = dt;
                                    //prodb.Entry(ProInfo).State = EntityState.Modified;
                                }
                                if (StockInfo != null)
                                {
                                    StockInfo.Qty = string.IsNullOrWhiteSpace(row["Inventory"].ToString()) ? StockInfo.Qty : Convert.ToInt32(row["Inventory"].ToString()) + StockInfo.QtyReg;
                                    StockInfo.UpdateUser = UserID;
                                    StockInfo.UpdateDate = dt;
                                    //stockdb.Entry(StockInfo).State = EntityState.Modified;
                                }
                                itemdb.Entry(ItemInfo).State = EntityState.Modified;
                                itemdb.SaveChanges();
                                //prodb.SaveChanges();
                                //stockdb.SaveChanges();

                                string priceChangeResult = "default";
                                // 判斷是否使用總價化API更新商品價格
                                if (HttpContext.Current.Request.Url.Authority == "localhost:57035")
                                {
                                    //開發區不呼叫總價化API，直接回傳成功
                                    priceChangeResult = string.Empty;
                                }
                                else
                                {
                                    log.Info("ItemID: " + intItemID + "; Type:EditCreatedItem - call IPP API before");  
                                    priceChangeResult = IPPPrice.PriceAPI(intItemID);
                                }

                                if (!string.IsNullOrEmpty(priceChangeResult))
                                {
                                    Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                                    errorInfo.Msg = "賣場編號：" + intItemID + "，" + priceChangeResult;
                                    errorInfo.Code = (int)ResponseCode.Error;
                                    errorInfo.IsSuccess = false;
                                    massage.Body.Add(errorInfo);
                                }
                            }
                        }
                        //scope.Complete();
                        if (massage.Body == null)
                        {
                            massage.Msg = saveCnt - 1 + "筆資料更新成功^^";
                            massage.Code = (int)ResponseCode.Success;
                            massage.IsSuccess = true;
                        }
                        else
                        {
                            massage.Msg = saveCnt - 1 + "筆資料更新成功^^，但總價化更新失敗，請將失敗列表提供給系統管理員..";
                            massage.Code = (int)ResponseCode.Error;
                            massage.IsSuccess = false;
                        }
                    //}
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    massage.IsSuccess = false;
                    massage.Code = (int)ResponseCode.Error;
                    massage.Msg = "例外發生: " + ex.Message;
                    massage.Body = null;

                    return massage;
                }                
            }
            else
            {
                massage.Msg = "資料更新失敗，請參考錯誤訊息列表> <";
                massage.Code = (int)ResponseCode.Error;
                massage.IsSuccess = false;
                massage.Body = result;
            }

            return massage;
        }

        //excel欄位檢查
        private void DataValidation(Row ExcelData, DB.TWSQLDB.Models.Product ProInfo, DB.TWSQLDB.Models.Item ItemInfo)
        {
            //****檢查數字字串是否為數字


            if (!string.IsNullOrWhiteSpace(ExcelData["ProductShortDesc"].ToString()))
            {
                if (ExcelData["ProductShortDesc"].ToString().Length >= 500)
                {
                    Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)不可超過中文250字 or 英數500字!!!";
                    errorInfo.Code = (int)ResponseCode.Error;
                    errorInfo.IsSuccess = false;
                    result.Add(errorInfo);
                }
                else
                {
                    #region 將商品簡要描述內容，拆成 List 待檢查

                    // 要拆成 List 檢查的商品簡要描述內容
                    string splitDescText = ExcelData["ProductShortDesc"].ToString();

                    // 商品簡要描述內容 List
                    List<string> checkDescTexts = new List<string>();

                    do
                    {
                        // 尋找 <li> 的位置
                        int startTag = splitDescText.IndexOf("<li>");

                        // 尋找 </li> 的位置
                        int endTag = splitDescText.IndexOf("</li>");

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
                            string splitFirstStartTag = splitDescText.Substring(4);

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
                                        splitLength = splitDescText.Substring(4).IndexOf("<li>") + 4;
                                    }
                                    else
                                    {
                                        // 尋找 <li> 的斷點位置
                                        splitLength = splitDescText.IndexOf("<li>");
                                    }

                                    break;
                                }
                            case "EndTag":
                                {
                                    // 找到 </li> 位置，並將斷點設在 </li> 之後
                                    splitLength = splitDescText.IndexOf("</li>") + 5;

                                    break;
                                }
                            default:
                            case "All":
                                {
                                    // 將全部內容視為一個斷點
                                    splitLength = splitDescText.Length;
                                    break;
                                }
                        }

                        // 將商品簡要描述內容斷點之前的內容，寫入 List 中
                        checkDescTexts.Add(splitDescText.Substring(0, splitLength));

                        // 刪除已寫入 List 中的商品簡要描述內容
                        splitDescText = splitDescText.Remove(0, splitLength);
                    }
                    while (!string.IsNullOrEmpty(splitDescText));

                    #endregion 將商品簡要描述內容，拆成 list 待檢查

                    // 判斷商品簡要描述內容是否使用 <li> 及 </li> 包覆
                    bool isColseByLiTag = true;

                    // 商品簡要描述內容計數 (商品簡要描述內容最多只允許3項)
                    int descCount = 0;

                    // 判斷商品簡要描述內容是否有空白行
                    bool isEmptyLine = false;

                    // 逐一檢查商品簡要描述內容 List
                    foreach (var text in checkDescTexts)
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
                                isColseByLiTag = false;

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
                    if (isColseByLiTag == false)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)句首需要加上<LI>，句尾需要加上</LI>!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }

                    if(descCount > 3)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)不可超過三點<LI></LI>!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }

                    if (isEmptyLine)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)不可以有空白行!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }

                    ////****加上<li></li>首尾檢查
                    //string liStart = "<LI>";
                    //string liEnd = "</LI>";
                    //string strDesc = ExcelData["ProductShortDesc"].ToString().ToUpper().Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    ////****加上三點<li>檢查
                    //int cnt = 0;
                    //for (int i = strDesc.Length; i > 0; )
                    //{
                    //    cnt++;
                    //    if (!(strDesc.StartsWith(liStart)) || !(strDesc.EndsWith(liEnd)))
                    //    {
                    //        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    //        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)句首需要加上<LI>，句尾需要加上</LI>!!!";
                    //        errorInfo.Code = (int)ResponseCode.Error;
                    //        errorInfo.IsSuccess = false;
                    //        result.Add(errorInfo);
                    //    }

                    //    int intStart = strDesc.IndexOf(liStart) + liStart.Length;
                    //    int intEnd = strDesc.IndexOf(liEnd);
                    //    string strMiddle = strDesc.Substring(intStart, intEnd - intStart);
                    //    if (strMiddle.IndexOf(liStart) >= 0)
                    //    {
                    //        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    //        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)<LI></LI>中間不可再加<LI>!!!";
                    //        errorInfo.Code = (int)ResponseCode.Error;
                    //        errorInfo.IsSuccess = false;
                    //        result.Add(errorInfo);
                    //    }
                    //    strDesc = strDesc.Substring(intEnd + liEnd.Length);
                    //    i = strDesc.Length;
                    //    if (cnt > 3)
                    //    {
                    //        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    //        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品簡要描述(Product Short Desc)不可超過三點<LI></LI>!!!";
                    //        errorInfo.Code = (int)ResponseCode.Error;
                    //        errorInfo.IsSuccess = false;
                    //        result.Add(errorInfo);
                    //        //break;
                    //    }
                    //}
                }
            }

            if (!string.IsNullOrWhiteSpace(ExcelData["ProductFeatureTitle"].ToString()))
            {
                if (ExcelData["ProductFeatureTitle"].ToString().Length >= 30)
                {
                    Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，商品特色標題(Product Feature Title)不可超過中文15字 or 英數30字!!!";
                    errorInfo.Code = (int)ResponseCode.Error;
                    errorInfo.IsSuccess = false;
                    result.Add(errorInfo);
                }
            }

            //依身分別檢查cost是否可填寫
            string accType = spdb.Seller_BasicInfo.Where(x => x.SellerID == ProInfo.SellerID).Select(x => x.AccountTypeCode).SingleOrDefault();
            
            if (!string.IsNullOrWhiteSpace(ExcelData["Cost"].ToString()) || !string.IsNullOrWhiteSpace(ExcelData["SellingPrice"].ToString()))
            {
                decimal sellprice = 0;
                decimal cost = 0;
                bool isSelling, isCost;
                isSelling = decimal.TryParse(ExcelData["SellingPrice"].ToString(), out sellprice);
                isCost = decimal.TryParse(ExcelData["Cost"].ToString(), out cost);
                if (!string.IsNullOrWhiteSpace(ExcelData["SellingPrice"].ToString()))
                {
                    if (!isSelling)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，售價必須為數字!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }
                    else
                    {
                        //if (Convert.ToDecimal(ExcelData["SellingPrice"].ToString()) <= 0)
                        if (sellprice <= 0)
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，售價不可小於等於0!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                    }
                }

                if (accType == "V")
                {
                    if (!string.IsNullOrWhiteSpace(ExcelData["Cost"].ToString()))
                    {
                        if (!isCost)
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，成本價必須為數字!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                        else
                        {
                            if (cost <= 0)
                            {
                                Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                                errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，成本價不可小於等於0!!!";
                                errorInfo.Code = (int)ResponseCode.Error;
                                errorInfo.IsSuccess = false;
                                result.Add(errorInfo);
                            }
                        }
                    }
                    if ((sellprice > 0) && (cost > 0))
                    {
                        if (cost > sellprice)
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，毛利率為負數，請重新設定售價或成本!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                    }
                    else if ((sellprice > 0) && (cost == 0))
                    {
                        if (ProInfo.Cost > sellprice)
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，成本價不可小於等於0!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                    }
                    else if ((sellprice == 0) && (cost > 0))
                    {
                        if (cost > ItemInfo.PriceCash)
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，售價不可小於等於0!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                    }
                }
                else
                {
                    if ((!string.IsNullOrWhiteSpace(ExcelData["Cost"].ToString())))
                    {
                        if ((!isCost) || (cost > 0))
                        {
                            Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                            errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，此筆資料為seller商品，不可修改cost!!!";
                            errorInfo.Code = (int)ResponseCode.Error;
                            errorInfo.IsSuccess = false;
                            result.Add(errorInfo);
                        }
                    }
                }
            }
            

            if (!string.IsNullOrWhiteSpace(ExcelData["MarketPrice"].ToString()))
            {
                decimal marketPrice = 0;
                bool isMarket;
                isMarket = decimal.TryParse(ExcelData["MarketPrice"].ToString(), out marketPrice);
                if (!isMarket)
                {
                    Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，市價必須為數字!!!";
                    errorInfo.Code = (int)ResponseCode.Error;
                    errorInfo.IsSuccess = false;
                    result.Add(errorInfo);
                }
                else
                {
                    if (marketPrice <= 0)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，市價不可小於等於0!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(ExcelData["Inventory"].ToString()))
            {
                int inventory = 0;
                bool isInventory;
                isInventory = int.TryParse(ExcelData["Inventory"].ToString(), out inventory);
                if (!isInventory)
                {
                    Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，庫存必須為整數!!!";
                    errorInfo.Code = (int)ResponseCode.Error;
                    errorInfo.IsSuccess = false;
                    result.Add(errorInfo);
                }
                else
                {
                    if (inventory <= 0)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，庫存不可小於等於0!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(ExcelData["Warranty"].ToString()))
            {
                int warranty = 0;
                bool isWarranty;
                isWarranty = int.TryParse(ExcelData["Warranty"].ToString(), out warranty);
                if (!isWarranty)
                {
                    Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                    errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，保固期必須為整數!!!";
                    errorInfo.Code = (int)ResponseCode.Error;
                    errorInfo.IsSuccess = false;
                    result.Add(errorInfo);
                }
                else
                {
                    if (warranty <= 0)
                    {
                        Models.ActionResponse<string> errorInfo = new Models.ActionResponse<string>();
                        errorInfo.Msg = "新蛋賣場編號：" + ExcelData["ItemID"].ToString() + "，保固期不可小於等於0!!!";
                        errorInfo.Code = (int)ResponseCode.Error;
                        errorInfo.IsSuccess = false;
                        result.Add(errorInfo);
                    }
                }
            }
        }
    }
}
