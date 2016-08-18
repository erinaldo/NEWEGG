using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.Website.ECWeb.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.ItemService.Models;
using System.Net;
using System.Net.Sockets;
using TWNewEgg.DB;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class SalesOrderService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private string userEmail = NEUser.Email;

        /// <summary>
        /// 檢查加購商品與貨到付款是否符合資格，加購商品需要與主商品為切會的商品一起購買，若無搭配切貨主商品則不可被購買，貨到付款只限切貨商品
        /// </summary>
        /// <param name="buyingItemslist"></param>
        /// <returns></returns>
        public SaleAndDelvTypeCheck CheckSaleType(List<TWNewEgg.ItemService.Models.BuyingItems> PostData)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            SaleAndDelvTypeCheck CheckSType = new SaleAndDelvTypeCheck();

            bool itemDelvTypeFlag = false; // 切貨商品旗標
            bool itemSaleTypeFlag = false; // 加購商品旗標
            bool notCashOnDelFlag = false; // 商品貨到付款資格
            try
            {
                foreach (TWNewEgg.ItemService.Models.BuyingItems row in PostData)
                {
                    int itemId = row.buyItemID;
                    var itemSelect = (from p in db_before.Item where p.ID == itemId select p).FirstOrDefault();

                    #region 檢查加購商品是否符合資格
                    if (itemSelect.DelvType == (int)Item.tradestatus.切貨 && itemSelect.SaleType == 1) itemDelvTypeFlag = true; // 若有一般主商品
                    if (itemSelect.DelvType == (int)Item.tradestatus.切貨 && itemSelect.SaleType == 2) itemSaleTypeFlag = true; // 若有加購主商品
                    #endregion 檢查加購商品是否符合資格
                    // 只要商品含非切貨商品則無法選擇貨到付款方式付款
                    if (itemSelect.DelvType != (int)Item.tradestatus.切貨) notCashOnDelFlag = true;
                    // 檢查付款方式是否錯誤(三角與delvtype(6)不應該走此function)
                    if (itemSelect.DelvType == (int)Item.tradestatus.三角 || itemSelect.DelvType == (int)Item.tradestatus.海外切貨) CheckSType.DelvTypeCheck = true;
                }
            }
            catch (Exception e)
            {
                string Message = e.Message;
                logger.Info("SalesOrderService:CheckSaleType : " + Message + "_:_" + e.StackTrace);
                throw new Exception("SalesOrderService:CheckSaleType : " + Message);
            }
            if (itemSaleTypeFlag) // 若有加購主商品 itemSaleTypeFlag == true
            {
                if (!itemDelvTypeFlag) CheckSType.AddItemCheck = true; // 則需同切貨商品一起購買 itemDelvTypeFlag == false
            }
            if (notCashOnDelFlag) // 當商品含切貨之外的商品時
            {
                CheckSType.CashOnDelCheck = true;
            }
            return CheckSType;
        }
        
        /// <summary>
        /// 攔截美蛋沒有的商品, 檢查價格有無異動, 檢查SellerInfo
        /// </summary>
        /// <param name="PostData"></param>
        /// <param name="sendcheckout"></param>
        /// <param name="checkItemAndPriceTurnON"></param>
        /// <returns></returns>
        public CheckNewEggResult CheckNeweggStock(List<TWNewEgg.ItemService.Models.BuyingItems> PostData, List<ShoppingCartItems> sendcheckout, string checkItemAndPriceTurnON)
        {
            CheckNewEggResult checkResult = new CheckNewEggResult();
            #region 攔截美蛋沒有的商品
            TWSqlDBContext db_before = new TWSqlDBContext();
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            string CheckItemStatusTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemStatusNeweggUSA"].ToLower();
            string CheckItemPriceTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemPriceNeweggUSA"].ToLower();
            string CheckStockQtyTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckStockQtyNeweggUSA"].ToLower();
            List<int> ItemIDList = PostData.Select(x => x.buyItemID).ToList();
            List<Item> ItemList = db_before.Item.Where(x => ItemIDList.Contains(x.ID)).Distinct().ToList();
            List<int> ProductIDList = ItemList.Select(x => x.ProductID).ToList();
            List<Product> ProductList = db_before.Product.Where(x => ProductIDList.Contains(x.ID)).Distinct().ToList();
            bool flag = false; // 商品是否有問題旗標
            string messageQty = ""; // 庫存數量暫存訊息
            string messageWSStatus = ""; // 商品上下架暫存訊息
            string messagePrice = ""; //9SI商品價格異動訊息
            try
            {
                foreach (TWNewEgg.ItemService.Models.BuyingItems row in PostData)
                {
                    Item item = ItemList.Where(x => x.ID == row.buyItemID).FirstOrDefault();

                    if (item == null)
                    {
                        logger.Info("[UserEmail] " + userEmail + " item: itemID[" + row.buyItemID + "] Not Exist");
                        checkResult.EventFlag = true;
                        checkResult.messageWSStatus = "親愛的顧客，賣場編號【" + row.buyItemID + "】已完售，請重新選擇商品!";
                        return checkResult;
                    }

                    var getItem = sendcheckout.Where(x => x.ItemID == row.buyItemID).FirstOrDefault();
                    if (getItem == null)
                    {
                        logger.Info("[UserEmail] " + userEmail + " get sendcheckout Item: itemID[" + row.buyItemID + "] Not Exist");
                        checkResult.EventFlag = true;
                        checkResult.messageQty = "親愛的顧客，賣場編號【" + row.buyItemID + "】已完售，請重新選擇商品!";
                        return checkResult;
                    }

                    int itemqtytemp = getItem.ItemSellingQty;
                    Product product = ProductList.Where(x => x.ID == item.ProductID).FirstOrDefault();
                    if (product == null)
                    {
                        logger.Info("[UserEmail] " + userEmail + " ProductID: " + item.ProductID.ToString() + " Not Exist");
                        checkResult.EventFlag = true;
                        checkResult.messageWSStatus = "親愛的顧客，賣場編號【" + row.buyItemID + "】已完售，請重新選擇商品!";
                        return checkResult;
                    }

                    if (item.DelvType != (int)Item.tradestatus.間配 && item.DelvType != (int)Item.tradestatus.三角) // 切貨不進美蛋做數量確認只確認自己倉庫的數量
                    {
                        if (row.buyingNumber > itemqtytemp)
                        {
                            flag = true;
                            messageQty += "【 " + getItem.ItemName + " 】";
                        }
                    }
                    else
                    {
                        checkItemAndPriceTurnON = checkItemAndPriceTurnON.ToLower();
                        if (checkItemAndPriceTurnON == "on")
                        {
                            // 檢查賣場狀態
                            Service.ItemService itemService = new ItemService();
                            // 商品來源為美蛋
                            if (product.SourceTable.ToLower() == "productfromws")
                            {
                                //檢查美蛋商品SellerInfo.PaymentType
                                var productDetailResult = Processor.Request<TWNewEgg.Models.DomainModels.NeweggUSA.ProductDetail, TWNewEgg.Models.DomainModels.NeweggUSA.ProductDetail>("Services.NeweggRequest", "GetProductDetail", product.SellerProductID);
                                if(productDetailResult.error != null)
                                {
                                    logger.Error(item.ID + "美蛋GetProductDetail失敗, Detail: " + productDetailResult.error);
                                    productDetailResult.results = new TWNewEgg.Models.DomainModels.NeweggUSA.ProductDetail();
                                }

                                TWNewEgg.Models.DomainModels.NeweggUSA.ProductDetail pd = productDetailResult.results;
                                if(pd != null && pd.SellerInfo != null && !string.IsNullOrWhiteSpace(pd.SellerInfo.PaymentType))
                                {
                                    logger.Info("[UserEmail] " + userEmail + " [message ProductDetail SellerInfo.PaymentType is not Null or Empty] 【" + getItem.ItemName + "(" + item.ID + ")】 該商品已下架");
                                    var offShelveResult = Processor.Request<string, string>("ShelveService", "ForceOffShelve", item.ID);
                                    if (offShelveResult.error != null)
                                    {
                                        logger.Error(item.ID + "強制下架失敗: " + offShelveResult.error);
                                    }

                                    messageWSStatus += "【 " + getItem.ItemName + " 】";
                                    flag = true;
                                }

                                if (!itemService.CheckItemStatusNeweggUSA(product.ID))
                                {
                                    logger.Info("[UserEmail] " + userEmail + " [messageWSStatus] 【" + getItem.ItemName + "】 該商品已下架");
                                    messageWSStatus += "【 " + getItem.ItemName + " 】";
                                    flag = true;
                                }
                                // 如果是9SI商品，還要檢查線上售價是否與資料庫中的價格相符
                                if (product.SellerProductID.IndexOf("9SI") >= 0)
                                {
                                    if (!itemService.CheckItemPriceNeweggUSA(product.ID))
                                    {
                                        logger.Info("[UserEmail] " + userEmail + " [messagePrice] 【" + getItem.ItemName + "】 發生價格異動，線上售價與資料庫不一致");
                                        // 價格異動，線上售價與資料庫不一致
                                        messagePrice += "【 " + getItem.ItemName + " 】";
                                        flag = true;
                                    }
                                }
                                // 檢查庫存數量
                                if (!Service.ItemStockService.CheckStockQtyNeweggUSA(product.ID, row.buyingNumber))
                                {
                                    logger.Info("[UserEmail] " + userEmail + " [messageQty] 【" + getItem.ItemName + "】 該商品庫存量不足");
                                    messageQty += "【 " + getItem.ItemName + " 】";
                                    flag = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = e.ToString();
                logger.Info("SalesOrderService:CheckNeweggAndJMstock : " + Message + "_:_" + e.StackTrace);
                throw new Exception("SalesOrderService:CheckNeweggAndJMstock : " + Message);
            }

            if (messageQty != "" || flag == true)
            {
                if (messageQty != "")
                {
                    messageQty = "親愛的顧客，您所選擇的" + messageQty + "庫存數量不足，請重新選擇數量或商品!";
                }
                if (messageWSStatus != "")
                {
                    messageWSStatus = "親愛的顧客，您所選擇的" + messageWSStatus + "已下架，請重新選擇商品!";
                }
                if (messagePrice != "")
                {
                    messagePrice = "親愛的顧客，您所選擇的" + messagePrice + "發生價格異動，請重新選擇商品!";
                }
            }

            checkResult.EventFlag = flag;
            checkResult.messageQty = messageQty;
            checkResult.messageWSStatus = messageWSStatus;
            checkResult.messagePrice = messagePrice;
            return checkResult;
            #endregion 攔截美蛋沒有的商品
        }

        public CheckNewEggResult CheckNeweggAndJMstock(List<TWNewEgg.ItemService.Models.BuyingItems> PostData, List<ShoppingCartItems> sendcheckout, string checkItemAndPriceTurnON)
        {
            #region 攔截美蛋與借賣網沒有的商品
            TWSqlDBContext db_before = new TWSqlDBContext();
            string environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];
            string CheckItemStatusTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemStatusNeweggUSA"].ToLower();
            string CheckItemPriceTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckItemPriceNeweggUSA"].ToLower();
            string CheckStockQtyTurnON = System.Configuration.ConfigurationManager.AppSettings[environment + "_CheckStockQtyNeweggUSA"].ToLower();
            List<int> ItemIDList = PostData.Select(x => x.buyItemID).ToList();
            List<Item> ItemList = db_before.Item.Where(x => ItemIDList.Contains(x.ID)).Distinct().ToList();
            List<int> ProductIDList = ItemList.Select(x => x.ProductID).ToList();
            List<Product> ProductList = db_before.Product.Where(x => ProductIDList.Contains(x.ID)).Distinct().ToList();
            bool flag = false; // 商品是否有問題旗標
            string messageQty = ""; // 庫存數量暫存訊息
            string messageWSStatus = ""; // 商品上下架暫存訊息
            try
            {
                foreach (TWNewEgg.ItemService.Models.BuyingItems row in PostData)
                {
                    Item item = ItemList.Where(x => x.ID == row.buyItemID).FirstOrDefault();
                    int? itemqtytemp = sendcheckout.Where(x => x.ItemID == row.buyItemID).FirstOrDefault().ItemSellingQty;
                    Product product = ProductList.Where(x => x.ID == item.ProductID).FirstOrDefault();
                    if (product == null)
                    {
                        throw new Exception("ProductID: " + product.ID.ToString() + " Not Exist");
                    }
                    if (item.DelvType == (int)Item.tradestatus.切貨 || item.DelvType == (int)Item.tradestatus.海外切貨 || item.DelvType == (int)Item.tradestatus.直配 || item.DelvType == (int)Item.tradestatus.B2C直配 || item.DelvType == (int)Item.tradestatus.MKPL寄倉 || item.DelvType == (int)Item.tradestatus.B2c寄倉) // 切貨不進美蛋做數量確認只確認自己倉庫的數量
                    {
                        if (row.buyingNumber > itemqtytemp || itemqtytemp == null)
                        {
                            messageQty += "【 " + sendcheckout.Where(x => x.ItemID == row.buyItemID).FirstOrDefault().ItemName + " 】";
                        }
                    }
                    else
                    {
                        checkItemAndPriceTurnON = checkItemAndPriceTurnON.ToLower();
                        if (checkItemAndPriceTurnON == "on")
                        {
                            // 檢查賣場狀態
                            Service.ItemService itemService = new ItemService();
                            // 商品來源為美蛋
                            if (product.SourceTable.ToLower() == "productfromws")
                            {
                                if (!itemService.CheckItemStatusNeweggUSA(product.ID))
                                {
                                    messageWSStatus += "【 " + sendcheckout.Where(x => x.ItemID == row.buyItemID).FirstOrDefault().ItemName + " 】";
                                    flag = true;
                                }
                                //如果是9SI商品，還要檢查線上售價是否與資料庫中的價格相符
                                if (product.SellerProductID.IndexOf("9SI") >= 0)
                                {
                                    if (!itemService.CheckItemPriceNeweggUSA(product.ID))
                                    {
                                        //價格異動，線上售價與資料庫不一致
                                        flag = true;
                                    }
                                }
                                // 檢查庫存數量
                                if (!Service.ItemStockService.CheckStockQtyNeweggUSA(product.ID, row.buyingNumber))
                                {
                                    messageQty += "【 " + sendcheckout.Where(x => x.ItemID == row.buyItemID).FirstOrDefault().ItemName + " 】";
                                    flag = true;
                                }
                            }
                        }
                    }
                    // 配件
                    foreach (TWNewEgg.ItemService.Models.BuyingItemList Listnumberrow in row.buyItemLists)
                    {
                        int? itemlistqtytemp = sendcheckout.Where(x => x.ItemListID == Listnumberrow.buyItemlistID).FirstOrDefault().ItemListSellingQty;
                        if (Listnumberrow.buyingNumber > itemlistqtytemp || itemlistqtytemp == null)
                        {
                            messageQty = "【 " + sendcheckout.Where(x => x.ItemListID == Listnumberrow.buyItemlistID).FirstOrDefault().ItemListName + " 】";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string Message = e.Message;
                logger.Info("SalesOrderService:CheckNeweggAndJMstock : " + Message + "_:_" + e.StackTrace);
                throw new Exception("SalesOrderService:CheckNeweggAndJMstock : " + Message);
            }
            if (messageQty != "" || flag == true)
            {
                if (messageQty != "")
                {
                    messageQty = "親愛的顧客，您所選擇的" + messageQty + "庫存數量不足，請重新選擇數量或商品!";
                }
                if (messageWSStatus != "")
                {
                    messageWSStatus = "親愛的顧客，您所選擇的" + messageWSStatus + "已下架，請重新選擇商品!";
                }
            }
            return new CheckNewEggResult { EventFlag = flag, messageQty = messageQty, messageWSStatus = messageWSStatus };
            #endregion 攔截美蛋與借賣網沒有的商品
        }

        public class CheckNewEggResult
        {
            public CheckNewEggResult()
            {
                this.EventFlag = false;
                this.messageQty = "";
                this.messageWSStatus = "";
                this.messagePrice = "";
            }

            public bool EventFlag { get; set; }
            public string messageQty { get; set; }
            public string messageWSStatus { get; set; }
            public string messagePrice { get; set; }
        }

        /// <summary>
        /// 購物車資料組合
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="getItemDisplayPriceList"></param>
        /// <param name="account_id"></param>
        /// <param name="getNumByDate"></param>
        /// <returns></returns>
        public InsertSalesOrdersBySellerInput SODataCombine(List<ShoppingCartItems> increasePurchaseItemList, List<TWNewEgg.ItemService.Models.BuyingItems> postData, List<ItemDisplayPrice> getItemDisplayPriceList, int account_id, int getNumByDate)
        {
            InsertSalesOrdersBySellerInput InsertData = new InsertSalesOrdersBySellerInput();
            // 一般商品
            List<TWNewEgg.ItemService.Models.BuyingItems> normalPostData = new List<BuyingItems>();
            // 加價購商品
            List<TWNewEgg.ItemService.Models.BuyingItems> increasePurchasePostData = new List<BuyingItems>();
            TWSqlDBContext db_before = new TWSqlDBContext();
            #region SalesOrder資料組合
            try
            {
                List<int> itemIDList = postData.Select(x => x.buyItemID).ToList();
                List<Item> itemList = db_before.Item.Where(x => itemIDList.Contains(x.ID)).ToList();
                List<int> productIDList = itemList.Select(x => x.ProductID).Distinct().ToList();
                List<Product> productList = db_before.Product.Where(x => productIDList.Contains(x.ID)).ToList();
                List<ItemDisplayPrice> itemDisplayPriceList = db_before.ItemDisplayPrice.Where(x => itemIDList.Contains(x.ItemID)).ToList();
                // 一般商品計算排除加價購商品
                List<int> ipurchaseItemIDList = new List<int>();
                if (increasePurchaseItemList != null)
                {
                    ipurchaseItemIDList = increasePurchaseItemList.Select(x => x.ItemID).ToList();
                }

                normalPostData = postData.Where(x => !ipurchaseItemIDList.Contains(x.buyItemID)).ToList();
                increasePurchasePostData = postData.Where(x => ipurchaseItemIDList.Contains(x.buyItemID)).ToList();
                // 一般商品資料組合
                NormalCart(ref InsertData, normalPostData, itemDisplayPriceList, itemList, productList);
                IncreasePurchaseCart(ref InsertData, increasePurchasePostData, itemDisplayPriceList, itemList, productList);
            }
            catch (Exception e)
            {
                string Message = e.Message;
                logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(SalesOrder資料組合) : " + Message + "_:_" + e.StackTrace);
                throw new Exception("SalesOrderService:SODataCombine(SalesOrder資料組合) : " + Message);
            }
            #endregion SalesOrder資料組合
            if (InsertData.pricesum <= 0)
            {
                //logger.Info("SalesOrderService:SODataCombine(商品價格錯誤請重新選擇商品:總金額為0)");
                //throw new Exception("商品價格錯誤請重新選擇商品");
            }

            #region 單一性資料組合
            try
            {
                InsertData.salesorderPrefix = "'LBO'";
                InsertData.salesorderitemPrefix = "'LBS'";
                InsertData.ordernum = getNumByDate;
                InsertData.note = "''";

                InsertData.salesorder_telday = "''";
                InsertData.salesorder_invoreceiver = "''";
                InsertData.salesorder_invoid = "''";
                InsertData.salesorder_invotitle = "''";
                InsertData.salesorder_involoc = "''";
                InsertData.salesorder_invozip = "''";
                InsertData.salesorder_invoaddr = "''";
                InsertData.salesorder_name = "''";
                InsertData.salesorder_paytypeid = 0;
                InsertData.salesorder_paytype = 1;
                InsertData.salesorder_email = (from p in db_before.Account where p.ID == account_id select p.Email).FirstOrDefault();
                InsertData.salesorder_delivloc = "''";
                InsertData.salesorder_delivzip = "''";
                InsertData.salesorder_delivaddr = "''";
                InsertData.salesorder_delivengaddr = "";

                SalesOrder salesorder = (from p in db_before.SalesOrder where p.AccountID == account_id select p).FirstOrDefault();
                if (salesorder == null)
                {
                    InsertData.salesorder_idno = "''";
                    InsertData.salesorder_mobile = "''";
                    InsertData.salesorder_recvname = "''";
                    InsertData.salesorder_recvmobile = "''";
                    InsertData.salesorder_recvtelday = "''";
                    InsertData.salesorder_authcode = "''";
                }
                else
                {
                    InsertData.salesorder_idno = "'" + salesorder.IDNO + "'";
                    InsertData.salesorder_mobile = "'" + salesorder.Mobile + "'";
                    InsertData.salesorder_recvname = "'" + salesorder.Name + "'";
                    InsertData.salesorder_recvmobile = "'" + salesorder.RecvMobile + "'";
                    InsertData.salesorder_recvtelday = "'" + salesorder.RecvTelDay + "'";
                    InsertData.salesorder_authcode = "'" + salesorder.AuthCode + "'";
                }

                InsertData.salesorder_accountid = account_id;
                InsertData.salesorder_recvengname = "";
                //InsertData.salesorder_cardno = "'ABCD-EFGH-IJKL-MNOP'";
                InsertData.salesorder_cardtype = "''";
                InsertData.salesorder_cardbank = "''";
                InsertData.salesorder_cardexpire = "''";  // 年/月
                InsertData.salesorder_cardbirthday = Convert.ToDateTime("1990/01/01");
                InsertData.salesorder_cardloc = "''";
                InsertData.salesorder_cardzip = "''";
                InsertData.salesorder_cardaddr = "''";
                InsertData.salesorder_status = (int)SalesOrder.status.未付款; //為驗證成功前先儲存為99

                try
                {
                    InsertData.salesorder_remoteip = "'" + GetClientIP() + "'";
                }
                catch (Exception e)
                {
                    logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(無法取得GetClientIP) [ErrorMsg]" + e.Message + " [ErrorStackTrace] " + e.StackTrace);
                    InsertData.salesorder_remoteip = "''";
                }
                //InsertData.salesorder_remoteip = "'127.0.0.1'";
                InsertData.salesorder_coservername = "'newegg.com.tw'";
                InsertData.salesorder_servername = "'web01'";
                InsertData.salesorder_authdate = Convert.ToDateTime(DateTime.Now.ToShortDateString().ToString());
                InsertData.salesorder_authnote = "''";
                InsertData.salesorder_updateuser = "'sys_insert'";
                InsertData.salesordergroupext_pscartid = 0;
                InsertData.salesordergroupext_pssellerid = "''";
                InsertData.salesordergroupext_pscarrynote = "''";
                InsertData.salesordergroupext_pshasact = 0;
                InsertData.salesordergroupext_pshaspartialauth = 0;
                //---------------------各別運費計算----------------------//
                #region 運費主單
                decimal getTotalShipping = 0;
                List<string> tempShippingList = InsertData.salesorderitems_shippingexpense.Replace("'", "").Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
                tempShippingList.ForEach(x =>
                {
                    getTotalShipping += Convert.ToDecimal(x);
                });
                if (Math.Floor(0.5m + getTotalShipping) != 0)
                {
                    //InsertData.item_id += "12451,"; // 國際運費專用item_id
                    //InsertData.salesorderitems_itemlistid += "0,";  // 只有配件與贈品的id才可放入
                    //InsertData.item_attribid += "0,";
                    //InsertData.itemlist_attribid += "0,";
                    //InsertData.salesorders_note += "國際運費,";  // 主單註記
                    //InsertData.salesorderitems_price += Math.Floor(0.5m + ShippingResults.ShippingTotal) + ",";
                    //InsertData.salesorderitems_shippingexpense += "0,";
                    //InsertData.salesorderitems_serviceexpense += "0,"; // 部分服務費
                    //InsertData.salesorderitems_tax += "0,"; // 部分稅金
                    //InsertData.salesorderitems_itempricesum += "0,"; // 部分商品總價
                    //InsertData.pricesum += Math.Floor(0.5m + ShippingResults.ShippingTotal); // 加上運費的總價
                    InsertData.pricesum += getTotalShipping; // 加上運費的總價
                    //InsertData.salesorders_delivtype += (from p in db_before.Item where p.ID == 12451 select p.DelvType).FirstOrDefault() + ",";
                    //InsertData.salesorders_delivdata += ",";
                    //InsertData.salesorders_itemname += "國際運費,";
                    //InsertData.salesorderitems_qty += "1,";  // 數量
                    //InsertData.salesorderitems_note += "國際運費,";  // 子單註記
                    //InsertData = subDataInsert(InsertData);
                }
                #endregion 運費主單
                #region 服務費主單
                decimal getTotalServicefees = 0;
                List<string> tempServicefeesList = InsertData.salesorderitems_serviceexpense.Replace("'", "").Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
                tempServicefeesList.ForEach(x =>
                {
                    getTotalServicefees += Convert.ToDecimal(x);
                });
                if (Math.Floor(0.5m + getTotalServicefees) > 0)
                {
                    //InsertData.item_id += "12453,"; // 服務費專用item_id
                    //InsertData.salesorderitems_itemlistid += "0,";  // 只有配件與贈品的id才可放入
                    //InsertData.item_attribid += "0,";
                    //InsertData.itemlist_attribid += "0,";
                    //InsertData.salesorders_note += "服務費,";  // 主單註記
                    //InsertData.salesorderitems_price += Math.Floor(0.5m + DataTemp.servicefees) + ",";
                    //InsertData.salesorderitems_shippingexpense += "0,";
                    //InsertData.salesorderitems_serviceexpense += "0,"; // 部分服務費
                    //InsertData.salesorderitems_tax += "0,"; // 部分稅金
                    //InsertData.salesorderitems_itempricesum += "0,"; // 部分商品總價
                    //--------加上服務費的總價--------//
                    InsertData.pricesum += getTotalServicefees;
                    //--------加上服務費的總價--------//
                    //InsertData.salesorders_delivtype += (from p in db_before.Item where p.ID == 12453 select p.DelvType).FirstOrDefault() + ",";
                    //InsertData.salesorders_delivdata += ",";
                    //InsertData.salesorders_itemname += "服務費,";
                    //InsertData.salesorderitems_qty += "1,";  // 數量
                    //InsertData.salesorderitems_note += "服務費,";  // 子單註記
                    //InsertData = subDataInsert(InsertData);
                }
                #endregion 服務費主單
                if (InsertData.pricesum <= 0)
                {
                    logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(商品價格錯誤請重新選擇商品:總金額為0)");
                    throw new Exception("商品價格錯誤請重新選擇商品");
                }
                #region 去除多餘字元
                InsertData.salesorderitems_displayprice = RemoveExcessComma(InsertData.salesorderitems_displayprice);
                InsertData.salesorderitems_discountprice = RemoveExcessComma(InsertData.salesorderitems_discountprice);
                InsertData.salesorderitems_shippingexpense = RemoveExcessComma(InsertData.salesorderitems_shippingexpense);
                InsertData.salesorderitems_serviceexpense = RemoveExcessComma(InsertData.salesorderitems_serviceexpense);
                InsertData.salesorderitems_tax = RemoveExcessComma(InsertData.salesorderitems_tax);
                InsertData.salesorderitems_itempricesum = RemoveExcessComma(InsertData.salesorderitems_itempricesum);
                // 商品分期利息
                InsertData.salesorderitems_installmentfee = "'" + RemoveExcessComma(InsertData.salesorderitems_installmentfee) + "'";

                //logger.Info("salesorderitems_isnew = [" + InsertData.salesorderitems_isnew + "]");
                InsertData.salesorderitems_isnew = "'" + RemoveExcessComma(InsertData.salesorderitems_isnew) + "'";

                InsertData.item_id = "'" + RemoveExcessComma(InsertData.item_id) + "'";
                InsertData.salesorderitems_itemlistid = "'" + RemoveExcessComma(InsertData.salesorderitems_itemlistid) + "'";
                InsertData.item_attribid = "'" + RemoveExcessComma(InsertData.item_attribid) + "'";
                InsertData.salesorders_note = "'" + RemoveExcessComma(InsertData.salesorders_note) + "'";
                InsertData.salesorders_delivtype = "'" + RemoveExcessComma(InsertData.salesorders_delivtype) + "'";
                InsertData.salesorders_delivdata = "'" + RemoveExcessComma(InsertData.salesorders_delivdata) + "'";
                InsertData.salesorders_itemname = RemoveExcessComma(InsertData.salesorders_itemname);
                InsertData.salesorders_itemname = InsertData.salesorders_itemname.Replace("'", "<apostrophe>");
                InsertData.salesorders_itemname = "'" + InsertData.salesorders_itemname + "'";

                InsertData.salesorderitems_qty = "'" + RemoveExcessComma(InsertData.salesorderitems_qty) + "'";
                InsertData.salesorderitems_note = "'" + RemoveExcessComma(InsertData.salesorderitems_note) + "'";
                InsertData.salesorderitems_price = "'" + RemoveExcessComma(InsertData.salesorderitems_price) + "'";
                InsertData.salesorderitems_priceinst = "'" + RemoveExcessComma(InsertData.salesorderitems_priceinst) + "'";
                InsertData.salesorderitems_pricecoupon = "'" + RemoveExcessComma(InsertData.salesorderitems_pricecoupon) + "'";
                InsertData.salesorderitems_coupons = "'" + RemoveExcessComma(InsertData.salesorderitems_coupons) + "'";
                InsertData.salesorderitems_redmbln = "'" + RemoveExcessComma(InsertData.salesorderitems_redmbln) + "'";
                InsertData.salesorderitems_redmtkout = "'" + RemoveExcessComma(InsertData.salesorderitems_redmtkout) + "'";
                InsertData.salesorderitems_redmfdbck = "'" + RemoveExcessComma(InsertData.salesorderitems_redmfdbck) + "'";
                InsertData.salesorderitems_wfbln = "'" + RemoveExcessComma(InsertData.salesorderitems_wfbln) + "'";
                InsertData.salesorderitems_wftkout = "'" + RemoveExcessComma(InsertData.salesorderitems_wftkout) + "'";
                InsertData.salesorderitems_actid = "'" + RemoveExcessComma(InsertData.salesorderitems_actid) + "'";
                InsertData.salesorderitems_acttkout = "'" + RemoveExcessComma(InsertData.salesorderitems_acttkout) + "'";
                InsertData.itemlist_attribid = "'" + RemoveExcessComma(InsertData.itemlist_attribid) + "'";
                InsertData.salesorderitemexts_psproductid = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psproductid) + "'";
                InsertData.salesorderitemexts_psmproductid = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psmproductid) + "'";
                InsertData.salesorderitemexts_psoriprice = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psoriprice) + "'";
                InsertData.salesorderitemexts_pssellcatid = "'" + RemoveExcessComma(InsertData.salesorderitemexts_pssellcatid) + "'";
                InsertData.salesorderitemexts_psattribname = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psattribname) + "'";
                InsertData.salesorderitemexts_psmodelno = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psmodelno) + "'";
                InsertData.salesorderitemexts_pscost = "'" + RemoveExcessComma(InsertData.salesorderitemexts_pscost) + "'";
                InsertData.salesorderitemexts_psfvf = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psfvf) + "'";
                InsertData.salesorderitemexts_psproducttype = "'" + RemoveExcessComma(InsertData.salesorderitemexts_psproducttype) + "'";
                #endregion
            }
            catch (Exception e)
            {
                string Message = e.Message;
                logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(單一性資料組合) : " + Message + "_:_" + e.StackTrace);
                throw new Exception("SalesOrderService:SODataCombine(單一性資料組合) : " + Message);
            }
            #endregion 單一性資料組合
            return InsertData;
        }

        /// <summary>
        /// 一般商品資料組合
        /// </summary>
        /// <param name="InsertData"></param>
        /// <param name="PostData"></param>
        /// <param name="itemDisplayPriceList"></param>
        /// <param name="itemList"></param>
        /// <param name="productList"></param>
        private void NormalCart(ref InsertSalesOrdersBySellerInput InsertData, List<TWNewEgg.ItemService.Models.BuyingItems> PostData, List<ItemDisplayPrice> itemDisplayPriceList, List<Item> itemList, List<Product> productList)
        {
            InsertSalesOrdersBySellerInput DataTemp = new InsertSalesOrdersBySellerInput();
            foreach (TWNewEgg.ItemService.Models.BuyingItems row in PostData)
            {
                ItemDisplayPrice searchItemDisplayPrice = itemDisplayPriceList.Where(x => x.ItemID == row.buyItemID).FirstOrDefault();
                //postdata saleseorder count
                Item item = null;
                Product product = null;
                item = itemList.Where(x => x.ID == row.buyItemID).FirstOrDefault();
                product = productList.Where(x => x.ID == item.ProductID).FirstOrDefault();
                if (item != null)
                {
                    item.Name += GetItemMarketGroupName(item.ID);
                }
                // Item與Product資料一致性檢查
                CheckSellerIDAndDelvType(item, product);
                DataTemp = DataClear(DataTemp);
                for (int i = 0; i < row.buyingNumber; i++)
                {
                    DataTemp.salesorderitems_isnew += item.IsNew + ",";
                    DataTemp.item_id += row.buyItemID + ",";      // 71,
                    DataTemp.salesorderitems_itemlistid += "0,";  //  0,
                    DataTemp.item_attribid += "0,";               //  0,
                    DataTemp.itemlist_attribid += "0,";           //  0,
                    DataTemp.salesorders_note += ","; // 主單位置填上主單註記
                    // 有屬性的item其價格跟隨該item的價格，有屬性的itemlist的價格則跟隨該itemlist的價格
                    decimal PriceTemp = 0, Shipping = 0, TaxTemp = 0, ServicefeesTemp = 0;
                    TaxTemp = searchItemDisplayPrice.DisplayTax;
                    Shipping = searchItemDisplayPrice.DisplayShipping - Math.Floor(0.5m + item.ServicePrice);
                    ServicefeesTemp = Math.Floor(0.5m + item.ServicePrice);
                    // 價格單一化顯示金額
                    DataTemp.salesorderitems_displayprice += searchItemDisplayPrice.DisplayPrice + ",";
                    // 價格單一化折扣金額
                    DataTemp.salesorderitems_discountprice += "0,";
                    // 台灣售價(price 含稅Tax) = item.PriceCash + 稅Tax
                    PriceTemp = Math.Floor(0.5m + item.PriceCash + TaxTemp);
                    DataTemp.salesorderitems_price += PriceTemp + ","; // 三角稅金由使用者自付，我們不代付，所以稅金0元 // 填入的金額為含稅價
                    DataTemp.salesorderitems_installmentfee += "0,"; // 商品分期利息
                    DataTemp.pricesum += PriceTemp; // 因為item.PriceCash 不含運費 所以不用額外再扣一次運費
                    DataTemp.servicefees += ServicefeesTemp;
                    // 部分稅金、運費、部分服務費，在後面做計算所以這裡暫時Mark起來 -- 2015-07-04間配聰明購取消，所以取消Mark
                    DataTemp.salesorderitems_tax += TaxTemp + ",";
                    DataTemp.salesorderitems_shippingexpense += Shipping + ",";
                    DataTemp.salesorderitems_serviceexpense += ServicefeesTemp + ",";
                    DataTemp.salesorderitems_itempricesum += (PriceTemp + Shipping + ServicefeesTemp) + ",";
                    if (Math.Floor(0.5m + item.PriceCash) <= 0)
                    {
                        logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(Step3:商品價格錯誤 " + item.ID + ": item.PriceCash <= 0)");
                        throw new Exception("商品價格錯誤請重新選擇商品");
                    }

                    DataTemp.salesorders_delivtype += item.DelvType + ",";
                    DataTemp.salesorders_delivdata += ",";
                    string itemname_temp = item.Name;
                    itemname_temp = itemname_temp.Replace(",", "repdot");
                    itemname_temp = itemname_temp.Replace("，", "repdot");
                    DataTemp.salesorders_itemname += itemname_temp + ",";
                    DataTemp.salesorderitems_qty += "1,";
                    DataTemp.salesorderitems_note += ",";
                    DataTemp = subDataInsert(DataTemp);
                }

                InsertData = DataSave(DataTemp, InsertData);
            }
        }

        /// <summary>
        /// 加價購商品資料組合
        /// </summary>
        /// <param name="InsertData"></param>
        /// <param name="increasePurchasePostData"></param>
        /// <param name="itemDisplayPriceList"></param>
        /// <param name="itemList"></param>
        /// <param name="productList"></param>
        private void IncreasePurchaseCart(ref InsertSalesOrdersBySellerInput InsertData, List<TWNewEgg.ItemService.Models.BuyingItems> increasePurchasePostData, List<ItemDisplayPrice> itemDisplayPriceList, List<Item> itemList, List<Product> productList)
        {
            InsertSalesOrdersBySellerInput DataTemp = new InsertSalesOrdersBySellerInput();
            foreach (TWNewEgg.ItemService.Models.BuyingItems row in increasePurchasePostData)
            {
                ItemDisplayPrice searchItemDisplayPrice = itemDisplayPriceList.Where(x => x.ItemID == row.buyItemID).FirstOrDefault();
                //postdata saleseorder count
                Item item = null;
                Product product = null;
                item = itemList.Where(x => x.ID == row.buyItemID).FirstOrDefault();
                product = productList.Where(x => x.ID == item.ProductID).FirstOrDefault();
                if (item != null)
                {
                    item.Name += GetItemMarketGroupName(item.ID);
                }
                // Item與Product資料一致性檢查
                CheckSellerIDAndDelvType(item, product);
                DataTemp = DataClear(DataTemp);
                for (int i = 0; i < row.buyingNumber; i++)
                {
                    DataTemp.salesorderitems_isnew += item.IsNew + ",";
                    DataTemp.item_id += row.buyItemID + ",";      // 71,
                    DataTemp.salesorderitems_itemlistid += "0,";  //  0,
                    DataTemp.item_attribid += "0,";               //  0,
                    DataTemp.itemlist_attribid += "0,";           //  0,
                    DataTemp.salesorders_note += ","; // 主單位置填上主單註記
                    // 有屬性的item其價格跟隨該item的價格，有屬性的itemlist的價格則跟隨該itemlist的價格
                    decimal PriceTemp = 0, Shipping = 0, TaxTemp = 0, ServicefeesTemp = 0;
                    TaxTemp = searchItemDisplayPrice.DisplayTax;
                    Shipping = searchItemDisplayPrice.DisplayShipping - Math.Floor(0.5m + item.ServicePrice);
                    ServicefeesTemp = Math.Floor(0.5m + item.ServicePrice);
                    // 價格單一化顯示金額
                    DataTemp.salesorderitems_displayprice += searchItemDisplayPrice.DisplayPrice + ",";
                    // 價格單一化折扣金額
                    DataTemp.salesorderitems_discountprice += "0,";
                    // 台灣售價(price 含稅Tax) = item.PriceCash + 稅Tax
                    PriceTemp = Math.Floor(0.5m + item.PriceCash + TaxTemp);
                    DataTemp.salesorderitems_price += PriceTemp + ","; // 三角稅金由使用者自付，我們不代付，所以稅金0元 // 填入的金額為含稅價
                    DataTemp.salesorderitems_installmentfee += "0,"; // 商品分期利息
                    DataTemp.pricesum += PriceTemp; // 因為item.PriceCash 不含運費 所以不用額外再扣一次運費
                    DataTemp.servicefees += ServicefeesTemp;
                    // 部分稅金、運費、部分服務費，在後面做計算所以這裡暫時Mark起來 -- 2015-07-04間配聰明購取消，所以取消Mark
                    DataTemp.salesorderitems_tax += TaxTemp + ",";
                    DataTemp.salesorderitems_shippingexpense += Shipping + ",";
                    DataTemp.salesorderitems_serviceexpense += ServicefeesTemp + ",";
                    DataTemp.salesorderitems_itempricesum += (PriceTemp + Shipping + ServicefeesTemp) + ",";
                    if (Math.Floor(0.5m + item.PriceCash) <= 0)
                    {
                        logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(Step3:商品價格錯誤 " + item.ID + ": item.PriceCash <= 0)");
                        throw new Exception("商品價格錯誤請重新選擇商品");
                    }

                    DataTemp.salesorders_delivtype += item.DelvType + ",";
                    DataTemp.salesorders_delivdata += ",";
                    string itemname_temp = item.Name;
                    itemname_temp = itemname_temp.Replace(",", "repdot");
                    itemname_temp = itemname_temp.Replace("，", "repdot");
                    DataTemp.salesorders_itemname += itemname_temp + ",";
                    DataTemp.salesorderitems_qty += "1,";
                    DataTemp.salesorderitems_note += ",";
                    DataTemp = subDataInsert(DataTemp);
                }

                InsertData = DataSave(DataTemp, InsertData);
            }
        }

        /// <summary>
        /// GetItemMarketGroupName
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        private string GetItemMarketGroupName(int itemID)
        {
            string result = string.Empty;
            var requestResult = TWNewEgg.Framework.ServiceApi.Processor.Request<string, string>("ItemGroupService", "GetItemMarketGroupNameByItemId", itemID);
            if (string.IsNullOrEmpty(requestResult.error))
            {
                result = requestResult.results;
            }

            return result;
        }

        /// <summary>
        /// Item與Product資料一致性檢查
        /// </summary>
        /// <param name="item"></param>
        /// <param name="product"></param>
        /// <param name="userEmail"></param>
        private void CheckSellerIDAndDelvType(Item item, Product product)
        {
            if (item == null || product == null)
            {
                throw new Exception("資料缺漏，請與客服聯繫!");
            }

            if (item.DelvType != product.DelvType || item.SellerID != product.SellerID) // 資料一致性檢查
            {
                string Message = "";
                if (item.DelvType != product.DelvType) Message += "ItemID: " + item.ID + " DelvType與Product DelvType不符 ";
                if (item.SellerID != product.SellerID) Message += "ItemID: " + item.ID + " SellerID與Product SellerID不符 ";
                logger.Info("[UserEmail] [" + userEmail + "] " + "SalesOrderService:SODataCombine(資料一致性檢查錯誤) : " + Message);
                throw new Exception("SalesOrderService:SODataCombine(資料一致性檢查錯誤) : " + Message);
            }
        }

        /// <summary>
        /// 去除多餘的逗號
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        private string RemoveExcessComma(string sourceString)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(sourceString))
            {
                result = sourceString.Substring(0, sourceString.Length - 1);
            }

            return result;
        }

        private InsertSalesOrdersBySellerInput subDataInsert(InsertSalesOrdersBySellerInput DataTemp)
        {
            DataTemp.salesorderitems_priceinst += "0,";
            DataTemp.salesorderitems_pricecoupon += "0,";
            DataTemp.salesorderitems_coupons += ",";
            DataTemp.salesorderitems_redmbln += "0,";
            DataTemp.salesorderitems_redmtkout += "0,";
            DataTemp.salesorderitems_redmfdbck += "0,";
            DataTemp.salesorderitems_wfbln += "0,";
            DataTemp.salesorderitems_wftkout += "0,";
            DataTemp.salesorderitems_actid += "0,";
            DataTemp.salesorderitems_acttkout += "0,";
            DataTemp.salesorderitemexts_psproductid += ",";
            DataTemp.salesorderitemexts_psmproductid += ",";
            DataTemp.salesorderitemexts_psoriprice += ",";
            DataTemp.salesorderitemexts_pssellcatid += ",";
            DataTemp.salesorderitemexts_psattribname += ",";
            DataTemp.salesorderitemexts_psmodelno += ",";
            DataTemp.salesorderitemexts_pscost += ",";
            DataTemp.salesorderitemexts_psfvf += ",";
            DataTemp.salesorderitemexts_psproducttype += ",";
            return DataTemp;
        }

        /// <summary>
        /// 清除暫存資料
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        private InsertSalesOrdersBySellerInput DataClear(InsertSalesOrdersBySellerInput Data)
        {
            Data.salesorderitems_isnew = "";
            Data.item_id = "";
            Data.salesorderitems_itemlistid = "";
            Data.item_attribid = "";
            Data.itemlist_attribid = "";

            Data.salesorders_note = "";

            Data.salesorderitems_price = "";
            // 商品分期利息
            Data.salesorderitems_installmentfee = "";
            Data.salesorderitems_displayprice = "";
            Data.salesorderitems_discountprice = "";
            Data.salesorderitems_shippingexpense = "";
            Data.salesorderitems_serviceexpense = "";
            Data.salesorderitems_tax = "";
            Data.salesorderitems_itempricesum = "";
            Data.pricesum = 0;

            Data.salesorders_delivtype = "";
            Data.salesorders_delivdata = "";
            Data.salesorders_itemname = "";
            Data.salesorderitems_qty = "";
            Data.salesorderitems_note = "";
            Data.salesorderitems_priceinst = "";
            Data.salesorderitems_pricecoupon = "";
            Data.salesorderitems_coupons = "";
            Data.salesorderitems_redmbln = "";
            Data.salesorderitems_redmtkout = "";
            Data.salesorderitems_redmfdbck = "";
            Data.salesorderitems_wfbln = "";
            Data.salesorderitems_wftkout = "";
            Data.salesorderitems_actid = "";
            Data.salesorderitems_acttkout = "";
            Data.salesorderitemexts_psproductid = "";
            Data.salesorderitemexts_psmproductid = "";
            Data.salesorderitemexts_psoriprice = "";
            Data.salesorderitemexts_pssellcatid = "";
            Data.salesorderitemexts_psattribname = "";
            Data.salesorderitemexts_psmodelno = "";
            Data.salesorderitemexts_pscost = "";
            Data.salesorderitemexts_psfvf = "";
            Data.salesorderitemexts_psproducttype = "";

            return Data;
        }

        /// <summary>
        /// 資料設置
        /// </summary>
        /// <param name="Datainsert"></param>
        /// <param name="Datasave"></param>
        /// <returns></returns>
        private InsertSalesOrdersBySellerInput DataSave(InsertSalesOrdersBySellerInput Datainsert, InsertSalesOrdersBySellerInput Datasave)
        {
            Datasave.salesorderitems_isnew += Datainsert.salesorderitems_isnew; // IsNew
            Datasave.item_id += Datainsert.item_id; // itemid
            Datasave.salesorderitems_itemlistid += Datainsert.salesorderitems_itemlistid; // itemlistid
            Datasave.item_attribid += Datainsert.item_attribid; // item屬性
            Datasave.itemlist_attribid += Datainsert.itemlist_attribid; // itemlist屬性
            Datasave.salesorders_note += Datainsert.salesorders_note; // 主單註記
            Datasave.salesorderitems_price += Datainsert.salesorderitems_price; // 單價
            Datasave.salesorderitems_displayprice += Datainsert.salesorderitems_displayprice; // 價格單一化顯示金額
            Datasave.salesorderitems_discountprice += Datainsert.salesorderitems_discountprice; // 價格單一化折扣金額
            Datasave.salesorderitems_shippingexpense += Datainsert.salesorderitems_shippingexpense; // 部分運費
            Datasave.salesorderitems_serviceexpense += Datainsert.salesorderitems_serviceexpense;  // 部分服務費
            Datasave.salesorderitems_tax += Datainsert.salesorderitems_tax; // 部分稅金
            Datasave.salesorderitems_itempricesum += Datainsert.salesorderitems_itempricesum;
            Datasave.pricesum += Datainsert.pricesum; //總價
            Datasave.salesorderitems_installmentfee += Datainsert.salesorderitems_installmentfee; // 商品分期利息

            Datasave.salesorders_delivtype += Datainsert.salesorders_delivtype;
            Datasave.salesorders_delivdata += Datainsert.salesorders_delivdata;
            Datasave.salesorders_itemname += Datainsert.salesorders_itemname;
            Datasave.salesorderitems_qty += Datainsert.salesorderitems_qty;
            Datasave.salesorderitems_note += Datainsert.salesorderitems_note;
            Datasave.salesorderitems_priceinst += Datainsert.salesorderitems_priceinst;
            Datasave.salesorderitems_pricecoupon += Datainsert.salesorderitems_pricecoupon;
            Datasave.salesorderitems_coupons += Datainsert.salesorderitems_coupons;
            Datasave.salesorderitems_redmbln += Datainsert.salesorderitems_redmbln;
            Datasave.salesorderitems_redmtkout += Datainsert.salesorderitems_redmtkout;
            Datasave.salesorderitems_redmfdbck += Datainsert.salesorderitems_redmfdbck;
            Datasave.salesorderitems_wfbln += Datainsert.salesorderitems_wfbln;
            Datasave.salesorderitems_wftkout += Datainsert.salesorderitems_wftkout;
            Datasave.salesorderitems_actid += Datainsert.salesorderitems_actid;
            Datasave.salesorderitems_acttkout += Datainsert.salesorderitems_acttkout;
            Datasave.salesorderitemexts_psproductid += Datainsert.salesorderitemexts_psproductid;
            Datasave.salesorderitemexts_psmproductid += Datainsert.salesorderitemexts_psmproductid;
            Datasave.salesorderitemexts_psoriprice += Datainsert.salesorderitemexts_psoriprice;
            Datasave.salesorderitemexts_pssellcatid += Datainsert.salesorderitemexts_pssellcatid;
            Datasave.salesorderitemexts_psattribname += Datainsert.salesorderitemexts_psattribname;
            Datasave.salesorderitemexts_psmodelno += Datainsert.salesorderitemexts_psmodelno;
            Datasave.salesorderitemexts_pscost += Datainsert.salesorderitemexts_pscost;
            Datasave.salesorderitemexts_psfvf += Datainsert.salesorderitemexts_psfvf;
            Datasave.salesorderitemexts_psproducttype += Datainsert.salesorderitemexts_psproducttype;
            return Datasave;
        }

        protected string GetClientIP()
        {
            string IP = "";
            try
            {
                string HostName = Dns.GetHostName();
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    // 從IP地址列表中篩選出IPv4類型的IP地址
                    // AddressFamily.InterNetwork表示此IP為IPv4,
                    // AddressFamily.InterNetworkV6表示此地址為IPv6類型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        IP = IpEntry.AddressList[i].ToString();
                    }
                }
            }
            catch (Exception e)
            {
                string Message = e.Message;
                logger.Info("SalesOrderService:GetClientIP : " + Message + "_:_" + e.StackTrace);
                throw new Exception("SalesOrderService:GetClientIP : " + Message);
            }
            return IP;
            /*
            if (Request.ServerVariables["HTTP_VIA"] == null) return Request.ServerVariables["REMOTE_ADDR"].ToString();
            else return Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            */
        }

        public void SettingItemInvoiceData(List<InsertSalesOrdersBySellerOutput> send, string invore3_2, string invoiceCarType, string invoiceCarCell, string invoiceCarNatu, string invoiceDonCode)
        {
            var carrierType = "";
            var carrierId1 = "";
            var carrierId2 = "";
            var donateCode = "";
            switch (invoiceCarType)
            {
                case "Cell":
                    carrierType = "3J0002";
                    carrierId1 = invoiceCarCell;
                    carrierId2 = invoiceCarCell;
                    break;
                case "Natu":
                    carrierType = "CQ0001";
                    carrierId1 = invoiceCarNatu;
                    carrierId2 = invoiceCarNatu;
                    break;
                default:
                    carrierType = "EG0085";
                    carrierId1 = send[0].salesorder_accountid.ToString("X8");
                    carrierId2 = send[0].salesorder_accountid.ToString("X8");
                    break;
            }
            switch (invore3_2)
            {
                case "開立二聯式發票":
                    carrierType = "EG0085_P";
                    break;
                case "開立三聯式發票":
                    carrierType = "EG0085";
                    break;
                default:
                    //carrierType = "EG0085_P";
                    break;
            }
            if (!string.IsNullOrEmpty(invoiceDonCode))
            {
                donateCode = invoiceDonCode;
            }
            if (send != null)
            {
                var soNOs = send.Select(x => x.salesorder_code).Distinct().ToList();
                using (TWSqlDBContext twsqlDB = new TWSqlDBContext())
                {
                    var allSO = twsqlDB.SalesOrder.Where(x => soNOs.Contains(x.Code)).ToList();
                    allSO.ForEach(orderInfo =>
                    {
                        orderInfo.invoiceCarrierType = carrierType;
                        orderInfo.invoiceCarrierId1 = carrierId1;
                        orderInfo.invoiceCarrierId2 = carrierId2;
                        orderInfo.invoiceDonateCode = donateCode;
                    });
                    twsqlDB.SaveChanges();
                }
            }
        }

        public void SettingItemCategory(List<InsertSalesOrdersBySellerOutput> send, string itemCategories)
        {
            if (string.IsNullOrEmpty(itemCategories))
            {
                return;
            }
            var itemCategoriesList = itemCategories.Split(',').ToList().Select(x => new { itemID = ((x.Split(':')[0] != null) ? x.Split(':')[0].ToString() : "-1").ToString(), categoryID = (x.Split(':')[1] != null ? x.Split(':')[1].ToString() : "-1") }).ToList();
            if (send != null)
            {
                var soNOs = send.Select(x => x.salesorder_code).Distinct().ToList();
                using (TWSqlDBContext twsqlDB = new TWSqlDBContext())
                {
                    var allSOItems = twsqlDB.SalesOrderItem.Where(x => soNOs.Contains(x.SalesorderCode)).ToList();
                    allSOItems.ForEach(orderInfo =>
                    {
                        var itemCategory = itemCategoriesList.Where(x => x.itemID == orderInfo.ItemID.ToString()).FirstOrDefault();
                        var categoryIDString = (itemCategory != null) ? itemCategory.categoryID : "-1";
                        var category = new int();
                        var convert = int.TryParse(categoryIDString, out category);
                        if (!convert)
                        {
                            category = -1;
                        }
                        var itemDB = twsqlDB.Item.Where(x => x.ID == orderInfo.ItemID).FirstOrDefault();
                        if (category == -1 && itemDB != null)
                        {
                            category = itemDB.CategoryID;
                        }
                        orderInfo.itemCategory = category;
                    });
                    twsqlDB.SaveChanges();
                }
            }
        }
    }
}