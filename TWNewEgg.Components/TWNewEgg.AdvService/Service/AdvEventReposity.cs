using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.SearchService.Service;
using TWNewEgg.AdvService.Models;
using TWNewEgg.ItemService.Service;
using System.Data;
using System.Data.Entity;
//using Autofac;
//using TWNewEgg.Framework.Autofac;
//using TWNewEgg.CategoryServices.Interface;

namespace TWNewEgg.AdvService.Service
{
    /// <summary>
    /// 與AdvEvent相關的函式
    /// </summary>
    public class AdvEventReposity : IAdvEvent, IDisposable
    {

        private const string ITEMURLLINK = "/item?itemid={0}&categoryid={1}&StoreID={2}";
        //private ILifetimeScope scope = AutofacConfig.Container.BeginLifetimeScope();
        //private ICategoryServices _categoryService;

        /// <summary>
        /// 共用的AdvEventDbService
        /// </summary>
        private AdvEventDBService memAdvDB = new AdvEventDBService();

        /// <summary>
        /// 共用的AdvItemService
        /// </summary>
        private AdvEventItemService memAdvService = new AdvEventItemService();

        /// <summary>
        /// 釋放記憶體
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 釋放記憶體
        /// </summary>
        /// <param name="disposing">boolean</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                memAdvDB.Dispose();
                
                //if (scope != null)
                //{
                //    scope.Dispose();
                //}
            }
        }

        public AdvEventReposity()
        {
            //this._categoryService = scope.Resolve<ICategoryServices>();
        }

        /// <summary>
        /// Get a AdvEvent from DB by ID.
        /// </summary>
        /// <param name="id">Adv Event ID</param>
        /// <returns>AdvEvent物件</returns>
        public TWNewEgg.DB.TWSQLDB.Models.AdvEvent GetOneAdvEventByID(int id)
        {
            TWNewEgg.DB.TWSqlDBContext oDb = null;
            TWNewEgg.DB.TWSQLDB.Models.AdvEvent objAdvEvent = null;

            try
            {
                oDb = new DB.TWSqlDBContext();
                objAdvEvent = oDb.AdvEvent.Where(x => x.ID == id).FirstOrDefault();
            }
            catch
            {
            }
            finally
            {
                if (oDb != null)
                {
                    oDb.Dispose();
                }

                oDb = null;
            }

            return objAdvEvent;
        }

        /// <summary>
        /// Get a AdvEvent from DB by hash code.
        /// </summary>
        /// <param name="hashCode">Adv Event Hash Code</param>
        /// <returns>AdvEventDisplay object</returns>
        public AdvEventDisplay GetOneAdvEventByHashCode(string hashCode)
        {
            return null;
        }

        /// <summary>
        /// Get AdvEvents from DB by IDs, and order 
        /// </summary>
        /// <param name="ids">list of id</param>
        /// <param name="orderCondition">排序條件</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetAdvEventsByIDs(List<int> ids, int orderCondition)
        {
            return null;
        }

        /// <summary>
        /// Get AdvEvents from DB by Hash Codes.
        /// </summary>
        /// <param name="hashCodes">List of Adv Event Hash Codes</param>
        /// <param name="orderCondition">排序條件</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetAdvEventsByHashCodes(List<string> hashCodes, int orderCondition)
        {
            return null;
        }

        /// <summary>
        /// Get AdvEventDisplay only for opening.
        /// </summary>
        /// <param name="advType">AdvType Id</param>
        /// <param name="dateTimeNow">現在時間</param>
        /// <param name="startDate">開始時間</param>
        /// <param name="endDate">結束時間</param>
        /// <param name="clearItemLinkifNotStart">clearItemLinkifNotStart</param>
        /// <param name="reallyDateTimeNow">reallyDateTimeNow</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetEveryDayTimeLimitAdvEvents(int advType, DateTime dateTimeNow, DateTime? startDate, DateTime? endDate, bool clearItemLinkifNotStart, DateTime? reallyDateTimeNow)
        {
            List<AdvEventDisplay> everyDayItems = new List<AdvEventDisplay>();

            // var advType = (int)AdvEvent.AdvTypeEnum.每日整點限時搶購;
            switch (advType)
            {
                case (int)AdvEventType.AdvTypeOption.每日整點限時搶購:
                    //this algo only for a week, if range days over a week, then it will get the wrong date.
                    int dayOfWeekInt = 1;
                    int diffDays = 1;
                    if (startDate != null && endDate != null)
                    {
                        dayOfWeekInt = (int)startDate.Value.DayOfWeek;
                        if (dayOfWeekInt == 0)
                        {
                            dayOfWeekInt = 7;
                        }

                        diffDays = (endDate.Value - startDate.Value).Days;
                        int dayOfWeekNowInt = (int)dateTimeNow.DayOfWeek;

                        if (dayOfWeekNowInt == 0)
                        {
                            dayOfWeekNowInt = 7;
                        }

                        int diffDayOfWeekInt = dayOfWeekInt - dayOfWeekNowInt;
                        startDate = dateTimeNow.AddDays(diffDayOfWeekInt);
                        endDate = startDate.Value.AddDays(diffDays).AddHours(endDate.Value.Hour).AddMinutes(endDate.Value.Minute);
                        startDate = startDate.Value.AddHours(startDate.Value.Hour).AddMinutes(startDate.Value.Minute);
                        dateTimeNow = dateTimeNow.AddDays(1);
                    }

                    break;
                default:
                    break;
            }

            var advEvents = memAdvDB.GetAdvEventByAdvType(advType, startDate, endDate);
            List<int> itemIDs = new List<int>();
            foreach (var itemID in advEvents.Select(x => x.ItemID))
            {
                if (itemID != null)
                {
                    itemIDs.Add(itemID.Value);
                }
            }

            IItemService getItemPrice = new ItemServiceRepository();
            var itemData = getItemPrice.GetItemsByIDs(itemIDs, 0);

            foreach (var objAdvEvent in advEvents)
            {
                //bool isItem = false;
                AdvEventDisplay newAdvEventDisplay = new AdvEventDisplay(objAdvEvent);
                Item advEventItem = itemData.Where(x => x.ID == objAdvEvent.ItemID).FirstOrDefault();
                string itemLink = "", itemImg = "", itemName = "";
                if (advEventItem != null)
                {
                    newAdvEventDisplay = memAdvService.TransItem2AdvEventDisplay(advEventItem, objAdvEvent, newAdvEventDisplay);
                    //ItemService ItemImgService = new ItemService();
                    //itemLink = string.Format(ITEMURLLINK, advEventItem.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    itemLink = string.Format(ITEMURLLINK, advEventItem.ID.ToString(), "0", "0");
                    itemImg = getItemPrice.GetItemImagePath(advEventItem.ID, newAdvEventDisplay.ItemIDPicStart, 300, "item");
                    itemName = advEventItem.Name;
                }

                if ((objAdvEvent.StartDate != null) && (objAdvEvent.EndDate != null))
                {
                    if ((DateTime.Compare(objAdvEvent.StartDate.Value, dateTimeNow) <= 0) && (DateTime.Compare(objAdvEvent.EndDate.Value, dateTimeNow) > 0))
                    {
                        newAdvEventDisplay.Title = objAdvEvent.StartTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.StartSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.StartLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.StartImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.StartImgAlt;

                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                    }
                    else if ((DateTime.Compare(objAdvEvent.StartDate.Value, dateTimeNow) > 0))
                    {
                        newAdvEventDisplay.Title = objAdvEvent.BeforeTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.BeforeSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.BeforeLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.BeforeImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.BeforeImgAlt;

                        // hide css style
                        // newAdvEventDisplay.SCN1 += " " + aAdvEvent.ImgFilterClassName1;
                        // clear item information if it's not reach start date
                        newAdvEventDisplay.ifBeforeClearItemInfo(true, false);
                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                        // hide css style
                        newAdvEventDisplay.SCN1 = objAdvEvent.StyleClassName1 + " " + objAdvEvent.ImgFilterClassName1;
                    }
                    else if ((DateTime.Compare(objAdvEvent.EndDate.Value, dateTimeNow) <= 0))
                    {
                        newAdvEventDisplay.Title = objAdvEvent.EndTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.EndSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.EndLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.EndImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.EndImgAlt;

                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                    }
                    else
                    {
                        newAdvEventDisplay.Title = objAdvEvent.StartTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.StartSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.StartLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.StartImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.StartImgAlt;

                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                    }

                    if (clearItemLinkifNotStart && reallyDateTimeNow != null)
                    {
                        if ((DateTime.Compare(objAdvEvent.StartDate.Value, reallyDateTimeNow.Value) > 0))
                        {
                            newAdvEventDisplay.LinkUrl = "#";
                            newAdvEventDisplay.setBeforeInfoUsingRealDate(objAdvEvent);
                            // hide css style
                            newAdvEventDisplay.SCN1 = objAdvEvent.StyleClassName1 + " " + objAdvEvent.ImgFilterClassName1;
                        }
                    }
                }
                else if ((objAdvEvent.StartDate != null) && (objAdvEvent.EndDate == null))
                {
                    if (DateTime.Compare(objAdvEvent.StartDate.Value, dateTimeNow) >= 0)
                    {
                        newAdvEventDisplay.Title = objAdvEvent.BeforeTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.BeforeSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.BeforeLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.BeforeImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.BeforeImgAlt;

                        // hide css style
                        // newAdvEventDisplay.SCN1 += " " + aAdvEvent.ImgFilterClassName1;
                        // clear item information if it's not reach start date
                        newAdvEventDisplay.ifBeforeClearItemInfo(true, false);
                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                        // hide css style
                        newAdvEventDisplay.SCN1 = objAdvEvent.StyleClassName1 + " " + objAdvEvent.ImgFilterClassName1;
                    }
                    else
                    {
                        newAdvEventDisplay.Title = objAdvEvent.StartTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.StartSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.StartLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.StartImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.StartImgAlt;

                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                    }

                    if (clearItemLinkifNotStart && reallyDateTimeNow != null)
                    {
                        if ((DateTime.Compare(objAdvEvent.StartDate.Value, reallyDateTimeNow.Value) > 0))
                        {
                            newAdvEventDisplay.LinkUrl = "#";
                            newAdvEventDisplay.setBeforeInfoUsingRealDate(objAdvEvent);
                            // hide css style
                            newAdvEventDisplay.SCN1 = objAdvEvent.StyleClassName1 + " " + objAdvEvent.ImgFilterClassName1;
                        }
                    }
                }
                else if ((objAdvEvent.StartDate == null) && (objAdvEvent.EndDate != null))
                {
                    if (DateTime.Compare(objAdvEvent.EndDate.Value, dateTimeNow) >= 0)
                    {
                        newAdvEventDisplay.Title = objAdvEvent.StartTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.StartSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.StartLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.StartImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.StartImgAlt;

                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                    }
                    else
                    {
                        newAdvEventDisplay.Title = objAdvEvent.EndTitle;
                        newAdvEventDisplay.Slogan = objAdvEvent.EndSlogan;
                        newAdvEventDisplay.LinkUrl = objAdvEvent.EndLinkUrl;
                        newAdvEventDisplay.ImgUrl = objAdvEvent.EndImgUrl;
                        newAdvEventDisplay.ImgAlt = objAdvEvent.EndImgAlt;

                        newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                    }
                }
                else
                {
                    newAdvEventDisplay.Title = objAdvEvent.StartTitle;
                    newAdvEventDisplay.Slogan = objAdvEvent.StartSlogan;
                    newAdvEventDisplay.LinkUrl = objAdvEvent.StartLinkUrl;
                    newAdvEventDisplay.ImgUrl = objAdvEvent.StartImgUrl;
                    newAdvEventDisplay.ImgAlt = objAdvEvent.StartImgAlt;

                    newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);
                }

                everyDayItems.Add(newAdvEventDisplay);
            }

            return everyDayItems.AsEnumerable();
        }

        /// <summary>
        /// Get Recommend Items from AdvEvent by AdvEvent's hash code
        /// </summary>
        /// <param name="hashCode">hash code</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetRecommendFromAdvEventByHashCode(string hashCode)
        {
            return null;
        }

        /// <summary>
        /// Get Recommend Items from AdvEvent which is close dateTimeNow
        /// </summary>
        /// <param name="dateTimeNow">現在時間</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetRecommendFromLimitAdvEventByDateTime(DateTime dateTimeNow)
        {
            var advType = (int)AdvEventType.AdvTypeOption.每日整點限時搶購;
            var advEvent = memAdvDB.GetAdvEventFromCloseDate(advType, dateTimeNow);
            if (advEvent == null)
            {
                return null;
            }

            List<string> extraApis = new List<string>();
            List<string> extraMethods = new List<string>();
            List<string> extraArgs = new List<string>();

            return null;
        }

        /// <summary>
        /// Get Recommend Items by string, and set Item img size.
        /// </summary>
        /// <param name="recommendIDs">string recommendIDs</param>
        /// <param name="imgSize">int image size</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetRecommendFromString(string recommendIDs, int imgSize)
        {
            List<AdvEventDisplay> recommendItems = new List<AdvEventDisplay>();

            var splitString = new string[] { "," };
            string[] itemIDsText = recommendIDs.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            List<int> itemIDs = new List<int>();

            foreach (var itemIDText in itemIDsText)
            {
                int itemID = 0;
                bool flag = int.TryParse(itemIDText, out itemID);
                if (flag)
                {
                    itemIDs.Add(itemID);
                }
            }

            //Controllers.Api.ItemInfoController GetItemPrice = new Controllers.Api.ItemInfoController();
            IItemService getItemPrice = new ItemServiceRepository();
            var itemData = getItemPrice.GetItemsByIDs(itemIDs, 0);
            var itemDataLength = itemData.Count;
            foreach (var orderItemID in itemIDs)
            {
                itemData.Add(itemData.Where(x => x.ID == orderItemID).FirstOrDefault());
            }
            itemData.RemoveRange(0, itemDataLength);
            
            foreach (var objItemData in itemData)
            {
                //bool isItem = false;
                AdvEventDisplay newAdvEventDisplay = new AdvEventDisplay();
                //Item advEventItem = itemData.Where(x => x.ID == aAdvEvent.ItemID).FirstOrDefault();
                string itemLink = "", itemImg = "", itemName = "";
                if (objItemData != null)
                {
                    //ItemService ItemImgService = new ItemService();
                    newAdvEventDisplay = memAdvService.TransItem2AdvEventDisplay(objItemData, null, newAdvEventDisplay);
                    //itemLink = string.Format(ITEMURLLINK, objItemData.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    itemLink = string.Format(ITEMURLLINK, objItemData.ID.ToString(), "0", "0");
                    itemImg = getItemPrice.GetItemImagePath(objItemData.ID, newAdvEventDisplay.ItemIDPicStart, imgSize, "item");
                    itemName = objItemData.Name;
                }

                newAdvEventDisplay.setItemLinkAndImgUrl(itemName, itemLink, itemImg);

                recommendItems.Add(newAdvEventDisplay);
            }

            return recommendItems.AsEnumerable();
        }

        /// <summary>
        /// Get Recommend Items by api string 
        /// </summary>
        /// <param name="extraApi">extra Api</param>
        /// <param name="extraMethod">extra Method</param>
        /// <param name="extraArg">extra argument</param>
        /// <param name="extraNumber">extra number</param>
        /// <param name="imgSize">size of image</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetRecommendFromString(string extraApi, string extraMethod, string extraArg, int extraNumber, int imgSize)
        {
            List<AdvEventDisplay> recommendItems = new List<AdvEventDisplay>();
            IApiSelector apiSelector = new ApiSelectorRepository();

            //Controllers.Api.ItemInfoController GetItemPrice = new Controllers.Api.ItemInfoController();
            IItemService getItemPrice = new ItemServiceRepository();

            switch (extraApi)
            {
                case "SearchApi":
                    var searchModel = apiSelector.SetSearchApiModel(extraArg);
                    TWNewEgg.SearchService.Service.SearchService searchData = new TWNewEgg.SearchService.Service.SearchService();

                    var searchItems = searchData.SearchResultV2(searchModel.SearchWord, searchModel.SrchIn, searchModel.Order, searchModel.Cat, searchModel.LID, searchModel.Cty, searchModel.BID, searchModel.SID, searchModel.minPrice, searchModel.maxPrice, searchModel.PageSize, searchModel.Page, searchModel.Mode, searchModel.Submit, searchModel.orderCats);
                    searchItems = searchItems.Take(extraNumber).ToList();
                    List<int> itemSearchIDs = new List<int>();
                    itemSearchIDs.AddRange(searchItems.Select(x => x.ID).ToList());

                    var itemSearchData = getItemPrice.GetItemsByIDs(itemSearchIDs, 0);
                    itemSearchData = itemSearchData.Where(x => x.PicStart != 0 || x.PicStart != null).ToList();

                    foreach (var objSearchItemData in searchItems)
                    {
                        var objItemData = itemSearchData.Where(x => x.ID == objSearchItemData.ID).FirstOrDefault();
                        if (objItemData == null)
                        {
                            continue;
                        }
                        //bool isItem = false;
                        AdvEventDisplay newAdvEventDisplay = new AdvEventDisplay();
                        //Item advEventItem = itemData.Where(x => x.ID == aAdvEvent.ItemID).FirstOrDefault();
                        string itemLink = "", itemImg = "";
                        if (objItemData != null)
                        {
                            //ItemService ItemImgService = new ItemService();
                            newAdvEventDisplay = memAdvService.TransItem2AdvEventDisplay(objItemData, null, newAdvEventDisplay);

                            itemLink = string.Format(ITEMURLLINK, objItemData.ID.ToString(), "0", "0");
                            //itemImg = ItemImgService.GetItemImagePath(aItemData.ID, newAdvEventDisplay.ItemIDPicStart, imgSize, "item");
                            itemImg = getItemPrice.GetItemImagePath(objItemData.ID, newAdvEventDisplay.ItemIDPicStart, imgSize, "item");
                        }

                        newAdvEventDisplay.setItemLinkAndImgUrl(objItemData.Name, itemLink, itemImg);
                        if (newAdvEventDisplay.ItemStock > 0)
                        {
                            recommendItems.Add(newAdvEventDisplay);
                        }
                    }

                    break;
                case "DealApi":
                    var dealModel = apiSelector.SetDealApiModel(extraArg);

                    IDeals dealData = new DealsRepository();

                    var dealItems = dealData.getItemUnderCategory(dealModel.page, dealModel.showNumber, dealModel.showAll, dealModel.showZero, dealModel.brandIds, dealModel.categoryIds, dealModel.orderByType, dealModel.orderBy, dealModel.priceCash);
                    dealItems = dealItems.Take(extraNumber).ToList();
                    List<int> itemDealIDs = new List<int>();
                    itemDealIDs.AddRange(dealItems.Select(x => x.ID).ToList());

                    var itemDealData = getItemPrice.GetItemsByIDs(itemDealIDs, 0);
                    itemDealData = itemDealData.Where(x => x.PicStart != 0 || x.PicStart != null).ToList();

                    foreach (var objDealItemData in dealItems)
                    {
                        var objItemData = itemDealData.Where(x => x.ID == objDealItemData.ID).FirstOrDefault();
                        if (objItemData == null)
                        {
                            continue;
                        }
                        //bool isItem = false;
                        AdvEventDisplay newAdvEventDisplay = new AdvEventDisplay();
                        //Item advEventItem = itemData.Where(x => x.ID == aAdvEvent.ItemID).FirstOrDefault();
                        string itemLink = "", itemImg = "";
                        if (objItemData != null)
                        {
                            //ItemService ItemImgService = new ItemService();
                            newAdvEventDisplay = memAdvService.TransItem2AdvEventDisplay(objItemData, null, newAdvEventDisplay);

                            //itemLink = string.Format(ITEMURLLINK, objItemData.ID.ToString(), categoryID.ToString(), storeID.ToString());
                            itemLink = string.Format(ITEMURLLINK, objItemData.ID.ToString(), "0", "0");
                            itemImg = getItemPrice.GetItemImagePath(objItemData.ID, newAdvEventDisplay.ItemIDPicStart, imgSize, "item");
                        }

                        newAdvEventDisplay.setItemLinkAndImgUrl(objItemData.Name, itemLink, itemImg);
                        if (newAdvEventDisplay.ItemStock > 0)
                        {
                            recommendItems.Add(newAdvEventDisplay);
                        }
                    }

                    break;
                default:
                    return recommendItems.AsEnumerable();
            }

            return recommendItems.AsEnumerable();
        }

        /// <summary>
        /// Get Recommend Items by string 
        /// </summary>
        /// <param name="recommendIDs">string recommendIDs</param>
        /// <param name="extraApis">extra Apis</param>
        /// <param name="extraMethods">extra Methods</param>
        /// <param name="extraArgs">extra Args</param>
        /// <param name="imgSize">img Size</param>
        /// <returns>IEnumerable AdvEventDisplay</returns>
        public IEnumerable<AdvEventDisplay> GetRecommendFromString(string recommendIDs, List<string> extraApis, List<string> extraMethods, List<string> extraArgs, int imgSize)
        {
            return null;
        }

        /// <summary>
        /// 根據AdvEventType取得旗下的所有AdvEvent
        /// </summary>
        /// <param name="arg_nAdvTypeId">AdvEventType.ID</param>
        /// <returns>null or List of AdvEvent</returns>
        public List<AdvEvent> GetAllAdvEventByAdvEventTypeId(int arg_nAdvTypeId)
        {
            AdvEventDBService objAdvService = null;
            List<AdvEvent> listAdvEvent = null;

            objAdvService = new AdvEventDBService();
            listAdvEvent = objAdvService.GetAllAdvEventByAdvEventTypeId(arg_nAdvTypeId);
            objAdvService = null;
            return listAdvEvent;
        }

        /// <summary>
        /// 根據AdvEventType取得旗下的所有上線的AdvEvent
        /// </summary>
        /// <param name="arg_AdvEventTypeId">AdvEventType的ID</param>
        /// <returns>AdvEvent列表或null</returns>
        public List<AdvEvent> GetActiveAdvEventByAdvEventTypeId(int arg_AdvEventTypeId)
        {
            AdvEventDBService objAdvService = null;
            List<AdvEvent> listAdvEvent = null;

            objAdvService = new AdvEventDBService();
            listAdvEvent = objAdvService.GetActiveAdvEventByAdvEventTypeId(arg_AdvEventTypeId);
            objAdvService = null;
            return listAdvEvent;
        }

        /// <summary>
        /// 修改AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">要修改的AdvEvent</param>
        /// <returns>true:修改成功; false: 修改失敗</returns>
        public bool UpdateAdvEvent(AdvEvent arg_oAdvEvent)
        {
            AdvEventDBService objAdvService = null;
            bool boolExec = false;
            objAdvService = new AdvEventDBService();
            boolExec = objAdvService.UpdateAdvEvent(arg_oAdvEvent, 0);
            objAdvService = null;

            return boolExec;
        }

        /// <summary>
        /// 新增AdvEvent
        /// </summary>
        /// <param name="arg_oAdvEvent">新增的AdvEvent</param>
        /// <returns>新增AdvEvent的ID; 0:新增失敗</returns>
        public int AddAdvEvent(AdvEvent arg_oAdvEvent)
        {
            AdvEventDBService objAdvService = null;
            int nId = 0;

            objAdvService = new AdvEventDBService();
            nId = objAdvService.AddAdvEvent(arg_oAdvEvent);
            objAdvService = null;

            return nId;
        }
    }
}
