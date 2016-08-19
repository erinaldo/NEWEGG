using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.API.Models.Models;
using TWNewEgg.DB.TWSELLERPORTALDB.Models;




namespace TWNewEgg.API.Models
{
    public partial class Connector
    {
        public string APIHost;

        public Connector(string APIConnector = "")
        {
            if (string.IsNullOrEmpty(APIConnector))
            {
                APIHost = System.Configuration.ConfigurationManager.AppSettings["APIHost"];

            }
            else
            {
                APIHost = APIConnector;
            }
        }

        //        public Connector()
        //        {
        //#if DEBUG
        //            //for 本機
        //            APIHost = "http://localhost:57035";

        //            //for GQC環境 and 正式環境
        //            //APIHost = "http://127.0.0.1:71";
        //            //for DEV環境
        //            //APIHost = "http://127.0.0.1:98";
        //#endif
        //#if !DEBUG 
        //            APIHost = "http://10.16.131.41:71"; 
        //#endif
        //        }

        public T Get<T>(string url, string auth, string token, int timeout = 50000)
        {
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Headers.Add("Authorization", auth);
            req.Headers.Add("Token", token);
            req.Method = "GET";
            req.Timeout = timeout;
            System.IO.Stream s = req.GetResponse().GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(s);
            string str = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            s.Close();
            s.Dispose();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            T data = js.Deserialize<T>(str);
            return data;
        }

        public T Post<T>(string url, string auth, string token, object body, int timeout = 500000)
        {
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            string serialStr = js.Serialize(body);
            System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
            req.Headers.Add("Authorization", auth);
            req.Headers.Add("Token", token);
            req.Method = "POST";
            req.ContentType = "application/json";
            req.Timeout = timeout;
            System.IO.Stream streamOut = req.GetRequestStream();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(streamOut);
            sw.Write(serialStr);
            sw.Flush();
            sw.Close();
            streamOut.Close();
            streamOut.Dispose();
            System.IO.Stream streamIn = req.GetResponse().GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(streamIn);
            string str = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            streamIn.Close();
            streamIn.Dispose();
            T data = js.Deserialize<T>(str);
            return data;

        }


        /// <summary>
        /// Get Account Detail
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<T> APIUserDetail<T>(string auth, string token, TWNewEgg.API.Models.LoginInfo info)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<T>>(APIHost + "/APIUser/UserDetail", auth, token, info);
            return result;
        }


        #region SO
        /// <summary>
        /// 查詢訂單/QueryOrderInfos
        /// 修改訂單/EditOrderInfo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<T> QueryOrderInfos<T>(string auth, string token, TWNewEgg.API.Models.QueryCartCondition condition)
        {
            //資料量大, timeout延長至15秒
            var result = Post<TWNewEgg.API.Models.ActionResponse<T>>(APIHost + "/SalesOrder/QueryOrderInfos", auth, token, condition, 60000);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo> EditOrderInfo(string auth, string token, TWNewEgg.API.Models.OrderInfo order)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.OrderInfo>>(APIHost + "/SalesOrder/EditOrderInfo", auth, token, order);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> SendPackage(string auth, string token, List<DB.TWSELLERPORTALDB.Models.Seller_DelvTrack> delvTrack)
        {
            //資料量大, timeout延長至15秒
            var result = Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/SalesOrder/SendPackage", auth, token, delvTrack, 15000);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>> UpdateDelvStatus(string auth, string token, string soCode, int delvStatus, string sellerID, string updateUser)
        {
            //資料量大, timeout延長至15秒
            var result = Get<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>>>(APIHost + "/SalesOrder/UpdateDelvStatus?soCode=" + soCode + "&delvStatus=" + delvStatus + "&sellerID=" + sellerID + "&updateUser=" + updateUser, auth, token, 15000);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>> Arrival(string auth, string token, string soCode, int delvStatus, int cartStatus, string sellerID, string updateUser)
        {
            //資料量大, timeout延長至15秒
            var result = Get<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.OrderInfo>>>(APIHost + "/SalesOrder/Arrival?soCode=" + soCode + "&delvStatus=" + delvStatus + "&cartStatus=" + cartStatus + "&sellerID=" + sellerID + "&updateUser=" + updateUser, auth, token, 15000);
            return result;
        }

        /// <summary>
        /// 遞送包裹撈取貨運公司
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> APIQueryShipCarrier(string auth, string token)
        {
            var result = Get<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/SalesOrder/APIQueryShipCarrier", auth, token);
            return result;
        }

        /// <summary>
        /// 匯出 Excel 訂單列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        public TWNewEgg.API.Models.ActionResponse<string> DownloadSalesOrderList(API.Models.OrderInfo.DownloadSalesOrderListModel dataInfo)
        {
            //資料量大, timeout延長至15秒
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SalesOrder/DownloadSalesOrderList", "", "", dataInfo, 600000);
        }
        #endregion

        //Ben Tseng
        #region Seller Payment Reports action

        #region summaryReports
        /// <summary>
        /// Get summaryReports
        /// </summary>
        /// <param name="sellerID"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<SettlementInfo>> GetDataSummary(int sellerID, string beginDate, string endDate)
        {
            //APIHost+"/Summary/index?sellerID=" + sellerID + "&beginDate=" + beginDate + "&endDate=" + endDate, string.Empty,string.Empty);
            var result = Get<API.Models.ActionResponse<List<SettlementInfo>>>(APIHost + "/Summary/index?sellerID=" + sellerID + "&beginDate=" + beginDate + "&endDate=" + endDate, string.Empty, string.Empty);
            return result;
        }

        // <summary>
        // summaryReport
        // exec SP_RPT_Summary
        // </summary>
        // <param name="auth"></param>
        // <param name="token"></param>
        // <param name="SummarySPSrarch"></param>
        // <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<SummarySPResult>> PostDataSummary(string auth, string token, TWNewEgg.API.Models.SummarySPSrarch SummarySPSrarch)
        {
            var result = Post<API.Models.ActionResponse<List<SummarySPResult>>>(APIHost + "/Summary/summaryReport", auth, token, SummarySPSrarch);
            return result;
        }
        #endregion

        #region transactionDetailsReport
        /// <summary>
        /// 交易明細SP
        /// </summary>
        /// <param name="inputOrderNumber"></param>
        /// <param name="inputInvoiceNumber"></param>
        /// <param name="inputSettlementID"></param>
        /// <param name="inputSellerPartNum"></param>
        /// <param name="inputNewEggItemNum"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TransactionSPResult>> GetDataTransactionSP(string inputOrderNumber, string inputInvoiceNumber, string inputSettlementID, string inputSellerPartNum, string inputNewEggItemNum)
        {
            var result = Get<API.Models.ActionResponse<List<TransactionSPResult>>>(APIHost + "/TransactionDetails/index?inputOrderNumber=" + inputOrderNumber + "&inputInvoiceNumber=" + inputInvoiceNumber + "&inputSettlementID=" + inputSettlementID + "&inputSellerPartNum=" + inputSellerPartNum + "&inputNewEggItemNum=" + inputNewEggItemNum, string.Empty, string.Empty);
            return result;
        }

        // <summary>
        // transactionDetailsReport
        // exec SP_RPT_TransDetails
        // </summary>
        // <param name="auth"></param>
        // <param name="token"></param>
        // <param name="TransactionSPSearch"></param>
        // <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TransactionSPResult>> PostDataTransactionSP(string auth, string token, TWNewEgg.API.Models.TransactionSPSearch TransactionSPSearch)
        {
            var result = Post<API.Models.ActionResponse<List<TransactionSPResult>>>(APIHost + "/TransactionDetails/transactionDetailsReport", auth, token, TransactionSPSearch);
            return result;
        }
        #endregion

        #region storageDetailReport
        /// <summary>
        /// 倉儲明細 Storage
        /// </summary>
        /// <param name="inputSellerName"></param>
        /// <param name="inputSellerID"></param>
        /// <param name="ProductID"></param>
        /// <param name="SellerProductID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<StorageDetailSPResult>> GetDataStorageDetail(string inputSellerName, string inputSellerID, string ProductID, string SellerProductID)
        {
            var result = Get<API.Models.ActionResponse<List<StorageDetailSPResult>>>(APIHost + "/StorageDetail/index?inputSellerName=" + inputSellerName + "&inputSellerID=" + inputSellerID + "&ProductID=" + ProductID + "&SellerProductID" + SellerProductID, string.Empty, string.Empty);
            return result;
        }

        // <summary>
        // storageDetailReport
        // exec SP_RPT_TransDetails
        // </summary>
        // <param name="auth"></param>
        // <param name="token"></param>
        // <param name="TransactionSPSearch"></param>
        // <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<StorageDetailSPResult>> PostDataStorageDetail(string auth, string token, TWNewEgg.API.Models.StorageDetailSPSearch StorageDetailSPSearch)
        {
            var result = Post<API.Models.ActionResponse<List<StorageDetailSPResult>>>(APIHost + "/StorageDetail/storageDetailReport", auth, token, StorageDetailSPSearch);
            return result;
        }
        #endregion

        #region settlementReport
        /// <summary>
        /// 結算Settlement
        /// </summary>
        /// <param name="inputSellerID"></param>
        /// <param name="inputStartDate"></param>
        /// <param name="inputEndDate"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<SettlementSPResult>> GetDataSettlement(int inputSellerID, string inputStartDate, string inputEndDate)
        {
            var result = Get<API.Models.ActionResponse<List<SettlementSPResult>>>(APIHost + "/Settlement/index?inputSellerID=" + inputSellerID + "&inputStartDate=" + inputStartDate + "&inputEndDate=" + inputEndDate, string.Empty, string.Empty);
            return result;
        }

        // <summary>
        // settlementReport
        // exec SP_RPT_Storage
        // </summary>
        // <param name="auth"></param>
        // <param name="token"></param>
        // <param name="StorageDetailSPSearch"></param>
        // <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<SettlementSPResult>> PostDataSettlement(string auth, string token, TWNewEgg.API.Models.SettlementSPSearch SettlementSPSearch)
        {
            var result = Post<API.Models.ActionResponse<List<SettlementSPResult>>>(APIHost + "/Settlement/settlementReport", auth, token, SettlementSPSearch);
            return result;
        }
        #endregion

        #endregion Seller Payment Reports action

        //Jack.W.Wu
        #region Manage Items
        /// <summary>
        /// 商品清單
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="itemSearch"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemInfoList>> APIGetItemList(string auth, string token, TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemInfoList>>>(APIHost + "/itemList/getitemList", auth, token, itemSearch, 20000);
            return result;
        }

        ///// <summary>
        ///// 商品已由新蛋運送清單 0117 Mark delete by Jack.W.Wu
        ///// </summary>
        ///// <param name="auth"></param>
        ///// <param name="token"></param>
        ///// <param name="itemSearch"></param>
        ///// <returns></returns>
        //public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ShippedByNeweggList>> APIShippedByNeweggList(string auth, string token, TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        //{
        //    var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ShippedByNeweggList>>>(APIHost+"/itemList/ShippedByNeweggList", auth, token, itemSearch);
        //    return result;
        //}

        /// <summary>
        /// 商品已由新蛋運送清單
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="itemSearch"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemInfoList>> APIItemFBNList(string auth, string token, TWNewEgg.API.Models.ItemSearchCondition itemSearch)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemInfoList>>>(APIHost + "/itemList/ItemFBNList", auth, token, itemSearch, 20000);
            return result;
        }

        /// <summary>
        /// 修改商品資訊
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="itemInfoList"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> APIItemModify(string auth, string token, List<TWNewEgg.API.Models.ItemInfoList> itemInfoList)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/itemList/ItemModify", auth, token, itemInfoList);
            return result;
        }

        /// <summary>
        /// 修改ShipByNewEgg
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="shippedByNeweggList"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> APIShippedByNeweggModify(string auth, string token, List<TWNewEgg.API.Models.ItemInfoList> itemInfoList)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/itemList/ShippedByNeweggModify", auth, token, itemInfoList);
            return result;
        }

        /// <summary>
        /// 查詢前台分類目錄
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="layer"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSQLDB.Models.Category>> APIQueryCategory(string auth, string token, int? layer, int? parentID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<DB.TWSQLDB.Models.Category>>>(APIHost + "/itemList/QueryCategory", auth, token, new { layer, parentID });
            return result;
        }

        /// <summary>
        /// 計算主類別數量，創建商品使用
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Dictionary<string, int> APICountCategory(string auth, string token)
        {
            var result = Get<Dictionary<string, int>>(APIHost + "/itemList/CountCategory", auth, token);
            return result;
        }

        /// <summary>
        /// 寄倉寫入資料進ProductDetail
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="addProducts"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> APIAddProductDetail(string auth, string token, List<TWNewEgg.API.Models.ProductDetail> addProducts)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/itemList/AddProductDetail", auth, token, addProducts);
            return result;
        }

        /// <summary>
        /// 刪除賣場，賣場狀態改為99
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="deleteItem"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> APIDeleteItem(string auth, string token, ItemInfoList deleteItem)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/itemList/DeleteItem", auth, token, deleteItem);
            return result;
        }

        /// <summary>
        /// 更新商品上、下架
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="updateItemStatus"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> APIUpdateItemStatus(string auth, string token, ItemInfoList updateItemStatus)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/itemList/UpdateItemStatus", auth, token, updateItemStatus);
            return result;
        }

        /// <summary>
        /// 搜尋創建商品
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="sellerID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<APIItemModel> APISearchCreatedItem(string auth, string token, int sellerID, int itemID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<APIItemModel>>(APIHost + "/itemList/SearchCreatedItem", auth, token, new { sellerID, itemID });
            return result;
        }

        /// <summary>
        /// 修改創建商品
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> APIEditCreatedItem(string auth, string token, API.Models.APIItemModel editItem)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/itemList/EditCreatedItem", auth, token, editItem);
            return result;
        }

        /// <summary>
        /// 上傳商品圖
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="pictureURL"></param>
        /// <param name="productID"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<bool> APIPostImageToDB(string auth, string token, List<string> pictureURL, string productID, string itemID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/itemList/PostImageToDB", auth, token, new { pictureURL, productID, itemID });
            return result;
        }

        /// <summary>
        /// 計算Seller/Vendor上架商品數量
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<Dictionary<string, int>> APICountActiveItem(string auth, string token)
        {
            var result = Get<TWNewEgg.API.Models.ActionResponse<Dictionary<string, int>>>(APIHost + "/itemList/APICountActiveItem", auth, token);
            return result;
        }

        /// <summary>
        /// 匯出 Excel 商品列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        public TWNewEgg.API.Models.ActionResponse<string> DownloadItemList(API.Models.DownloadItemListModel dataInfo)
        {
            //資料量大, timeout延長至15秒
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemList/DownloadItemList", "", "", dataInfo, 600000);
        }

        /// <summary>
        /// 匯出 Excel 商品列表
        /// </summary>
        /// <param name="dataInfo">匯出資訊</param>
        /// <returns>成功及失敗訊息</returns>
        public TWNewEgg.API.Models.ActionResponse<string> DownloadItemListToExcel(API.Models.DownloadItemListModel dataInfo)
        {
            //資料量大, timeout延長至15秒
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemList/DownloadItemListToExcel", "", "", dataInfo, 600000);
        }

        #endregion

        #region Seller BasicInfo action
        /// <summary>
        /// Get Seller BasicInfo
        /// </summary>
        /// <param name="Seller">give SellerID or SellerName to find out Seller BasicInfo</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> GetSeller_BasicInfo(string Seller, int type)
        {
            var result = Get<API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>>(APIHost + "/SellerBasicInfo/GetSeller_BasicInfo?Seller=" + Seller + "&type=" + type, "", "");
            //var result = Get<TWNewEgg.API.Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>>(APIHost+"/SellerBasicInfo/GetSeller_BasicInfo?Seller=", Seller);
            return result;
        }

        /// <summary>
        /// Save Seller BasicInfo
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="BasicInfo"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSeller_BasicProInfo(string auth, string token, API.Models.Seller_BasicProInfo BasicProInfo)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerBasicInfo/SaveSeller_BasicProInfo", auth, token, BasicProInfo);
            return result;
        }
        /// <summary>
        /// Save Seller BasicInfo
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="BasicInfo"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSeller_BasicafterInfo(string auth, string token, API.Models.Seller_BasicafterInfo BasicafterInfo)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerBasicInfo/SaveSeller_BasicafterInfo", auth, token, BasicafterInfo);
            return result;
        }

        /// <summary>
        /// Save Seller Logo Image
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="LogoImage"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSellerLogoImage(string auth, string token, API.Models.SellerLogoInfo LogoImageInfo)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerBasicInfo/SaveSellerLogoImage", auth, token, LogoImageInfo);
            return result;
        }
        #endregion Seller BasicInfo action

        #region Find Seller action
        /// <summary>
        /// Get all seller name list
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> GetSellerName()
        {
            var result = Get<API.Models.ActionResponse<List<string>>>(APIHost + "/FindSeller/GetSellerName", "", "");
            return result;
        }

        /// <summary>
        /// Find out Account Info by searchword for many type
        /// </summary>
        /// <param name="type"> 0. by ID
        ///                     1. by Name
        ///                     2. by Email
        ///                     3. by Phone   </param>
        /// <param name="searchword"> keyword </param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>> GetAccountInfo(int type = 0, string searchword = "")
        {
            var result = Get<API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_BasicInfo>>>(APIHost + "/FindSeller/GetAccountInfo?type=" + type + "&searchword=" + searchword, "", "");
            return result;
        }
        #endregion Find Seller action

        #region Seller Contact Address action
        /// <summary>
        /// Get all seller name list
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<API.Models.Seller_ContactInfoData>> GetSeller_ContactInfo(string Seller)
        {
            var result = Get<API.Models.ActionResponse<List<API.Models.Seller_ContactInfoData>>>(APIHost + "/SellerContactInfo/GetSeller_ContactInfo?Seller=" + Seller, "", "");
            return result;
        }

        /// <summary>
        /// Save seller contact address list
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="ContactInfo"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSeller_ContactInfo(string auth, string token, API.Models.Seller_ContactInfoData ContactInfo)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerContactInfo/SaveSeller_ContactInfo", auth, token, ContactInfo);
            return result;
        }

        /// <summary>
        /// Delete seller contact address
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="ContactInfo"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> DeleteSeller_ContactInfo(string auth, string token, API.Models.Seller_ContactInfoData ContactInfo)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerContactInfo/DeleteSeller_ContactInfo", auth, token, ContactInfo);
            return result;
        }

        /// <summary>
        /// Delete seller contact address
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="ContactInfo"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>> GetSeller_ContactType()
        {
            var result = Get<API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ContactType>>>(APIHost + "/SellerContactInfo/GetSeller_ContactType", "", "");
            return result;
        }
        #endregion Seller Contact Address action

        #region Seller Financial action
        /// <summary>
        /// Get seller financial
        /// </summary>
        /// <param name="Seller"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> GetSeller_Financial(string Seller, int type)
        {
            var result = Get<API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial>>(APIHost + "/SellerFinancial/GetSeller_Financial?Seller=" + Seller + "&type=" + type, "", "");
            return result;
        }

        /// <summary>
        /// Save seller financial
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="Financial"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSeller_Financial(string auth, string token, DB.TWSELLERPORTALDB.Models.Seller_Financial Financial)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerFinancial/SaveSeller_Financial", auth, token, Financial);
            return result;
        }

        #endregion Seller Financial action

        #region Seller Return Address action
        /// <summary>
        /// Get seller return address
        /// </summary>
        /// <param name="Seller"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo> GetSeller_ReturnAddress(string Seller, int type)
        {
            var result = Get<API.Models.ActionResponse<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo>>(APIHost + "/SellerReturnAddress/GetSeller_ReturnAddress?Seller=" + Seller + "&type=" + type, "", "");
            return result;
        }

        /// <summary>
        /// Save seller return address
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="ReturnAddress"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSeller_ReturnAddress(string auth, string token, TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_ReturnInfo ReturnInfo)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerReturnAddress/SaveSeller_ReturnAddress", auth, token, ReturnInfo);
            return result;
        }

        #endregion Seller Return Address action

        #region Seller Notification action
        /// <summary>
        /// Get seller notification
        /// </summary>
        /// <param name="Seller"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Notification>> GetSeller_Notification(string Seller, int type)
        {
            var result = Get<API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_Notification>>>(APIHost + "/SellerNotification/GetSeller_Notification?Seller=" + Seller + "&type=" + type, "", "");
            return result;
        }

        /// <summary>
        /// Save seller financial
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="Notification"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> SaveSeller_Notification(string auth, string token, List<DB.TWSELLERPORTALDB.Models.Seller_Notification> Notification)
        {
            var result = Post<API.Models.ActionResponse<List<string>>>(APIHost + "/SellerNotification/SaveSeller_Notification", auth, token, Notification);
            return result;
        }

        #endregion Seller Notification action

        #region Seller User action
        /// <summary>
        /// Get seller user
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_User>> GetSeller_User(string User, int type)
        {
            var result = Get<API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_User>>>(APIHost + "/SellerUser/GetSeller_User?User=" + User + "&type=" + type, "", "");
            return result;
        }

        /// <summary>
        /// Save seller user
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="token"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSeller_User(string auth, string token, DB.TWSELLERPORTALDB.Models.Seller_User User)
        {
            var result = Post<API.Models.ActionResponse<string>>(APIHost + "/SellerUser/SaveSeller_User", auth, token, User);
            return result;
        }

        #endregion Seller User action

        //Ron
        #region Seller Function action(all about Navigation Menu)

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.Seller_FunctionJoinCategory>> GetSeller_FuctionBySellerLanguage(string auth, string token, string sellerID, string languageCode)
        {
            var result = Get<API.Models.ActionResponse<List<TWNewEgg.API.Models.Seller_FunctionJoinCategory>>>(APIHost + "/NavigationMenu/GetSeller_FuctionBySellerLanguage?sellerID=" + sellerID + "&languageCode=" + languageCode, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.User_Group> GetUser_GroupByUser(string auth, string token, string user, int type)
        {
            var result = Get<API.Models.ActionResponse<DB.TWSELLERPORTALDB.Models.User_Group>>(APIHost + "/SellerUserGroupRelationshipSearch/GetUser_GroupByUser?user=" + user + "&type=" + type, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.User_Group>> GetUser_GroupBySeller(string auth, string token, string seller, int type)
        {
            var result = Get<API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.User_Group>>>(APIHost + "/SellerUserGroupRelationshipSearch/GetUser_GroupBySeller?seller=" + seller + "&type=" + type, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> CheckSellerUserEmailUnique(string auth, string token, string Email)
        {
            var result = Get<API.Models.ActionResponse<bool>>(APIHost + "/SellerUser/CheckSellerUserEmailUnique?Email=" + Email, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial> GetSeller_BasicInfoWithFinancialByID(string auth, string token, string sellerID)
        {
            var result = Get<API.Models.ActionResponse<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>>(APIHost + "/SellerBasicInfo/GetSeller_BasicInfoWithFinancialByID?sellerID=" + sellerID, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> GetSeller_BasicInfoWithFinancialByEmail(string auth, string token, string sellerEmail)
        {
            var result = Get<API.Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>>>(APIHost + "/SellerBasicInfo/GetSeller_BasicInfoWithFinancialByEmail?sellerEmail=" + sellerEmail, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>> GetAllSeller_BasicInfoWithFinancial(string auth, string token)
        {
            var result = Get<API.Models.ActionResponse<List<TWNewEgg.API.Models.Seller_BasicInfoWithFinancial>>>(APIHost + "/SellerBasicInfo/GetAllSeller_BasicInfoWithFinancial", auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> CheckSellerNameUnique(string auth, string token, string SellerID, string SellerName)
        {
            var result = Get<API.Models.ActionResponse<bool>>(APIHost + "/SellerBasicInfo/CheckSellerNameUnique?SellerID=" + SellerID + "&SellerName=" + SellerName, auth, token);
            return result;
        }


        public TWNewEgg.API.Models.ActionResponse<List<Seller_FunctionJoinCategory>> GetSeller_FuctionsByQuery(string auth, string token, QueryFunctionCondition query)
        {
            var result = Post<API.Models.ActionResponse<List<Seller_FunctionJoinCategory>>>(APIHost + "/NavigationMenu/GetSeller_FuctionsByQuery", auth, token, query);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<Seller_FunctionCategoryLocalized>> GetSeller_FunctionCategoryByLanguage(string auth, string token, string language)
        {
            var result = Get<API.Models.ActionResponse<List<Seller_FunctionCategoryLocalized>>>(APIHost + "/NavigationMenu/GetSeller_FunctionCategoryByLanguage?language=" + language, auth, token);
            return result;
        }


        public TWNewEgg.API.Models.ActionResponse<int> GetSeller_PurviewCount(string auth, string token, string seller)
        {
            var result = Get<API.Models.ActionResponse<int>>(APIHost + "/Purview/GetSeller_PurviewCount?seller=" + seller, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<int> GetUser_PurviewCount(string auth, string token, int userID)
        {
            var result = Get<API.Models.ActionResponse<int>>(APIHost + "/Purview/GetUser_PurviewCount?userID=" + userID, auth, token);
            return result;
        }

        #endregion

        //Zevi
        #region Seller Relationship Management
        public TWNewEgg.API.Models.ActionResponse<List<ValueTextItem>> GetRegionList(string auth, string token)
        {
            var result = Get<API.Models.ActionResponse<List<ValueTextItem>>>(APIHost + "/SellerRelationshipManage/GetRegionList", auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<VM_Seller_BasicInfo>> GetSeller_BasicInfosbyQuery(string auth, string token, SellerRMQuery smrq)
        {
            var result = Post<API.Models.ActionResponse<List<VM_Seller_BasicInfo>>>(APIHost + "/SellerRelationshipManage/GetSeller_BasicInfosbyQuery", auth, token, smrq);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<VM_Seller_BasicInfo>> SearchSellerForRelationship(string auth, string token, SellerRelationshipSearchCondition serachCondition)
        {
            var result = Post<API.Models.ActionResponse<List<VM_Seller_BasicInfo>>>(APIHost + "/SellerRelationshipManage/SearchSellerForRelationship", auth, token, serachCondition);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<List<ValueTextItem>> GetCMList(string auth, string token)
        {
            var result = Get<API.Models.ActionResponse<List<ValueTextItem>>>(APIHost + "/SellerRelationshipManage/GetCMList", auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> EditSeller_BasicInfo(string auth, string token, Seller_BasicInfo sinfo)
        {
            var result = Post<API.Models.ActionResponse<bool>>(APIHost + "/SellerRelationshipManage/EditSeller_BasicInfo", auth, token, sinfo);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> EditSeller_BasicInfoList(string auth, string token, List<Seller_BasicInfo> sinfoList)
        {
            var result = Post<API.Models.ActionResponse<bool>>(APIHost + "/SellerRelationshipManage/EditSeller_BasicInfoList", auth, token, sinfoList);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<VM_Seller_BasicInfo> GetSeller_BasicInfobyID(string auth, string token, string id)
        {
            var result = Get<API.Models.ActionResponse<VM_Seller_BasicInfo>>(APIHost + "/SellerRelationshipManage/GetSeller_BasicInfobyID/" + id, auth, token);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<string> sellerrelationshipmanagement_Edit(List<TWNewEgg.DB.TWSELLERPORTALDB.Models.Seller_BasicInfo> editData, string userid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SellerRelationshipManage/sellerrelationshipmanagement_Edit", "", "", new { editData = editData, userid = userid });
        }

        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>> GetSellerSearchAutoComplete()
        {
            return this.Get<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.DomainModel.AutoCompleteModel>>>(APIHost + "/SellerRelationshipManage/GetSeller_BasicInfos", "", "");
        }

        #endregion seller Relationship Management

        //Jack Lin
        #region Seller Invitation Action
        /// <summary>
        /// Get country list
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<API.Models.GetRegionListResult>> GetRegionList()
        {
            var result = Get<TWNewEgg.API.Models.ActionResponse<List<API.Models.GetRegionListResult>>>(APIHost + "/SellerInvitation/GetRegionList", "", "");

            return result;
        }

        /// <summary>
        /// Get currency list
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<API.Models.GetCurrencyListResult>> GetCurrencyList()
        {
            var result = Get<TWNewEgg.API.Models.ActionResponse<List<API.Models.GetCurrencyListResult>>>(APIHost + "/SellerInvitation/GetCurrencyList", "", "");

            return result;
        }

        /// <summary>
        /// Get seller charge
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<API.Models.GetSellerChargeResult>> GetSellerCharge(string auth, string token, TWNewEgg.API.Models.GetSellerCharge getSellerCharge)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<API.Models.GetSellerChargeResult>>>(APIHost + "/SellerInvitation/GetSellerCharge", auth, token, getSellerCharge);

            return result;
        }

        /// <summary>
        /// Save seller charge
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> SaveSellerCharge(string auth, string token, TWNewEgg.API.Models.SaveSellerCharge saveSellerCharge)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SellerInvitation/SaveSellerCharge", auth, token, saveSellerCharge);

            return result;
        }

        /// <summary>
        /// Send an invitation email
        /// </summary>
        /// <returns></returns>
        public API.Models.ActionResponse<SendInvitationEmailResult> SendInvitationEmail(string auth, string token, TWNewEgg.API.Models.SendInvitationEmail sendInvitationEmail)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<SendInvitationEmailResult>>(APIHost + "/SellerInvitation/SendInvitationEmail", auth, token, sendInvitationEmail);

            return result;
        }

        #endregion Seller Invitation Action

        #region User Action

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> Login(string auth, string token, TWNewEgg.API.Models.UserLogin userLogin)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult>>(APIHost + "/User/Login", auth, token, userLogin);

            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<int> GetSellerUserIDCon(string auth, string token, TWNewEgg.API.Models.Cookie _cookie)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<int>>(APIHost + "/User/GetSellerUserID", auth, token, _cookie);

            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> AutoLogin(string auth, string token, TWNewEgg.API.Models.UserLogin userLogin)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult>>(APIHost + "/User/AutoLogin", auth, token, userLogin);

            return result;
        }
        /// <summary>
        /// Check user's status
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult> CheckStatus(string auth, string token, TWNewEgg.API.Models.UserCheckStatus userCheckStatus)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult>>(APIHost + "/User/CheckStatus", auth, token, userCheckStatus);

            return result;
        }

        /// <summary>
        /// Check whether an User exist
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<int> CheckExist(string auth, string token, string UserEmail)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<int>>(APIHost + "/User/CheckExist", auth, token, new { UserEmail = UserEmail });

            return result;
        }

        /// <summary>
        /// Set a new password
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> SetPassword(string auth, string token, TWNewEgg.API.Models.UserChangePassword changePassword)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult>>(APIHost + "/User/SetPassword", auth, token, changePassword);

            return result;
        }

        /// <summary>
        /// Change old password
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult> ChangeOldPassword(string auth, string token, TWNewEgg.API.Models.UserChangePassword changePassword)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserLoginResult>>(APIHost + "/User/ChangeOldPassword", auth, token, changePassword);

            return result;
        }

        /// <summary>
        /// Create a new seller
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.SellerCreationResult> CreateSeller(string auth, string token, TWNewEgg.API.Models.SellerCreation sellerCreation)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.SellerCreationResult>>(APIHost + "/User/CreateSeller", auth, token, sellerCreation);

            return result;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCreationResult> CreateUser(string auth, string token, TWNewEgg.API.Models.UserCreation userCreation)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCreationResult>>(APIHost + "/User/CreateUser", auth, token, userCreation);

            return result;
        }

        /// <summary>
        /// Reset Password Request
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ResetPasswordResult> ResetPassword(string auth, string token, TWNewEgg.API.Models.ResetPassword resetPassword)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.ResetPasswordResult>>(APIHost + "/User/ResetPassword", auth, token, resetPassword);

            return result;
        }

        /// <summary>
        /// Change User Status
        /// </summary>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult> ChangeUserStatus(string auth, string token, TWNewEgg.API.Models.UserChangeStatus userChangeStatus)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.UserCheckStatusResult>>(APIHost + "/User/ChangeUserStatus", auth, token, userChangeStatus);

            return result;
        }

        #endregion User Action

        #region Purview Action
        /// <summary>
        /// Get user purviews
        /// </summary>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> GetUserPurview(string auth, string token, int userID, string accounttypecode)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>>(APIHost + "/Purview/GetUserPurview", auth, token, new { userID = userID, accounttypecode = accounttypecode });

            return result;
        }

        /// <summary>
        /// Save user purviews
        /// </summary>
        public TWNewEgg.API.Models.ActionResponse<string> SaveUserPurview(string auth, string token, API.Models.SaveUserPurview userPurview)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Purview/SaveUserPurview", auth, token, userPurview);

            return result;
        }

        /// <summary>
        /// Get seller purviews
        /// </summary>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> GetSellerPurview(string auth, string token, int sellerID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>>(APIHost + "/Purview/GetSellerPurview", auth, token, new { sellerID = sellerID });

            return result;
        }

        /// <summary>
        /// Get group purviews
        /// </summary>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>> GetGroupPurview(string auth, string token, int groupID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetPurviewResult>>>(APIHost + "/Purview/GetGroupPurview", auth, token, new { groupID = groupID });

            return result;
        }

        /// <summary>
        /// Get user list
        /// </summary>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetUserListResult>> GetUserList(string auth, string token, int sellerID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetUserListResult>>>(APIHost + "/Purview/GetUserList", auth, token, new { sellerID = sellerID });

            return result;
        }

        /// <summary>
        /// Get function list
        /// </summary>
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.GetFunctionListResult>> GetFunctionList()
        {
            var result = Get<TWNewEgg.API.Models.ActionResponse<List<API.Models.GetFunctionListResult>>>(APIHost + "/Purview/GetFunctionList", "", "");

            return result;
        }


        #endregion Purview Action

        #region Mail Action
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.MailResult> SendMail(string auth, string token, TWNewEgg.API.Models.Mail mail)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.MailResult>>(APIHost + "/Mail/SendMail", auth, token, mail);

            return result;
        }
        #endregion Mail Action

        //Manufacturer
        #region Manufacturer

        /// <summary>
        /// Create ManufacturerInfo
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>> CreateManufacturerInfo(List<Manufacturer> manufacturer)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ManufactureInfo_Edit>>>(APIHost + "/Manufacturer/CreateManufacturerInfo", "", "", manufacturer, 60000000);
        }

        /// <summary>
        /// Create Seller User
        /// </summary>
        /// <param name="intSellerID"></param>
        /// <param name="strUserEmail"></param>
        /// <param name="intInUserID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<int> CreateUserEmail(int intSellerID = 0, string strUserEmail = null, int intInUserID = 0)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<int>>(APIHost + "/Manufacturer/CreateUserEmail", "", "", new { intSellerID = intSellerID, strUserEmail = strUserEmail, intInUserID = intInUserID });
        }


        /// <summary>
        /// Delete ManufactureInfo
        /// </summary>
        /// <param name="SN"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> DeleteManufactureInfo(string url)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Manufacturer/DeleteManufacturerInfo", "", "", new { url = url });
        }

        /// <summary>
        /// Edit Manufacturer
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> EditManufacturerInfo(List<API.Models.Manufacturer> UpdateInfo)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Manufacturer/EditManufacturerInfo", "", "", UpdateInfo);
        }

        /// <summary>
        /// 讀取審核結果通知對象清單
        /// </summary>
        /// <param name="intSellerID">商家 ID</param>
        /// <returns>審核結果通知對象清單</returns>
        public TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>> GetEmailToList(int intSellerID = -1)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerEmailToListResultModel>>>(APIHost + "/Manufacturer/GetEmailToList", "", "", new { intSellerID = intSellerID });
        }

        /// <summary>
        /// Get Seller Name
        /// </summary>
        /// <param name="intUserID">integer type</param>
        /// <returns>Action Response</returns>
        public TWNewEgg.API.Models.ActionResponse<string> GetSellerName(int intSellerID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Manufacturer/GetSellerName", "", "", new { intSellerID = intSellerID });
        }

        /// <summary>
        /// Edit Permission
        /// </summary>
        /// <param name="intUserID">integer type</param>
        /// <returns>Action Response</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> IsRatifyPermission(int intUserID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/Manufacturer/IsRatifyPermission", "", "", new { intUserID = intUserID });
        }

        #region List (已與 Search 結合)
        ///// <summary>
        ///// Get Manufacture List
        ///// </summary>
        ///// <param name="intSellerID"></param>
        ///// <returns></returns>
        //public TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerListResultModel>> ListManufacturerInfo(int intUserID, int intSellerID = -1, string strStatus = null, string strSearchWord = null)
        //{
        //    var result = Post<TWNewEgg.API.Models.ActionResponse<List<API.Models.ManufacturerListResultModel>>>(APIHost+"/Manufacturer/ListManufacturerInfo", "", "", new { intUserID = intUserID, intSellerID = intSellerID, strStatus = strStatus, strSearchWord = strSearchWord });
        //    return result;
        //}
        #endregion

        /// <summary>
        /// Search ManufacturerInfo
        /// </summary>
        /// <param name="SearchData">SearchData Model</param>
        /// <returns>Action Response</returns>
        public TWNewEgg.API.Models.ActionResponse<List<Manufacturer>> SearchManufacturerInfo(SearchDataModel SearchData)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<List<Manufacturer>>>(APIHost + "/Manufacturer/SearchManufacturerInfo", "", "", SearchData);
        }

        /// <summary>
        /// Ratify Manufacturer
        /// </summary>
        /// <param name="UpdateStatusData">ManufacturerUpdateStatusInfo Model</param>
        /// <returns>ActionResponse</returns>
        public TWNewEgg.API.Models.ActionResponse<string> UpdateStatus(ManufacturerUpdateStatusInfo UpdateStatusData)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Manufacturer/UpdateStatus", "", "", UpdateStatusData);
        }

        #endregion Manuacturer

        #region 草稿儲存與送審

        /// <summary>
        /// 草稿儲存與送審
        /// </summary>
        /// <param name="itemSketckList">送審資訊</param>
        /// <param name="userId">使用者ID</param>
        /// <param name="actionType">執行送審的類型</param>
        /// <returns>返回草稿儲存與送審結果</returns>
        public TWNewEgg.API.Models.ActionResponse<string> SendItemSketchToPending(List<TWNewEgg.API.Models.ItemSketch> itemSketckList, string userId, int actionType)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemSketch/SendItemSketchToPending", "", "", new { itemSketckList = itemSketckList, userId = userId, actionType = actionType });
        }

        #endregion 草稿儲存與送審


        /// <summary>
        /// 
        /// </summary>
        /// <param name="StetchID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> VerifyStetch(List<int> ProductID, string userid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemSketch/VerifyStetch", "", "", new { ProductID = ProductID, Userid = userid });
        }
        public TWNewEgg.API.Models.ActionResponse<string> VerifyStetchByModel(TWNewEgg.API.Models.ItemSketch _itemSketck, string userid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemSketch/VerifyStetchByModel", "", "", new { _ItemSketck = _itemSketck, Userid = userid });
        }
        //add by Ian and Thisway(批次上傳)
        #region Batch Upload Creation Item Info //增加 Product Property 相關查詢
        public TWNewEgg.API.Models.ActionResponse<bool> GetBatchUploadItemsInfo(string auth, string token, List<ItemInfoResult> QueryItemsInfo)
        {
            var result = Post<API.Models.ActionResponse<bool>>(APIHost + "/AddProductsInfo/PostItemInfoToAPIService", auth, token, QueryItemsInfo, 60000000);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> GetBatchUploadProductsInfo(string auth, string token, List<ProductsInfoResult> QueryProductsInfo)
        {
            var result = Post<API.Models.ActionResponse<bool>>(APIHost + "/AddProductsInfo/PostProductsInfoToAPIService", auth, token, QueryProductsInfo, 60000000);
            return result;
        }

        public TWNewEgg.API.Models.ActionResponse<bool> GetBatchUploadDetailInfo(string auth, string token, List<DetailInfoResult> QueryDetailInfo)
        {
            var result = Post<API.Models.ActionResponse<bool>>(APIHost + "/AddProductsInfo/PostDetailInfoToAPIService", auth, token, QueryDetailInfo, 60000000);
            return result;
        }

        //給SellerID與選擇的CategoryID獲取傭金費率
        public TWNewEgg.API.Models.ActionResponse<decimal> GetCommision(string auth, string token, int SellerID, int CategoryID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<decimal>>(APIHost + "/AddProductsInfo/GetCommisionSevice", auth, token, new { SellerID, CategoryID });
            return result;
        }

        //若之後需要檢查創建商品資訊上傳是否有重覆，可修改，來獲取唯一值(目前是連結Category)
        public TWNewEgg.API.Models.ActionResponse<List<CategoryResult>> GetCategoryInfo(string auth, string token)
        {
            var result = Get<API.Models.ActionResponse<List<CategoryResult>>>(APIHost + "/AddProductsInfo/GetProductCategory", auth, token);
            return result;
        }

        // 新增批次上傳(新版)的 Controller and Service
        public TWNewEgg.API.Models.ActionResponse<BathItemCreateInfoResult> BathItemCreateInfo(BathItemCreateInfo bathCreateItemInfoList)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<API.Models.BathItemCreateInfoResult>>(APIHost + "/AddProductsInfo/BathCreateItem", "", "", bathCreateItemInfoList, 60000);
        }
        // 單一創建商品
        public TWNewEgg.API.Models.ActionResponse<Dictionary<string, bool>> CreateItemInfo(SPItemCreation spCreateitemInfos)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<Dictionary<string, bool>>>(APIHost + "/AddProductsInfo/PostItemCreation", "", "", spCreateitemInfos, 60000);
        }

        // 量化更新商品
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ActionResponse<string>>> BatchItemUpdate(string fileName, string sheetName, string UserID)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ActionResponse<string>>>>(APIHost + "/ItemList/BatchItemUpdate", "", "", new { fileName, sheetName, UserID }, 60000);
        }

        // 取得 property (add by Ted)
        public TWNewEgg.API.Models.ActionResponse<List<PropertyResult>> GetProperty(string auth, string token, int CategoryID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<PropertyResult>>>(APIHost + "/AddProductsInfo/GetProperty", "", "", new { CategoryID }, 60000);
            return result;
        }

        // 取得商品資訊 Get Product properties
        public TWNewEgg.API.Models.ActionResponse<List<GetProductProperty>> GetProductProperty(string auth, string token, int ProductID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<GetProductProperty>>>(APIHost + "/AddProductsInfo/GetProductProperty", "", "", new { ProductID }, 60000);
            return result;
        }

        // Save Product Property
        public TWNewEgg.API.Models.ActionResponse<string> SaveProductPropertyClick(List<SaveProductProperty> proInfo)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/AddProductsInfo/SaveProductPropertyClick", "", "", proInfo, 60000);
            return result;
        }
        #endregion

        //ItemInventoryAlert add by jack.c(庫存警示)
        #region



        /// <summary>
        /// 
        /// </summary>
        /// <param name="SearchItem"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<API.Models.VM_ItemInventoryAlertInfo>> SearchItemInventoryAlert(API.Models.InventoryAlertSearchModel SearchItem)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<API.Models.VM_ItemInventoryAlertInfo>>>(APIHost + "/ItemInventoryAlert/SearchItemInventoryAlert", "", "", SearchItem, 60000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SaveAllModelList"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>> SaveAllUpdateInventoryAlertList(List<VM_ItemInventoryAlertInfo> SaveAllModel)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<DB.TWSELLERPORTALDB.Models.Seller_ProductDetail>>>(APIHost + "/ItemInventoryAlert/SaveAllUpdateInventoryAlert", "", "", SaveAllModel, 60000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeleteInventoryList"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> DeleteItemInventoryAlert(List<API.Models.DeleteItemInventory> DeleteInventoryList)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemInventoryAlert/DeleteItemInventoryAlert", "", "", DeleteInventoryList, 60000);
        }

        /// <summary>
        /// Mail寄送內容未確定(暫時無使用)
        /// </summary>
        /// <param name="UserMail"></param>
        /// <returns></returns>
        public API.Models.ActionResponse<API.Models.MailResult> SendItemInventoryEmail(API.Models.ItemInventoryMailInfo mailInfo)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<API.Models.MailResult>>(APIHost + "/ItemInventoryAlert/SendInventoryAlertEmail", "", "", mailInfo, 60000);
        }
        #endregion

        #region 查詢 API AppConfig

        // 查詢 WebConfig  
        public string GetAPIWebConfigSetting(string querySetting)
        {
            return Get<string>(APIHost + "/QueryWebConfig/GetAPIWebConfig?configSetting=" + querySetting, " ", " ", 5000);
        }

        #endregion

        #region 取得可用功能清單

        /// <summary>
        /// 取得可用功能清單
        /// </summary>
        /// <param name="userID">個人ID</param>
        /// <param name="sellerID">商家ID</param>
        /// <param name="groupID">群組ID</param>
        /// <param name="userEmail">商家電子郵件</param>
        /// <param name="accountTypeCode">身分別</param>
        /// <returns>返回功能清單</returns>
        public ActionResponse<List<MenuList>> GetMenuListAPI(string userID, string sellerID, string groupID, string userEmail, string accountTypeCode)
        {
            return Get<ActionResponse<List<MenuList>>>(APIHost + "/Home/GetMenuList?UserID=" + userID + "&SellerID=" + sellerID + "&GroupID=" + groupID + "&UserEmail=" + userEmail + "&AccountTypeCode=" + accountTypeCode, " ", " ", 5000);
        }

        #endregion

        #region 草稿

        /// <summary>
        /// 搜尋草稿
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns>草稿清單</returns>
        public TWNewEgg.API.Models.ActionResponse<List<ItemSketch>> GetItemSketchList(ItemSketchSearchCondition condition)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<ItemSketch>>>(APIHost + "/ItemSketch/GetItemSketchList", "", "", condition, 60000000);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<ItemSketch>> GetItemSketchListRemoveDes(ItemSketchSearchCondition condition)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<ItemSketch>>>(APIHost + "/ItemSketch/GetItemSketchListRemoveDes", "", "", condition, 60000000);
        }

        /// <summary>
        /// ItemStetch delete
        /// </summary>
        /// <param name="StetchID"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> DeleteStetch(int StetchID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemSketch/DeleteStetch", "", "", new { StetchID = StetchID });
        }

        /// <summary>
        /// 建立草稿
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <returns>成功、失敗資訊</returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> CreateItemSketch(List<ItemSketch> itemSketchCell)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemSketch/CreateItemSketch", "", "", itemSketchCell, 60000000);
        }

        /// <summary>
        /// 編輯草稿
        /// </summary>
        /// <param name="itemSketchCell">商品資訊</param>
        /// <returns>成功、失敗資訊</returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> EditItemSketch(ItemSketchEditType editType, List<ItemSketch> itemSketchCell)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemSketch/EditItemSketch", "", "", new { editType = editType, itemSketchCell = itemSketchCell }, 60000000);
        }

        // 取得商品資訊 Get Product properties
        public TWNewEgg.API.Models.ActionResponse<List<SaveProductProperty>> GetProductPropertySketch(string auth, string token, int ProductID)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<List<SaveProductProperty>>>(APIHost + "/ItemSketch/GetProductProperty", "", "", new { ProductID }, 60000);
            return result;
        }

        // Save Product Property
        public TWNewEgg.API.Models.ActionResponse<string> SaveProductPropertySketchClick(List<SaveProductProperty> proInfo)
        {
            var result = Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/ItemSketch/SaveProductPropertyClick", "", "", proInfo, 60000);
            return result;
        }

        #endregion 草稿

        #region 待審區

        /// <summary>
        /// 待審搜尋
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<ItemSketch>> GetItemTempList(ItemSketchSearchCondition condition, bool boolDefault)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<ItemSketch>>>(APIHost + "/ItemTemp/Search", "", "", new { itemSearch = condition, boolDefault = boolDefault }, 60000000);
        }

        /// <summary>
        /// 待審刪除
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> DeleteItemTemp(List<int> DeleteItems)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemTemp/DeleteTemp", "", "", DeleteItems, 60000000);
        }

        /// <summary>
        /// 待審清單編輯
        /// </summary>
        /// <param name="UpdateItemTemp"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> EditListTemp(List<TWNewEgg.API.Models.ItemSketch> UpdateListItemTemp)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemTemp/EditListTemp", "", "", UpdateListItemTemp, 60000000);
        }

        /// <summary>
        /// 待審清單編輯
        /// </summary>
        /// <param name="UpdateItemTemp"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<string>> EditDetailTemp(TWNewEgg.API.Models.ItemSketch EditModel)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemTemp/EditDetailTemp", "", "", EditModel, 60000000);
        }

        /// <summary>
        /// 取得商品屬性
        /// </summary>
        /// <param name="UpdateItemTemp"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<SaveProductProperty>> GetProductPropertyTemp(int productTempID)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<SaveProductProperty>>>(APIHost + "/ItemTemp/ProductPropertyTemp", "", "", new { productTempID = productTempID }, 60000000);
        }

        public TWNewEgg.API.Models.ActionResponse<bool> CheckCategoryParentId(int main, List<int> checkcategoryid)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/ItemList/CheckCategoryParentId", "", "", new { main = main, checkcategoryid = checkcategoryid }, 60000000);
        }

        #endregion
        #region get xml api
        public TWNewEgg.API.Models.ActionResponse<string> xmlBulletinsRead()
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Bulletins/ReadXml", "", "", 60000000);
        }
        public TWNewEgg.API.Models.ActionResponse<string> xmlBulletinsWrite(string xmlContent, int userid, int updateNumber)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Bulletins/WriteXml", "", "", new { xmlContent = xmlContent, userid = userid, updateNumber = updateNumber }, 60000000);
        }
        #endregion
        public TWNewEgg.API.Models.ActionResponse<string> createSellerVendor(TWNewEgg.API.Models.SellerCreation sellerInfoCon, TWNewEgg.API.Models.SaveSellerCharge sellerChargeCon, int userid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/User/CreateVendorOrSeller", "", "", new { sellerInfo = sellerInfoCon, sellerCharge = sellerChargeCon, userid = userid });
        }
        #region 批次送審
        public TWNewEgg.API.Models.ActionResponse<List<string>> ItemTempBatchExamine(List<TWNewEgg.API.Models.ItemSketch> _itemSketch, string email, string password)
        {
            //var result = Post<TWNewEgg.API.Models.ActionResponse<T>>(APIHost + "/SalesOrder/QueryOrderInfos", auth, token, condition, 60000);
            return this.Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemTemp/ItemTempBatchCreationJson", "", "", new { batchItemTempCreation = _itemSketch, userEmail = email, passWord = password }, 3600000);
        }
        #endregion
        #region 批次修改
        public TWNewEgg.API.Models.ActionResponse<List<string>> BatchEditDetailTemp(List<ItemSketch> EditModel, int UserID, int CurrentSellerID)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<string>>>(APIHost + "/ItemTemp/BatchEditDetailTemp", "", "", new { EditModel = EditModel, UserID = UserID, CurrentSellerID = CurrentSellerID }, 60000000);
        }
        #endregion
        #region 規格品
        #region 屬性商品清單草稿查詢
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemSketch>> propertySketchSearch(ItemSketchSearchCondition itemSkSearCondition)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.ItemSketch>>>(APIHost + "/SketchProperty/ItemSketchSearch", "", "", new { itemSkSearCondition = itemSkSearCondition });
        }
        #endregion
        #region 屬性商品清單草稿修改
        public TWNewEgg.API.Models.ActionResponse<string> propertySketchEdit(ItemSketchEditType editType, List<ItemSketch> itemSketchCell)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemSketchEdit", "", "", new { editType = editType, itemSketchCell = itemSketchCell });
        }
        #endregion
        #region 屬性商品清單草稿刪除
        public TWNewEgg.API.Models.ActionResponse<string> propertySketchDelete(int toDeleteId)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemSketchDelete", "", "", new { toDeleteId = toDeleteId });
        }
        #endregion
        #region 屬性商品清單草稿送審
        public TWNewEgg.API.Models.ActionResponse<string> propertySketchExamine(List<int> toExamineId, int userid, int sellerid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemSketchExamine", "", "", new { toExamineId = toExamineId, userid = userid, sellerid = sellerid }, 3600000);
        }
        #endregion
        #region 屬性商品清單待審區查詢
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>> propertySketchExamineSearch(ItemSketchSearchCondition itemsketchListData, bool isSearch = true)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.sketchPropertyExamine>>>(APIHost + "/SketchProperty/ItemSketchPropertyListSearch", "", "", new { itemsketchListData = itemsketchListData, isSearch = isSearch });
        }
        #endregion
        #region 屬性商品清單待審區刪除
        public TWNewEgg.API.Models.ActionResponse<string> propertySketchExamineDelete(List<int> deleteID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemSketchPropertyListDelete", "", "", new { deleteID = deleteID });
        }
        #endregion
        #region 屬性商品清單待審區修改
        public TWNewEgg.API.Models.ActionResponse<string> propertySketchExamineUpdate(List<TWNewEgg.API.Models.ItemSketch> itemsketckDeleteModel)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemSketchPropertyListUpdate", "", "", new { itemsketckDeleteModel = itemsketckDeleteModel });
        }
        #endregion
        #region 屬性商品清單待審區開啟修改畫面修改
        public TWNewEgg.API.Models.ActionResponse<string> ItemPropertyOpenViewEdit(TWNewEgg.API.Models.ItemSketch itemsketch)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemPropertyOpenViewEdit", "", "", new { itemsketch = itemsketch });
        }
        #endregion

        #region 規格商品建立
        /// <summary>
        /// 規格商品建立
        /// </summary>
        /// <param name="createStandardProductSketch">商品資訊</param>
        /// <returns>成功、失敗資訊</returns>
        public TWNewEgg.API.Models.ActionResponse<string> TwoDimensionProductCreate(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch createStandardProductSketch)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/TwoDimProductProperty/TwoDimensionProductCreate", "", "", createStandardProductSketch, 60000);
        }
        #endregion
        #region 規格商品建立直接送審
        /// <summary>
        /// 規格商品建立直接送審
        /// </summary>
        /// <param name="createStandardProductSketch"></param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> TwoDimensionProductCreateExamine(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch createStandardProductSketch, int userid, int sellerid, bool isNewItem = true)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/TwoDimProductProperty/TwoDimensionProductCreateExamine", "", "", new { twodimItemSketch = createStandardProductSketch, userid = userid, sellerid = sellerid, isNewItem = isNewItem }, 60000);
        }
        #endregion
        #region 規格商品編輯
        /// <summary>
        /// 規格商品編輯
        /// </summary>
        /// <param name="createStandardProductSketch">商品資訊</param>
        /// <returns>成功、失敗資訊</returns>
        public TWNewEgg.API.Models.ActionResponse<string> TwoDimensionProductDetailEdit(TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch createStandardProductSketch)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/TwoDimProductProperty/TwoDimensionProductDetailEdit", "", "", createStandardProductSketch, 60000);
        }
        #endregion
        #region 取得單筆規格商品詳細資訊
        /// <summary>
        /// 取得單筆規格商品詳細資訊
        /// </summary>
        /// <param name="createStandardProductSketch">查詢條件</param>
        /// <returns>單筆規格商品詳細資訊</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch> GetTwoDimensionProductDetailData(ItemSketchSearchCondition condition, bool isTempCopy = false)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.DomainModel.CreateStandardProductSketch>>(APIHost + "/TwoDimProductProperty/GetTwoDimensionProduct", "", "", new { condition = condition, isTempCopy = isTempCopy }, 60000);
        }
        #endregion
        #region 屬性商品清單待審區匯出Excel
        public TWNewEgg.API.Models.ActionResponse<string> excelSearchProperty(ItemSketchSearchCondition itemsketchListData)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/excelSearchProperty", "", "", new { itemsketchListData = itemsketchListData });
        }
        #endregion
        #region 屬性商品清單批次送審
        public TWNewEgg.API.Models.ActionResponse<string> propertyExamine(List<TWNewEgg.API.Models.BatchExamineModel> BatchExamineModel, string userEmail, string password)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/SketchProperty/ItemSketchBatchExamine", "", "", new { BatchExamineModel = BatchExamineModel, userEmail = userEmail, password = password }, 3600000);
        }
        #endregion
        #endregion

        #region 對帳單

        #region 對帳單主單查詢

        public ActionResponse<List<MainStatement>> GetMainStatement(MainStatementSearchCondition searchCondition)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<MainStatement>>>(APIHost + "/Financial/GetMainStatement", "", "", searchCondition);
        }

        #endregion 對帳單主單查詢

        #region 對帳單子單查詢
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.financialModel> getfinancialDetail(string SettlementID, int sellerid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.financialModel>>(APIHost + "/Financial/GetDetailData", "", "", new { SettlementID = SettlementID, sellerid = sellerid });
        }
        #endregion
        #region 對帳單子單壓發票
        public TWNewEgg.API.Models.ActionResponse<string> pushInvoNumAndInvoDate(string InvoDate, string InvoNum, string SettlementID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Financial/pushInvoNumAndInvoDate", "", "", new { InvoDate = InvoDate, InvoNum = InvoNum, SettlementID = SettlementID });
        }

        //依據 BSATW-232 供應商對帳單新增調整項目---------------add by bruce 20160721
        public TWNewEgg.API.Models.ActionResponse<string> pushInvoNumAndInvoDate(string InvoDate, string InvoNum, string SettlementID, int sellerID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Financial/pushInvoNumAndInvoDate", "", "", new { InvoDate = InvoDate, InvoNum = InvoNum, SettlementID = SettlementID, sellerID = sellerID });
        }
        #endregion
        #region 對帳單產生 Excel 對應的資料
        public ActionResponse<string> _FinancialExportExcel(string SettlementIDNumber, int sellerid)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/Financial/_FinancialExportExcel", "", "", new { SettlementIDNumber = SettlementIDNumber, sellerid = sellerid });
        }
        #endregion

        #endregion 對帳單

        #region 退貨

        #region 主單

        /// <summary>
        /// 取得退貨清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>退貨清單</returns>
        public TWNewEgg.API.Models.ActionResponse<List<MainRetgood>> GetMainRetgood(MainRetgoodSearchCondition searchCondition)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<MainRetgood>>>(APIHost + "/Return/GetMainRetgood", "", "", searchCondition, 60000000);
        }

        #endregion 主單

        #region 派車功能

        /// <summary>
        /// 取得退貨商品資訊
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodAPIModel> retgoodInfomation(string cart_id)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<RetgoodAPIModel>>(APIHost + "/Return/retgoodInfomation", "", "", new { cartid = cart_id });
        }

        /// <summary>
        /// 更新退貨商品相關資訊
        /// </summary>
        /// <param name="updateRetGoodsInfo">所要更新資訊</param>
        /// <returns>返回更新結果</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> UpdateRetGoods(UpdateRetGoodsInfo updateRetGoodsInfo)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/Return/UpdateRetGoods", "", "", new { updateRetGoodsInfo = updateRetGoodsInfo });
        }

        /// <summary>
        /// 廠商已查看派車明細備註
        /// </summary>
        /// <param name="userID">使用者ID</param>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回執行結果</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> HasBeenViewed(int userID, string cartID)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/Return/HasBeenViewed", "", "", new { userID = userID, cartID = cartID });
        }

        #endregion

        #region 回報功能

        /// <summary>
        /// 取得退貨商品狀態與更新訊息資訊
        /// </summary>
        /// <param name="cart_id">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.RetgoodNote> RetgoodsNote(string cart_id)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<RetgoodNote>>(APIHost + "/Return/retgoodNote", "", "", new { cartid = cart_id });
        }

        #endregion

        #region 查看功能

        /// <summary>
        /// 新增備註
        /// </summary>
        /// <param name="userID">使用者</param>
        /// <param name="cartID">訂單編號</param>
        /// <param name="updateNote">新增備註</param>
        /// <returns>返回結果</returns>
        public TWNewEgg.API.Models.ActionResponse<bool> UpdateRetGoodsNote(int userID, string cartID, string updateNote)
        {
            return this.Post<TWNewEgg.API.Models.ActionResponse<bool>>(APIHost + "/Return/UpdateRetGoodsNote", "", "", new { userID = userID, cartID = cartID, updateNote = updateNote });
        }

        #endregion

        #endregion

        #region 新訂單(優化查詢功能)

        #region 主單

        /// <summary>
        /// 取得訂單主單清單
        /// </summary>
        /// <param name="searchCondition">搜尋條件</param>
        /// <returns>訂單主單清單</returns>
        public TWNewEgg.API.Models.ActionResponse<MainOrderResult> GetMainOrder(MainOrderSearchCondition searchCondition)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<MainOrderResult>>(APIHost + "/SalesOrderSearch/GetMainOrder", "", "", searchCondition);
        }

        /// <summary>
        /// 取得訂單編號的完整資訊
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回查詢結果</returns>
        public ActionResponse<TinyCart> GetCartInfo(string cartID)
        {
            return Post<ActionResponse<TinyCart>>(APIHost + "/SalesOrderSearch/GetCartInfo", "", "", new { cartID = cartID });
        }

        /// <summary>
        /// 更新訂單遞送狀態
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <param name="delvStatus">訂單出貨狀態</param>
        /// <param name="sellerID">sellerID</param>
        /// <param name="updateUser">updateUser</param>
        /// <returns>返回結果</returns>
        public ActionResponse<TinyCart> UpdateCartDelvStatus(string cartID, int delvStatus, string updateUser)
        {
            return this.Post<ActionResponse<TinyCart>>(APIHost + "/SalesOrderSearch/UpdateCartDelvStatus", "", "", new { cartID = cartID, delvStatus = delvStatus, updateUser = updateUser });
        }

        #endregion 主單

        #region 子單
        /// <summary>
        /// 訂單子單資料蒐集
        /// </summary>
        /// <param name="cartID">訂單編號</param>
        /// <returns>返回蒐集結果</returns>
        public ActionResponse<OrderDetail> OrderDetail(string cartID)
        {
            return this.Post<ActionResponse<OrderDetail>>(APIHost + "/SalesOrderSearch/OrderDetail", "", "", new { cartID = cartID });
        }
        #endregion

        #endregion

        #region 加價購

        #region 查詢
        /// <summary>
        /// 加價購搜尋
        /// </summary>
        /// <param name="condition">查詢條件</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<List<AdditionalPurchase>> GetAdditionalPurchaseItem(ItemSketchSearchCondition condition, bool boolDefault)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<List<AdditionalPurchase>>>(APIHost + "/AdditionalPurchase/Search", "", "", new { itemSearch = condition, boolDefault = boolDefault }, 60000000);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 加價購修改
        /// </summary>
        /// <param name="condition">修改目標</param>
        /// <returns></returns>
        public TWNewEgg.API.Models.ActionResponse<string> EditAdditionalPurchaseItem(TWNewEgg.API.Models.AdditionalPurchase condition)
        {
            return Post<TWNewEgg.API.Models.ActionResponse<string>>(APIHost + "/AdditionalPurchase/Edit", "", "", new { AdditionalPurchaseItem = condition }, 60000000);
        }
        #endregion

        #endregion
    }
}
