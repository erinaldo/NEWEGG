using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.AdvService.Models;
using TWNewEgg.ItemService.Service;

namespace TWNewEgg.AdvService.Service
{
    /// <summary>
    /// 取得AdvEvent相關的Item或是轉換成AdvEventDisplay的Service
    /// </summary>
    public class AdvEventItemService : IAdvEventItem
    {
        private const string ITEMURLLINK = "/item?itemid={0}&categoryid={1}&StoreID={2}";
        private const string ITEMPICURL = "/pic/item/";

        public AdvEventItemService()
        {
        }
        /// <summary>
        /// 轉換AdvEvent為AdvEventDisplay
        /// </summary>
        /// <param name="argITem">Item</param>
        /// <param name="argAdvEvent">AdvEvent</param>
        /// <param name="fromAdvEventDisplay">欲轉換的AdvEventDisplay</param>
        /// <returns>轉換完成的AdvEventDisplay</returns>
        public AdvEventDisplay TransItem2AdvEventDisplay(Item argITem, AdvEvent argAdvEvent, AdvEventDisplay fromAdvEventDisplay)
        {
            AdvEventDisplay objAdvEventDisplay = new AdvEventDisplay();
            objAdvEventDisplay = fromAdvEventDisplay;
            //isItem = true;
            IItemService testItemSellorNot = new ItemServiceRepository();

            objAdvEventDisplay.ItemID = argITem.ID;
            objAdvEventDisplay.ItemIDPicStart = (argITem.PicStart ?? 0);
            objAdvEventDisplay.ItemIDPicEnd = (argITem.PicEnd ?? 0);
            objAdvEventDisplay.ItemID = argITem.ID;
            objAdvEventDisplay.ItemStock = testItemSellorNot.GetSellingQty(argITem.ID, "Item");
            if (objAdvEventDisplay.ItemStock < 1)
            {
                objAdvEventDisplay.SCN1 += " " + ((argAdvEvent != null) ? argAdvEvent.SoldoutClassName : "");
            }

            objAdvEventDisplay.ItemModel = (argITem.Model ?? "");
            objAdvEventDisplay.ItemManufacture = argITem.ManufactureID.ToString();
            objAdvEventDisplay.MarketPrice = (argITem.MarketPrice ?? 0);
            objAdvEventDisplay.SalePrice = argITem.PriceCash;
            objAdvEventDisplay.ShippingPrice = argITem.PriceGlobalship;
            objAdvEventDisplay.TaxPrice = (argITem.Taxfee ?? 0);
            objAdvEventDisplay.ServiceFee = argITem.ServicePrice;
            if (objAdvEventDisplay.MarketPrice >= objAdvEventDisplay.SalePrice)
            {
                if (objAdvEventDisplay.MarketPrice != 0)
                {
                    objAdvEventDisplay.SavingPercent = (Math.Round((objAdvEventDisplay.SalePrice / objAdvEventDisplay.MarketPrice), 2, MidpointRounding.AwayFromZero) * 100).ToString();
                }
                else
                {
                    objAdvEventDisplay.SavingPercent = "100";
                }
            }
            else
            {
                objAdvEventDisplay.SavingPercent = "";
            }

            objAdvEventDisplay.CategoryID = argITem.CategoryID.ToString();
            objAdvEventDisplay.CategoryName = argITem.CategoryID.ToString();

            return objAdvEventDisplay;
        }

        /// <summary>
        /// 根據傳入的AdvEventTypeID, 回傳旗下的廣告, 並且旗下的廣告資料皆已轉換完成
        /// </summary>
        /// <param name="arg_numAdvEventTypeId">AdvEventTypeID</param>
        /// <returns>AdvEventDisplay列表或是null</returns>
        public List<AdvEventDisplay> GetAdvEventDisplayListByAdvEventTypeId(int arg_numAdvEventTypeId)
        {
            AdvEventReposity objAdvEventService = null;
            AdvEventTypeReposity objAdvTypeService = null;
            List<AdvEvent> listAdvEvent = null;
            AdvEventType objAdvType = null;
            List<AdvEventDisplay> listAdvEventDisplay = null;

            objAdvTypeService = new AdvEventTypeReposity();
            objAdvType = objAdvTypeService.GetAdvEventTypeById(arg_numAdvEventTypeId);
            objAdvTypeService = null;

            if (objAdvType != null)
            {
                objAdvEventService = new AdvEventReposity();
                listAdvEvent = objAdvEventService.GetAllAdvEventByAdvEventTypeId(objAdvType.ID);
                objAdvEventService = null;

                if (listAdvEvent != null && listAdvEvent.Count > 0)
                {
                    listAdvEventDisplay = this.TransAdvEventDisplayListByAdvEventList(listAdvEvent);
                }
            }

            return listAdvEventDisplay;
        }

        /// <summary>
        /// 根據傳入的AdvEventTypeID, 回傳旗下Active的廣告, 並且旗下的廣告資料皆已轉換完成
        /// </summary>
        /// <param name="arg_numAdvEventTypeId">AdvEventTtypeId</param>
        /// <returns>AdvEventDisplay列表或是null</returns>
        public List<AdvEventDisplay> GetActiveAdvEventDisplayListByAdvEventTypeId(int arg_numAdvEventTypeId)
        {
            AdvEventReposity objAdvEventService = null;
            AdvEventTypeReposity objAdvTypeService = null;
            List<AdvEvent> listAdvEvent = null;
            AdvEventType objAdvType = null;
            List<AdvEventDisplay> listAdvEventDisplay = null;

            // 根據AdvEventTypeId取得該AdvEventType物件
            objAdvTypeService = new AdvEventTypeReposity();
            objAdvType = objAdvTypeService.GetActiveAdvEventTypeById(arg_numAdvEventTypeId);
            objAdvTypeService = null;

            // 若可以取得AdvType物件, 就執行此區段
            if (objAdvType != null)
            {
                // 取得此AdvType旗下可以顯示的AdvEvent
                objAdvEventService = new AdvEventReposity();
                listAdvEvent = objAdvEventService.GetActiveAdvEventByAdvEventTypeId(objAdvType.ID);
                objAdvEventService = null;

                // 若旗下有AdvEvent, 就將AdvEvent轉換成AdvEventDisplay
                if (listAdvEvent != null && listAdvEvent.Count > 0)
                {
                    listAdvEventDisplay = this.TransAdvEventDisplayListByAdvEventList(listAdvEvent);
                }
            }

            return listAdvEventDisplay;
        }

        /// <summary>
        /// 傳入一整列的AdvEvent, 並將之轉換為AdvEventDisplay
        /// </summary>
        /// <param name="arg_listAdvEventList">List of AdvEvent</param>
        /// <returns>AdvEventDisplay列表或是null</returns>
        public List<AdvEventDisplay> TransAdvEventDisplayListByAdvEventList(List<AdvEvent> arg_listAdvEventList)
        {
            if (arg_listAdvEventList == null)
            {
                return null;
            }

            List<AdvEventDisplay> listAdvEventDisplay = null;
            AdvEventDisplay objAdvEventDisplay = null;
            ItemServiceRepository objItemService = null;
            List<Item> listItem = null;
            Item objItem = null;
            List<int> listItemId = null;
            List<int?> tempListItemId = null;
            IItemService testItemSellorNot = new ItemServiceRepository();
            
            // 取得EventList是否有指定Item, 若有的話, 將Item Id取出, 以利之後的Contain查詢, 加強執行效率
            tempListItemId = arg_listAdvEventList.Where(x => x.ItemID != null).Select(x => x.ItemID).ToList<int?>();

            // 若是EventList列表內有指定需要用Item的話, 就執行此區段
            if (tempListItemId != null && tempListItemId.Count > 0)
            {
                // 進行Item Id的資料轉換
                listItemId = new List<int>();
                foreach (int? numItemId in tempListItemId)
                {
                    if (numItemId != null)
                    {
                        listItemId.Add(Convert.ToInt32(numItemId));
                    }
                }

                // 取得Item
                if (listItemId.Count > 0)
                {
                    objItemService = new ItemServiceRepository();
                    listItem = objItemService.GetItemsByIDs(listItemId, 0);
                    objItemService = null;
                }
            }

            /* ------ start of 進行AdvEvent轉換成AdvEventDisplay ------ */
            listAdvEventDisplay = new List<AdvEventDisplay>();
            foreach (AdvEvent objSubAdvEvent in arg_listAdvEventList)
            {
                DB.TWSqlDBContext twsql = new DB.TWSqlDBContext();
                int categoryID = new int(), storeID = new int();

                objAdvEventDisplay = new AdvEventDisplay();
                if (objSubAdvEvent.ItemID != null)
                {
                    // 若AdvEvent有指定Item, 則從listItem裡取出該Item的資訊
                    objItem = listItem.Where(x => x.ID == objSubAdvEvent.ItemID).FirstOrDefault();
                    #region Sorry...時間緊迫=  = 只能這樣寫...Orz
                    TWNewEgg.DB.TWSQLDB.Models.Category category = twsql.Category.Where(x => x.ID == objItem.CategoryID).FirstOrDefault();
                    TWNewEgg.DB.TWSQLDB.Models.Category second = null;
                    TWNewEgg.DB.TWSQLDB.Models.Category store = null;
                    if (category != null)
                    {
                        categoryID = category.ID;
                        second = twsql.Category.Where(x => x.ID == category.ParentID).FirstOrDefault();
                        if (second != null)
                        {
                            storeID = second.ID;
                            store = twsql.Category.Where(x => x.ID == second.ParentID).FirstOrDefault();
                        }
                    }
                    if (store != null)
                    {
                        storeID = store.ID;
                    }
                    #endregion Sorry...時間緊迫=  = 只能這樣寫...Orz 
                }
                else
                {
                    // 若不是Item項, 一定要將objItem設為null, 以利下方判斷
                    objItem = null;
                }

                //Hash Code
                objAdvEventDisplay.HC = objSubAdvEvent.HashCode;
                //點擊次數
                objAdvEventDisplay.CN = objSubAdvEvent.ClickNumber;
                //廣告型態
                objAdvEventDisplay.AT = objSubAdvEvent.AdvType.ToString();
                //起始日期
                objAdvEventDisplay.SD = objSubAdvEvent.StartDate;
                //結束日期
                objAdvEventDisplay.ED = objSubAdvEvent.EndDate;
                //CSS-1
                objAdvEventDisplay.SCN1 = objSubAdvEvent.StyleClassName1;
                //CSS-2
                objAdvEventDisplay.SCN2 = objSubAdvEvent.StyleClassName2;
                //完售圖片遮罩CSS
                objAdvEventDisplay.SoldOut = objSubAdvEvent.SoldoutClassName;
                //圖片遮罩CSS1
                objAdvEventDisplay.IFC1 = objSubAdvEvent.ImgFilterClassName1;
                //圖片遮罩CSS2
                objAdvEventDisplay.IFC2 = objSubAdvEvent.ImgFilterClassName2;

                // 起始時間
                if (DateTime.Now < objSubAdvEvent.StartDate)
                {
                    // 尚未開始:標題
                    if ((objSubAdvEvent.BeforeTitle == null || objSubAdvEvent.BeforeTitle.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.Title = objItem.Name;
                    }
                    else
                    {
                        objAdvEventDisplay.Title = objSubAdvEvent.BeforeTitle;
                    }

                    // 尚未開始:標語Slogan
                    if ((objSubAdvEvent.BeforeSlogan == null || objSubAdvEvent.BeforeSlogan.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.Slogan = objItem.Sdesc;
                    }
                    else
                    {
                        objAdvEventDisplay.Slogan = objSubAdvEvent.BeforeSlogan;
                    }

                    // 尚未開始:連結位置
                    if ((objSubAdvEvent.BeforeLinkUrl == null || objSubAdvEvent.BeforeLinkUrl.Length <= 0) && objItem != null)
                    {
                        // 連到賣場頁
                        //objAdvEventDisplay.LinkUrl = ITEMURLLINK + objItem.ID.ToString();
                        objAdvEventDisplay.LinkUrl = string.Format(ITEMURLLINK, objItem.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    }
                    else
                    {
                        objAdvEventDisplay.LinkUrl = objSubAdvEvent.BeforeLinkUrl;
                    }

                    // 尚未開始:圖片位置
                    if ((objSubAdvEvent.BeforeImgUrl == null || objSubAdvEvent.BeforeImgUrl.Length <= 0) && objItem != null)
                    {
                        // 賣場產品圖
                        objAdvEventDisplay.ImgUrl = ITEMPICURL + objItem.ID.ToString().PadLeft(8, '0').Substring(0, 4) + "/" + objItem.ID.ToString().PadLeft(8, '0').Substring(4) + "_1_300.jpg";
                    }
                    else
                    {
                        objAdvEventDisplay.ImgUrl = objSubAdvEvent.BeforeImgUrl;
                    }

                    // 尚未開始:圖片說明
                    if ((objSubAdvEvent.BeforeImgAlt == null || objSubAdvEvent.BeforeImgAlt.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.ImgAlt = objItem.Name;
                    }
                    else
                    {
                        objAdvEventDisplay.ImgAlt = objSubAdvEvent.BeforeImgAlt;
                    }
                }
                else if (DateTime.Now >= objSubAdvEvent.StartDate && DateTime.Now <= objSubAdvEvent.EndDate)
                {
                    // 進行中:標題
                    if ((objSubAdvEvent.StartTitle == null || objSubAdvEvent.StartTitle.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.Title = objItem.Name;
                    }
                    else
                    {
                        objAdvEventDisplay.Title = objSubAdvEvent.StartTitle;
                    }

                    // 進行中:標語Slogan
                    if ((objSubAdvEvent.StartSlogan == null || objSubAdvEvent.StartSlogan.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.Slogan = objItem.Sdesc;
                    }
                    else
                    {
                        objAdvEventDisplay.Slogan = objSubAdvEvent.StartSlogan;
                    }

                    // 進行中:連結位置
                    if ((objSubAdvEvent.StartLinkUrl == null || objSubAdvEvent.StartLinkUrl.Length <= 0) && objItem != null)
                    {
                        // Item項要連到賣場頁
                        //objAdvEventDisplay.LinkUrl = ITEMURLLINK + objItem.ID.ToString();
                        objAdvEventDisplay.LinkUrl = string.Format(ITEMURLLINK, objItem.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    }
                    else
                    {
                        objAdvEventDisplay.LinkUrl = objSubAdvEvent.StartLinkUrl;
                    }

                    // 進行中:圖片位置
                    if ((objSubAdvEvent.StartImgUrl == null || objSubAdvEvent.StartImgUrl.Length <= 0) && objItem != null)
                    {
                        // 賣場產品圖
                        objAdvEventDisplay.ImgUrl = ITEMPICURL + objItem.ID.ToString().PadLeft(8, '0').Substring(0, 4) + "/" + objItem.ID.ToString().PadLeft(8, '0').Substring(4) + "_1_300.jpg";
                    }
                    else
                    {
                        objAdvEventDisplay.ImgUrl = objSubAdvEvent.StartImgUrl;
                    }

                    // 進行中:圖片說明
                    if ((objSubAdvEvent.StartImgAlt == null || objSubAdvEvent.StartImgAlt.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.ImgAlt = objItem.Name;
                    }
                    else
                    {
                        objAdvEventDisplay.ImgAlt = objSubAdvEvent.StartImgAlt;
                    }
                }
                else
                {
                    // 已結束:標題
                    if ((objSubAdvEvent.EndTitle == null || objSubAdvEvent.EndTitle.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.Title = objItem.Name;
                    }
                    else
                    {
                        objAdvEventDisplay.Title = objSubAdvEvent.EndTitle;
                    }

                    // 已結束:標語Slogan
                    if ((objSubAdvEvent.EndSlogan == null || objSubAdvEvent.EndSlogan.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.Slogan = objItem.Sdesc;
                    }
                    else
                    {
                        objAdvEventDisplay.Slogan = objSubAdvEvent.EndSlogan;
                    }

                    // 已結束:連結位置
                    if ((objSubAdvEvent.EndLinkUrl == null || objSubAdvEvent.EndLinkUrl.Length <= 0) && objItem != null)
                    {
                        // 連到賣場頁
                        //objAdvEventDisplay.LinkUrl = ITEMURLLINK + objItem.ID.ToString();
                        objAdvEventDisplay.LinkUrl = string.Format(ITEMURLLINK, objItem.ID.ToString(), categoryID.ToString(), storeID.ToString());
                    }
                    else
                    {
                        objAdvEventDisplay.LinkUrl = objSubAdvEvent.EndLinkUrl;
                    }

                    // 已結束:圖片位置
                    if ((objSubAdvEvent.EndImgUrl == null || objSubAdvEvent.EndImgUrl.Length <= 0) && objItem != null)
                    {
                        //產品圖
                        objAdvEventDisplay.ImgUrl = ITEMPICURL + objItem.ID.ToString().PadLeft(8, '0').Substring(0, 4) + "/" + objItem.ID.ToString().PadLeft(8, '0').Substring(4) + "_1_300.jpg";
                    }
                    else
                    {
                        objAdvEventDisplay.ImgUrl = objSubAdvEvent.EndImgUrl;
                    }

                    // 已結束:圖片說明
                    if ((objSubAdvEvent.EndImgAlt == null || objSubAdvEvent.EndImgAlt.Length <= 0) && objItem != null)
                    {
                        objAdvEventDisplay.ImgAlt = objItem.Name;
                    }
                    else
                    {
                        objAdvEventDisplay.ImgAlt = objSubAdvEvent.EndImgAlt;
                    }
                }

                if (objItem != null)
                {
                    // 本廣告ItemID
                    objAdvEventDisplay.ItemID = objSubAdvEvent.ItemID == null ? 0 : Convert.ToInt32(objSubAdvEvent.ItemID);
                    // 本廣告ItemID Pic起始編號
                    objAdvEventDisplay.ItemIDPicStart = (objItem.PicStart ?? 0);
                    // 本廣告ItemID Pic結束編號
                    objAdvEventDisplay.ItemIDPicEnd = (objItem.PicEnd ?? 0);
                    // 本廣告ItemID數量
                    objAdvEventDisplay.ItemStock = testItemSellorNot.GetSellingQty(objItem.ID, "Item");
                    // 本廣告ItemID model
                    objAdvEventDisplay.ItemModel = (objItem.Model ?? "");
                    // 本廣告ItemID manufacture
                    objAdvEventDisplay.ItemManufacture = objItem.ManufactureID.ToString();
                    // 本廣告ItemID市場價格
                    objAdvEventDisplay.MarketPrice = (objItem.MarketPrice ?? 0);
                    // 本廣告ItemID販售價格
                    objAdvEventDisplay.SalePrice = objItem.PriceCash;
                    // 本廣告ItemID運輸價格
                    objAdvEventDisplay.ShippingPrice = objItem.PriceGlobalship;
                    // 本廣告ItemID稅負價格
                    objAdvEventDisplay.TaxPrice = (objItem.Taxfee ?? 0);
                    // 本廣告ItemID服務費價格
                    objAdvEventDisplay.ServiceFee = objItem.ServicePrice;
                    // 本廣告ItemID折扣
                    if (objAdvEventDisplay.MarketPrice >= objAdvEventDisplay.SalePrice)
                    {
                        if (objItem.MarketPrice != 0)
                        {
                            objAdvEventDisplay.SavingPercent = (Math.Round((objAdvEventDisplay.SalePrice / objAdvEventDisplay.MarketPrice), 2, MidpointRounding.AwayFromZero) * 100).ToString();
                        }
                        else
                        {
                            objAdvEventDisplay.SavingPercent = "100";
                        }
                    }
                    else
                    {
                        objAdvEventDisplay.SavingPercent = "";
                    }

                    objAdvEventDisplay.CategoryID = objItem.CategoryID.ToString();
                    objAdvEventDisplay.CategoryName = objItem.CategoryID.ToString();
                }

                listAdvEventDisplay.Add(objAdvEventDisplay);
            } //end foreach
            /* ------ end of of 進行AdvEvent轉換成AdvEventDisplay ------ */

            return listAdvEventDisplay;
        }
    }
}
