using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TWNewEgg.DB;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.ItemService.Models;
using TWNewEgg.ItemService.PayTypeService;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemService.Service
{
    public class O2OHiLifeService
    {
        private string recursiveCalculation = System.Configuration.ConfigurationManager.AppSettings["RecursiveCalculation"];
        private string storeDeliverCodes = System.Configuration.ConfigurationManager.AppSettings["StoreDeliverCodes"];
        /// <summary>
        /// 賣場頁配達方式查詢
        /// </summary>
        /// <param name="itemID">賣場ID</param>
        /// <param name="qty">商品數量</param>
        /// <returns>返回查詢結果</returns>
        public List<PaymentandDelivery> getPaymentandDelivery(int itemID, int qty)
        {
            List<PaymentandDelivery> paymentandDeliveryList = new List<PaymentandDelivery>();
            if (itemID == 0)
            {
                return paymentandDeliveryList;
            }

            List<GetItemTaxDetail> promotionGiftActivity = new List<GetItemTaxDetail>();
            List<BuyingItems> buyingItemPostData = new List<BuyingItems>();
            BuyingItems buyingItems = new BuyingItems();
            buyingItems.buyItemID = itemID;
            buyingItems.buyingNumber = qty;
            buyingItemPostData.Add(buyingItems);
            List<PayTypeDeliverInfo> payTypeDeliverInfoList = new List<PayTypeDeliverInfo>();
            payTypeDeliverInfoList = StoreDeliveryCheck(buyingItemPostData, "", promotionGiftActivity);
            // 十二期分期零利率資格判定
            CreditCardPayTypeCheck cardPayTypeCheck = new CreditCardPayTypeCheck();
            // 檢驗是否符合十二期零利率的條件
            bool payCondition = cardPayTypeCheck.CheckPayCondition(buyingItemPostData);
            // 十二期零利率與十二期分期互斥
            if (payCondition)
            {
                // 若符合十二期零利率分期條件
                payTypeDeliverInfoList = payTypeDeliverInfoList.Where(x => x.PayType != (int)PayType.nPayType.十二期分期).ToList();
            }
            else
            {
                // 若不符合十二期零利率分期條件
                payTypeDeliverInfoList = payTypeDeliverInfoList.Where(x => x.PayType != (int)PayType.nPayType.十二期零利率).ToList();
            }

            foreach (PayTypeDeliverInfo pDInfo in payTypeDeliverInfoList)
            {
                // 若可使用宅配
                if (pDInfo.DeliverWay.Delivery == true)
                {
                    PaymentandDelivery delivery = new PaymentandDelivery();
                    delivery.PayTypeID = pDInfo.PayTypeID;
                    delivery.PayType0rateNum = pDInfo.PayType;
                    delivery.DeliverType = (int)Deliver.Type.Delivery;
                    paymentandDeliveryList.Add(delivery);
                }
                // 若可使用店配
                //if (pDInfo.DeliverWay.PickupByStore == true)
                //{
                //    PaymentandDelivery storePickUP = new PaymentandDelivery();
                //    storePickUP.PayTypeID = pDInfo.PayTypeID;
                //    storePickUP.PayType0rateNum = pDInfo.PayType;
                //    storePickUP.DeliverType = (int)Deliver.Type.StorePickUP;
                //    paymentandDeliveryList.Add(storePickUP);
                //}
            }

            return paymentandDeliveryList;
        }

        /// <summary>
        /// 驗證是否符合使用便利商店店配的限制
        /// </summary>
        /// <param name="buyingItemPostData">購物車商品詳細資訊</param>
        /// <param name="strJsonCoupon">購物車使用Coupon張數與金額的JSON格式詳細資訊</param>
        /// <param name="promotionGiftActivity">購物車使用的所有promotion活動資訊</param>
        /// <returns>返回驗證結果詳細清單</returns>
        public List<PayTypeDeliverInfo> StoreDeliveryCheck(List<BuyingItems> buyingItemPostData, string strJsonCoupon, List<GetItemTaxDetail> promotionGiftActivity)
        {
            // 取得所需資訊
            TWSqlDBContext db_before = new TWSqlDBContext();

            List<PayTypeDeliverInfo> paytypeDeliverInfoList = new List<PayTypeDeliverInfo>();
            // 找出所有啟用中的金流付款方式
            List<PayType> payTypeList = db_before.PayType.Where(x => x.Status == (int)PayType.PayTypeStatus.啟動).OrderBy(x => x.PayType0rateNum).ToList();
            // 包含itemID與Qty的Dictionary
            Dictionary<int, int> itemIDQtyList = new Dictionary<int, int>();
            // itemID的清單
            List<int> itemIDList = new List<int>();
            // 購物車內所有Item的詳細資訊
            List<TWNewEgg.DB.TWSQLDB.Models.Item> itemList = new List<TWNewEgg.DB.TWSQLDB.Models.Item>();
            // 購物車內所有SellerID的清單
            List<int> sellerIDList = new List<int>();
            // 商品實際售價金額清單
            List<ItemDisplayPrice> itemDisplayPriceList = new List<ItemDisplayPrice>();
            // 包含Item詳細資訊(Item)與商品購買數量(Qty)的Dictionary
            Dictionary<TWNewEgg.DB.TWSQLDB.Models.Item, int> dicItemAndQty = new Dictionary<TWNewEgg.DB.TWSQLDB.Models.Item, int>();
            // 組合出itemIDQtyList與itemIDList
            foreach (BuyingItems buyingitem in buyingItemPostData)
            {
                // 組合出itemIDQtyList與itemIDList
                itemIDQtyList.Add(buyingitem.buyItemID, buyingitem.buyingNumber);
                itemIDList.Add(buyingitem.buyItemID);
            }
            // 取得所有Item詳細資訊的清單
            itemList = db_before.Item.Where(x => itemIDList.Contains(x.ID)).ToList();
            itemDisplayPriceList = db_before.ItemDisplayPrice.Where(x => itemIDList.Contains(x.ItemID)).ToList();
            // 找出該購物車內所有Item的SellerID清單
            sellerIDList = itemList.Select(x => x.SellerID).Distinct().ToList();
            List<Seller> sellerList = db_before.Seller.Where(x => sellerIDList.Contains(x.ID)).ToList();
            // 組合出dicItemQty
            foreach (var itemIDQty in itemIDQtyList)
            {
                TWNewEgg.DB.TWSQLDB.Models.Item getItem = itemList.Where(x => x.ID == itemIDQty.Key).FirstOrDefault();
                dicItemAndQty.Add(getItem, itemIDQty.Value);
            }
            // 開始邏輯運算
            // 驗證購物車內商品DelivType是否符合規定(超商付款、超商取貨皆需驗證)
            if (!ItemDelivTypeCheck(itemList))
            {
                Dictionary<int, Dictionary<string, bool>> dicDeliverSwitch = DeliverSwitch(false, false, itemList, sellerList);
                paytypeDeliverInfoList = PayTypeDeliverDataCollect(dicDeliverSwitch, payTypeList);
                // 黑白名單處理
                BlackAndWhiteList(itemIDList, ref paytypeDeliverInfoList);
                FilterIllegalPayType(dicItemAndQty, itemIDQtyList, itemDisplayPriceList, ref paytypeDeliverInfoList, strJsonCoupon, promotionGiftActivity);
                return paytypeDeliverInfoList;
            }
            // 驗證購物車內每張LBO總金額是否皆小於20000-HiLife規則(超商付款、超商取貨皆需驗證)
            // 除超商付款其餘信用卡金流方式目前皆只驗證售價金額(itemdisplayprice.displayprice)
            Dictionary<int, Dictionary<string, bool>> successDeliverSwitch = DeliverSwitch(OnlinePayment(itemIDQtyList, itemDisplayPriceList), OfflinePayment(itemIDQtyList, itemDisplayPriceList, strJsonCoupon, promotionGiftActivity), itemList, sellerList);
            paytypeDeliverInfoList = PayTypeDeliverDataCollect(successDeliverSwitch, payTypeList);
            // 驗證購物車內商品的材積重是否符合規定-HiLife規則(超商取貨需驗證)
            if (!ItemArrivalHiLife(dicItemAndQty))
            {
                Dictionary<int, Dictionary<string, bool>> dicDeliverSwitch = DeliverSwitch(false, false, itemList, sellerList);
                paytypeDeliverInfoList = PayTypeDeliverDataCollect(dicDeliverSwitch, payTypeList);
                // 黑白名單處理
                BlackAndWhiteList(itemIDList, ref paytypeDeliverInfoList);
                FilterIllegalPayType(dicItemAndQty, itemIDQtyList, itemDisplayPriceList, ref paytypeDeliverInfoList, strJsonCoupon, promotionGiftActivity);
                return paytypeDeliverInfoList;
            }
            // 黑白名單處理
            BlackAndWhiteList(itemIDList, ref paytypeDeliverInfoList);
            FilterIllegalPayType(dicItemAndQty, itemIDQtyList, itemDisplayPriceList, ref paytypeDeliverInfoList, strJsonCoupon, promotionGiftActivity);
            return paytypeDeliverInfoList;
        }

        /// <summary>
        /// 各項付款方式規則驗證
        /// </summary>
        /// <param name="dicItemAndQty">賣場詳細資料與數量的清單</param>
        /// <param name="itemIDQtyList">賣場ID與數量清單</param>
        /// <param name="itemDisplayPriceList">賣場實際售價清單</param>
        /// <param name="paytypeDeliverInfoList">付款資訊詳細清單</param>
        /// <param name="strJsonCoupon">折價卷資訊</param>
        /// <param name="promotionGiftActivity">活動折價清單</param>
        public void FilterIllegalPayType(Dictionary<TWNewEgg.DB.TWSQLDB.Models.Item, int> dicItemAndQty, Dictionary<int, int> itemIDQtyList, List<ItemDisplayPrice> itemDisplayPriceList, ref List<PayTypeDeliverInfo> paytypeDeliverInfoList, string strJsonCoupon, List<GetItemTaxDetail> promotionGiftActivity)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            decimal priceSum = 0m;
            
            System.Web.Script.Serialization.JavaScriptSerializer javaConvert = new JavaScriptSerializer();
            List<UsedCoupon> listUsedCoupon = new List<UsedCoupon>();
            if (strJsonCoupon != null && strJsonCoupon.Length > 0)
            {
                listUsedCoupon = javaConvert.Deserialize<List<UsedCoupon>>(strJsonCoupon);
            }

            List<PromotionGiftDetail> promotionGiftDetailList = new List<PromotionGiftDetail>();
            foreach (GetItemTaxDetail activity in promotionGiftActivity)
            {
                PromotionGiftDetail subPromotionGift = javaConvert.Deserialize<PromotionGiftDetail>(activity.pricetaxdetail);
                promotionGiftDetailList.Add(subPromotionGift);
            }

            foreach (var itemAndQty in dicItemAndQty)
            {
                // 取得displayPrice
                ItemDisplayPrice itemDisplayPrice = itemDisplayPriceList.Where(x => x.ItemID == itemAndQty.Key.ID).FirstOrDefault();
                // 累積總金額
                priceSum += itemDisplayPrice.DisplayPrice * itemAndQty.Value;

                // 取得CouponPrice，並扣除CouponPrice折抵金額
                if (listUsedCoupon.Count > 0)
                {
                    UsedCoupon getUsedCoupon = listUsedCoupon.Where(x => x.ItemId == itemAndQty.Key.ID).FirstOrDefault();
                    // 因為此function是緊接在UsedCoupon組成後就立即取得資訊，不會有被使用者修改的疑慮才直接取Str內的值，否則要使用下方方式取值才不會出現被使用者修改的問題
                    getUsedCoupon.Coupons.ForEach(x => priceSum -= x.Value);
                }

                // 取得promotionPrice，並扣除promotionPrice折抵金額
                priceSum -= PromotionPriceAnalysis(itemAndQty.Key.ID, itemIDQtyList, itemDisplayPriceList, promotionGiftDetailList);
            }

            // 各項付款方式規則驗證
            foreach (var itemAndQty in dicItemAndQty)
            {
                // 貨到付款規則驗證
                if (itemAndQty.Key.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.切貨)
                {
                    List<PayTypeDeliverInfo> searchPayTypeDeliverInfoList = paytypeDeliverInfoList.Where(x => x.PayType == (int)PayType.nPayType.貨到付款).ToList();
                    if (searchPayTypeDeliverInfoList != null)
                    {
                        foreach(PayTypeDeliverInfo subInfo in searchPayTypeDeliverInfoList)
                        {
                            paytypeDeliverInfoList.Remove(subInfo);
                        }
                    }
                    if (itemAndQty.Key.DelvType == (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.間配 || itemAndQty.Key.DelvType == (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.三角)
                    {
                        List<PayTypeDeliverInfo> searchATMList = paytypeDeliverInfoList.Where(x => x.PayType == (int)PayType.nPayType.實體ATM).ToList();
                        if (searchATMList != null)
                        {
                            foreach (PayTypeDeliverInfo subInfo in searchATMList)
                            {
                                paytypeDeliverInfoList.Remove(subInfo);
                            }
                        }
                    }
                }
                else
                {
                    if (priceSum >= 50000m)
                    {
                        List<PayTypeDeliverInfo> searchPayTypeDeliverInfoList = paytypeDeliverInfoList.Where(x => x.PayType == (int)PayType.nPayType.貨到付款).ToList();
                        if (searchPayTypeDeliverInfoList != null)
                        {
                            foreach (PayTypeDeliverInfo subInfo in searchPayTypeDeliverInfoList)
                            {
                                paytypeDeliverInfoList.Remove(subInfo);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 處理黑白名單
        /// </summary>
        /// <param name="itemIDList">賣場ID清單</param>
        /// <param name="paytypeDeliverInfoList">驗證是否符合使用超商付款或超商店配的詳細資訊清單</param>
        public void BlackAndWhiteList(List<int> itemIDList, ref List<PayTypeDeliverInfo> paytypeDeliverInfoList)
        {
            // 取得所需資訊
            TWSqlDBContext db_before = new TWSqlDBContext();
            List<ItemDeliverBlack> itemDeliverBlackList = db_before.ItemDeliverBlack.Where(x => itemIDList.Contains(x.ItemID) && x.IsEnable == (int)ItemDeliverBlack.EnableStatus.啟用).ToList();
            List<ItemDeliverWhite> itemDeliverWhiteList = db_before.ItemDeliverWhite.Where(x => itemIDList.Contains(x.ItemID) && x.IsEnable == (int)ItemDeliverWhite.EnableStatus.啟用).ToList();
            // 白名單處理
            foreach (ItemDeliverWhite itemDeliverWhite in itemDeliverWhiteList)
            {
                PayTypeDeliverInfo searchPayTypeDeliverInfo = paytypeDeliverInfoList.Where(x => x.PayTypeID == itemDeliverWhite.PayTypeID).FirstOrDefault();
                if (searchPayTypeDeliverInfo != null)
                {
                    // 宅配
                    if (itemDeliverWhite.DeliverType == (int)Deliver.Type.Delivery)
                    {
                        searchPayTypeDeliverInfo.DeliverWay.Delivery = true;
                        if (itemDeliverWhite.DeliverCode == (int)Deliver.ShippingCompany.HCT新竹貨運)
                        {
                            searchPayTypeDeliverInfo.Deliver.HCT = true;
                            searchPayTypeDeliverInfo.DeliverCode = (int)Deliver.ShippingCompany.HCT新竹貨運;
                        }
                    }
                    else
                    {
                        searchPayTypeDeliverInfo.DeliverWay.PickupByStore = true;
                        // 店配
                        if (itemDeliverWhite.DeliverCode == (int)Deliver.ConvenienceStoreCode.SevenEleven)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.SevenEleven = true;
                            searchPayTypeDeliverInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.SevenEleven;
                        }
                        if (itemDeliverWhite.DeliverCode == (int)Deliver.ConvenienceStoreCode.FamilyMart)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.FamilyMart = true;
                            searchPayTypeDeliverInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.FamilyMart;
                        }
                        if (itemDeliverWhite.DeliverCode == (int)Deliver.ConvenienceStoreCode.HiLife)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.HiLife = true;
                            searchPayTypeDeliverInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.HiLife;
                        }
                        if (itemDeliverWhite.DeliverCode == (int)Deliver.ConvenienceStoreCode.OKMart)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.OKMart = true;
                            searchPayTypeDeliverInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.OKMart;
                        }
                    }
                }
                else
                {
                }
            }
            // 黑名單處理
            foreach (ItemDeliverBlack itemDeliverBlack in itemDeliverBlackList)
            {
                PayTypeDeliverInfo searchPayTypeDeliverInfo = paytypeDeliverInfoList.Where(x => x.PayTypeID == itemDeliverBlack.PayTypeID).FirstOrDefault();
                if (searchPayTypeDeliverInfo != null)
                {
                    // 宅配
                    if (itemDeliverBlack.DeliverType == (int)Deliver.Type.Delivery)
                    {
                        searchPayTypeDeliverInfo.DeliverWay.Delivery = false;
                        if (itemDeliverBlack.DeliverCode == (int)Deliver.ShippingCompany.HCT新竹貨運)
                        {
                            searchPayTypeDeliverInfo.Deliver.HCT = false;
                        }
                    }
                    else
                    {
                        searchPayTypeDeliverInfo.DeliverWay.PickupByStore = false;
                        // 店配
                        if (itemDeliverBlack.DeliverCode == (int)Deliver.ConvenienceStoreCode.SevenEleven)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.SevenEleven = false;
                        }
                        if (itemDeliverBlack.DeliverCode == (int)Deliver.ConvenienceStoreCode.FamilyMart)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.FamilyMart = false;
                        }
                        if (itemDeliverBlack.DeliverCode == (int)Deliver.ConvenienceStoreCode.HiLife)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.HiLife = false;
                        }
                        if (itemDeliverBlack.DeliverCode == (int)Deliver.ConvenienceStoreCode.OKMart)
                        {
                            searchPayTypeDeliverInfo.ConvenienceStore.OKMart = false;
                        }
                    }
                    searchPayTypeDeliverInfo.DeliverCode = 0;
                }
                else
                {
                }
            }
            //return paytypeDeliverInfoList;
        }

        /// <summary>
        /// 控制配達配達方式有哪些
        /// </summary>
        /// <returns>返回配達方式清單</returns>
        public List<int> InsertDeliverCodeList()
        {
            List<int> deliverCodeList = new List<int>();
            storeDeliverCodes.Split(',').ToList().ForEach(x => {
                int newDeliverCode = Convert.ToInt32(x);
                deliverCodeList.Add(newDeliverCode);
            });

            return deliverCodeList;
        }

        /// <summary>
        /// 設定配達方式的開啟與關閉
        /// </summary>
        /// <param name="onlinePayment">線上付款是否可使用HiLife店配</param>
        /// <param name="offlinePayment">HiLife是否可使用線下付款</param>
        /// <param name="itemList">賣場詳細資訊清單</param>
        /// <param name="sellerList">Seller詳細資訊清單</param>
        /// <returns>返回配達方式開關資訊</returns>
        public Dictionary<int, Dictionary<string, bool>> DeliverSwitch(bool onlinePayment, bool offlinePayment, List<TWNewEgg.DB.TWSQLDB.Models.Item> itemList, List<Seller> sellerList)
        {
            // onlinePayment、offlinePayment如果之後要新增7-11、FamilyMart...等等的話，要再做修改，目前僅用於HiLife
            Dictionary<int, Dictionary<string, bool>> switchResult = new Dictionary<int, Dictionary<string, bool>>();
            // 7-11，FamilyMart，OK暫時先不加入，之後若有談妥再行加入
            //bool sevenEleven = true;
            //bool familyMart = true;
            bool hiLifeItem = true;
            //bool oKMart = true;
            bool neweggItem = true;
            bool sellerVendorItem = true;
            foreach (TWNewEgg.DB.TWSQLDB.Models.Item item in itemList)
            {
                if (item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.切貨
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.自貿區
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.MKPL寄倉
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2c寄倉)
                {
                    neweggItem = false;
                }
                if (item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.直配)
                {
                    Seller searchSeller = sellerList.Where(x => x.ID == item.SellerID).FirstOrDefault();
                    // 若商品非HiLife商品時
                    if (searchSeller.ID != 167)
                    {
                        hiLifeItem = false;
                    }
                    //if (searchSeller.ID != xx)
                    //{
                    //    // 非7-11商品
                    //    sevenEleven = false;
                    //}
                    //if (searchSeller.ID != xx)
                    //{
                    //    // 非全家商品
                    //    familyMart = false;
                    //}
                    //if (searchSeller.ID != xx)
                    //{
                    //    // 非OK商品
                    //    oKMart = false;
                    //}
                    sellerVendorItem = false;
                }
            }

            List<int> deliverCodeList = InsertDeliverCodeList();
            
            // 當商品皆為新蛋商品時
            if (neweggItem == true)
            {
                foreach (int deliverCode in deliverCodeList)
                {
                    Dictionary<string, bool> subDeliverStatus = new Dictionary<string, bool>();
                    subDeliverStatus.Add("OnlinePayment", onlinePayment);
                    subDeliverStatus.Add("OfflinePayment", offlinePayment);
                    switchResult.Add(deliverCode, subDeliverStatus);
                }
            }
            else if (hiLifeItem == true)
            {
                // 當商品皆為hiLife商品時
                foreach (int deliverCode in deliverCodeList)
                {
                    Dictionary<string, bool> subDeliverStatus = new Dictionary<string, bool>();
                    if (deliverCode == (int)Deliver.ConvenienceStoreCode.HiLife || deliverCode == (int)Deliver.ShippingCompany.HCT新竹貨運)
                    {
                        subDeliverStatus.Add("OnlinePayment", onlinePayment);
                        subDeliverStatus.Add("OfflinePayment", offlinePayment);
                    }
                    else
                    {
                        subDeliverStatus.Add("OnlinePayment", false);
                        subDeliverStatus.Add("OfflinePayment", false);
                    }

                    switchResult.Add(deliverCode, subDeliverStatus);
                }
            }
            else if (sellerVendorItem == true)
            {
                // 當商品皆為Seller&Vendor商品時
                foreach (int deliverCode in deliverCodeList)
                {
                    Dictionary<string, bool> subDeliverStatus = new Dictionary<string, bool>();
                    if (deliverCode == (int)Deliver.ShippingCompany.HCT新竹貨運)
                    {
                        subDeliverStatus.Add("OnlinePayment", onlinePayment);
                        subDeliverStatus.Add("OfflinePayment", offlinePayment);
                    }
                    else
                    {
                        subDeliverStatus.Add("OnlinePayment", false);
                        subDeliverStatus.Add("OfflinePayment", false);
                    }

                    switchResult.Add(deliverCode, subDeliverStatus);
                }
            }
            else
            {
                // 當有商品非全部為neweggItem或hiLifeItem或sellerVendorItem或為DelivType = 1 (間配)時
                foreach (int deliverCode in deliverCodeList)
                {
                    Dictionary<string, bool> subDeliverStatus = new Dictionary<string, bool>();
                    if (deliverCode == (int)Deliver.ShippingCompany.HCT新竹貨運)
                    {
                        subDeliverStatus.Add("OnlinePayment", onlinePayment);
                        subDeliverStatus.Add("OfflinePayment", offlinePayment);
                    }
                    else
                    {
                        subDeliverStatus.Add("OnlinePayment", false);
                        subDeliverStatus.Add("OfflinePayment", false);
                    }

                    switchResult.Add(deliverCode, subDeliverStatus);
                }
            }

            return switchResult;
        }

        /// <summary>
        /// 取得金流與物流可用詳細資訊
        /// </summary>
        /// <param name="deliverStatus">配達方式的開啟與關閉狀態</param>
        /// <param name="payTypeList">啟用中的金流清單</param>
        /// <returns>返回金流與物流可用詳細資訊</returns>
        public List<PayTypeDeliverInfo> PayTypeDeliverDataCollect(Dictionary<int, Dictionary<string, bool>> deliverStatus, List<PayType> payTypeList)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            TWBackendDBContext db_after = new TWBackendDBContext();
            List<PayTypeDeliverInfo> dataCollect = new List<PayTypeDeliverInfo>();
            List<int> bankIDList = payTypeList.Select(x => (int)x.BankID).Distinct().ToList();
            List<Bank> bankList = db_before.Bank.Where(x => bankIDList.Contains(x.ID)).ToList();
            foreach (PayType subPayType in payTypeList)
            {
                PayTypeDeliverInfo pDInfo = new PayTypeDeliverInfo();
                pDInfo.PayTypeID = subPayType.ID;
                pDInfo.PayType = (int)subPayType.PayType0rateNum;
                if (subPayType.PayType0rateNum == (int)PayType.nPayType.超商付款)
                {
                    #region 超商付款
                    // 此條件用於確認該金流所指定的取貨超商為何
                    string bankCode = bankList.Where(x => x.ID == subPayType.BankID).Select(x => x.Code).FirstOrDefault();
                    foreach (var subDic in deliverStatus)
                    {
                        if (bankCode == ((int)Bank.BankCode.SevenEleven).ToString() && subDic.Key == (int)Deliver.ConvenienceStoreCode.SevenEleven)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                // 因為金流"超商付款"為線下付款，所以使用OfflinePayment來做判斷
                                if (boolResult.Key == "OfflinePayment" && boolResult.Value == true)
                                {
                                    pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.SevenEleven;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.SevenEleven = true;
                                }
                            }
                            // Model 3 在此需要加入是否可使用宅配
                            // 條件 + pDInfo.DeliverWay.Delivery = true; pDInfo.Deliver.HCT = true;
                        }
                        else if (bankCode == ((int)Bank.BankCode.FamilyMart).ToString() && subDic.Key == (int)Deliver.ConvenienceStoreCode.FamilyMart)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OfflinePayment" && boolResult.Value == true)
                                {
                                    pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.FamilyMart;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.FamilyMart = true;
                                }
                            }
                            // Model 3 在此需要加入是否可使用宅配
                            // 條件 + pDInfo.DeliverWay.Delivery = true; pDInfo.Deliver.HCT = true;
                        }
                        else if (bankCode == ((int)Bank.BankCode.HiLife).ToString() && subDic.Key == (int)Deliver.ConvenienceStoreCode.HiLife)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OfflinePayment" && boolResult.Value == true)
                                {
                                    pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.HiLife;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.HiLife = true;
                                }
                            }
                            // Model 3 在此需要加入是否可使用宅配
                            // 條件 + pDInfo.DeliverWay.Delivery = true; pDInfo.Deliver.HCT = true;
                        }
                        else if (bankCode == ((int)Bank.BankCode.OKMart).ToString() && subDic.Key == (int)Deliver.ConvenienceStoreCode.OKMart)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OfflinePayment" && boolResult.Value == true)
                                {
                                    pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.OKMart;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.OKMart = true;
                                }
                            }
                            // Model 3 在此需要加入是否可使用宅配
                            // 條件 + pDInfo.DeliverWay.Delivery = true; pDInfo.Deliver.HCT = true;
                        }
                        // 超商付款Model 1暫時不開放宅配
                        pDInfo.DeliverWay.Delivery = false;
                        pDInfo.Deliver.HCT = false;
                    }
                    #endregion 超商付款
                }
                else if (subPayType.PayType0rateNum == (int)PayType.nPayType.貨到付款)
                {
                    #region 貨到付款
                    pDInfo.DeliverCode = (int)Deliver.ShippingCompany.HCT新竹貨運;
                    pDInfo.DeliverWay.Delivery = true;
                    pDInfo.Deliver.HCT = true;
                    #endregion 貨到付款
                }
                else if (subPayType.PayType0rateNum == (int)PayType.nPayType.電匯)
                {
                    #region 電匯
                    pDInfo.DeliverCode = (int)Deliver.ShippingCompany.HCT新竹貨運;
                    pDInfo.DeliverWay.Delivery = true;
                    pDInfo.Deliver.HCT = true;
                    #endregion 電匯
                }
                else if (subPayType.PayType0rateNum == (int)PayType.nPayType.實體ATM)
                {
                    #region 實體ATM
                    #endregion 實體ATM
                }
                else
                {
                    #region 線上付款
                    foreach (var subDic in deliverStatus)
                    {
                        // 預設
                        pDInfo.DeliverCode = (int)Deliver.ShippingCompany.HCT新竹貨運;
                        if (subDic.Key == (int)Deliver.ConvenienceStoreCode.SevenEleven)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OnlinePayment" && boolResult.Value == true)
                                {
                                    //pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.SevenEleven;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.SevenEleven = true;
                                }
                            }
                        }
                        else if (subDic.Key == (int)Deliver.ConvenienceStoreCode.FamilyMart)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OnlinePayment" && boolResult.Value == true)
                                {
                                    //pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.FamilyMart;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.FamilyMart = true;
                                }
                            }
                        }
                        else if (subDic.Key == (int)Deliver.ConvenienceStoreCode.HiLife)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OnlinePayment" && boolResult.Value == true)
                                {
                                    //pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.HiLife;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.HiLife = true;
                                }
                            }
                        }
                        else if (subDic.Key == (int)Deliver.ConvenienceStoreCode.OKMart)
                        {
                            foreach (var boolResult in subDic.Value)
                            {
                                if (boolResult.Key == "OnlinePayment" && boolResult.Value == true)
                                {
                                    //pDInfo.DeliverCode = (int)Deliver.ConvenienceStoreCode.OKMart;
                                    pDInfo.DeliverWay.PickupByStore = true;
                                    pDInfo.ConvenienceStore.OKMart = true;
                                }
                            }
                        }
                        else if (subDic.Key == (int)Deliver.ShippingCompany.HCT新竹貨運)
                        {
                            //foreach (var boolResult in subDic.Value)
                            //{
                            //    if (boolResult.Key == "OnlinePayment" && boolResult.Value == true)
                            //    {
                            //        pDInfo.DeliverWay.Delivery = true;
                            //        pDInfo.Deliver.HCT = true;
                            //    }
                            //}
                        }

                        //dataCollect.Add(pDInfo);
                    }
                    #endregion 線上付款
                }

                dataCollect.Add(pDInfo);
            }
            // 返回所有商品可共同使用的配送方式
            return dataCollect;
        }

        /// <summary>
        /// 取聯集
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="source">來源清單</param>
        /// <param name="target">目標清單</param>
        /// <returns>返回聯集結果</returns>
        public List<T> Merge<T>(List<T> source, List<T> target)
        {
            List<T> mergedList = new List<T>(source);
            mergedList.AddRange(target.Except(source));
            return mergedList;
        }

        /// <summary>
        /// 將Deliver.Code與Deliver.DeliverType的string拆解開來
        /// </summary>
        /// <param name="JsonDeliverCodeList">Deliver.Code與Deliver.Type的String格式</param>
        /// <returns>返回拆解後的結果Deliver.Type為Key與Deliver.Code List為Value的Dictionary</returns>
        public Dictionary<int, List<int>> TakeApart(string strDeliverCodeList)
        {
            Dictionary<int, List<int>> deliverTakeApart = new Dictionary<int, List<int>>();
            Dictionary<int, int> tempTakeApart = new Dictionary<int, int>();
            List<int> listDeliverType = new List<int>();
            // 拆解string
            strDeliverCodeList.Split(',').ToList().ForEach(x =>
            {
                int deliverCode = Convert.ToInt32(x.Split(':')[0]);
                int deliverType = Convert.ToInt32(x.Split(':')[1]);
                listDeliverType.Add(deliverType);
                tempTakeApart.Add(deliverType, deliverCode);
            });

            listDeliverType = listDeliverType.Distinct().ToList();
            // 填入deliverTakeApart中
            foreach (int subDeliverType in listDeliverType)
            {
                List<int> tempDeliverCode = new List<int>();
                foreach (var subTemp in tempTakeApart)
                {
                    if (subTemp.Key == subDeliverType)
                    {
                        tempDeliverCode.Add(subTemp.Value);
                    }
                }
                // Deliver.Type為Key,Deliver.Code List為Value
                deliverTakeApart.Add(subDeliverType, tempDeliverCode);
            }

            return deliverTakeApart;
        }

        /// <summary>
        /// 購物車中所有賣場售價金額是否符合使用線上付款超商取貨的金額限制條件
        /// </summary>
        /// <param name="itemIDQtyList">賣場ID與數量的Dictionary</param>
        /// <param name="itemDisplayPriceList">賣場實際售價清單</param>
        /// <returns>符合則返回true，否則返回false</returns>
        public bool OnlinePayment(Dictionary<int, int> itemIDQtyList, List<ItemDisplayPrice> itemDisplayPriceList)
        {
            foreach (var itemAndQty in itemIDQtyList)
            {
                ItemDisplayPrice priceCheck = itemDisplayPriceList.Where(x => x.ItemID == itemAndQty.Key).FirstOrDefault();
                decimal totalItemPrice = 0m;
                if (priceCheck != null)
                {
                    totalItemPrice = priceCheck.DisplayPrice * itemAndQty.Value;
                }
                // HiLife規則
                if (totalItemPrice > 20000m)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 購物車中所有賣場售價金額是否符合使用線下超商付款的金額限制條件
        /// </summary>
        /// <param name="itemIDQtyList">賣場ID與數量的Dictionary</param>
        /// <param name="itemDisplayPriceList">賣場實際售價清單</param>
        /// <param name="strJsonCoupon">購物車使用Coupon張數與金額的JSON格式詳細資訊</param>
        /// <param name="promotionGiftActivity">購物車使用的所有promotion活動資訊</param>
        /// <returns>符合則返回true，否則返回false</returns>
        public bool OfflinePayment(Dictionary<int, int> itemIDQtyList, List<ItemDisplayPrice> itemDisplayPriceList, string strJsonCoupon, List<GetItemTaxDetail> promotionGiftActivity)
        {
            // strJsonCoupon 格式
            //[
            //{"ItemId":"101719","BuyNumber":"3","Coupons":[{"CouponId":"46242", "Value":"100.00", "Categoryies":";0;"},{"CouponId":"46240", "Value":"200.00", "Categoryies":";0;"}]},
            //{"ItemId":"85127","BuyNumber":"5","Coupons":[{"CouponId":"46246", "Value":"100.00", "Categoryies":";0;"},{"CouponId":"46232", "Value":"500.00", "Categoryies":";0;"},{"CouponId":"46231", "Value":"500.00", "Categoryies":";0;"}]},
            //{"ItemId":"90413","BuyNumber":"1","Coupons":[]},{"ItemId":"96598","BuyNumber":"5","Coupons":[{"CouponId":"46229", "Value":"500.00", "Categoryies":";0;"},{"CouponId":"46228", "Value":"500.00", "Categoryies":";0;"}]},{"ItemId":"101715","BuyNumber":"1","Coupons":[]}
            //]
            // promotionGiftActivity 格式
            //{"PromotionGiftBasicID":5,"Priority":1,"Description":"買一送一","ShowDesc":"買一送一優惠","ReferencesList":"white","AcceptedItems":[3,39299,74513],"NotAcceptedItems":null,"PromotionGiftIntervalID":4,"ApportionedAmount":5188,"PromotionGiftBasicStartDate":"\/Date(1412006400000)\/","CSS":"cartDiscount bMcW","HighLight":"即日起 至10/13 (一) 11:00止"}
            TWSqlDBContext db_before = new TWSqlDBContext();
            System.Web.Script.Serialization.JavaScriptSerializer javaConvert = new JavaScriptSerializer();
            List<UsedCoupon> listUsedCoupon = new List<UsedCoupon>();
            if (strJsonCoupon != null && strJsonCoupon.Length > 0)
            {
                listUsedCoupon = javaConvert.Deserialize<List<UsedCoupon>>(strJsonCoupon);
            }

            List<PromotionGiftDetail> promotionGiftDetailList = new List<PromotionGiftDetail>();
            foreach (GetItemTaxDetail activity in promotionGiftActivity)
            {
                PromotionGiftDetail subPromotionGift = javaConvert.Deserialize<PromotionGiftDetail>(activity.pricetaxdetail);
                promotionGiftDetailList.Add(subPromotionGift);
            }
            // 限定金額 = displayPrice(實際售價金額) - CouponPrice - promotionPrice 
            decimal checkPrice = 0m;
            foreach (var itemAndQty in itemIDQtyList)
            {
                checkPrice = 0m;
                // 取得displayPrice
                ItemDisplayPrice itemDisplayPrice = itemDisplayPriceList.Where(x => x.ItemID == itemAndQty.Key).FirstOrDefault();
                if (itemDisplayPrice != null)
                {
                    checkPrice = itemDisplayPrice.DisplayPrice * itemAndQty.Value;
                }
                // 取得CouponPrice，並扣除CouponPrice折抵金額
                if (listUsedCoupon.Count > 0)
                {
                    UsedCoupon getUsedCoupon = listUsedCoupon.Where(x => x.ItemId == itemAndQty.Key).FirstOrDefault();
                    // 因為此function是緊接在UsedCoupon組成後就立即取得資訊，不會有被使用者修改的疑慮才直接取Str內的值，否則要使用下方方式取值才不會出現被使用者修改的問題
                    getUsedCoupon.Coupons.ForEach(x => checkPrice -= x.Value);
                }
                // 取得promotionPrice，並扣除promotionPrice折抵金額
                checkPrice -= PromotionPriceAnalysis(itemAndQty.Key, itemIDQtyList, itemDisplayPriceList, promotionGiftDetailList);
                // 檢驗限定金額是否小於等於20000(HiLife規則)
                if (checkPrice > 20000m)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 解析該Item所分配到的PromotionGiftPrice金額
        /// </summary>
        /// <param name="itemID">賣場ID</param>
        /// <param name="itemIDQtyList">賣場ID與商品數量的Dictionary</param>
        /// <param name="itemDisplayPriceList">該購物車內所有ItemDisplayPrice的清單</param>
        /// <param name="promotionGiftDetailList">該購物車所使用到的所有活動資訊清單</param>
        /// <returns>返回分配的活動折抵金額</returns>
        public decimal PromotionPriceAnalysis(int itemID, Dictionary<int, int> itemIDQtyList, List<ItemDisplayPrice> itemDisplayPriceList, List<PromotionGiftDetail> promotionGiftDetailList)
        {
            TWSqlDBContext db_before = new TWSqlDBContext();
            // 商品售價總金額
            decimal totalDisplayPrice = 0m;
            ItemDisplayPrice findItemDisplayPrice = itemDisplayPriceList.Where(x => x.ItemID == itemID).FirstOrDefault();
            int getQty = itemIDQtyList.Where(x => x.Key == itemID).FirstOrDefault().Value;
            if (findItemDisplayPrice != null)
            {
                totalDisplayPrice = itemDisplayPriceList.Where(x => x.ItemID == itemID).FirstOrDefault().DisplayPrice * getQty;
            }
            // 分配到的活動金額
            decimal apportionPromotionPrice = 0m;
            // 該活動內所有商品售價總金額
            decimal totalItemDisplayPrice = 0m;
            promotionGiftDetailList.ForEach(promotionGiftDetail =>
            {
                // 歸0該活動內所有商品售價總金額
                totalItemDisplayPrice = 0m;
                PromotionGiftInterval promotionGiftInterval = db_before.PromotionGiftInterval.Where(interval => interval.ID == promotionGiftDetail.PromotionGiftIntervalID).FirstOrDefault();
                // 檢查此活動內是否包還該ItemID，如果沒有就不需繼續解析
                int findItemFromPromtionGift = promotionGiftDetail.AcceptedItems.Where(accepted => accepted == itemID).FirstOrDefault();
                if (promotionGiftInterval != null && findItemFromPromtionGift != 0)
                {
                    // 直接折扣金額
                    if (promotionGiftInterval.DiscountAmount > 0m && promotionGiftInterval.DiscountPercent == 0m)
                    {
                        // 取出該活動內所有Item的商品售價資訊
                        List<ItemDisplayPrice> listItemDisplayPrice = itemDisplayPriceList.Where(subItemDP => promotionGiftDetail.AcceptedItems.Contains(subItemDP.ItemID)).ToList();
                        // 取得該活動內所有商品售價總金額
                        listItemDisplayPrice.ForEach(subIDP =>
                        {
                            int findItemQty = itemIDQtyList.Where(itemQty => itemQty.Key == subIDP.ItemID).FirstOrDefault().Value;
                            totalItemDisplayPrice += subIDP.DisplayPrice * findItemQty;
                        });
                        // 若該商品是重複參加不同活動，則扣除已執行過的活動優惠金額
                        if (apportionPromotionPrice != 0m)
                        {
                            totalItemDisplayPrice -= apportionPromotionPrice;
                        }
                        // 取得分配金額後再四捨五入
                        apportionPromotionPrice += Math.Floor(0.5m + ((totalDisplayPrice / totalItemDisplayPrice) * promotionGiftInterval.DiscountAmount));
                    }
                    // 折抵百分比
                    if (promotionGiftInterval.DiscountAmount == 0m && promotionGiftInterval.DiscountPercent > 0m)
                    {
                        // 取得分配金額後再四捨五入
                        apportionPromotionPrice += Math.Floor(0.5m + totalDisplayPrice * (1 - (promotionGiftInterval.DiscountPercent / 100)));
                    }
                    // 若商品可重複參加活動時，則扣除已計算過的活動折抵後再做後續計算
                    if (recursiveCalculation.ToLower() == "on")
                    {
                        totalDisplayPrice -= apportionPromotionPrice;
                    }
                }
            });

            return apportionPromotionPrice;
        }

        /// <summary>
        /// 判斷購物車商品的交易模式是否符合使用便利商店店配的規定(目前僅0、2、5、7、8、9交易模式可供使用)(交易模式6因為與交易模式3在同一車暫不列入考慮，合併購物車後可加入)
        /// </summary>
        /// <param name="itemList">購物車商品詳細資訊</param>
        /// <returns>若可使用店配則返回true否則返回false</returns>
        public bool ItemDelivTypeCheck(List<TWNewEgg.DB.TWSQLDB.Models.Item> itemList)
        {
            foreach (TWNewEgg.DB.TWSQLDB.Models.Item item in itemList)
            {
                if (item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.切貨
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.直配
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.自貿區
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2C直配
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.MKPL寄倉
                    && item.DelvType != (int)TWNewEgg.DB.TWSQLDB.Models.Item.tradestatus.B2c寄倉)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判斷是否可以使用萊爾富配送
        /// </summary>
        /// <param name="dicItem">是一個 Dictionary 存放使用者所買的 item 跟數量</param>
        /// <returns>回傳 T:可配送、F：不可配送</returns>
        public bool ItemArrivalHiLife(Dictionary<TWNewEgg.DB.TWSQLDB.Models.Item, int> dicItem)
        {
            TWNewEgg.DB.TWSqlDBContext db = new DB.TWSqlDBContext();
            List<int> itemIdList = new List<int>();
            List<int> productIdList = new List<int>();
            //利用 itemIdList 抓取相對應的 Item 資料
            foreach (var item in dicItem.Keys)
            {
                itemIdList.Add(item.ID);
            }

            var getProductDataByItmeid = db.Item.Where(p => itemIdList.Contains(p.ID)).ToList();
            productIdList = getProductDataByItmeid.Select(q => q.ProductID).ToList();
            var productList = db.Product.Where(p => productIdList.Contains(p.ID)).ToList();

            foreach (var item in dicItem)
            {
                int itemIdTemp = item.Key.ID;
                int qty = item.Value;
                int productid = getProductDataByItmeid.Where(p => p.ID == itemIdTemp).Select(q => q.ProductID).FirstOrDefault();
                var productInfo = productList.Where(p => p.ID == productid).FirstOrDefault();
                //Product 的 Length, Width, Height 相加的總和成以數量不可以超過 90 公分
                if ((productInfo.Length + productInfo.Width + productInfo.Height) * qty > 90)
                {
                    return false;
                }
                //Product 的 Length, Width, Height 個別的值成以數量不可以大於 45 公分
                if ((productInfo.Length * qty > 45) || productInfo.Width * qty > 45 || productInfo.Height * qty > 45)
                {
                    return false;
                }
                //Product 的重量成以數量不可以大於 5 公斤
                if (productInfo.Weight * qty > 5)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
