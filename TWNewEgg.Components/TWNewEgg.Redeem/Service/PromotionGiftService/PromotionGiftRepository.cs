using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
/*
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;

using TWNewEgg.Redeem.Model.PromotionGiftExport;
 */
using TWNewEgg.Models.DBModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.PromotionRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;
//using TWNewEgg.StoredProceduresRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.ShoppingCartServices.Interface;

namespace TWNewEgg.Redeem.Service.PromotionGiftService
{
    public class PromotionGiftRepository : IPromotionGiftService
    {
        private string recursiveCalculation = System.Configuration.ConfigurationManager.AppSettings["RecursiveCalculation"];
        private string discoloration = System.Configuration.ConfigurationManager.AppSettings["Discoloration"];
        private string twoForOneOffer = System.Configuration.ConfigurationManager.AppSettings["TwoForOneOffer"];
        private IPromotionRepoAdapter _PromotionRepo = null;
        private IItemDetailService _ItemDetailService = null;
        private IPromotionRecordsRepoAdapter _PromotionRecRepo = null;
        private ISORepoAdapter _SoRepo = null;
        //private ICartRepoAdapter _CartRepo = null;
        private IPromotionGiftRecordService _PromotionRecordService = null;
        private IItemRepoAdapter _ItemRepo = null;
        private IItemStockService _ItemStockService = null;
        private IItemDisplayPriceRepoAdapter _ItemDisplayPriceRepo = null;
        private IShoppingCartService _IShoppingCartService = null;
        //private IStoredProceduresRepoAdapter _IStoredProceduresRepoAdapter = null;

        public PromotionGiftRepository(IPromotionRepoAdapter argObjPromotionRepo
            , IItemDetailService argItemServices
            , IPromotionRecordsRepoAdapter argObjPromotionRecRepo
            , ISORepoAdapter argObjSoRepo
            , IPromotionGiftRecordService argObjGiftRecService
            , IItemRepoAdapter argObjItemRepo
            , IItemStockService argObjItemStockService
            , IItemDisplayPriceRepoAdapter argObjItemDisplayPriceRepo
            , IShoppingCartService argObjShoppingCartService
            //, IStoredProceduresRepoAdapter argStoredProceduresRepoAdapter
            )
        {
            this._PromotionRepo = argObjPromotionRepo;
            this._ItemDetailService = argItemServices;
            this._PromotionRecRepo = argObjPromotionRecRepo;
            this._SoRepo = argObjSoRepo;
            this._PromotionRecordService = argObjGiftRecService;
            this._ItemRepo = argObjItemRepo;
            this._ItemStockService = argObjItemStockService;
            this._ItemDisplayPriceRepo = argObjItemDisplayPriceRepo;
            this._IShoppingCartService = argObjShoppingCartService;
            //this._IStoredProceduresRepoAdapter = argStoredProceduresRepoAdapter;
        }

        /// <summary>
        /// 解析滿額贈資訊與返回符合的賣場ID與折扣金額
        /// </summary>
        /// <param name="oriPriceData">商品原始金額</param>
        /// <param name="turnOn">是否啟用正式機狀態的開關閥，正式機on、測試機off</param>
        /// <returns>返回符合滿額贈的賣場ID與折扣金額</returns>
        public List<Models.DomainModels.Redeem.GetItemTaxDetail> PromotionGiftParsing(int accountID, Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData, string turnOn)
        {
            turnOn = turnOn.ToLower();
            List<Models.DomainModels.Redeem.GetItemTaxDetail> promationGiftDetail = null;
            // 購物車中商品itemID與金額
            Dictionary<int, decimal> itemPriceData = null;
            promationGiftDetail = new List<GetItemTaxDetail>();
            itemPriceData = new Dictionary<int, decimal>();
            foreach (var subOriPriceDate in oriPriceData)
            {
                subOriPriceDate.Value.ForEach(x => itemPriceData.Add(x.item_id, Convert.ToDecimal(x.pricetaxdetail.Split(',')[12])));
            }

            // 是否啟用正式機狀態的開關閥，正式機on、測試機off
            if (turnOn == "on")
            {
                promationGiftDetail = this.GetPromotionGiftFromItems(accountID, itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.Used, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.Used, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.Used);
            }
            else
            {
                promationGiftDetail = this.GetPromotionGiftFromItems(accountID, itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.TempUsed, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.TempUsed, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.TempUsed);
            }

            return promationGiftDetail;
        }

        /// <summary>
        /// 解析滿額贈資訊與返回符合的賣場ID與折扣金額
        /// </summary>
        /// <param name="PromotionInputList">PromotionInputList資訊</param>
        /// <param name="turnOn">是否啟用正式機狀態的開關閥，正式機on、測試機off</param>
        /// <returns>返回符合滿額贈的賣場ID與折扣金額</returns>
        public List<Models.DomainModels.Redeem.PromotionDetail> PromotionGiftParsingV2(int accountID, List<Models.DomainModels.Redeem.PromotionInput> PromotionInputList, string turnOn)
        {
            turnOn = turnOn.ToLower();
            List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail> promationGiftDetail = new List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail>(); ;
            // 購物車中商品itemID與金額
            Dictionary<int, decimal> itemPriceData = new Dictionary<int, decimal>();

            foreach (var PromotionInput in PromotionInputList)
            {
                itemPriceData.Add(PromotionInput.ItemID, PromotionInput.SumPrice);
            }

            // 是否啟用正式機狀態的開關閥，正式機on、測試機off
            if (turnOn == "on")
            {
                promationGiftDetail = this.GetPromotionGiftFromItemsV2(accountID, itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.Used, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.Used, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.Used);
            }
            else
            {
                promationGiftDetail = this.GetPromotionGiftFromItemsV2(accountID, itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.TempUsed, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.TempUsed, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.TempUsed);
            }

            return promationGiftDetail;
        }

        /// <summary>
        /// 傳入購物車所有的Item
        /// </summary>
        /// <param name="itemPriceData">購物車中所有Item的ID與該商品金額乘數量後的總金額</param>
        /// <param name="usedStatus">搜尋PromotionGiftBasic哪種狀態下的資訊</param>
        /// <param name="whiteListStatus">搜尋PromotionGiftWhiteList哪種狀態下的資訊</param>
        /// <param name="blackListStatus">搜尋PromotionGiftBlackList哪種狀態下的資訊</param>
        /// <returns>List<GetItemTaxDetail>物件, 可使用滿額贈的ItemIDList, 滿額贈折扣金額</returns>
        public List<Models.DomainModels.Redeem.GetItemTaxDetail> GetPromotionGiftFromItems(int accountID, Dictionary<int, decimal> itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus usedStatus, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus)
        {
            //TWSqlDBContext db_before = new TWSqlDBContext();
            List<Models.DomainModels.Redeem.PromotionGiftDetail> promotionGiftDetailList = new List<Models.DomainModels.Redeem.PromotionGiftDetail>();
            // 返回的List<GetItemTaxDetail>
            List<Models.DomainModels.Redeem.GetItemTaxDetail> itemTaxDetailList = new List<Models.DomainModels.Redeem.GetItemTaxDetail>();
            // 暫存購物車中所有Item的ID與該商品金額乘數量後的總金額
            Dictionary<int, decimal> tempItemPriceData = new Dictionary<int, decimal>();
            List<Models.DomainModels.Item.ItemDetail> listItem = null;
            // 找出所有Item的ID
            List<int> itemIDs = null;
            // 找出所有Item的類別ID
            List<int> categryIDs = null;
            // 取得可使用的滿額贈相關資訊
            List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasicList = null;
            // 白名單清單
            List<Models.DomainModels.Redeem.PromotionGiftWhiteList> promotionGiftWhiteList = null;
            // 黑名單清單
            List<Models.DomainModels.Redeem.PromotionGiftBlackList> promotionGiftBlackList = null;
            // 符合滿額贈的購物車商品總金額
            decimal totalPrice = 0m;
            // 找出所有Item資訊 -- 將影響同一個賣場是否能參與多活動
            foreach (var subItemPriceData in itemPriceData)
            {
                tempItemPriceData.Add(subItemPriceData.Key, subItemPriceData.Value);
            }

            // 買一送一中，商品數量不足的ItemID
            List<int> listInsufficientAmount = new List<int>();
            listItem = FindItemData(itemPriceData.Select(x => x.Key).ToList());
            if (listItem == null || listItem.Count == 0)
            {
                return new List<GetItemTaxDetail>();
            }
            // 篩選出不得參與活動的ItemID並排除 (如加價購商品不得參與優惠活動折扣)
            List<int> notToParticipateItemIDList = this.FindNotToParticipateItemIDList(accountID);
            listItem = listItem.Where(x => !notToParticipateItemIDList.Contains(x.Main.ItemBase.ID)).ToList();
            // 找出所有Item的ID
            itemIDs = listItem.Select(x => x.Main.ItemBase.ID).ToList();
            // 找出所有Item的類別ID
            categryIDs = listItem.Select(x => x.Main.ItemBase.CategoryID).Distinct().ToList();
            // 取得活動時間內且可使用並已排序好的滿額贈相關資訊清單
            promotionGiftBasicList = this.GetPromotionGiftBasicByStatus(usedStatus);
            List<int> getpromotionGiftBasicIDList = promotionGiftBasicList.Select(x => x.ID).ToList();
            // 白名單清單
            promotionGiftWhiteList = this.GetPromotionGiftWhiteListByBasicAndItemAndStatus(getpromotionGiftBasicIDList, itemIDs, whiteListStatus);
            // 黑名單清單
            promotionGiftBlackList = this.GetPromotionGiftBlackListByBasicAndItemAndStatus(getpromotionGiftBasicIDList, itemIDs, blackListStatus);
            // 符合各種活動的Item的資料解析
            foreach (Models.DomainModels.Redeem.PromotionGiftBasic promotionGiftBasic in promotionGiftBasicList)
            {
                // 紀錄滿額贈細節資訊
                Models.DomainModels.Redeem.PromotionGiftDetail subPromotionGiftDetail = null;
                // 滿額贈的級距清單
                List<Models.DomainModels.Redeem.PromotionGiftInterval> promotionGiftIntervals = null;
                // 符合此滿額贈活動的類別清單
                List<int> categoryList = null;
                // 屬於這些類別的Item有哪些(同時檢查是否有設定全站類別 categoryID = 0)
                List<Models.DomainModels.Item.ItemDetail> subItemList = null;
                // 可使用的白名單ItemIDs
                List<Models.DomainModels.Redeem.PromotionGiftWhiteList> subPromotionGiftWhiteList = null;
                List<int> whiteListItemIDs = null;
                // 需排除的黑名單ItemIDs
                List<Models.DomainModels.Redeem.PromotionGiftBlackList> subPromotionGiftBlackList = null;
                List<int> blackListItemIDs = null;
                // 可使用滿額贈的Items
                List<Models.DomainModels.Item.ItemDetail> availableItems = null;
                // 符合滿額贈的購物車商品總金額
                totalPrice = 0m;
                // 取得符合的滿額贈級距資訊
                Models.DomainModels.Redeem.PromotionGiftInterval promotionGiftInterval = null;
                subPromotionGiftDetail = new Models.DomainModels.Redeem.PromotionGiftDetail();

                // 滿額贈的級距清單
                promotionGiftIntervals = this.GetPromotionGiftIntervalList(new List<int>() { promotionGiftBasic.ID });
                // 符合此滿額贈活動的類別清單
                categoryList = this.CategoryParsing(promotionGiftBasic.Categories);
                // 屬於這些類別的Item有哪些(同時檢查是否有設定全站類別 categoryID = 0)
                subItemList = this.GetLegalItem(listItem, categoryList);
                if (promotionGiftBasic.ID.ToString() == twoForOneOffer)
                {
                    // 找出買一送一商品庫存數量
                    Dictionary<int, int> sellingQtyList = this._ItemStockService.GetSellingQtyByItemList(subItemList.Select(x=>x.Main.ItemBase.ID).ToList());
                    // 排除買一送一中，商品數量不足2的item
                    foreach (var subItemQty in sellingQtyList)
                    {
                        if (subItemQty.Value < 2)
                        {
                            Models.DomainModels.Item.ItemDetail removeItem = subItemList.Where(x => x.Main.ItemBase.ID == subItemQty.Key).FirstOrDefault();
                            //subItemList.Remove(removeItem);
                            int twoForOneOfferInt = 0;
                            if (twoForOneOffer.Length > 0)
                            {
                                twoForOneOfferInt = Convert.ToInt32(twoForOneOffer);
                            }

                            //PromotionGiftWhiteList checkPromotionGiftWhiteList = db_before.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == twoForOneOfferInt && x.ItemID == removeItem.ID).FirstOrDefault();
                            Models.DBModels.TWSQLDB.PromotionGiftWhiteList checkPromotionGiftWhiteList = this._PromotionRepo.GetAllPromotionGiftWhiteList().Where(x => x.PromotionGiftBasicID == twoForOneOfferInt && x.ItemID == removeItem.Main.ItemBase.ID).FirstOrDefault();
                            if (checkPromotionGiftWhiteList != null)
                            {
                                listInsufficientAmount.Add(removeItem.Main.ItemBase.ID);
                            }
                        }
                    }
                }

                if (promotionGiftIntervals != null)
                {
                    if (promotionGiftBasic.ReferencesList.ToLower() == "white" && promotionGiftWhiteList != null && promotionGiftWhiteList.Count > 0)
                    {
                        // 可使用的白名單ItemIDs
                        subPromotionGiftWhiteList = promotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == promotionGiftBasic.ID).ToList();
                        if (subPromotionGiftWhiteList != null && subPromotionGiftWhiteList.Count > 0)
                        {
                            whiteListItemIDs = subPromotionGiftWhiteList.Select(x => x.ItemID).ToList();
                            // 活動可用的Items(排除買一送一中，商品數量不足的Item)
                            availableItems = subItemList.Where(x => whiteListItemIDs.Contains(x.Main.ItemBase.ID) && !listInsufficientAmount.Contains(x.Main.ItemBase.ID)).ToList();
                        }
                    }
                    else if (promotionGiftBasic.ReferencesList.ToLower() == "black" && promotionGiftBlackList != null && promotionGiftBlackList.Count > 0)
                    {
                        // 需排除的黑名單ItemIDs
                        subPromotionGiftBlackList = promotionGiftBlackList.Where(x => x.PromotionGiftBasicID == promotionGiftBasic.ID).ToList();
                        if (subPromotionGiftBlackList != null && subPromotionGiftBlackList.Count > 0)
                        {
                            blackListItemIDs = subPromotionGiftBlackList.Select(x => x.ItemID).ToList();
                            if (discoloration.ToLower() == "on")
                            {
                                promotionGiftWhiteList.ForEach(x => blackListItemIDs.Add(x.ItemID));
                            }
                            // 可使用滿額贈的Items
                            availableItems = subItemList.Where(x => !blackListItemIDs.Contains(x.Main.ItemBase.ID)).ToList();
                        }
                    }
                    else { }
                }
                
                // 符合滿額贈的購物車商品總金額
                totalPrice = this.GetItemTotalPrice(availableItems, tempItemPriceData);
                // 取得符合的滿額贈級距資訊
                promotionGiftInterval = this.GetPromotionGiftInterval(totalPrice, promotionGiftIntervals);
                // 若有取得符合的滿額贈級距
                if (promotionGiftInterval != null)
                {
                    // 活動ID
                    subPromotionGiftDetail.PromotionGiftBasicID = promotionGiftBasic.ID;
                    // 活動優先權
                    subPromotionGiftDetail.Priority = promotionGiftBasic.Priority;
                    // 活動創建日期
                    subPromotionGiftDetail.PromotionGiftBasicStartDate = promotionGiftBasic.StartDate;
                    // 活動名稱
                    subPromotionGiftDetail.Description = promotionGiftBasic.Description;
                    // 購物車顯示活動優惠名稱
                    subPromotionGiftDetail.ShowDesc = promotionGiftBasic.ShowDesc;
                    // CSS設定
                    subPromotionGiftDetail.CSS = promotionGiftBasic.CSS;
                    // 商品下方所要顯示的訊息
                    subPromotionGiftDetail.HighLight = promotionGiftBasic.HighLight;
                    // 使用白名單或是黑名單等等
                    subPromotionGiftDetail.ReferencesList = promotionGiftBasic.ReferencesList;
                    List<int> acceptedItems = availableItems.Select(x => x.Main.ItemBase.ID).ToList();
                    // 符合滿額贈的ItemID List
                    subPromotionGiftDetail.AcceptedItems = new List<int>();
                    subPromotionGiftDetail.AcceptedItems.AddRange(acceptedItems);
                    // 補入買一送一中，數量不足2者
                    if (promotionGiftBasic.ID.ToString() == twoForOneOffer)
                    {
                        if (subPromotionGiftDetail.AcceptedItems.Count > 0 && listInsufficientAmount.Count > 0)
                        {
                            subPromotionGiftDetail.AcceptedItems.AddRange(listInsufficientAmount);
                            subPromotionGiftDetail.NotAcceptedItems = new List<int>();
                            subPromotionGiftDetail.NotAcceptedItems.AddRange(listInsufficientAmount);
                        }
                    }
                    // 滿額贈級距ID
                    subPromotionGiftDetail.PromotionGiftIntervalID = promotionGiftInterval.ID;
                    // 滿額贈折扣金額
                    subPromotionGiftDetail.ApportionedAmount = this.DecideDiscountAmount(totalPrice, promotionGiftInterval);
                    // 將每種滿額贈資訊皆存入清單中
                    promotionGiftDetailList.Add(subPromotionGiftDetail);
                    
                    // 暫存該活動總累積金額
                    decimal activityPriceSum = 0m;
                    // 已被使用過後的商品DisplayPrice扣除分攤過後的折價金額
                    acceptedItems.ForEach(x =>
                    {
                        var subitemPriceData = tempItemPriceData.Where(i => i.Key == x).FirstOrDefault();
                        // 該商品的displayPrice
                        decimal displayPrice = subitemPriceData.Value;
                        activityPriceSum += Math.Floor(0.5m + ((displayPrice / totalPrice) * subPromotionGiftDetail.ApportionedAmount));
                        // 暫存更新後金額
                        displayPrice -= Math.Floor(0.5m + ((displayPrice / totalPrice) * subPromotionGiftDetail.ApportionedAmount));
                        // 刪除原資訊，填入更新後金額
                        tempItemPriceData.Remove(x);
                        tempItemPriceData.Add(x, displayPrice);
                    });

                    // 將不足金額回補至第一個可使用的Item DisplayPrice裡
                    if (activityPriceSum != subPromotionGiftDetail.ApportionedAmount)
                    {
                        int firstItemID = acceptedItems[0];
                        var coveringItemPriceData = tempItemPriceData.Where(i => i.Key == firstItemID).FirstOrDefault();
                        decimal coveringPrice = coveringItemPriceData.Value + subPromotionGiftDetail.ApportionedAmount - activityPriceSum;
                        tempItemPriceData.Remove(coveringItemPriceData.Key);
                        tempItemPriceData.Add(coveringItemPriceData.Key, coveringPrice);
                    }

                    // 當不使用同商品可執行重複折扣時
                    if (recursiveCalculation.ToLower() != "on")
                    {
                        // 排除已被使用過的item
                        listItem = listItem.Where(x => !subPromotionGiftDetail.AcceptedItems.Contains(x.Main.ItemBase.ID)).ToList();
                        // 排除已使用過的itemID與Price資訊
                        availableItems.Select(x => x.Main.ItemBase.ID).ToList().ForEach(x => tempItemPriceData.Remove(x));
                    }
                }
                //else
                //{
                //    // 若無符合的滿額贈級距則資料填入0
                //    List<int> nullList = new List<int>();
                //    nullList.Add(0);
                //    // 符合滿額贈的ItemID List
                //    subPromotionGiftDetail.AcceptedItems = nullList;
                //    // 滿額贈級距ID
                //    subPromotionGiftDetail.PromotionGiftIntervalID = 0;
                //    // 滿額贈折扣金額
                //    subPromotionGiftDetail.ApportionedAmount = 0;
                //    // 將每種滿額贈資訊皆存入清單中
                //    promotionGiftDetailList.Add(subPromotionGiftDetail);
                //}
            }

            itemTaxDetailList = this.ConvertToItemTaxDetailList(promotionGiftDetailList);

            return itemTaxDetailList;
        }

        /// <summary>
        /// 傳入購物車所有的Item
        /// </summary>
        /// <param name="itemPriceData">購物車中所有Item的ID與該商品金額乘數量後的總金額</param>
        /// <param name="usedStatus">搜尋PromotionGiftBasic哪種狀態下的資訊</param>
        /// <param name="whiteListStatus">搜尋PromotionGiftWhiteList哪種狀態下的資訊</param>
        /// <param name="blackListStatus">搜尋PromotionGiftBlackList哪種狀態下的資訊</param>
        /// <returns>List<GetItemTaxDetail>物件, 可使用滿額贈的ItemIDList, 滿額贈折扣金額</returns>
        public List<Models.DomainModels.Redeem.PromotionDetail> GetPromotionGiftFromItemsV2(int accountID, Dictionary<int, decimal> itemPriceData, Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus usedStatus, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus)
        {
            List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail> promotionGiftDetailList = new List<TWNewEgg.Models.DomainModels.Redeem.PromotionDetail>();
            // 暫存購物車中所有Item的ID與該商品金額乘數量後的總金額
            Dictionary<int, decimal> tempItemPriceData = new Dictionary<int, decimal>();
            List<Models.DomainModels.Item.ItemDetail> listItem = null;
            // 找出所有Item的ID
            List<int> itemIDs = null;
            // 找出所有Item的類別ID
            List<int> categryIDs = null;
            // 取得可使用的滿額贈相關資訊
            List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasicList = null;
            // 白名單清單
            List<Models.DomainModels.Redeem.PromotionGiftWhiteList> promotionGiftWhiteList = null;
            // 黑名單清單
            List<Models.DomainModels.Redeem.PromotionGiftBlackList> promotionGiftBlackList = null;
            // 符合滿額贈的購物車商品總金額
            decimal totalPrice = 0m;
            // 找出所有Item資訊 -- 將影響同一個賣場是否能參與多活動
            foreach (var subItemPriceData in itemPriceData)
            {
                tempItemPriceData.Add(subItemPriceData.Key, subItemPriceData.Value);
            }

            // 買一送一中，商品數量不足的ItemID
            List<int> listInsufficientAmount = new List<int>();
            listItem = FindItemData(itemPriceData.Select(x => x.Key).ToList());
            if (listItem == null || listItem.Count == 0)
            {
                return new List<PromotionDetail>();
            }
            // 找出不得參與活動的ItemID
            List<int> notToParticipateItemIDList = this.FindNotToParticipateItemIDList(accountID);
            listItem = listItem.Where(x => !notToParticipateItemIDList.Contains(x.Main.ItemBase.ID)).ToList();
            // 找出所有Item的ID
            itemIDs = listItem.Select(x => x.Main.ItemBase.ID).ToList();
            // 找出所有Item的類別ID
            categryIDs = listItem.Select(x => x.Main.ItemBase.CategoryID).Distinct().ToList();
            // 取得活動時間內且可使用並已排序好的滿額贈相關資訊清單
            promotionGiftBasicList = this.GetPromotionGiftBasicByStatus(usedStatus);
            List<int> getpromotionGiftBasicIDList = promotionGiftBasicList.Select(x => x.ID).ToList();
            // 白名單清單
            promotionGiftWhiteList = this.GetPromotionGiftWhiteListByBasicAndItemAndStatus(getpromotionGiftBasicIDList, itemIDs, whiteListStatus);
            // 黑名單清單
            promotionGiftBlackList = this.GetPromotionGiftBlackListByBasicAndItemAndStatus(getpromotionGiftBasicIDList, itemIDs, blackListStatus);
            // 符合各種活動的Item的資料解析
            foreach (Models.DomainModels.Redeem.PromotionGiftBasic promotionGiftBasic in promotionGiftBasicList)
            {
                // 紀錄滿額贈細節資訊
                TWNewEgg.Models.DomainModels.Redeem.PromotionDetail subPromotionGiftDetail = new TWNewEgg.Models.DomainModels.Redeem.PromotionDetail();
                // 滿額贈的級距清單
                List<Models.DomainModels.Redeem.PromotionGiftInterval> promotionGiftIntervals = null;
                // 符合此滿額贈活動的類別清單
                List<int> categoryList = null;
                // 屬於這些類別的Item有哪些(同時檢查是否有設定全站類別 categoryID = 0)
                List<Models.DomainModels.Item.ItemDetail> subItemList = null;
                // 可使用的白名單ItemIDs
                List<Models.DomainModels.Redeem.PromotionGiftWhiteList> subPromotionGiftWhiteList = null;
                List<int> whiteListItemIDs = null;
                // 需排除的黑名單ItemIDs
                List<Models.DomainModels.Redeem.PromotionGiftBlackList> subPromotionGiftBlackList = null;
                List<int> blackListItemIDs = new List<int>();
                // 可使用滿額贈的Items
                List<Models.DomainModels.Item.ItemDetail> availableItems = new List<Models.DomainModels.Item.ItemDetail>();
                // 符合滿額贈的購物車商品總金額
                totalPrice = 0m;
                // 取得符合的滿額贈級距資訊
                Models.DomainModels.Redeem.PromotionGiftInterval promotionGiftInterval = null;
                // 滿額贈的級距清單
                promotionGiftIntervals = this.GetPromotionGiftIntervalList(new List<int>() { promotionGiftBasic.ID });
                // 符合此滿額贈活動的類別清單
                categoryList = this.CategoryParsing(promotionGiftBasic.Categories);
                // 屬於這些類別的Item有哪些(同時檢查是否有設定全站類別 categoryID = 0)
                subItemList = this.GetLegalItem(listItem, categoryList);
                if (promotionGiftBasic.ID.ToString() == twoForOneOffer)
                {
                    // 找出買一送一商品庫存數量
                    Dictionary<int, int> sellingQtyList = this._ItemStockService.GetSellingQtyByItemList(subItemList.Select(x=>x.Main.ItemBase.ID).ToList());
                    // 排除買一送一中，商品數量不足2的item
                    foreach (var subItemQty in sellingQtyList)
                    {
                        if (subItemQty.Value < 2)
                        {
                            Models.DomainModels.Item.ItemDetail removeItem = subItemList.Where(x => x.Main.ItemBase.ID == subItemQty.Key).FirstOrDefault();
                            //subItemList.Remove(removeItem);
                            int twoForOneOfferInt = 0;
                            if (twoForOneOffer.Length > 0)
                            {
                                twoForOneOfferInt = Convert.ToInt32(twoForOneOffer);
                            }

                            //PromotionGiftWhiteList checkPromotionGiftWhiteList = db_before.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == twoForOneOfferInt && x.ItemID == removeItem.ID).FirstOrDefault();
                            Models.DBModels.TWSQLDB.PromotionGiftWhiteList checkPromotionGiftWhiteList = this._PromotionRepo.GetAllPromotionGiftWhiteList().Where(x => x.PromotionGiftBasicID == twoForOneOfferInt && x.ItemID == removeItem.Main.ItemBase.ID).FirstOrDefault();
                            if (checkPromotionGiftWhiteList != null)
                            {
                                listInsufficientAmount.Add(removeItem.Main.ItemBase.ID);
                            }
                        }
                    }
                }

                if (promotionGiftIntervals != null)
                {
                    if (promotionGiftBasic.ReferencesList.ToLower() == "white" && promotionGiftWhiteList != null && promotionGiftWhiteList.Count > 0)
                    {
                        // 可使用的白名單ItemIDs
                        subPromotionGiftWhiteList = promotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == promotionGiftBasic.ID).ToList();
                        if (subPromotionGiftWhiteList != null && subPromotionGiftWhiteList.Count > 0)
                        {
                            whiteListItemIDs = subPromotionGiftWhiteList.Select(x => x.ItemID).ToList();
                            // 活動可用的Items(排除買一送一中，商品數量不足的Item)
                            availableItems = subItemList.Where(x => whiteListItemIDs.Contains(x.Main.ItemBase.ID) && !listInsufficientAmount.Contains(x.Main.ItemBase.ID)).ToList();
                        }
                    }
                    else if (promotionGiftBasic.ReferencesList.ToLower() == "black" && promotionGiftBlackList != null && promotionGiftBlackList.Count > 0)
                    {
                        // 需排除的黑名單ItemIDs
                        subPromotionGiftBlackList = promotionGiftBlackList.Where(x => x.PromotionGiftBasicID == promotionGiftBasic.ID).ToList();
                        if (subPromotionGiftBlackList != null && subPromotionGiftBlackList.Count > 0)
                        {
                            blackListItemIDs = subPromotionGiftBlackList.Select(x => x.ItemID).ToList();
                            if (discoloration.ToLower() == "on")
                            {
                                promotionGiftWhiteList.ForEach(x => blackListItemIDs.Add(x.ItemID));
                            }
                        }
                        // 可使用滿額贈的Items
                        availableItems = subItemList.Where(x => !blackListItemIDs.Contains(x.Main.ItemBase.ID)).ToList();
                    }
                    else if (promotionGiftBasic.ReferencesList.ToLower() == "black" && (promotionGiftBlackList == null || promotionGiftBlackList.Count <= 0))
                    {
                        availableItems = subItemList;
                    }
                }

                // 符合滿額贈的購物車商品總金額
                totalPrice = this.GetItemTotalPrice(availableItems, tempItemPriceData);
                // 取得符合的滿額贈級距資訊
                promotionGiftInterval = this.GetPromotionGiftInterval(totalPrice, promotionGiftIntervals);
                // 若有取得符合的滿額贈級距
                if (promotionGiftInterval != null)
                {
                    // 活動ID
                    subPromotionGiftDetail.PromotionGiftBasicID = promotionGiftBasic.ID;
                    // 活動優先權
                    subPromotionGiftDetail.Priority = promotionGiftBasic.Priority;
                    // 活動創建日期
                    subPromotionGiftDetail.PromotionGiftBasicStartDate = promotionGiftBasic.StartDate;
                    // 活動名稱
                    subPromotionGiftDetail.Description = promotionGiftBasic.Description;
                    // 購物車顯示活動優惠名稱
                    subPromotionGiftDetail.ShowDesc = promotionGiftBasic.ShowDesc;
                    // CSS設定
                    subPromotionGiftDetail.CSS = promotionGiftBasic.CSS;
                    // 商品下方所要顯示的訊息
                    subPromotionGiftDetail.HighLight = promotionGiftBasic.HighLight;
                    // 使用白名單或是黑名單等等
                    subPromotionGiftDetail.ReferencesList = promotionGiftBasic.ReferencesList;
                    List<int> acceptedItems = availableItems.Select(x => x.Main.ItemBase.ID).ToList();
                    // 符合滿額贈的ItemID List
                    subPromotionGiftDetail.AcceptedItems = new List<int>();
                    subPromotionGiftDetail.AcceptedItems.AddRange(acceptedItems);
                    // 補入買一送一中，數量不足2者
                    if (promotionGiftBasic.ID.ToString() == twoForOneOffer)
                    {
                        if (subPromotionGiftDetail.AcceptedItems.Count > 0 && listInsufficientAmount.Count > 0)
                        {
                            subPromotionGiftDetail.AcceptedItems.AddRange(listInsufficientAmount);
                            subPromotionGiftDetail.NotAcceptedItems = new List<int>();
                            subPromotionGiftDetail.NotAcceptedItems.AddRange(listInsufficientAmount);
                        }
                    }
                    // 滿額贈級距ID
                    subPromotionGiftDetail.PromotionGiftIntervalID = promotionGiftInterval.ID;
                    // 滿額贈折扣金額
                    subPromotionGiftDetail.ApportionedAmount = this.DecideDiscountAmount(totalPrice, promotionGiftInterval);
                    // 將每種滿額贈資訊皆存入清單中
                    promotionGiftDetailList.Add(subPromotionGiftDetail);

                    // 暫存該活動總累積金額
                    decimal activityPriceSum = 0m;
                    // 已被使用過後的商品DisplayPrice扣除分攤過後的折價金額
                    acceptedItems.ForEach(x =>
                    {
                        var subitemPriceData = tempItemPriceData.Where(i => i.Key == x).FirstOrDefault();
                        // 該商品的displayPrice
                        decimal displayPrice = subitemPriceData.Value;
                        activityPriceSum += Math.Floor(0.5m + ((displayPrice / totalPrice) * subPromotionGiftDetail.ApportionedAmount));
                        // 暫存更新後金額
                        displayPrice -= Math.Floor(0.5m + ((displayPrice / totalPrice) * subPromotionGiftDetail.ApportionedAmount));
                        // 刪除原資訊，填入更新後金額
                        tempItemPriceData.Remove(x);
                        tempItemPriceData.Add(x, displayPrice);
                    });

                    // 將不足金額回補至第一個可使用的Item DisplayPrice裡
                    if (activityPriceSum != subPromotionGiftDetail.ApportionedAmount)
                    {
                        int firstItemID = acceptedItems[0];
                        var coveringItemPriceData = tempItemPriceData.Where(i => i.Key == firstItemID).FirstOrDefault();
                        decimal coveringPrice = coveringItemPriceData.Value + subPromotionGiftDetail.ApportionedAmount - activityPriceSum;
                        tempItemPriceData.Remove(coveringItemPriceData.Key);
                        tempItemPriceData.Add(coveringItemPriceData.Key, coveringPrice);
                    }

                    // 當不使用同商品可執行重複折扣時
                    if (recursiveCalculation.ToLower() != "on")
                    {
                        // 排除已被使用過的item
                        listItem = listItem.Where(x => !subPromotionGiftDetail.AcceptedItems.Contains(x.Main.ItemBase.ID)).ToList();
                        // 排除已使用過的itemID與Price資訊
                        availableItems.Select(x => x.Main.ItemBase.ID).ToList().ForEach(x => tempItemPriceData.Remove(x));
                    }
                }
                //else
                //{
                //    // 若無符合的滿額贈級距則資料填入0
                //    List<int> nullList = new List<int>();
                //    nullList.Add(0);
                //    // 符合滿額贈的ItemID List
                //    subPromotionGiftDetail.AcceptedItems = nullList;
                //    // 滿額贈級距ID
                //    subPromotionGiftDetail.PromotionGiftIntervalID = 0;
                //    // 滿額贈折扣金額
                //    subPromotionGiftDetail.ApportionedAmount = 0;
                //    // 將每種滿額贈資訊皆存入清單中
                //    promotionGiftDetailList.Add(subPromotionGiftDetail);
                //}
            }

            return promotionGiftDetailList;
        }
                
        /// <summary>
        /// 組合List<GetItemTaxDetail>
        /// </summary>
        /// <param name="promotionGiftDetailList">滿額贈清單</param>
        /// <returns>返回List<GetItemTaxDetail></returns>
        public List<Models.DomainModels.Redeem.GetItemTaxDetail> ConvertToItemTaxDetailList(List<Models.DomainModels.Redeem.PromotionGiftDetail> promotionGiftDetailList)
        {
            promotionGiftDetailList = SortItemTaxDetail(promotionGiftDetailList);
            List<Models.DomainModels.Redeem.GetItemTaxDetail> itemTaxDetailList = new List<GetItemTaxDetail>();
            // 可使用滿額贈的商品賣場ID
            string acceptedItems = string.Empty;

            if (promotionGiftDetailList != null && promotionGiftDetailList.Count > 0)
            {
                foreach (Models.DomainModels.Redeem.PromotionGiftDetail subPromotionGiftDetail in promotionGiftDetailList)
                {
                    acceptedItems = string.Empty;
                    Models.DomainModels.Redeem.GetItemTaxDetail itemTaxDetail = new GetItemTaxDetail();
                    // 符合的級距Table中的ID
                    itemTaxDetail.item_id = subPromotionGiftDetail.PromotionGiftIntervalID;
                    // 折價金額
                    itemTaxDetail.itemlist_id = Convert.ToInt32(Math.Floor(0.5m + subPromotionGiftDetail.ApportionedAmount));
                    // PromotionGiftDetail Model轉為JSON格式
                    itemTaxDetail.pricetaxdetail = ObjToJson1(subPromotionGiftDetail);
                    itemTaxDetailList.Add(itemTaxDetail);
                }
            }
            //else
            //{
            //    GetItemTaxDetail itemTaxDetail = new GetItemTaxDetail();
            //    itemTaxDetail.item_id = 0;
            //    itemTaxDetail.itemlist_id = 0;
            //    itemTaxDetail.pricetaxdetail = "";
            //    itemTaxDetailList.Add(itemTaxDetail);
            //}

            // 返回List<GetItemTaxDetail>
            return itemTaxDetailList;
        }

        private List<Models.DomainModels.Redeem.PromotionGiftDetail> SortItemTaxDetail(List<Models.DomainModels.Redeem.PromotionGiftDetail> sortData)
        {
            List<Models.DomainModels.Redeem.PromotionGiftDetail> result = new List<PromotionGiftDetail>();
            List<Models.DomainModels.Redeem.PromotionGiftDetail> hightPriority = new List<PromotionGiftDetail>();
            List<Models.DomainModels.Redeem.PromotionGiftDetail> lowPriority = new List<PromotionGiftDetail>();
            hightPriority = sortData.Where(x => x.Priority != 0 && x.ApportionedAmount != 0).OrderBy(x => x.Priority).ThenBy(x => x.PromotionGiftBasicStartDate).ToList();
            lowPriority = sortData.Where(x => x.Priority == 0 && x.ApportionedAmount != 0).OrderBy(x => x.PromotionGiftBasicStartDate).ToList();
            result.AddRange(hightPriority);
            result.AddRange(lowPriority);
            return result;
        }

        /// <summary>
        /// 物件轉JSON
        /// </summary>
        /// <typeparam name="T">Model類別</typeparam>
        /// <param name="data">Model的資訊</param>
        /// <returns>返回JSON格式的String</returns>
        private static string ObjToJson1<T>(T data)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string jsonStr = javaScriptSerializer.Serialize(data);
            return jsonStr;
        }

        /// <summary>
        /// JSON轉物件後返回所要顯示的優惠折扣資訊字串
        /// </summary>
        /// <param name="promationGiftDetail">Model資訊</param>
        /// <returns>返回所要顯示的優惠折扣資訊字串</returns>
        private string GetPromotionOfferStr(List<Models.DomainModels.Redeem.GetItemTaxDetail> promationGiftDetail)
        {
            string promotionOfferStr = string.Empty;
            Models.DomainModels.Redeem.PromotionGiftDetail promotionDetail = null;
            promationGiftDetail.ForEach(x =>
            {
                if (x.pricetaxdetail != null && x.pricetaxdetail.Length > 0)
                {
                    promotionDetail = new Models.DomainModels.Redeem.PromotionGiftDetail();
                    System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    promotionDetail = js.Deserialize<Models.DomainModels.Redeem.PromotionGiftDetail>(x.pricetaxdetail);
                    promotionOfferStr += "<tr><th>" + promotionDetail.ShowDesc + "</th><td><span class='red sumPrice'> - $ " + promotionDetail.ApportionedAmount + " 元</span></td></tr>";
                }
            });

            return promotionOfferStr;
        }

        /// <summary>
        /// 解析Json並返回PromotionGiftDetail型態的資訊
        /// </summary>
        /// <param name="jsonData">Json格式資訊</param>
        /// <returns>返回PromotionGiftDetail型態的資訊</returns>
        private Models.DomainModels.Redeem.PromotionGiftDetail GetPromotionOfferList(string jsonData)
        {
            Models.DomainModels.Redeem.PromotionGiftDetail promotionDetail = null;
            
            promotionDetail = new PromotionGiftDetail();
            System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
            promotionDetail = js.Deserialize<Models.DomainModels.Redeem.PromotionGiftDetail>(jsonData);

            return promotionDetail;
        }

        /// <summary>
        /// 判定滿額贈折價金額使用何種方式取得
        /// </summary>
        /// <param name="totalPrice">可使用滿額贈商品總金額</param>
        /// <param name="promotionGiftInterval">符合的滿額贈級距資訊</param>
        /// <returns>返回滿額贈折價金額</returns>
        public decimal DecideDiscountAmount(decimal totalPrice, Models.DomainModels.Redeem.PromotionGiftInterval promotionGiftInterval)
        {
            decimal discountAmount = 0m;
            // 若該級距資訊中折價金額為0時，使用折價百分比作為計算，否則返回該級距設定的折價金額
            if (promotionGiftInterval.DiscountAmount == 0m)
            {
                discountAmount = Math.Floor(0.5m + totalPrice * (1 - (promotionGiftInterval.DiscountPercent / 100)));
            }
            else
            {
                discountAmount = Math.Floor(0.5m + promotionGiftInterval.DiscountAmount);
            }

            return discountAmount;
        }

        /// <summary>
        /// 找出所有Item的資訊
        /// </summary>
        /// <param name="itemPriceData">賣場ID與其扣除聰明購後顯示金額</param>
        /// <returns>返回Item清單</returns>
        public List<Models.DomainModels.Item.ItemDetail> FindItemData(List<int> itemIDList)
        {
            List<Models.DomainModels.Item.ItemDetail> getItemData = new List<Models.DomainModels.Item.ItemDetail>();
            Models.DomainModels.Item.ItemDetail objItemDetail = null;
            //取得Item基本資料
            foreach (int numItemId in itemIDList)
            {
                objItemDetail = this._ItemDetailService.GetItemDetail(numItemId);
                if (objItemDetail != null)
                {
                    getItemData.Add(objItemDetail);
                }
            }

            return getItemData;
        }

        /// <summary>
        /// 屬於這些類別的Item有哪些(同時檢查是否有設定全站類別 categoryID = 0)
        /// </summary>
        /// <param name="listItem">購物車中所有Item</param>
        /// <param name="categoryList">符合此滿額贈活動的類別清單</param>
        /// <returns>返回屬於這些類別的Item</returns>
        public List<Models.DomainModels.Item.ItemDetail> GetLegalItem(List<Models.DomainModels.Item.ItemDetail> listItem, List<int> categoryList)
        {
            List<Models.DomainModels.Item.ItemDetail> subItemList = new List<Models.DomainModels.Item.ItemDetail>();
            // 檢查類別中是否包含全站類別(categoryID = 0)
            bool boolAllCategory = categoryList.Contains(0) ? true : false;
            // 若為全站類別則所有Item皆符合條件
            if (boolAllCategory)
            {
                subItemList = listItem;
            }
            else
            {
                // 若非全站類別則只挑選符合條件之Item
                subItemList = listItem.Where(x => categoryList.Contains(x.Main.ItemBase.CategoryID)).ToList();
            }

            return subItemList;
        }

        /// <summary>
        /// 取得商品總金額
        /// </summary>
        /// <param name="item">賣場資訊</param>
        /// <param name="itemPriceData">購物車中所有賣場ID與其扣除聰明購後顯示金額</param>
        /// <returns>返回總金額</returns>
        public decimal GetItemTotalPrice(Models.DomainModels.Item.ItemDetail item, Dictionary<int, decimal> itemPriceData)
        {
            decimal totalPrice = 0m;
            totalPrice = itemPriceData.Where(x => x.Key == item.Main.ItemBase.ID).Select(x => x.Value).FirstOrDefault();
            return totalPrice;
        }

        /// <summary>
        /// 取得商品清單總金額
        /// </summary>
        /// <param name="items">賣場資訊清單</param>
        /// <param name="itemPriceData">購物車中所有Item的ID與金額</param>
        /// <returns>返回總金額</returns>
        public decimal GetItemTotalPrice(List<Models.DomainModels.Item.ItemDetail> items, Dictionary<int, decimal> itemPriceData)
        {
            decimal totalPrice = 0m;
            if (items != null && itemPriceData != null)
            {
                foreach (Models.DomainModels.Item.ItemDetail subItem in items)
                {
                    totalPrice += itemPriceData.Where(x => x.Key == subItem.Main.ItemBase.ID).Select(x => x.Value).FirstOrDefault();
                }
            }

            return totalPrice;
        }
        
        /// <summary>
        /// 解析分類清單
        /// </summary>
        /// <param name="categories">分類String</param>
        /// <returns>返回類別的int清單</returns>
        public List<int> CategoryParsing(string categories)
        {
            List<string> categoriesStringList = new List<string>();
            List<int> categoriesIntList = new List<int>();
            categoriesStringList = categories.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (categoriesStringList.Count > 0)
            {
                foreach (string subCategories in categoriesStringList)
                {
                    categoriesIntList.Add(Convert.ToInt32(subCategories));
                }
            }

            return categoriesIntList;
        }
        
        /// <summary>
        /// 取得所有可用的PromotionGiftBasic
        /// </summary>
        /// <param name="usedStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回PromotionGiftBasic清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftBasic> GetPromotionGiftBasicByStatus(Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus usedStatus)
        {
            //TWSqlDBContext db_before = new TWSqlDBContext();
            DateTime getDateTime = DateTime.Now;
            List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasicForWhite = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBasic> promotionGiftDbBasicForWhiteTemp = new List<Models.DBModels.TWSQLDB.PromotionGiftBasic>();
            List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasicForBlack = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBasic> promotionGiftDbBasicForBlackTemp = new List<Models.DBModels.TWSQLDB.PromotionGiftBasic>();
            List<Models.DBModels.TWSQLDB.PromotionGiftBasic> listPromotionGiftDbBasicSearch = new List<Models.DBModels.TWSQLDB.PromotionGiftBasic>();
            List<Models.DomainModels.Redeem.PromotionGiftBasic> listBasicResult = null;
            List<Models.DomainModels.Redeem.PromotionGiftBasic> listTempDomainBasic = null;
            //listPromotionGiftBasic = db_before.PromotionGiftBasic.Where(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now && x.Status == (int)usedStatus).ToList();
            listBasicResult = new List<Models.DomainModels.Redeem.PromotionGiftBasic>();
            listPromotionGiftDbBasicSearch = this._PromotionRepo.GetAllPromotionGiftBasic().Where(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now && x.Status == (int)usedStatus).ToList();
            if (listPromotionGiftDbBasicSearch != null && listPromotionGiftDbBasicSearch.Count > 0)
            {
                //取得白名單
                promotionGiftDbBasicForWhiteTemp = listPromotionGiftDbBasicSearch.Where(x => x.ReferencesList == "white").ToList();
                if (promotionGiftDbBasicForWhiteTemp != null && promotionGiftDbBasicForWhiteTemp.Count > 0)
                {
                    //資料轉型
                    listTempDomainBasic = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBasic>>(promotionGiftDbBasicForWhiteTemp);
                    //取得優先順序
                    promotionGiftBasicForWhite = this.OrderProcess(listTempDomainBasic);
                    //合併資料
                    if (promotionGiftBasicForWhite != null && promotionGiftBasicForWhite.Count > 0)
                    {
                        listBasicResult.AddRange(promotionGiftBasicForWhite);
                    }
                }

                //取得黑白單
                promotionGiftDbBasicForBlackTemp = listPromotionGiftDbBasicSearch.Where(x => x.ReferencesList == "black").ToList();
                if (promotionGiftDbBasicForBlackTemp != null && promotionGiftDbBasicForBlackTemp.Count > 0)
                {
                    //資料轉型
                    listTempDomainBasic = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBasic>>(promotionGiftDbBasicForBlackTemp);
                    //取得優先順序
                    promotionGiftBasicForBlack = this.OrderProcess(listTempDomainBasic);
                    //合併資料
                    if (promotionGiftBasicForBlack != null && promotionGiftBasicForBlack.Count > 0)
                    {
                        listBasicResult.AddRange(promotionGiftBasicForBlack);
                    }
                }
            }
            //// Priority非0
            //List<PromotionGiftBasic> priorityOrderBasicList = new List<PromotionGiftBasic>();
            //// Priority為0
            //List<PromotionGiftBasic> zeroPriorityOrderBasicList = new List<PromotionGiftBasic>();
            //List<PromotionGiftBasic> listpromotionGiftBasic = new List<PromotionGiftBasic>();
            //listpromotionGiftBasic = db_before.PromotionGiftBasic.Where(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now && x.Status == (int)usedStatus).ToList();
            //listpromotionGiftBasic.ForEach(x =>
            //{
            //    if (x.Priority != 0)
            //    {
            //        priorityOrderBasicList.Add(x);
            //    }
            //    else
            //    {
            //        zeroPriorityOrderBasicList.Add(x);
            //    }
            //});
            //// 執行優先權排序
            //// 非0的Priority中，1的優先權最高，其餘次之。排序好後填入promotionGiftBasicList中
            //priorityOrderBasicList.OrderBy(x => x.Priority).ThenBy(x => x.StartDate).ToList().ForEach(x => promotionGiftBasicList.Add(x));
            //// Priority為0的優先權以活動起始時間最早的優先權最高，其餘次之。排序好後填入promotionGiftBasicList中
            //zeroPriorityOrderBasicList.OrderBy(x => x.StartDate).ToList().ForEach(x => promotionGiftBasicList.Add(x));


            //return promotionGiftBasicList;
            return listBasicResult;
        }

        private List<Models.DomainModels.Redeem.PromotionGiftBasic> OrderProcess(List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasic)
        {
            // 返回PromotionGiftBasic的清單
            List<Models.DomainModels.Redeem.PromotionGiftBasic> promotionGiftBasicList = new List<Models.DomainModels.Redeem.PromotionGiftBasic>();
            if (promotionGiftBasic != null)
            {
                // Priority非0
                List<Models.DomainModels.Redeem.PromotionGiftBasic> priorityOrderBasicList = new List<Models.DomainModels.Redeem.PromotionGiftBasic>();
                // Priority為0
                List<Models.DomainModels.Redeem.PromotionGiftBasic> zeroPriorityOrderBasicList = new List<Models.DomainModels.Redeem.PromotionGiftBasic>();
                promotionGiftBasic.ForEach(x =>
                {
                    if (x.Priority != 0)
                    {
                        priorityOrderBasicList.Add(x);
                    }
                    else
                    {
                        zeroPriorityOrderBasicList.Add(x);
                    }
                });
                // 執行優先權排序
                // 非0的Priority中，1的優先權最高，其餘次之。排序好後填入promotionGiftBasicList中
                priorityOrderBasicList.OrderBy(x => x.Priority).ThenBy(x => x.StartDate).ToList().ForEach(x => promotionGiftBasicList.Add(x));
                // Priority為0的優先權以活動起始時間最早的優先權最高，其餘次之。排序好後填入promotionGiftBasicList中
                zeroPriorityOrderBasicList.OrderBy(x => x.StartDate).ToList().ForEach(x => promotionGiftBasicList.Add(x));
            }

            return promotionGiftBasicList;
        }

        /// <summary>
        /// 根據PromotionGiftBasic Id取得該物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <returns>null or PromotionGiftBasic object</returns>
        public Models.DomainModels.Redeem.PromotionGiftBasic GetPromotionGiftBasicByBasicId(int argNumPromotionGiftBasicId)
        {
            if (argNumPromotionGiftBasicId <= 0)
                return null;

            //TWSqlDBContext objDb = null;
            Models.DBModels.TWSQLDB.PromotionGiftBasic objDbBasic = null;
            Models.DomainModels.Redeem.PromotionGiftBasic objBasic = null;

            //objBasic = objDb.PromotionGiftBasic.Where(x => x.ID == argNumPromotionGiftBasicId).FirstOrDefault();
            objDbBasic = this._PromotionRepo.GetAllPromotionGiftBasic().Where(x => x.ID == argNumPromotionGiftBasicId).FirstOrDefault();
            if (objDbBasic != null)
            {
                objBasic = ModelConverter.ConvertTo<Models.DomainModels.Redeem.PromotionGiftBasic>(objDbBasic);
            }

            return objBasic;
        }

        /// <summary>
        /// 取得所有PromotionGiftBasic清單
        /// </summary>
        /// <returns>null or List of PromotionGiftBasic</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftBasic> GetAllPromotionGiftBasic()
        {
            List<Models.DBModels.TWSQLDB.PromotionGiftBasic> listDbBasic = null;
            List<Models.DomainModels.Redeem.PromotionGiftBasic> listBasic = null;

            //listBasic = objDb.PromotionGiftBasic.DefaultIfEmpty().ToList();
            listDbBasic = this._PromotionRepo.GetAllPromotionGiftBasic().ToList();
            if (listDbBasic != null && listDbBasic.Count > 0)
            {
                listBasic = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBasic>>(listDbBasic);
            }

            return listBasic;
        }

        /// <summary>
        /// 根據PromotionGiftBasicID(滿額贈ID)取得該級距清單
        /// </summary>
        /// <param name="basicIDList">PromotionGiftBasicID(滿額贈ID)清單</param>
        /// <returns>返回級距清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftInterval> GetPromotionGiftIntervalList(List<int> basicIDList)
        {
            //TWSqlDBContext db_before = new TWSqlDBContext();
            List<Models.DomainModels.Redeem.PromotionGiftInterval> listInterval = null;
            //List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listDbInterval = db_before.PromotionGiftInterval.Where(x => x.PromotionGiftBasicID == basicID).ToList();
            List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listDbInterval = null;

            listDbInterval = this._PromotionRepo.GetAllPromotionGiftInterval().Where(x => basicIDList.Contains(x.PromotionGiftBasicID)).ToList();

            if (listDbInterval != null && listDbInterval.Count > 0)
            {
                listInterval = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftInterval>>(listDbInterval);
            }

            return listInterval;
        }

        /// <summary>
        /// 取得滿額贈級距資訊
        /// </summary>
        /// <param name="totalPrice">購物車可用滿額贈商品總額</param>
        /// <param name="promotionGiftIntervals">滿額贈級距清單</param>
        /// <returns>返回滿額贈級距資訊</returns>
        public Models.DomainModels.Redeem.PromotionGiftInterval GetPromotionGiftInterval(decimal totalPrice, List<Models.DomainModels.Redeem.PromotionGiftInterval> promotionGiftIntervals)
        {
            if (promotionGiftIntervals == null)
            {
                return null;
            }

            Models.DomainModels.Redeem.PromotionGiftInterval getPromotionGiftInterval = promotionGiftIntervals.Where(x => x.LowerLimit <= totalPrice && x.UpperLimit > totalPrice).FirstOrDefault();
            return getPromotionGiftInterval;
        }

        /// <summary>
        /// 取據級距ID取得該資料
        /// </summary>
        /// <param name="argNumIntervalId">級距Id</param>
        /// <returns>nuul or PromotionGiftInterval object</returns>
        public Models.DomainModels.Redeem.PromotionGiftInterval GetPromotionGiftIntervalByIntervalId(int argNumIntervalId)
        {
            if (argNumIntervalId <= 0)
                return null;

            Models.DomainModels.Redeem.PromotionGiftInterval objInterval = null;
            Models.DBModels.TWSQLDB.PromotionGiftInterval objDbInterval = null;

            objDbInterval = this._PromotionRepo.GetAllPromotionGiftInterval().Where(x => x.ID == argNumIntervalId).FirstOrDefault();
            if (objDbInterval != null)
            {
                objInterval = ModelConverter.ConvertTo<Models.DomainModels.Redeem.PromotionGiftInterval>(objDbInterval);
            }

            return objInterval;
        }

        /// <summary>
        /// 根據itemID取得白名單清單
        /// </summary>
        /// <param name="itemID">所要取得的itemID</param>
        /// <param name="whiteListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回白名單清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftWhiteList> GetPromotionGiftWhiteListByItemAndStatus(List<int> basicIDList, int itemID, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus)
        {
            //List<PromotionGiftWhiteList> promotionGiftWhiteLists = db_before.PromotionGiftWhiteList.Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && x.ItemID == itemID && x.Status == (int)whiteListStatus).ToList();
            List<Models.DomainModels.Redeem.PromotionGiftWhiteList> listWhiteList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listDbWhiteList = null;

            listDbWhiteList = this._PromotionRepo.GetAllPromotionGiftWhiteList().Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && x.ItemID == itemID && x.Status == (int)whiteListStatus).ToList();
            if (listDbWhiteList != null && listDbWhiteList.Count > 0)
            {
                listWhiteList = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftWhiteList>>(listWhiteList);
            }

            return listWhiteList;
        }

        /// <summary>
        /// 根據itemID取得黑名單清單
        /// </summary>
        /// <param name="itemID">所要取得的itemID</param>
        /// <param name="blackListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回黑名單清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftBlackList> GetPromotionGiftBlackListByItemAndStatus(List<int> basicIDList, int itemID, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus)
        {
            //List<PromotionGiftBlackList> promotionGiftBlackLists = db_before.PromotionGiftBlackList.Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && x.ItemID == itemID && x.Status == (int)blackListStatus).ToList();
            List<Models.DomainModels.Redeem.PromotionGiftBlackList> listBlackList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listDbBlackList = null;

            listDbBlackList = this._PromotionRepo.GetAllPromotionGiftBlackList().Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && x.ItemID == itemID && x.Status == (int)blackListStatus).ToList();
            if (listDbBlackList != null && listDbBlackList.Count > 0)
            {
                listBlackList = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBlackList>>(listDbBlackList);
            }

            return listBlackList;
        }

        /// <summary>
        /// 根據itemID清單取得白名單清單
        /// </summary>
        /// <param name="itemIDs">所要取得的itemID清單</param>
        /// <param name="blackListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回白名單清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftWhiteList> GetPromotionGiftWhiteListByBasicAndItemAndStatus(List<int> basicIDList, List<int> itemIDs, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus whiteListStatus)
        {
            //List<PromotionGiftWhiteList> promotionGiftWhiteLists = db_before.PromotionGiftWhiteList.Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && itemIDs.Contains(x.ItemID) && x.Status == (int)whiteListStatus).ToList();
            List<Models.DomainModels.Redeem.PromotionGiftWhiteList> listWhiteList = new List<Models.DomainModels.Redeem.PromotionGiftWhiteList>();
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listDbWhiteList = null;

            listDbWhiteList = this._PromotionRepo.GetAllPromotionGiftWhiteList().Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && itemIDs.Contains(x.ItemID) && x.Status == (int)whiteListStatus).ToList();
            if (listDbWhiteList != null && listDbWhiteList.Count > 0)
            {
                listWhiteList = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftWhiteList>>(listDbWhiteList);
            }

            return listWhiteList;
        }

        /// <summary>
        /// 根據itemID清單取得黑名單清單
        /// </summary>
        /// <param name="itemIDs">所要取得的itemID清單</param>
        /// <param name="blackListStatus">搜尋哪種狀態下的資訊</param>
        /// <returns>返回黑名單清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftBlackList> GetPromotionGiftBlackListByBasicAndItemAndStatus(List<int> basicIDList, List<int> itemIDs, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus blackListStatus)
        {
            //List<PromotionGiftBlackList> promotionGiftBlackLists = db_before.PromotionGiftBlackList.Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && itemIDs.Contains(x.ItemID) && x.Status == (int)blackListStatus).ToList();
            List<Models.DomainModels.Redeem.PromotionGiftBlackList> listBlackList = new List<Models.DomainModels.Redeem.PromotionGiftBlackList>();
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listDbBlackList = null;

            listDbBlackList = this._PromotionRepo.GetAllPromotionGiftBlackList().Where(x => basicIDList.Contains(x.PromotionGiftBasicID) && itemIDs.Contains(x.ItemID) && x.Status == (int)blackListStatus).ToList();
            if (listDbBlackList != null && listDbBlackList.Count > 0)
            {
                listBlackList = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBlackList>>(listDbBlackList);
            }

            return listBlackList;
        }

        /// <summary>
        /// 傳入購物車ID與商品資訊，會自動拆單並建立滿額贈的拆單記錄
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="oriPriceData">初始金額資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>拆單成功:true, 拆單失敗:false</returns>
        public bool CreatePromotionGiftRecord(int salesOrderGroupID, Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData, string turnOn)
        {
            bool boolExec = false;
            turnOn = turnOn.ToLower();
            List<Models.DomainModels.Redeem.PromotionGiftRecords> newGiftRecordList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRec = null;

            // 檢查該車是否不符合任何滿額贈條件，若是則不執行後續動作
            int priceCheck = 0;
            this.GetPromotionGiftData(oriPriceData).ForEach(x => priceCheck += x.itemlist_id);
            if (priceCheck == 0)
            {
                return true;
            }
            // 解析promotionData並執行拆單動作
            try
            {
                newGiftRecordList = this.DecompositionAndStorage(salesOrderGroupID, oriPriceData, turnOn);
            }
            catch(Exception e)
            {
                //logger.Info("解析promotionData並執行拆單動作失敗 [ErrorMessage] : " + e.Message + " [ErrorStackTrace] : " + e.StackTrace);
                return boolExec;
            }

            // 建立PromotionGiftRecords清單
            //boolExec = objGiftRecordService.CreatePromotionGiftRecord(newGiftRecordList);
            try
            {
                listDbGiftRec = ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftRecords>>(newGiftRecordList);
                this._PromotionRecRepo.CreateRangePromotionGiftRecords(listDbGiftRec);
                boolExec = true;
            }
            catch (Exception ex)
            {
                //logger.Info("建立PromotionGiftRecords清單動作失敗");
                boolExec = false;
            }

            return boolExec;
        }

        /// <summary>
        /// 傳入購物車ID與商品資訊，會自動拆單並建立滿額贈的拆單記錄
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="promotionGiftDetailList">promotionGiftDetailList資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>拆單成功:true, 拆單失敗:false</returns>
        public bool CreatePromotionGiftRecordV2(int salesOrderGroupID, List<Models.DomainModels.Redeem.PromotionDetail> promotionGiftDetailList, string turnOn)
        {
            turnOn = turnOn.ToLower();
            bool boolExec = false;
            List<Models.DomainModels.Redeem.PromotionGiftRecords> newGiftRecordList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRec = null;
            newGiftRecordList = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
            // 檢查該車是否不符合任何滿額贈條件，若是則不執行後續動作
            int priceCheck = 0;
            //this.GetPromotionGiftData(oriPriceData).ForEach(x => priceCheck += x.itemlist_id);
            promotionGiftDetailList.ForEach(x => priceCheck += Convert.ToInt32(Math.Floor(0.5m + x.ApportionedAmount)));
            if (priceCheck == 0)
            {
                return true;
            }
            // 解析promotionData並執行拆單動作
            try
            {
                newGiftRecordList = this.DecompositionAndStorageV2(salesOrderGroupID, promotionGiftDetailList, turnOn);
            }
            catch (Exception e)
            {
                //logger.Info("解析promotionData並執行拆單動作失敗 [ErrorMessage] : " + e.Message + " [ErrorStackTrace] : " + e.StackTrace);
                return boolExec;
            }

            // 建立PromotionGiftRecords清單
            //boolExec = objGiftRecordService.CreatePromotionGiftRecord(newGiftRecordList);
            try
            {
                listDbGiftRec = ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftRecords>>(newGiftRecordList);
                this._PromotionRecRepo.CreateRangePromotionGiftRecords(listDbGiftRec);
                boolExec = true;
            }
            catch (Exception ex)
            {
                //logger.Info("建立PromotionGiftRecords清單動作失敗");
                boolExec = false;
            }

            return boolExec;
        }

        /// <summary>
        /// 解析promotionData並執行拆單與儲存動作
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="oriPriceData">初始金額資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>滿額贈紀錄清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftRecords> DecompositionAndStorage(int salesOrderGroupID, Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData, string turnOn)
        {
            List<Models.DomainModels.Redeem.PromotionGiftRecords> promotionGiftRecords = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRec = null;
            // 取得紀錄PromotionGiftItemTaxDetail的List資訊
            List<Models.DomainModels.Redeem.GetItemTaxDetail> promotionGiftItemTaxDetails = new List<Models.DomainModels.Redeem.GetItemTaxDetail>();
            List<string> stringItemIDs = new List<string>();
            decimal promotionGiftPrice = 0m;
            List<Models.DBModels.TWSQLDB.SalesOrder> salesOrderList = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            List<Models.DBModels.TWSQLDB.SalesOrderItem> salesOrderItemList = new List<Models.DBModels.TWSQLDB.SalesOrderItem>();
            List<Models.DomainModels.Cart.SOItemBase> listDmTempSoItem = null;
            Models.DomainModels.Cart.SOItemBase objDmTempSoItem = null;
            List<string> salesOrderCodeList = new List<string>();
            // 間配訂單
            List<Models.DBModels.TWSQLDB.SalesOrder> transshipmentSalesOrder = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            List<Models.DomainModels.Cart.SOBase> listDmTransshipmentSalesOrder = new List<Models.DomainModels.Cart.SOBase>(); //轉型用
            // 非間配訂單
            List<Models.DBModels.TWSQLDB.SalesOrder> notTransshipmentSalesOrder = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            List<Models.DomainModels.Cart.SOBase> listDmNotTransshipmentSalesOrder = new List<Models.DomainModels.Cart.SOBase>();//轉型用

            // 取得紀錄PromotionGiftItemTaxDetail的List資訊
            promotionGiftItemTaxDetails = this.GetPromotionGiftData(oriPriceData);
            // 滿額贈級距ID
            int promotionGiftIntervalID = 0;
            int promotionGiftBasicID = 0;

            //salesOrderList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).OrderBy(x => x.Code).ToList();
            salesOrderList = this._SoRepo.GetSOs(salesOrderGroupID).OrderBy(x => x.Code).ToList();
            salesOrderCodeList = salesOrderList.Select(x => x.Code).ToList();
            //salesOrderItemList = db_before.SalesOrderItem.Where(x => salesOrderCodeList.Contains(x.SalesorderCode)).OrderBy(x => x.Code).ToList();
            salesOrderItemList = this._SoRepo.GetSOItemsByCodes(salesOrderCodeList).ToList();

            foreach (Models.DomainModels.Redeem.GetItemTaxDetail subItemTaxDetail in promotionGiftItemTaxDetails)
            {
                List<Models.DomainModels.Redeem.PromotionGiftRecords> subPromotionGiftRecords = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
                List<Models.DomainModels.Redeem.GetItemTaxDetail> subGetItemTaxDetail = new List<Models.DomainModels.Redeem.GetItemTaxDetail>();
                // 屬於此滿額贈中的SalesOrder List
                List<Models.DBModels.TWSQLDB.SalesOrder> subSalesOrders = new List<Models.DBModels.TWSQLDB.SalesOrder>();
                // 屬於此滿額贈中的SalesOrderItem List
                List<Models.DBModels.TWSQLDB.SalesOrderItem> subSalesOrderItems = new List<Models.DBModels.TWSQLDB.SalesOrderItem>();
                List<int> itemIDs = new List<int>();
                promotionGiftIntervalID = 0;
                promotionGiftBasicID = 0;

                Models.DomainModels.Redeem.PromotionGiftDetail promotionGiftDetail = new Models.DomainModels.Redeem.PromotionGiftDetail();
                promotionGiftDetail = GetPromotionOfferList(subItemTaxDetail.pricetaxdetail);

                // 滿額贈活動ID
                promotionGiftBasicID = promotionGiftDetail.PromotionGiftBasicID;
                
                // 滿額贈折價金額
                promotionGiftPrice = promotionGiftDetail.ApportionedAmount;
                
                // 找出符合該滿額贈的賣場ID清單，並將其轉存為Int格式
                itemIDs.AddRange(promotionGiftDetail.AcceptedItems);
                // 排除買一送一商品庫存數不足2者
                if (promotionGiftBasicID.ToString() == twoForOneOffer && promotionGiftDetail.NotAcceptedItems != null && promotionGiftDetail.NotAcceptedItems.Count > 0)
                {
                    promotionGiftDetail.NotAcceptedItems.ForEach(x => itemIDs.Remove(x));
                }
                // 取得所有滿足滿額贈商品賣場ID與金額的List<GetItemTaxDetail>
                foreach (var subOriPriceData in oriPriceData)
                {
                    if (subOriPriceData.Key != "Promotion")
                    {
                        // 將oriPriceData中符合該滿額贈的賣場ID的List<GetItemTaxDetail>都存入subGetItemTaxDetail中
                        subOriPriceData.Value.Where(x => itemIDs.Contains(x.item_id)).ToList().ForEach(i => subGetItemTaxDetail.Add(i));
                    }
                }

                // 算出該滿額贈類型所包含的Item總金額
                decimal totalPrice = 0m;
                subGetItemTaxDetail.ForEach(x =>{
                    totalPrice += Math.Floor(0.5m + Convert.ToDecimal(x.pricetaxdetail.Split(',')[12]));
                });

                // 將滿額贈金額分攤存入訂單子單中
                // 間配訂單與非間配訂單分開處理
                // 除間配商品以外，其餘商品顯示總金額除以商品數量都應該要能被整除，間配商品則直接挑出後除商品數量後四捨五入，在將差價捕到間配商品中的第一個子單
                itemIDs = subGetItemTaxDetail.Select(x => x.item_id).ToList();
                // 找出屬於此滿額贈中的部分SalesOrderItem List
                subSalesOrderItems = salesOrderItemList.Where(x => itemIDs.Contains(x.ItemID)).OrderBy(x => x.Code).ToList();
                // 找出屬於此滿額贈中的部分SalesOrder List
                List<string> subSalesOrderCodes = subSalesOrderItems.Select(x => x.SalesorderCode).Distinct().ToList();
                subSalesOrders = salesOrderList.Where(x => subSalesOrderCodes.Contains(x.Code)).OrderBy(x => x.Code).ToList();

                // 間配訂單
                transshipmentSalesOrder = subSalesOrders.Where(x => x.DelivType == (int)SalesOrder.DelivTypeList.Transshipment).OrderBy(x => x.Code).ToList();
                //轉型間配訂單資料
                listDmTransshipmentSalesOrder = ModelConverter.ConvertTo<List<Models.DomainModels.Cart.SOBase>>(transshipmentSalesOrder);
                listDmTempSoItem = ModelConverter.ConvertTo<List<Models.DomainModels.Cart.SOItemBase>>(subSalesOrderItems);
                // 間配訂單分配滿額贈金額
                subPromotionGiftRecords = this.CombinePromotionGiftRecords(
                    totalPrice
                    , promotionGiftPrice
                    , listDmTransshipmentSalesOrder
                    , listDmTempSoItem
                    , subGetItemTaxDetail
                    , subPromotionGiftRecords
                    , promotionGiftBasicID
                    , promotionGiftIntervalID);

                // 非間配訂單
                notTransshipmentSalesOrder = subSalesOrders.Where(x => x.DelivType != (int)SalesOrder.DelivTypeList.Transshipment).OrderBy(x => x.Code).ToList();
                listDmNotTransshipmentSalesOrder = ModelConverter.ConvertTo<List<Models.DomainModels.Cart.SOBase>>(notTransshipmentSalesOrder);
                listDmTempSoItem = ModelConverter.ConvertTo<List<Models.DomainModels.Cart.SOItemBase>>(subSalesOrderItems);
                //轉型非間配訂單資料
                // 非間配訂單分配滿額贈金額
                subPromotionGiftRecords = this.CombinePromotionGiftRecords(
                    totalPrice
                    , promotionGiftPrice
                    , listDmNotTransshipmentSalesOrder
                    , listDmTempSoItem
                    , subGetItemTaxDetail
                    , subPromotionGiftRecords
                    , promotionGiftBasicID
                    , promotionGiftIntervalID);

                // 算出該PromotionGift加起來的總金額是否與原設定的金額相符
                decimal tempPromotionGiftPrice = 0m;
                subPromotionGiftRecords.ForEach(x => tempPromotionGiftPrice += x.ApportionedAmount);
                // 若不相符則將差價補入該訂單中的第一筆子單內
                if (tempPromotionGiftPrice != promotionGiftPrice)
                {
                    // 計算差額方法為 原該滿額贈級距金額 - 後來分配後的總金額
                    subPromotionGiftRecords[0].ApportionedAmount += promotionGiftPrice - tempPromotionGiftPrice;
                }

                // 更新SalesOrderItem中ApportionedAmount(滿額贈分攤金額)
                foreach (Models.DomainModels.Redeem.PromotionGiftRecords objPromotionGiftRecords in subPromotionGiftRecords)
                {
                    Models.DBModels.TWSQLDB.SalesOrderItem oldSalesOrderItem = subSalesOrderItems.Where(x => x.Code == objPromotionGiftRecords.SalesOrderItemCode).FirstOrDefault();
                    // 累加活動折扣金額
                    oldSalesOrderItem.ApportionedAmount += objPromotionGiftRecords.ApportionedAmount;
                    int updatedCount = oldSalesOrderItem.Updated ?? 0;
                    oldSalesOrderItem.Updated += updatedCount + 1;
                    oldSalesOrderItem.UpdateDate = DateTime.Now;
                    oldSalesOrderItem.UpdateUser = "system";

                    if (turnOn.ToLower() == "on")
                    {
                        objPromotionGiftRecords.UsedStatus = (int)Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.Used;
                    }
                }

                try
                {
                    //db_before.SaveChanges();
                    this._SoRepo.UpdateRangeSOItem(subSalesOrderItems);
                    //listDbGiftRec = ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftRecords>>(subPromotionGiftRecords);
                    //this._PromotionRecRepo.UpdateRangePromotionGiftRecords(listDbGiftRec);
                }
                catch(Exception e)
                {
                    throw new Exception(e.Message);
                }

                promotionGiftRecords.AddRange(subPromotionGiftRecords);
            }

            return promotionGiftRecords;
        }

        /// <summary>
        /// 解析promotionData並執行拆單與儲存動作
        /// </summary>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <param name="promotionGiftDetailList">promotionGiftDetailList資訊</param>
        /// <param name="turnOn">是否啟動正式機的Status</param>
        /// <returns>滿額贈紀錄清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftRecords> DecompositionAndStorageV2(int salesOrderGroupID, List<Models.DomainModels.Redeem.PromotionDetail> promotionGiftDetailList, string turnOn)
        {
            List<Models.DomainModels.Redeem.PromotionGiftRecords> promotionGiftRecords = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
            List<Models.DBModels.TWSQLDB.PromotionGiftRecords> listDbGiftRecords = null;
            List<string> stringItemIDs = new List<string>();
            decimal promotionGiftPrice = 0m;
            List<Models.DBModels.TWSQLDB.SalesOrder> salesOrderList = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            List<Models.DBModels.TWSQLDB.SalesOrderItem> salesOrderItemList = new List<Models.DBModels.TWSQLDB.SalesOrderItem>();
            List<string> salesOrderCodeList = new List<string>();
            List<SalesOrderItem> listDbSoItem = null;
            // 間配訂單
            List<Models.DBModels.TWSQLDB.SalesOrder> transshipmentSalesOrder = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            // 非間配訂單
            List<Models.DBModels.TWSQLDB.SalesOrder> notTransshipmentSalesOrder = new List<Models.DBModels.TWSQLDB.SalesOrder>();
            // 滿額贈級距ID
            int promotionGiftIntervalID = 0;
            int promotionGiftBasicID = 0;

            //salesOrderList = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).OrderBy(x => x.Code).ToList();
            salesOrderList = this._SoRepo.GetSOs(salesOrderGroupID).ToList();
            salesOrderCodeList = salesOrderList.Select(x => x.Code).ToList();
            //salesOrderItemList = db_before.SalesOrderItem.Where(x => salesOrderCodeList.Contains(x.SalesorderCode)).OrderBy(x => x.Code).ToList();
            salesOrderItemList = this._SoRepo.GetSOItemsByCodes(salesOrderCodeList).ToList();

            foreach (Models.DomainModels.Redeem.PromotionDetail subPromotionDetail in promotionGiftDetailList)
            {
                List<Models.DomainModels.Redeem.PromotionGiftRecords> subPromotionGiftRecords = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
                // 屬於此滿額贈中的SalesOrder List
                List<Models.DBModels.TWSQLDB.SalesOrder> subSalesOrders = new List<Models.DBModels.TWSQLDB.SalesOrder>();
                List<Models.DomainModels.Cart.SOBase> listDmTempSo = null;
                // 屬於此滿額贈中的SalesOrderItem List
                List<Models.DBModels.TWSQLDB.SalesOrderItem> subSalesOrderItems = new List<Models.DBModels.TWSQLDB.SalesOrderItem>();
                List<Models.DomainModels.Cart.SOItemBase> listDmTempSoItem = null;
                List<Models.DBModels.TWSQLDB.ItemDisplayPrice> searchItemDisplayPriceList = null;
                List<Models.DomainModels.Item.ItemPrice> listDmTempItemDisplayPrice = null;
                List<int> itemIDs = new List<int>();
                promotionGiftIntervalID = 0;
                promotionGiftBasicID = 0;
                // 滿額贈活動ID
                promotionGiftBasicID = subPromotionDetail.PromotionGiftBasicID;
                // 滿額贈折價金額
                promotionGiftPrice = subPromotionDetail.ApportionedAmount;
                // 找出符合該滿額贈的賣場ID清單，並將其轉存為Int格式
                itemIDs.AddRange(subPromotionDetail.AcceptedItems);
                // 排除買一送一商品庫存數不足2者
                if (promotionGiftBasicID.ToString() == twoForOneOffer && subPromotionDetail.NotAcceptedItems != null && subPromotionDetail.NotAcceptedItems.Count > 0)
                {
                    subPromotionDetail.NotAcceptedItems.ForEach(x => itemIDs.Remove(x));
                }

                decimal totalPrice = 0m;
                // 將滿額贈金額分攤存入訂單子單中
                // 間配訂單與非間配訂單分開處理
                // 除間配商品以外，其餘商品顯示總金額除以商品數量都應該要能被整除，間配商品則直接挑出後除商品數量後四捨五入，在將差價捕到間配商品中的第一個子單
                // 找出屬於此滿額贈中的部分SalesOrderItem List
                subSalesOrderItems = salesOrderItemList.Where(x => itemIDs.Contains(x.ItemID)).OrderBy(x => x.Code).ToList();
                // 算出該滿額贈類型所包含的Item總金額
                subSalesOrderItems.ForEach(x => {
                    totalPrice += (decimal)x.DisplayPrice;
                });
                // 找出屬於此滿額贈中的部分SalesOrder List
                List<string> subSalesOrderCodes = subSalesOrderItems.Select(x => x.SalesorderCode).Distinct().ToList();
                subSalesOrders = salesOrderList.Where(x => subSalesOrderCodes.Contains(x.Code)).OrderBy(x => x.Code).ToList();

                //List<Models.DBModels.TWSQLDB.ItemDisplayPrice> searchItemDisplayPriceList = db_before.ItemDisplayPrice.Where(x => itemIDs.Contains(x.ItemID)).ToList();
                searchItemDisplayPriceList = this._ItemDisplayPriceRepo.GetItemDisplayPriceList(itemIDs).ToList();
                //資料轉型
                listDmTempItemDisplayPrice = ModelConverter.ConvertTo<List<Models.DomainModels.Item.ItemPrice>>(searchItemDisplayPriceList);
                listDmTempSo = ModelConverter.ConvertTo<List<Models.DomainModels.Cart.SOBase>>(subSalesOrders);
                listDmTempSoItem = ModelConverter.ConvertTo<List<Models.DomainModels.Cart.SOItemBase>>(subSalesOrderItems);
                //// 非間配訂單分配滿額贈金額
                subPromotionGiftRecords = this.CombinePromotionGiftRecordsV2(
                    totalPrice
                    , promotionGiftPrice
                    , listDmTempSo
                    , listDmTempSoItem
                    , listDmTempItemDisplayPrice
                    , subPromotionGiftRecords
                    , promotionGiftBasicID
                    , promotionGiftIntervalID);
                
                // 算出該PromotionGift加起來的總金額是否與原設定的金額相符
                decimal tempPromotionGiftPrice = 0m;
                subPromotionGiftRecords.ForEach(x => tempPromotionGiftPrice += x.ApportionedAmount);
                // 若不相符則將差價補入該訂單中的第一筆子單內
                if (tempPromotionGiftPrice != promotionGiftPrice)
                {
                    // 計算差額方法為 原該滿額贈級距金額 - 後來分配後的總金額
                    subPromotionGiftRecords[0].ApportionedAmount += promotionGiftPrice - tempPromotionGiftPrice;
                }

                // 更新SalesOrderItem中ApportionedAmount(滿額贈分攤金額)
                listDbSoItem = new List<SalesOrderItem>();
                foreach (Models.DomainModels.Redeem.PromotionGiftRecords objPromotionGiftRecords in subPromotionGiftRecords)
                {
                    SalesOrderItem oldSalesOrderItem = subSalesOrderItems.Where(x => x.Code == objPromotionGiftRecords.SalesOrderItemCode).FirstOrDefault();
                    // 累加活動折扣金額
                    oldSalesOrderItem.ApportionedAmount += objPromotionGiftRecords.ApportionedAmount;
                    int updatedCount = oldSalesOrderItem.Updated ?? 0;
                    oldSalesOrderItem.Updated += updatedCount + 1;
                    oldSalesOrderItem.UpdateDate = DateTime.Now;
                    oldSalesOrderItem.UpdateUser = "system";

                    if (turnOn.ToLower() == "on")
                    {
                        objPromotionGiftRecords.UsedStatus = (int)Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.Used;
                    }
                }

                try
                {
                    //db_before.SaveChanges();
                    this._SoRepo.UpdateRangeSOItem(subSalesOrderItems);
                    //listDbGiftRecords = ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftRecords>>(subPromotionGiftRecords);
                    //this._PromotionRecRepo.UpdateRangePromotionGiftRecords(listDbGiftRecords);

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                promotionGiftRecords.AddRange(subPromotionGiftRecords);
            }

            return promotionGiftRecords;
        }

        /// <summary>
        /// 自動分配與拆解滿額贈金額
        /// </summary>
        /// <param name="totalPrice">符合滿額贈的商品總金額</param>
        /// <param name="promotionGiftPrice">滿額贈折價級距金額</param>
        /// <param name="salesOrderList">SalesOrder清單</param>
        /// <param name="salesOrderItemList">SalesOrderItem清單</param>
        /// <param name="itemTaxDetail">商品稅金與金額細項</param>
        /// <param name="promotionGiftRecords">滿額贈紀錄表</param>
        /// <param name="promotionGiftBasicID">滿額贈活動ID</param>
        /// <param name="promotionGiftIntervalID">滿額贈級距ID</param>
        /// <returns>滿額贈紀錄清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftRecords> CombinePromotionGiftRecords(
            decimal totalPrice
            , decimal promotionGiftPrice
            //, List<SalesOrder> salesOrderList
            //, List<SalesOrderItem> salesOrderItemList
            , List<Models.DomainModels.Cart.SOBase> salesOrderList
            , List<Models.DomainModels.Cart.SOItemBase> salesOrderItemList
            , List<Models.DomainModels.Redeem.GetItemTaxDetail> itemTaxDetail
            , List<Models.DomainModels.Redeem.PromotionGiftRecords> promotionGiftRecords
            , int promotionGiftBasicID
            , int promotionGiftIntervalID)
        {
            decimal tempPrice = 0m;
            List<Models.DomainModels.Redeem.PromotionGiftRecords> tempPromotionGiftRecords = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
            foreach (Models.DomainModels.Cart.SOBase subSalesOrder in salesOrderList)
            {
                List<Models.DomainModels.Cart.SOItemBase> salesOrderItems = salesOrderItemList.Where(x => x.SalesorderCode == subSalesOrder.Code).OrderBy(x => x.Code).ToList();
                int itemID = salesOrderItems.FirstOrDefault().ItemID;
                // 同賣場ID總金額
                decimal priceSum = Math.Floor(0.5m + Convert.ToDecimal(itemTaxDetail.Where(x => x.item_id == itemID).FirstOrDefault().pricetaxdetail.Split(',')[12]));
                // 單一商品平均金額
                decimal singlePrice = Math.Floor(0.5m + (priceSum / salesOrderItems.Count));
                // 總金額暫存
                tempPrice = 0m;

                foreach (Models.DomainModels.Cart.SOItemBase subSalesOrderItem in salesOrderItems)
                {
                    Models.DomainModels.Redeem.PromotionGiftRecords subPromotionGiftRecords = new Models.DomainModels.Redeem.PromotionGiftRecords();
                    subPromotionGiftRecords.SalesOrderItemCode = subSalesOrderItem.Code;
                    subPromotionGiftRecords.PromotionGiftIntervalID = promotionGiftIntervalID;
                    subPromotionGiftRecords.PromotionGiftBasicID = promotionGiftBasicID;
                    subPromotionGiftRecords.DiscountAmount = promotionGiftPrice;
                    subPromotionGiftRecords.UsedStatus = (int)Models.DBModels.TWSQLDB.PromotionGiftRecords.UsedStatusOption.TempUsed;
                    // 暫存該類商品單個商品的分配金額，此金額非真正分配金額只是暫存
                    subPromotionGiftRecords.ApportionedAmount = singlePrice;
                    tempPrice += singlePrice;
                    subPromotionGiftRecords.CreateDate = DateTime.Now;
                    tempPromotionGiftRecords.Add(subPromotionGiftRecords);
                }

                if (tempPrice != priceSum)
                {
                    // 將該類商品總價扣除與暫存總金額後，差價補入該類商品中的第一筆子單中
                    tempPromotionGiftRecords[0].ApportionedAmount += priceSum - tempPrice;
                }
            }

            // 計算出真正的滿額折價分攤金額
            foreach (Models.DomainModels.Redeem.PromotionGiftRecords subPromotionGiftRecords in tempPromotionGiftRecords)
            {
                // 該商品的滿額折價分攤金額 = ((該類商品單個商品的分配金額 / 符合滿額贈的商品總金額) * 滿額贈折價級距金額) 後執行四捨五入
                subPromotionGiftRecords.ApportionedAmount = Math.Floor(0.5m + ((subPromotionGiftRecords.ApportionedAmount / totalPrice) * promotionGiftPrice));
                promotionGiftRecords.Add(subPromotionGiftRecords);
            }

            promotionGiftRecords = promotionGiftRecords.OrderBy(x => x.SalesOrderItemCode).ToList();

            return promotionGiftRecords;
        }

        /// <summary>
        /// 自動分配與拆解滿額贈金額
        /// </summary>
        /// <param name="totalPrice">符合滿額贈的商品總金額</param>
        /// <param name="promotionGiftPrice">滿額贈折價級距金額</param>
        /// <param name="salesOrderList">SalesOrder清單</param>
        /// <param name="salesOrderItemList">SalesOrderItem清單</param>
        /// <param name="itemDisplayPriceList">itemDisplayPriceList細項</param>
        /// <param name="promotionGiftRecords">滿額贈紀錄表</param>
        /// <param name="promotionGiftBasicID">滿額贈活動ID</param>
        /// <param name="promotionGiftIntervalID">滿額贈級距ID</param>
        /// <returns>滿額贈紀錄清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftRecords> CombinePromotionGiftRecordsV2(
            decimal totalPrice
            , decimal promotionGiftPrice
            //, List<SalesOrder> salesOrderList
            //, List<SalesOrderItem> salesOrderItemList
            //, List<ItemDisplayPrice> itemDisplayPriceList
            , List<Models.DomainModels.Cart.SOBase> salesOrderList
            , List<Models.DomainModels.Cart.SOItemBase> salesOrderItemList
            , List<Models.DomainModels.Item.ItemPrice> itemDisplayPriceList
            , List<Models.DomainModels.Redeem.PromotionGiftRecords> promotionGiftRecords
            , int promotionGiftBasicID
            , int promotionGiftIntervalID)
        {
            //decimal tempPrice = 0m;
            List<Models.DomainModels.Redeem.PromotionGiftRecords> tempPromotionGiftRecords = new List<Models.DomainModels.Redeem.PromotionGiftRecords>();
            foreach (Models.DomainModels.Cart.SOBase subSalesOrder in salesOrderList)
            {
                List<Models.DomainModels.Cart.SOItemBase> salesOrderItems = salesOrderItemList.Where(x => x.SalesorderCode == subSalesOrder.Code).OrderBy(x => x.Code).ToList();
                int itemID = salesOrderItems.FirstOrDefault().ItemID;
                // 同賣場ID總金額
                
                //decimal priceSum = Math.Floor(0.5m + Convert.ToDecimal(itemTaxDetail.Where(x => x.item_id == itemID).FirstOrDefault().pricetaxdetail.Split(',')[12]));
                //decimal priceSum = itemDisplayPriceList.Where(x => x.ItemID == itemID).Select(x => x.DisplayPrice).FirstOrDefault() * salesOrderItems.Count;
                // 單一商品平均金額
                decimal singlePrice = itemDisplayPriceList.Where(x => x.ItemID == itemID).Select(x => x.DisplayPrice).FirstOrDefault();
                // 總金額暫存
                //tempPrice = 0m;
                foreach (Models.DomainModels.Cart.SOItemBase subSalesOrderItem in salesOrderItems)
                {
                    Models.DomainModels.Redeem.PromotionGiftRecords subPromotionGiftRecords = new Models.DomainModels.Redeem.PromotionGiftRecords();
                    subPromotionGiftRecords.SalesOrderItemCode = subSalesOrderItem.Code;
                    subPromotionGiftRecords.PromotionGiftIntervalID = promotionGiftIntervalID;
                    subPromotionGiftRecords.PromotionGiftBasicID = promotionGiftBasicID;
                    subPromotionGiftRecords.DiscountAmount = promotionGiftPrice;
                    subPromotionGiftRecords.UsedStatus = (int)Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption.TempUsed;
                    // 暫存該類商品單個商品的分配金額，此金額非真正分配金額只是暫存
                    subPromotionGiftRecords.ApportionedAmount = singlePrice;
                    //tempPrice += singlePrice;
                    subPromotionGiftRecords.CreateDate = DateTime.Now;
                    tempPromotionGiftRecords.Add(subPromotionGiftRecords);
                }

                //if (tempPrice != priceSum)
                //{
                //    // 將該類商品總價扣除與暫存總金額後，差價補入該類商品中的第一筆子單中
                //    tempPromotionGiftRecords[0].ApportionedAmount += priceSum - tempPrice;
                //}
            }

            // 計算出真正的滿額折價分攤金額
            foreach (Models.DomainModels.Redeem.PromotionGiftRecords subPromotionGiftRecords in tempPromotionGiftRecords)
            {
                // 該商品的滿額折價分攤金額 = ((該類商品單個商品的分配金額 / 符合滿額贈的商品總金額) * 滿額贈折價級距金額) 後執行四捨五入
                subPromotionGiftRecords.ApportionedAmount = Math.Floor(0.5m + ((subPromotionGiftRecords.ApportionedAmount / totalPrice) * promotionGiftPrice));
                promotionGiftRecords.Add(subPromotionGiftRecords);
            }

            promotionGiftRecords = promotionGiftRecords.OrderBy(x => x.SalesOrderItemCode).ToList();

            return promotionGiftRecords;
        }

        /// <summary>
        /// 取得紀錄PromotionGift的資訊
        /// </summary>
        /// <param name="oriPriceData">初始金額資訊</param>
        /// <returns>返回滿額贈資訊清單</returns>
        public List<Models.DomainModels.Redeem.GetItemTaxDetail> GetPromotionGiftData(Dictionary<string, List<Models.DomainModels.Redeem.GetItemTaxDetail>> oriPriceData)
        {
            List<Models.DomainModels.Redeem.GetItemTaxDetail> searchPromotionData = oriPriceData.Where(x => x.Key == "Promotion").Select(x => x.Value).FirstOrDefault();

            return searchPromotionData;
        }

        /// <summary>
        /// 根據整個購物車ID, 找出旗下滿額贈的記錄, 修改其PromotionGiftRecord的狀態
        /// </summary>
        /// <param name="argSalesOrderGroupId">購物車的ID</param>
        /// <param name="argUsedStatus">欲修改的狀態</param>
        /// <returns>修改成功:true; 修改失敗:false</returns>
        public bool UpdatePromotionGiftRecordBySOGroupId(int argSalesOrderGroupId, Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption argUsedStatus)
        {
            //PromotionGiftRecordRepository oGiftRecordService = null;
            List<Models.DomainModels.Redeem.PromotionGiftRecords> listGiftRecords = null;
            List<Models.DBModels.TWSQLDB.SalesOrderItem> listDbSoItem = null;
            List<Models.DBModels.TWSQLDB.SalesOrder> listDbSo = null;
            bool boolExec = false;

            if (argSalesOrderGroupId <= 0)
                return false;

            // 根據取得SalesOrderGroupId取得相關的GiftRecords
            //listGiftRecords = oGiftRecordService.GetGiftRecordsBySalesOrderGroupId(argSalesOrderGroupId);
            listGiftRecords = this._PromotionRecordService.GetGiftRecordsBySalesOrderGroupId(argSalesOrderGroupId);

            // 將取得的GiftRecords的狀態, 都修改成傳入的狀態
            if (listGiftRecords != null && listGiftRecords.Count > 0)
            {
                foreach (Models.DomainModels.Redeem.PromotionGiftRecords objSubGiftRecord in listGiftRecords)
                {
                    objSubGiftRecord.UsedStatus = (int)argUsedStatus;
                }
                // end foreach

                //boolExec = oGiftRecordService.UpdatePromotionGiftRecord(listGiftRecords);
                boolExec = this._PromotionRecordService.UpdatePromotionGiftRecordByList(listGiftRecords);
            }
            // end if (listGiftRecords != null && listGiftRecords.Count > 0)
            

            // 釋放所有記憶體
            listGiftRecords = null;
            
            return boolExec;
        }

        /// <summary>
        /// 根據SalesOrderItemCode, 修改其PromotionGiftRecord的狀態
        /// </summary>
        /// <param name="argSoItemCode">SalesOrderItemCode</param>
        /// <param name="argUsedStatus">欲修改的狀態</param>
        /// <returns>修改成功:true; 修改失敗:false</returns>
        public bool UpdatePromotionGiftRecordBySoItemCode(string argSoItemCode, Models.DomainModels.Redeem.PromotionGiftRecords.UsedStatusOption argUsedStatus)
        {
            //PromotionGiftRecordRepository objGiftRecordService = null;
            Models.DomainModels.Redeem.PromotionGiftRecords objGiftRecord = null;
            bool boolExec = false;

            if (argSoItemCode == null || argSoItemCode.Length <= 0)
                return false;

            // 根據傳入的SoItemCode找到對應的PromotionGiftRecord
            objGiftRecord = this._PromotionRecordService.GetGiftRecordsBySalesOrderItemCode(argSoItemCode);
            if (objGiftRecord != null)
            {
                objGiftRecord.UsedStatus = (int)argUsedStatus;
            }

            // 修改GiftRecord
            //boolExec = objGiftRecordService.UpdatePromotionGiftRecord(objGiftRecord);
            boolExec = this._PromotionRecordService.UpdatePromotionGiftRecord(objGiftRecord);

            // 釋放記憶體
            objGiftRecord = null;

            return boolExec;
            
        }
        
        /// <summary>
        /// 訂單填入買一送一優惠訊息
        /// </summary>
        /// <param name="salesOrderGroupID">購物車編號</param>
        /// <param name="buyingItemIDs">購物車商品賣場ID清單</param>
        /// <returns>返回執行結果execResult，若execResult為空字串則表示執行成功，若失敗則返回執行失敗原因</returns>
        public string SalesOrderComment(int salesOrderGroupID, List<int> buyingItemIDs)
        {
            // 執行結果
            string execResult = string.Empty;
            
            // 若無此活動直接返回
            if (twoForOneOffer.Length == 0)
            {
                return execResult;
            }
            // 取得買一送一活動的ID
            int promotionGiftBasicID = Convert.ToInt32(twoForOneOffer);
            List<int> twoForOneOfferItemIDList = null;
            // 需要Insert 買一送一註解的商品
            List<int> commentInserts = null;
            List<Models.DBModels.TWSQLDB.SalesOrder> listSalesOrder = null;
            List<string> listSalesOrderCode = null;
            List<Models.DBModels.TWSQLDB.SalesOrderItem> listSalesOrderItem = null;
            List<string> insertCommentSOCodes = null;
            List<int> listNumItemId = null;
            commentInserts = new List<int>();
            // 找出買一送一活動是否在活動期限內
            Models.DBModels.TWSQLDB.PromotionGiftBasic promotionGiftBasic = this._PromotionRepo.GetAllPromotionGiftBasic().Where(x => x.ID == promotionGiftBasicID && x.StartDate < DateTime.Now && x.EndDate > DateTime.Now).FirstOrDefault();
            // 若無買一送一活動則直接返回不執行下列動作
            if (promotionGiftBasic == null)
            {
                return execResult;
            }
            // 找出買一送一優惠商品ID清單
            //twoForOneOfferItemIDList = db_before.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == promotionGiftBasicID).Select(x => x.ItemID).ToList();
            twoForOneOfferItemIDList = this._PromotionRepo.GetPromotionGiftWhiteList(promotionGiftBasicID).Select(x => x.ItemID).ToList();
            // 若無買一送一商品則直接返回不執行下列動作
            if(twoForOneOfferItemIDList == null)
            {
                return execResult;
            }

            //List<Item> listItem = db_before.Item.Where(x => twoForOneOfferItemIDList.Contains(x.ID)).ToList();
            List<Models.DBModels.TWSQLDB.Item> listItem = this._ItemRepo.GetAll().Where(x => twoForOneOfferItemIDList.Contains(x.ID)).ToList();
            listNumItemId = listItem.Select(x=>x.ID).ToList();
            // 找出買一送一商品庫存數量
            Dictionary<int, int> sellingQtyList = this._ItemStockService.GetSellingQtyByItemList(listNumItemId);
            // 排除買一送一中，商品數量不足2的itemID
            foreach (var subItemQty in sellingQtyList)
            {
                if (subItemQty.Value < 2)
                {
                    twoForOneOfferItemIDList.Remove(subItemQty.Key);
                }
            }
            // 找出購買的商品內是否有需要加入買一送一註解的商品
            buyingItemIDs.Where(x => twoForOneOfferItemIDList.Contains(x)).ToList().ForEach(x => commentInserts.Add(x));
            // 若購買商品內含買一送一優惠商品則執行下列動作
            if (commentInserts.Count > 0)
            {
                // 找出該購物車的所有SalesOrder清單
                //listSalesOrder = db_before.SalesOrder.Where(x => x.SalesOrderGroupID == salesOrderGroupID).ToList();
                listSalesOrder = this._SoRepo.GetSOs(salesOrderGroupID).ToList();
                // 找出該車所有SalesOrderCode
                listSalesOrderCode = listSalesOrder.Select(x => x.Code).ToList();
                // 找出資料庫中該車中為買一送一優惠商品的SalesOrderItem清單
                //listSalesOrderItem = db_before.SalesOrderItem.Where(x => listSalesOrderCode.Contains(x.SalesorderCode) && commentInserts.Contains(x.ItemID)).ToList();
                listSalesOrderItem = this._SoRepo.GetSOItemsByCodes(listSalesOrderCode).Where(x => commentInserts.Contains(x.ItemID)).ToList();
                // 找出需填入買一送一優惠訊息的SalesOrderCodes
                insertCommentSOCodes = listSalesOrderItem.Select(x=>x.SalesorderCode).ToList();
                listSalesOrder = listSalesOrder.Where(x => insertCommentSOCodes.Contains(x.Code)).ToList();
                
                // 資料儲存
                //listSalesOrder.ForEach(x => x.Note2 += "<買一送一優惠>");
                try
                {
                    //db_before.SaveChanges();
                    // 將買一送一優惠的訊息填入該SalesOrder的Note2裡面
                    foreach(Models.DBModels.TWSQLDB.SalesOrder objSubSo in listSalesOrder)
                    {
                        objSubSo.Note2 += "<買一送一優惠>";
                        this._SoRepo.UpdateSO(objSubSo);
                    }
                }
                catch (Exception e)
                {
                    // 若失敗則返回造成資料儲存失敗的原因
                    execResult = "[ErrorMessage] " + e.Message + " [ErrorStackTrace] " + e.StackTrace;
                }
            }

            // 返回執行結果
            return execResult;
        }

        /// <summary>
        /// 新增PromotionGiftBasic
        /// </summary>
        /// <param name="argObjPromotionGiftBasic">新增的PromotionGiftBasic物件</param>
        /// <returns>create success return new PromotionGiftBasic ID, else return 0</returns>
        public int CreatePromotionGiftBasic(Models.DomainModels.Redeem.PromotionGiftBasic argObjPromotionGiftBasic)
        {
            // 新增活動時，狀態一定要是下線
            if (argObjPromotionGiftBasic == null || argObjPromotionGiftBasic.Status != (int)TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.NotUsed)
            {
                return 0;
            }

            int numPromotionGiftBasicId = 0;
            Models.DBModels.TWSQLDB.PromotionGiftBasic objDbGiftBasic = null;

            try
            {
                objDbGiftBasic = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftBasic>(argObjPromotionGiftBasic);
                objDbGiftBasic.CreateDate = DateTime.Now;
                this._PromotionRepo.CreatePromotionGiftBasic(objDbGiftBasic);
                numPromotionGiftBasicId = objDbGiftBasic.ID;
            }
            catch
            {
                numPromotionGiftBasicId = 0;
            }

            return numPromotionGiftBasicId;
        }

        /// <summary>
        /// 修改PromotionGiftBasic
        /// </summary>
        /// <param name="argObjPromotionGiftBasic"></param>
        /// <returns>update success return true, else return false</returns>
        public ActionResponse<bool> UpdatePromotionGiftBasic(Models.DomainModels.Redeem.PromotionGiftBasic argObjPromotionGiftBasic)
        {
            ActionResponse<bool> result = new ActionResponse<bool>();
            result.Body = false;
            Models.DBModels.TWSQLDB.PromotionGiftBasic objDbPromotionGiftBasic = null;
            List<Models.DomainModels.Redeem.PromotionGiftInterval> getPromotionGiftIntervalListByBasicId = null;

            bool isException = true;

            try
            {
                if (argObjPromotionGiftBasic == null)
                {
                    isException = false;
                    throw new Exception("未傳入參數");
                }

                // 若修改活動成上線或測試，則先檢查級距是否有資料
                if (argObjPromotionGiftBasic.Status != (int)TWNewEgg.Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.NotUsed)
                {
                    // 取得級距資料
                    getPromotionGiftIntervalListByBasicId = this.GetPromotionGiftIntervalListByBasicId(argObjPromotionGiftBasic.ID);

                    if (getPromotionGiftIntervalListByBasicId == null || getPromotionGiftIntervalListByBasicId.All(x => x == null))
                    {
                        isException = false;
                        throw new Exception("未設定級距");
                    }

                    getPromotionGiftIntervalListByBasicId = null;
                }

                objDbPromotionGiftBasic = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftBasic>(argObjPromotionGiftBasic);
                objDbPromotionGiftBasic.UpdateDate = DateTime.Now;
                this._PromotionRepo.UpdatePromotionGiftBasic(objDbPromotionGiftBasic);

                result.Body = true;
                result.IsSuccess = true;
                result.Code = (int)ResponseCode.Success;
                result.Msg = "修改成功。";
            }
            catch(Exception ex)
            {
                result.Body = false;
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Msg = "修改失敗。";

                if (isException == false)
                {
                    result.Msg += string.Format("失敗原因：{0}。", ex.Message);
                }
            }

            return result;
        }

        /// <summary>
        /// 根據PromotionGiftBasic取得白名單清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic.ID</param>
        /// <returns>返回白名單清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftWhiteList> GetPromotionGiftWhiteListByBasicId(int argNumPromotionGiftBasicId)
        {
            if (argNumPromotionGiftBasicId <= 0)
            {
                return null;
            }

            List<Models.DomainModels.Redeem.PromotionGiftWhiteList> listPromotionGiftWhite = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listDbGiftWhiteList = null;

            //listPromotionGiftWhite = objDb.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).DefaultIfEmpty().ToList();
            listDbGiftWhiteList = this._PromotionRepo.GetPromotionGiftWhiteList(argNumPromotionGiftBasicId).ToList();
            if (listDbGiftWhiteList != null && listDbGiftWhiteList.Count > 0)
            {
                try
                {
                    listPromotionGiftWhite = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftWhiteList>>(listDbGiftWhiteList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return listPromotionGiftWhite;
        }

        /// <summary>
        /// 根據PromotionGiftBasic Id 及PromotionGiftWhiteList Id取得黑名單物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argNumPromotionGiftWhiteListId">PromotionGiftBasicList Id</param>
        /// <returns>null or PromotionGiftWhiteList object</returns>
        public Models.DomainModels.Redeem.PromotionGiftWhiteList GetPromotionGiftWhiteList(int argNumPromotionGiftBasicId, int argNumPromotionGiftWhiteListId)
        {
            if (argNumPromotionGiftBasicId <= 0 || argNumPromotionGiftWhiteListId <= 0)
            {
                return null;
            }

            Models.DomainModels.Redeem.PromotionGiftWhiteList objWhite = null;
            Models.DBModels.TWSQLDB.PromotionGiftWhiteList objDbWhite = null;

            //objWhite = objDb.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ID == argNumPromotionGiftWhiteListId).FirstOrDefault();
            objDbWhite = this._PromotionRepo.GetPromotionGiftWhiteList(argNumPromotionGiftBasicId).Where(x => x.ID == argNumPromotionGiftWhiteListId).FirstOrDefault();
            if (objDbWhite != null)
            {
                try
                {
                    objWhite = ModelConverter.ConvertTo<Models.DomainModels.Redeem.PromotionGiftWhiteList>(objDbWhite);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objWhite;
        }

        /// <summary>
        /// 新增一個WhiteList物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftWhiteList">object of PromotionGiftWhiteList</param>
        /// <returns>created success return WhiteList Id, else return 0</returns>
        public int CreatePromotionGiftWhiteList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftWhiteList argObjPromotionGiftWhiteList)
        {
            if (argNumPromotionGiftBasicId <= 0 || argObjPromotionGiftWhiteList == null)
                return 0;

            int numWhiteId = 0;
            Models.DBModels.TWSQLDB.PromotionGiftWhiteList objDbWhite = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listDbWhiteList = null;
            try
            {
                listDbWhiteList = this._PromotionRepo.GetAllPromotionGiftWhiteList().Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ItemID == argObjPromotionGiftWhiteList.ItemID).ToList();
                if (listDbWhiteList != null && listDbWhiteList.Count > 0)
                {
                    return 0;
                }

                objDbWhite = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>(argObjPromotionGiftWhiteList);
                objDbWhite.CreateDate = DateTime.Now;
                this._PromotionRepo.CreatePromotionGiftWhiteList(objDbWhite);
                numWhiteId = objDbWhite.ID;
            }
            catch
            {
                numWhiteId = 0;
            }

            return numWhiteId;
        }

        /// <summary>
        /// 更新該PromotionGifbtBasic下的White清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argListPromotionGiftWhiteList">White List</param>
        /// <returns>update success return true, else return false</returns>
        public bool UpdatePromotionGiftWhiteListByList(int argNumPromotionGiftBasicId, List<Models.DomainModels.Redeem.PromotionGiftWhiteList> argListPromotionGiftWhiteList)
        {
            if (argNumPromotionGiftBasicId <= 0)
            {
                return false;
            }

            //TWSqlDBContext objDb = null;
            bool boolExec = false;
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listDbWhiteList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> listDbUpdateWhiteList = null;
            Models.DBModels.TWSQLDB.PromotionGiftWhiteList objDbTempWhiteList = null;


            //若傳入的WhiteList為空白, 表示要清除所有的WhiteList
            if (argListPromotionGiftWhiteList == null)
            {
                //objDb.PromotionGiftWhiteList.SqlQuery("update PromotionGiftWhiteList set Status=" + Convert.ToString(Convert.ToInt32(PromotionGiftWhiteList.WhiteListStatus.NotUsed)) + " where PromotionGiftBasicID=" + Convert.ToString(argNumPromotionGiftBasicId));
                this._PromotionRepo.GetDatabase().ExecuteSqlCommand("update PromotionGiftWhiteList set Status=" + Convert.ToString(Convert.ToInt32(Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.NotUsed)) + " where PromotionGiftBasicID=" + Convert.ToString(argNumPromotionGiftBasicId));
            }
            else
            {
                //取得Old WhiteList及其ID
                //listDbWhiteList = objDb.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).DefaultIfEmpty().ToList();
                listDbWhiteList = this._PromotionRepo.GetPromotionGiftWhiteList(argNumPromotionGiftBasicId).ToList();

                if (listDbWhiteList == null)
                {
                    listDbWhiteList = new List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>();
                    //若沒有舊資料, 全部新增
                    foreach (Models.DomainModels.Redeem.PromotionGiftWhiteList objSubWhite in argListPromotionGiftWhiteList)
                    {
                        objDbTempWhiteList = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>(objSubWhite);
                        objDbTempWhiteList.CreateDate = DateTime.Now;
                        listDbWhiteList.Add(objDbTempWhiteList);
                    }
                }
                else
                {
                    //若有資料, 則依資料是否存在於資料庫進行新增或修改
                    listDbUpdateWhiteList = new List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>();
                    foreach (Models.DomainModels.Redeem.PromotionGiftWhiteList objSubWhite in argListPromotionGiftWhiteList)
                    {
                        objDbTempWhiteList = listDbWhiteList.Where(x => x.PromotionGiftBasicID == objSubWhite.PromotionGiftBasicID && x.ItemID == objSubWhite.ItemID).FirstOrDefault();
                        if (objDbTempWhiteList == null)
                        {
                            //新增
                            objDbTempWhiteList = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>(objSubWhite);
                            objDbTempWhiteList.CreateDate = DateTime.Now;
                            listDbWhiteList.Add(objDbTempWhiteList);
                        }
                        else
                        {
                            //修改
                            objDbTempWhiteList = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>(objSubWhite);
                            objDbTempWhiteList.UpdateUser = objSubWhite.UpdateUser;
                            objDbTempWhiteList.UpdateDate = DateTime.Now;
                            listDbUpdateWhiteList.Add(objDbTempWhiteList);
                        }
                    }////end foreach
                }
            }

            ///儲存資料
            try
            {
                if (listDbWhiteList != null && listDbWhiteList.Count > 0)
                {
                    this._PromotionRepo.CreateRangePromotionGiftWhiteList(listDbWhiteList);
                }
                if (listDbUpdateWhiteList != null && listDbUpdateWhiteList.Count > 0)
                {
                    this._PromotionRepo.UpdateRangePromotionGiftWhiteList(listDbUpdateWhiteList);
                }
                boolExec = true;
            }
            catch
            {
                boolExec = false;
            }

            return boolExec;
        }

        /// <summary>
        /// 更新該PromotionGifbtBasic下的White清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftWhiteList">White List object</param>
        /// <returns>updated success return true, else return false</returns>
        public bool UpdatePromotionGiftWhiteList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftWhiteList argObjPromotionGiftWhiteList)
        {
            if (argNumPromotionGiftBasicId <= 0 || argObjPromotionGiftWhiteList == null || argObjPromotionGiftWhiteList.UpdateUser == null || argObjPromotionGiftWhiteList.UpdateUser.Trim().Length <= 0)
            {
                return false;
            }

            bool boolExec = false;
            Models.DBModels.TWSQLDB.PromotionGiftWhiteList objDbWhiteList = null;

            //objDbWhiteList = objDb.PromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ItemID == argObjPromotionGiftWhiteList.ItemID).FirstOrDefault();
            objDbWhiteList = this._PromotionRepo.GetPromotionGiftWhiteList(argNumPromotionGiftBasicId).Where(x => x.ItemID == argObjPromotionGiftWhiteList.ItemID).FirstOrDefault();

            if (objDbWhiteList != null)
            {
                try
                {
                    objDbWhiteList = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>(argObjPromotionGiftWhiteList);
                    objDbWhiteList.UpdateDate = DateTime.Now;
                    this._PromotionRepo.UpdatePromotionGiftWhiteList(objDbWhiteList);
                    boolExec = true;
                }
                catch
                {
                    boolExec = false;
                }
            }

            return boolExec;
        }

        /// <summary>
        /// 根據PromotionGiftBasic清單取得黑名單清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic.ID</param>
        /// <returns>返回黑名單清單</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftBlackList> GetPromotionGiftBlackListByBasicId(int argNumPromotionGiftBasicId)
        {
            if (argNumPromotionGiftBasicId <= 0)
                return null;

            List<Models.DomainModels.Redeem.PromotionGiftBlackList> listPromotionGiftBlack = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listDbGiftBlack = null;

            //listPromotionGiftBlack = objDb.PromotionGiftBlackList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).DefaultIfEmpty().ToList();
            listDbGiftBlack = this._PromotionRepo.GetPromotionGiftBlackList(argNumPromotionGiftBasicId).ToList();

            if (listDbGiftBlack != null && listDbGiftBlack.Count > 0)
            {
                try
                {
                    listPromotionGiftBlack = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBlackList>>(listDbGiftBlack);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return listPromotionGiftBlack;
        }

        /// <summary>
        /// 根據PromotionGiftBasic Id 及PromotionGiftBlackList Id取得黑名單物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argNumPromotionGiftBlackListId">PromotionGiftBasicList Id</param>
        /// <returns>null or PromotionGiftBlackList object</returns>
        public Models.DomainModels.Redeem.PromotionGiftBlackList GetPromotionGiftBlackList(int argNumPromotionGiftBasicId, int argNumPromotionGiftBlackListId)
        {
            if (argNumPromotionGiftBasicId <= 0 || argNumPromotionGiftBlackListId <= 0)
                return null;

            Models.DomainModels.Redeem.PromotionGiftBlackList objBlack = null;
            Models.DBModels.TWSQLDB.PromotionGiftBlackList objDbBlack = null;

            //objBlack = objDb.PromotionGiftBlackList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ID == argNumPromotionGiftBlackListId).FirstOrDefault();
            objDbBlack = this._PromotionRepo.GetPromotionGiftBlackList(argNumPromotionGiftBasicId).Where(x => x.ID == argNumPromotionGiftBlackListId).FirstOrDefault();
            if (objDbBlack != null)
            {
                objBlack = ModelConverter.ConvertTo<Models.DomainModels.Redeem.PromotionGiftBlackList>(objDbBlack);
            }

            return objBlack;
        }

        /// <summary>
        /// 新增一個BlackList物件
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftBlackList">object of PromotionGiftBlackList</param>
        /// <returns>created success return BlackList Id, else return 0</returns>
        public int CreatePromotionGiftBlackList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftBlackList argObjPromotionGiftBlackList)
        {
            if (argNumPromotionGiftBasicId <= 0 || argObjPromotionGiftBlackList == null)
                return 0;

            int numBlackId = 0;
            Models.DBModels.TWSQLDB.PromotionGiftBlackList objDbGiftBlack = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listDbBlackList = null;
            try
            {
                listDbBlackList = this._PromotionRepo.GetAllPromotionGiftBlackList().Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ItemID == argObjPromotionGiftBlackList.ItemID).ToList();
                if (listDbBlackList != null && listDbBlackList.Count > 0)
                {
                    return 0;
                }

                objDbGiftBlack = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftBlackList>(argObjPromotionGiftBlackList);
                objDbGiftBlack.CreateDate = DateTime.Now;
                this._PromotionRepo.CreatePromotionGiftBlackList(objDbGiftBlack);
                numBlackId = objDbGiftBlack.ID;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return numBlackId;
        }

        /// <summary>
        /// 更新該PromotionGifbtBasic下的Black清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argListPromotionGiftBlackList">Black List</param>
        /// <returns></returns>
        public bool UpdatePromotionGiftBlackListByList(int argNumPromotionGiftBasicId, List<Models.DomainModels.Redeem.PromotionGiftBlackList> argListPromotionGiftBlackList)
        {
            if (argNumPromotionGiftBasicId <= 0)
                return false;

            bool boolExec = false;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listOldBlackList = null;
            Models.DBModels.TWSQLDB.PromotionGiftBlackList objTempDbBlackList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listDbAddBlackList = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> listDbUpdateBlackList = null;

            //若傳入的BlackList為空白, 表示要清除所有的BlackList
            if (argListPromotionGiftBlackList == null)
            {
                
                //objDb.PromotionGiftBlackList.SqlQuery("update PromotionGiftBlackList set Status=" + Convert.ToString(Convert.ToInt32(Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.NotUsed)) + " where PromotionGiftBasicID=" + Convert.ToString(argNumPromotionGiftBasicId));
                this._PromotionRepo.GetDatabase().ExecuteSqlCommand("update PromotionGiftBlackList set Status=" + Convert.ToString(Convert.ToInt32(Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.NotUsed)) + " where PromotionGiftBasicID=" + Convert.ToString(argNumPromotionGiftBasicId));
            }
            else
            {
                //取得Old BlackList及其ID
                //listOldBlackList = objDb.PromotionGiftBlackList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).DefaultIfEmpty().ToList();
                listOldBlackList = this._PromotionRepo.GetAllPromotionGiftBlackList().Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).DefaultIfEmpty().ToList();

                if (listOldBlackList == null)
                {
                    //若沒有舊資料, 全部新增
                    foreach (Models.DomainModels.Redeem.PromotionGiftBlackList objSubBlack in argListPromotionGiftBlackList)
                    {
                        objSubBlack.CreateDate = DateTime.Now;
                        //objDb.PromotionGiftBlackList.Add(objSubBlack);
                    }
                    listDbAddBlackList = ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.PromotionGiftBlackList>>(argListPromotionGiftBlackList);
                }
                else
                {
                    //若有資料, 進行變更
                    listDbAddBlackList = new List<Models.DBModels.TWSQLDB.PromotionGiftBlackList>();
                    listDbUpdateBlackList = new List<Models.DBModels.TWSQLDB.PromotionGiftBlackList>();
                    foreach (Models.DomainModels.Redeem.PromotionGiftBlackList objSubBlack in argListPromotionGiftBlackList)
                    {
                        objTempDbBlackList = listOldBlackList.Where(x => x.PromotionGiftBasicID == objSubBlack.PromotionGiftBasicID && x.ItemID == objSubBlack.ItemID).FirstOrDefault();
                        if (objTempDbBlackList == null)
                        {
                            //新增
                            objSubBlack.CreateDate = DateTime.Now;
                            //objDb.PromotionGiftBlackList.Add(objSubBlack);
                            listDbAddBlackList.Add(objTempDbBlackList);
                        }
                        else
                        {
                            //修改
                            objTempDbBlackList.Status = objSubBlack.Status;
                            objTempDbBlackList.UpdateUser = objSubBlack.UpdateUser;
                            objTempDbBlackList.UpdateDate = DateTime.Now;
                            listDbUpdateBlackList.Add(objTempDbBlackList);
                        }
                    }
                }
            }

            try
            {
                //儲存所有變更
                //objDb.SaveChanges();
                if (listDbAddBlackList != null && listDbAddBlackList.Count > 0)
                {
                    this._PromotionRepo.CreateRangePromotionGiftBlackList(listDbAddBlackList);
                }

                if (listDbUpdateBlackList != null && listDbUpdateBlackList.Count > 0)
                {
                    this._PromotionRepo.UpdateRangePromotionGiftBlackList(listDbUpdateBlackList);
                }
                
                boolExec = true;
            }
            catch
            {
                boolExec = false;
            }

            return boolExec;
        }

        /// <summary>
        /// 更新該PromotionGifbtBasic下的Black清單
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftBlackList">Black List object</param>
        /// <returns>updated success return true, else return false</returns>
        public bool UpdatePromotionGiftBlackList(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftBlackList argObjPromotionGiftBlackList)
        {
            if (argNumPromotionGiftBasicId <= 0 || argObjPromotionGiftBlackList == null || argObjPromotionGiftBlackList.UpdateUser == null || argObjPromotionGiftBlackList.UpdateUser.Trim().Length <= 0)
                return false;

            //TWSqlDBContext objDb = null;
            bool boolExec = false;
            Models.DBModels.TWSQLDB.PromotionGiftBlackList objDbBlackList = null;

            //objDbBlackList = objDb.PromotionGiftBlackList.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ItemID == argObjPromotionGiftBlackList.ItemID).FirstOrDefault();
            objDbBlackList = this._PromotionRepo.GetAllPromotionGiftBlackList().Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ItemID == argObjPromotionGiftBlackList.ItemID).FirstOrDefault();

            if (objDbBlackList != null)
            {
                objDbBlackList.Status = argObjPromotionGiftBlackList.Status;
                objDbBlackList.UpdateUser = argObjPromotionGiftBlackList.UpdateUser;
                objDbBlackList.UpdateDate = DateTime.Now;

                try
                {
                    //objDb.SaveChanges();
                    this._PromotionRepo.UpdatePromotionGiftBlackList(objDbBlackList);
                    boolExec = true;
                }
                catch
                {
                    boolExec = false;
                }
            }

            return boolExec;
        }

        /// <summary>
        /// 新增級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftInterval">新級的級距物件</param>
        /// <returns>if create success return interval object id , else return 0</returns>
        public int CreatePromotionGiftInterval(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftInterval argObjPromotionGiftInterval)
        {
            int numIntervalId = 0;
            Models.DBModels.TWSQLDB.PromotionGiftBasic objDbBasic = null;
            Models.DBModels.TWSQLDB.PromotionGiftInterval objDbGiftInterval = null;

            //objBasic = objDb.PromotionGiftBasic.Where(x => x.ID == argNumPromotionGiftBasicId).FirstOrDefault();
            objDbBasic = this._PromotionRepo.GetAllPromotionGiftBasic().Where(x => x.ID == argNumPromotionGiftBasicId).FirstOrDefault();
            if (objDbBasic != null)
            {
                try
                {
                    objDbGiftInterval = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftInterval>(argObjPromotionGiftInterval);
                    objDbGiftInterval.CreateDate = DateTime.Now;
                    this._PromotionRepo.CreatePromotionGiftInterval(objDbGiftInterval);
                    numIntervalId = objDbGiftInterval.ID;
                }
                catch
                {
                    numIntervalId = 0;
                }

                objDbBasic = null;
            }

            return numIntervalId;
        }

        /// <summary>
        /// 刪除級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId"></param>
        /// <param name="argObjPromotionGiftInterval"></param>
        /// <returns></returns>
        public string DeletePromotionGiftInterval(int PromotionGiftIntervalId)
        {
            string Msg = "";
            if (PromotionGiftIntervalId <= 0)
            {
                Msg = "刪除失敗!抓取ID失敗";
            }
            else
            {
                Models.DBModels.TWSQLDB.PromotionGiftInterval objDbInterval = null;
                objDbInterval = this._PromotionRepo.GetAllPromotionGiftInterval().Where(x => x.ID == PromotionGiftIntervalId).FirstOrDefault();
                if (objDbInterval != null)
                    this._PromotionRepo.DeletePromotionGiftInterval(objDbInterval);

            }
            return Msg;
        }
        /// <summary>
        /// 修改級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argObjPromotionGiftInterval">要修改的級距</param>
        /// <returns>if updated success return true, else return false</returns>
        public bool UpdatePromotionGiftInterval(int argNumPromotionGiftBasicId, Models.DomainModels.Redeem.PromotionGiftInterval argObjPromotionGiftInterval)
        {
            if (argNumPromotionGiftBasicId <= 0 || argObjPromotionGiftInterval == null || argObjPromotionGiftInterval.UpdateUser == null || argObjPromotionGiftInterval.UpdateUser.Trim().Length <= 0)
            {
                return false;
            }

            Models.DBModels.TWSQLDB.PromotionGiftInterval objDbInterval = null;
            bool boolExec = false;

            //objInterval = objDb.PromotionGiftInterval.Where(x => x.ID == argObjPromotionGiftInterval.ID).FirstOrDefault();
            objDbInterval = this._PromotionRepo.GetAllPromotionGiftInterval().Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId && x.ID == argObjPromotionGiftInterval.ID).FirstOrDefault();

            if (objDbInterval != null)
            {
                try
                {
                    objDbInterval = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftInterval>(argObjPromotionGiftInterval);
                    objDbInterval.UpdateDate = DateTime.Now;
                    this._PromotionRepo.UpdatePromotionGiftInterval(objDbInterval);
                    boolExec = true;
                }
                catch
                {
                    boolExec = false;
                }
            }

            return boolExec;
        }

        /// <summary>
        /// 修改級距
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <param name="argListPromotionGiftInterval">要修改的級距列表</param>
        /// <returns>updated success return true, else return false</returns>
        public bool UpdatePromotionGiftIntervalList(int argNumPromotionGiftBasicId, List<Models.DomainModels.Redeem.PromotionGiftInterval> argListPromotionGiftInterval)
        {
            if (argNumPromotionGiftBasicId <= 0 || argListPromotionGiftInterval == null)
                return false;

            List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listDbAddGiftInterval = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listDbSearchGiftInterval = null;
            List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listDbUpdateGiftInterval = null;
            Models.DBModels.TWSQLDB.PromotionGiftInterval objDbTempInterval = null;
            bool boolExec = false;


            //取得原來舊的Interval List
            //listDbAddGiftInterval = objDb.PromotionGiftInterval.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).DefaultIfEmpty().ToList();
            listDbSearchGiftInterval = this._PromotionRepo.GetPromotionGiftInterval(argNumPromotionGiftBasicId).ToList();

            if (listDbSearchGiftInterval == null || listDbSearchGiftInterval.Count <= 0)
            {
                //若是舊的Interval不存在,就新增所有的Interval列表
                listDbAddGiftInterval = new List<Models.DBModels.TWSQLDB.PromotionGiftInterval>();
                foreach (Models.DomainModels.Redeem.PromotionGiftInterval objSubInterval in argListPromotionGiftInterval)
                {
                    objDbTempInterval = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftInterval>(objSubInterval);
                    objDbTempInterval.UpdateDate = DateTime.Now;
                    listDbAddGiftInterval.Add(objDbTempInterval);
                }
            }
            else
            {
                //若是舊的存在, 就比對之後一一更新Interval物件
                listDbAddGiftInterval = new List<Models.DBModels.TWSQLDB.PromotionGiftInterval>();
                listDbUpdateGiftInterval = new List<Models.DBModels.TWSQLDB.PromotionGiftInterval>();
                foreach (Models.DomainModels.Redeem.PromotionGiftInterval objSubInterval in argListPromotionGiftInterval)
                {
                    objDbTempInterval = listDbSearchGiftInterval.Where(x => x.ID == objSubInterval.ID).FirstOrDefault();
                    if (objDbTempInterval == null)
                    {
                        //新增
                        objDbTempInterval = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftInterval>(objSubInterval);
                        objDbTempInterval.CreateUser = objSubInterval.CreateUser;
                        objDbTempInterval.CreateDate = DateTime.Now;
                        listDbAddGiftInterval.Add(objDbTempInterval);
                    }
                    else
                    {
                        //修改
                        objDbTempInterval = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.PromotionGiftInterval>(objSubInterval);
                        objDbTempInterval.UpdateDate = DateTime.Now;
                        listDbUpdateGiftInterval.Add(objDbTempInterval);
                    }
                }
            }

            //儲存變更
            try
            {
                if (listDbAddGiftInterval != null && listDbAddGiftInterval.Count > 0)
                {
                    this._PromotionRepo.CreateRangePromotionGiftInterval(listDbAddGiftInterval);
                }

                if (listDbUpdateGiftInterval != null && listDbUpdateGiftInterval.Count > 0)
                {
                    this._PromotionRepo.UpdateRangePromotionGiftInterval(listDbUpdateGiftInterval);
                }
                boolExec = true;
            }
            catch
            {
                boolExec = false;
            }

            return boolExec;
        }

        /// <summary>
        /// 根據Basic Id取得其相關的級距列表
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <returns>null or List of PromotionGiftInterval</returns>
        public List<Models.DomainModels.Redeem.PromotionGiftInterval> GetPromotionGiftIntervalListByBasicId(int argNumPromotionGiftBasicId)
        {
            if (argNumPromotionGiftBasicId <= 0)
            {
                return null;
            }

            List<Models.DBModels.TWSQLDB.PromotionGiftInterval> listDbGiftInterval = null;
            List<Models.DomainModels.Redeem.PromotionGiftInterval> listPromotionGiftInterval = null;

            //listPromotionGiftInterval = objDb.PromotionGiftInterval.Where(x => x.PromotionGiftBasicID == argNumPromotionGiftBasicId).OrderBy(x => x.LowerLimit).DefaultIfEmpty().ToList();
            listDbGiftInterval = this._PromotionRepo.GetPromotionGiftInterval(argNumPromotionGiftBasicId).OrderBy(x => x.LowerLimit).DefaultIfEmpty().ToList();
            if (listDbGiftInterval != null && listDbGiftInterval.Count > 0)
            {
                listPromotionGiftInterval = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftInterval>>(listDbGiftInterval);
            }

            return listPromotionGiftInterval;
        }

        /// <summary>
        /// 根據Basic Id刪除其名單
        /// </summary>
        public ActionResponse<string> DeleteList(int argNumPromotionGiftBasicId, string flag)
        {
            ActionResponse<string> result = new ActionResponse<string>();
            if (argNumPromotionGiftBasicId <= 0)
                return null;

            if (string.IsNullOrEmpty(flag))
            {
                result.IsSuccess = false;
                result.Msg = "意外錯誤";
                return result;
            }

            try
            {
                switch (flag)
                {
                    case "black":
                        result.IsSuccess = this._PromotionRepo.DeletePromotionGiftBlackListByBasicId(argNumPromotionGiftBasicId);
                        break;
                    case "white":
                        result.IsSuccess = this._PromotionRepo.DeletePromotionGiftWhiteListByBasicId(argNumPromotionGiftBasicId);
                        break;
                    default:
                        result.IsSuccess = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 根據Basic Id取得其相關的級距列表
        /// </summary>
        /// <param name="argNumPromotionGiftBasicId">PromotionGiftBasic Id</param>
        /// <returns>null or List of PromotionGiftExportToExcel</returns>
        /// 
        public ActionResponse<List<Models.DomainModels.Redeem.PromotionGiftExportToExcel>> PromotionGiftBasicIdToExcel(int PromotionGiftBasicId, string ListType)
        {
            ActionResponse<List<Models.DomainModels.Redeem.PromotionGiftExportToExcel>> result = new ActionResponse<List<Models.DomainModels.Redeem.PromotionGiftExportToExcel>>();
            List<Models.DBModels.TWSQLDB.PromotionGiftBlackList> PromotionGiftBlackList = new List<Models.DBModels.TWSQLDB.PromotionGiftBlackList>();
            List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList> PromotionGiftWhiteList = new List<Models.DBModels.TWSQLDB.PromotionGiftWhiteList>();
            List<Models.DomainModels.Redeem.PromotionGiftExportToExcel> tempDataList = new List<Models.DomainModels.Redeem.PromotionGiftExportToExcel>();
            result.IsSuccess = true;
            result.Code = (int)ResponseCode.Success;
            result.Msg = "";

            try
            {
                //撈全部資料
                if (ListType == "black")
                {
                    //call 黑名單service拿資料
                    PromotionGiftBlackList = this._PromotionRepo.GetPromotionGiftBlackList(PromotionGiftBasicId).ToList();
                    //若第一筆為空，則無資料
                    if (PromotionGiftBlackList == null || PromotionGiftBlackList.Count <= 0)
                    {
                        Models.DomainModels.Redeem.PromotionGiftExportToExcel tempData = new Models.DomainModels.Redeem.PromotionGiftExportToExcel();
                        tempData.ItemID = "";
                        tempData.Status = "";
                        tempDataList.Add(tempData);
                        //logger.Info("GetPromotionGiftBlackList" + PromotionGiftBasicId + "為空值。");
                    }
                    else
                    {
                        //若第一筆不為空，則有資料
                        foreach (Models.DBModels.TWSQLDB.PromotionGiftBlackList item in PromotionGiftBlackList)
                        {
                            //開始塞進model
                            PromotionGiftExportToExcel tempData = new PromotionGiftExportToExcel();
                            //itemid
                            tempData.ItemID = item.ItemID.ToString();                           
                            //將狀態改為國語顯示                            
                            tempData.Status = ((PromotionGiftExportToExcel.Status2)item.Status).ToString();                            
                            //將一筆資料塞入list裡面
                            tempDataList.Add(tempData);
                        }
                    }
                }
                else
                {
                    //call 白名單service拿資料
                    PromotionGiftWhiteList = this._PromotionRepo.GetPromotionGiftWhiteList(PromotionGiftBasicId).ToList();
                    //若第一筆為空，則無資料
                    if (PromotionGiftWhiteList == null || PromotionGiftWhiteList.Count <= 0)
                    {
                        PromotionGiftExportToExcel tempData = new PromotionGiftExportToExcel();
                        tempData.ItemID = "";
                        tempData.Status = "";
                        tempDataList.Add(tempData);
                        //logger.Info("GetPromotionGiftBlackList" + PromotionGiftBasicId + "為空值。");
                    }
                    else
                    {
                        //若第一筆不為空，則有資料
                        foreach (Models.DBModels.TWSQLDB.PromotionGiftWhiteList item in PromotionGiftWhiteList)
                        {
                            //開始塞進model
                            PromotionGiftExportToExcel tempData = new PromotionGiftExportToExcel();
                            //itemid
                            tempData.ItemID = item.ItemID.ToString();
                            //將狀態改為國語顯示
                            tempData.Status = ((PromotionGiftExportToExcel.Status2)item.Status).ToString();
                            //將一筆資料塞入list裡面
                            tempDataList.Add(tempData);
                        }
                    }
                }
                if (result.IsSuccess)
                {
                    result.IsSuccess = true;
                    result.Code = (int)ResponseCode.Success;
                    result.Msg = "成功查詢";
                    result.Body = tempDataList;
                    //logger.Info("PromotionGiftRespository\\PromotionGiftBasicIdToExcel 成功查詢。 ID: "+ PromotionGiftBasicId +" 資料量 :" + tempDataList.Count);
                }
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = null;
                result.Msg = ex.Message + " [StackTrace] : " + ex.StackTrace;
                //logger.Info("PromotionGiftRespository\\PromotionGiftBasicIdToExcel error: " + ex.Message + " [StackTrace] : " + ex.StackTrace);
                return result;

            }
        }

        public ActionResponse<string> BatchCreatePomoteBasicPromotionGift(int basicID, List<int> ItemID, List<int> Status, string list,string UserName)
        {
            ActionResponse<string> deleteResult = new ActionResponse<string>();
            ActionResponse<string> creatResult = new ActionResponse<string>();

            try
            {
                if (ItemID.Count() == 1 && ItemID[0] == 0)
                {
                    creatResult.IsSuccess = false;
                    creatResult.Code = (int)ResponseCode.AccessError;
                    creatResult.Msg = "批次匯入文件內容不得為空!";
                    return creatResult;
                }

                deleteResult = DeleteList(basicID, list);
                if (deleteResult.IsSuccess == true)
                {
                    string errMsg = "";                    
                    switch (list.ToLower())
                    {
                        case "black":
                            
                            for (int i=0 ;i<ItemID.Count();i++)
                            {
                                if (ItemID[i] == 0)
                                {
                                    continue;
                                }

                                Models.DBModels.TWSQLDB.PromotionGiftBlackList PromotionGiftBlackList = new Models.DBModels.TWSQLDB.PromotionGiftBlackList();
                                PromotionGiftBlackList.PromotionGiftBasicID = basicID;
                                PromotionGiftBlackList.ItemID = ItemID[i];
                                PromotionGiftBlackList.Status = Status[i];
                                PromotionGiftBlackList.CreateUser = UserName;
                                //int success = CreatePromotionGiftBlackList(basicID, PromotionGiftBlackList);
                                this._PromotionRepo.CreatePromotionGiftBlackList(PromotionGiftBlackList);
                                int success = PromotionGiftBlackList.ID;
                                
                                if (success <= 0)
                                {
                                    errMsg +=  i + " , ";
                                }
                            }
                            break;
                        case "white":
                            for (int i = 0; i < ItemID.Count(); i++)
                            {
                                if (ItemID[i] == 0)
                                {
                                    continue;
                                }

                                Models.DBModels.TWSQLDB.PromotionGiftWhiteList PromotionGiftWhiteList = new Models.DBModels.TWSQLDB.PromotionGiftWhiteList();
                                PromotionGiftWhiteList.PromotionGiftBasicID = basicID;
                                PromotionGiftWhiteList.ItemID = ItemID[i];
                                PromotionGiftWhiteList.Status = Status[i];
                                PromotionGiftWhiteList.CreateUser = UserName;
                                //int success = CreatePromotionGiftWhiteList(basicID, PromotionGiftWhiteList);
                                this._PromotionRepo.CreatePromotionGiftWhiteList(PromotionGiftWhiteList);
                                int success = PromotionGiftWhiteList.ID;
                                
                                if (success <= 0)
                                {
                                    errMsg = i + " , ";  
                                }
                            }
                            break;
                        default:
                            creatResult.IsSuccess = false;
                            break;
                    }
                    if (errMsg == "")
                    {
                        creatResult.IsSuccess = true;
                    }
                    else
                    {
                        creatResult.IsSuccess = false;
                        creatResult.Msg = errMsg;
                    }
                    return creatResult;
                }
                else
                {
                    creatResult.IsSuccess = false;
                    creatResult.Msg = deleteResult.Msg;
                    return creatResult;
                }
            }
            catch(Exception ex)
            {
                creatResult.IsSuccess = false;
                creatResult.Code = (int)ResponseCode.Error;
                creatResult.Msg = ((ex.InnerException != null) ? "InnerEx:" + ex.InnerException.Message : "例外發生: " + ex.Message);
                //logger.Error(  ((ex.InnerException != null) ? "InnerEx:" + ex.InnerException.Message : "例外發生: " + ex.Message));
            }
            return creatResult;
        }

        public List<Models.DomainModels.Redeem.PromotionGiftBasic> GetPromotionGiftBasicByIdList(List<int> argListBasicId)
        {
            if (argListBasicId == null || argListBasicId.Count <= 0)
            {
                return null;
            }

            List<Models.DBModels.TWSQLDB.PromotionGiftBasic> listDbBasic = null;
            List<Models.DomainModels.Redeem.PromotionGiftBasic> listBasic = null;

            listDbBasic = this._PromotionRepo.GetAllPromotionGiftBasic().Where(x => argListBasicId.Contains(x.ID)).ToList();
            if (listDbBasic != null && listDbBasic.Count > 0)
            {
                try
                {
                    listBasic = ModelConverter.ConvertTo<List<Models.DomainModels.Redeem.PromotionGiftBasic>>(listDbBasic);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return listBasic;
        }

        /// <summary>
        /// 找出不能參與優惠活動的Item
        /// </summary>
        /// <param name="accountID">客戶ID</param>
        /// <returns>返回ItemID清單</returns>
        private List<int> FindNotToParticipateItemIDList(int accountID)
        {
            // 不參與優惠活動的Item.ShowOrder狀態清單
            List<ViewTracksCartItems> getNotToParticipateList = new List<ViewTracksCartItems>();
            // 不參與活動的itemID
            List<int> result = new List<int>();
            getNotToParticipateList = this._IShoppingCartService.NoDiscountsGoods(accountID);

            if (getNotToParticipateList.Count > 0)
            {
                result = getNotToParticipateList.Select(x => x.ItemID).Distinct().ToList();
            }

            return result;
        }

        /// <summary>
        /// 透過Item.ID List取得該Item所有可參與的PromotionGift
        /// </summary>
        /// <param name="itemIDList">賣場ID清單</param>
        /// <param name="turnOn">是否啟用正式機狀態的開關閥，正式機on、測試機off</param>
        /// <returns>返回結果</returns>
        public Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>> getItemPromotionGiftListInfo(int accountID, List<int> itemIDList, string turnOn)
        {
            turnOn = turnOn.ToLower();
            // 查詢資料集合
            List<Models.DomainModels.Item.ItemDetail> listItem = null;
            Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>> result = new Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>();
            DateTime dt = DateTime.UtcNow.AddHours(8);
            // 找出所有需要查找的賣場清單
            listItem = FindItemData(itemIDList);
            if (listItem == null || listItem.Count == 0)
            {
                return new Dictionary<int, List<Models.DomainModels.Cart.GroupDiscount>>();
            }

            // 找出不得參與活動的ItemID
            List<int> notToParticipateItemIDList = this.FindNotToParticipateItemIDList(accountID);
            listItem = listItem.Where(x => !notToParticipateItemIDList.Contains(x.Main.ItemBase.ID)).ToList();
            // 查詢在時間範圍內啟用的PromotionGift有哪些
            List<Models.DomainModels.Redeem.PromotionGiftBasic> queryPromotionGiftBasicByDate = null;
            if (turnOn == "on")
            {
                queryPromotionGiftBasicByDate = this.GetPromotionGiftBasicByStatus(Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.Used);
            }
            else
            {
                queryPromotionGiftBasicByDate = this.GetPromotionGiftBasicByStatus(Models.DomainModels.Redeem.PromotionGiftBasic.UsedStatus.TempUsed);
            }

            if (queryPromotionGiftBasicByDate.Count == 0)
            {
                //throw new Exception("查無此賣場優惠活動資訊");
                return new Dictionary<int, List<Models.DomainModels.Cart.GroupDiscount>>();
            }
            // 取出查詢出的所有PromotionGift.ID清單
            List<int> promotionGiftIDList = queryPromotionGiftBasicByDate.Select(x => x.ID).ToList();
            // 取得在時間範圍內啟用的PromotionGift可使用的級距清單
            List<Models.DomainModels.Redeem.PromotionGiftInterval> getAllPromotionGiftInterval = new List<Models.DomainModels.Redeem.PromotionGiftInterval>();
            // 以PromotionGift.ID篩選的黑名單清單
            List<Models.DomainModels.Redeem.PromotionGiftBlackList> getAllQueryPromotionGiftBlackList = new List<Models.DomainModels.Redeem.PromotionGiftBlackList>();
            // 以PromotionGift.ID的白名單清單
            List<Models.DomainModels.Redeem.PromotionGiftWhiteList> getAllQueryPromotionGiftWhiteList = new List<Models.DomainModels.Redeem.PromotionGiftWhiteList>();
            getAllPromotionGiftInterval = this.GetPromotionGiftIntervalList(promotionGiftIDList);
            if (turnOn == "on")
            {
                getAllQueryPromotionGiftBlackList = this.GetPromotionGiftBlackListByBasicAndItemAndStatus(promotionGiftIDList, itemIDList, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.Used);
                getAllQueryPromotionGiftWhiteList = this.GetPromotionGiftWhiteListByBasicAndItemAndStatus(promotionGiftIDList, itemIDList, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.Used);
            }
            else
            {
                getAllQueryPromotionGiftBlackList = this.GetPromotionGiftBlackListByBasicAndItemAndStatus(promotionGiftIDList, itemIDList, Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.TempUsed);
                getAllQueryPromotionGiftWhiteList = this.GetPromotionGiftWhiteListByBasicAndItemAndStatus(promotionGiftIDList, itemIDList, Models.DomainModels.Redeem.PromotionGiftWhiteList.WhiteListStatus.TempUsed);
            }

            foreach (Models.DomainModels.Item.ItemDetail itemDetail in listItem)
            {
                List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount> gvList = new List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>();
                // 黑名單檢驗
                // 使用CategoryID篩選出的優惠折扣活動且為黑名單者
                string cidString = ";" + itemDetail.Main.ItemBase.CategoryID.ToString() + ";";
                List<Models.DomainModels.Redeem.PromotionGiftBasic> blackPromotionGiftBasicByCategory = queryPromotionGiftBasicByDate
                    .Where(x => ((";" + x.Categories + ";").Contains(cidString) || (";" + x.Categories + ";").Contains(";0;")) && x.ReferencesList == "black").ToList();
                // 使用CategoryID篩選的優惠折扣ID清單
                List<int> blackPromotionGiftBasicID = blackPromotionGiftBasicByCategory.Select(x => x.ID).ToList();
                // 滿足條件下所有優惠折扣級距清單
                List<Models.DomainModels.Redeem.PromotionGiftInterval> queryPromotionGiftIntervalBlack = getAllPromotionGiftInterval.Where(x => blackPromotionGiftBasicID.Contains(x.PromotionGiftBasicID)).ToList();
                // 以CategoryID篩選的黑名單清單
                List<Models.DomainModels.Redeem.PromotionGiftBlackList> queryPromotionGiftBlackList = getAllQueryPromotionGiftBlackList.Where(x => blackPromotionGiftBasicID.Contains(x.PromotionGiftBasicID)).ToList();
                // 透過Category篩選出的優惠折扣活動資格檢查
                blackPromotionGiftBasicByCategory.ForEach(subPB =>
                {
                    // 若該活動參考依據為黑名單則需檢查該賣場ID是否在活動的黑名單中
                    Models.DomainModels.Redeem.PromotionGiftBlackList searchBlackList = null;
                    if (turnOn == "on")
                    {
                        searchBlackList = queryPromotionGiftBlackList.Where(x =>
                                x.PromotionGiftBasicID == subPB.ID
                                && x.ItemID == itemDetail.Main.ItemBase.ID
                                && x.Status == (int)Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.Used).FirstOrDefault();
                    }
                    else
                    {
                        searchBlackList = queryPromotionGiftBlackList.Where(x =>
                                x.PromotionGiftBasicID == subPB.ID
                                && x.ItemID == itemDetail.Main.ItemBase.ID
                                && x.Status == (int)Models.DomainModels.Redeem.PromotionGiftBlackList.BlackListStatus.TempUsed).FirstOrDefault();
                    }
                    // 若該賣場ID不在該優惠活動的黑名單中，則加入返回結果
                    if (searchBlackList == null)
                    {
                        InsertPromotionInfo(ref gvList, subPB);
                    }
                });
                // 白名單檢驗
                List<Models.DomainModels.Redeem.PromotionGiftBasic> whitePromotionGiftList = queryPromotionGiftBasicByDate.Where(x => x.ReferencesList == "white").ToList();
                whitePromotionGiftList.ForEach(subPB =>
                {
                    List<Models.DomainModels.Redeem.PromotionGiftWhiteList> queryWhiteList = getAllQueryPromotionGiftWhiteList.Where(x => x.PromotionGiftBasicID == subPB.ID).ToList();
                    Models.DomainModels.Redeem.PromotionGiftWhiteList getWhiteListByItemID = queryWhiteList.Where(y => y.ItemID == itemDetail.Main.ItemBase.ID).FirstOrDefault();
                    if (getWhiteListByItemID != null)
                    {
                        InsertPromotionInfo(ref gvList, subPB);
                    }
                });

                result.Add(itemDetail.Main.ItemBase.ID, gvList);
            }

            return result;
        }

        /// <summary>
        /// 整合返回結果資訊
        /// </summary>
        /// <param name="isSuccess">是否執行成功</param>
        /// <param name="gDM"></param>
        /// <param name="message">執行結果訊息</param>
        /// <returns>返回整合結果</returns>
        private ResponseMessage<Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>> returnMessage(bool isSuccess, Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>> gDM, string message)
        {
            ResponseMessage<Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>> result = new ResponseMessage<Dictionary<int, List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount>>>();
            result.IsSuccess = isSuccess;
            if (isSuccess == true)
            {
                result.Data = gDM;
                result.Message = message;
            }
            else
            {
                result.Message = message;
            }

            return result;
        }

        /// <summary>
        /// 將符合條件的PromotionGift資訊填入GroupDiscount Model中
        /// </summary>
        /// <param name="result">填入目標</param>
        /// <param name="basic">資料來源</param>
        private void InsertPromotionInfo(ref List<TWNewEgg.Models.DomainModels.Cart.GroupDiscount> result, Models.DomainModels.Redeem.PromotionGiftBasic basic)
        {
            TWNewEgg.Models.DomainModels.Cart.GroupDiscount gDM = new TWNewEgg.Models.DomainModels.Cart.GroupDiscount();
            gDM.EventID = basic.ID;
            gDM.EventName = basic.ShowDesc;
            gDM.Desc = basic.HighLight;
            gDM.CSSStyle = basic.CSS;
            result.Add(gDM);
        }
    }//end class
}//end namespace
