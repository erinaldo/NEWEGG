using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryItem.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.MobileStoreRepoAdapters;
using TWNewEgg.MobileStoreRepoAdapters.Interface;
using TWNewEgg.MobileStoreServices;
using TWNewEgg.MobileStoreServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.MobileStore;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.Models.DomainModels.GroupBuy;
using TWNewEgg.StoreRepoAdapters.Interface;
using TWNewEgg.StoreServices.Const;
using TWNewEgg.StoreServices.Interface;
using TWNewEgg.ItemServices;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.GroupBuyServices;
using TWNewEgg.GroupBuyServices.Interface;

namespace TWNewEgg.MobileStoreServices
{
    public class MobileStoreService : IMobileStoreService
    {
        private IStoreRepoAdapter _iStoreRepoAdapter;
        private IMobileStoreRepoAdapter _iMobileStoreRepoAdapter;
        private IItemRepoAdapter _iItemRepoAdapter;
        //private IItemStockRepoAdapter _itemStockRepoAdapter;
        private IItemStockService _ItemStockServices = null;

        private ICategoryItemService _icategoryItemService;
        private IStoreService _StoreService;
        private IItemImageUrlService _itemImageUrlService;
        private IGroupBuyService _igroupBuyService;
        private IItemDetailService _itemDetailService;
        private IItemDisplayPriceService _itemDisplayPriceService;

        private const int FEATURED_CATEGORYID = 0; // 用來判別是否為精選分類.
        private const string FEATURED_CATEGORY_TITLE = "精選商品";
        private const int LogoImageSize = 300;
        private const int HOMESTORE_CATEGORYID = 0; // 指定CategoryID為0就代表是首頁.

        public MobileStoreService(IStoreRepoAdapter iStoreRepoAdapter, IMobileStoreRepoAdapter iMobileStoreRepoAdapter, IItemRepoAdapter iItemRepoAdapter)
        {
            this._iStoreRepoAdapter = iStoreRepoAdapter;
            this._iMobileStoreRepoAdapter = iMobileStoreRepoAdapter;
            this._iItemRepoAdapter = iItemRepoAdapter;

            this._ItemStockServices = AutofacConfig.Container.Resolve<IItemStockService>();
            this._icategoryItemService = AutofacConfig.Container.Resolve<ICategoryItemService>();
            this._StoreService = AutofacConfig.Container.Resolve<IStoreService>();
            this._itemImageUrlService = AutofacConfig.Container.Resolve<IItemImageUrlService>();
            this._igroupBuyService = AutofacConfig.Container.Resolve<IGroupBuyService>();
            this._itemDetailService = AutofacConfig.Container.Resolve<IItemDetailService>();
            this._itemDisplayPriceService = AutofacConfig.Container.Resolve<IItemDisplayPriceService>();
        }

        //public List<ShopWindow> GetShopWindows(int mainCategoryID, int selCategoryID,int? accountID=null)
        //{
        //    //mainCategoryID 指的是category.Layer=0
        //    //selCategoryID 指的是category.Layer=2,如果selCategoryID = 0表示選"精選商品"
        //    //用mainCategoryID抓category.categoryID=mainCategoryID得到[MStoreInfo]
        //    //用mainCategoryID抓advertising.BannerType=4得到[List<StoreBanner> PullDownBannerList]
        //    //用mainCategoryID 找到category.Layer=1 再找 category.Layer=2 AND ParentID IN(category.Layer=1) AND category.mobileSel=1 ORDER BY category.mobileorder的同層category 放[List<MShopWindow> ShopWindowList]
        //    //用group by + count 過濾掉商品數0 的category item.categoryID=selCategoryID http://stackoverflow.com/questions/11564311/sql-to-entity-framework-count-group-by
        //    //用MainCategoryID到ItemForChoice"計算"是否有精選商品,有的話.要去categorylist多一個"精選商品"
        //    //如果selCategoryID=0,則到ItemForChoice撈categoryID = mainCategoryID的item,但如果item.count = 0,則改抓categorylist第一個,放入[List<MStoreItemCell> ItemList]
        //    //如果selCategoryID!=0,則用CategoryItemService ,再過濾出前40筆,放入[List<MStoreItemCell> ItemList]
        //    //用GroupBuyService.QueryViewInfo判斷和取得閃購,特價資訊
        //    //用accountID去TrackService.GetTracksStatus判斷item已經加過追蹤,需登入,才有這部分資訊
        //    /*
        //    declare @cateid int;
        //    Set @cateid=1;
        //    select * from category WHERE ID=@cateid;
        //    select * from category WHERE Layer=2 AND ShowAll=1 AND ParentID IN(select ID from category WHERE ParentID IN(select ID from category WHERE ID=@cateid));
        //    select * from ItemForChoice WHERE CategoryID=@cateid ORDER BY Showorder;
        //    select * from advertising WHERE CategoryID=@cateid AND BannerType=4 ORDER BY Showorder;
        //     */
        //    List<ShopWindow> ret = new List<ShopWindow>();

        //    return ret;
        //}

        #region Service Interface

        public MStoreInfo GetMobileStoreInfo(int categoryID, int subCategoryID)
        {
            // 先找出Layer=0,1的節點.
            List<Category> caterogyList = this._iStoreRepoAdapter.Category_GetAll().Where(x =>
                (x.ID == categoryID || (x.ParentID == categoryID && x.Layer == 1)) && x.ShowAll == ConstShowAll.Show).ToList();

            // 找出Layer=0的主Store, 並排除不合法的情況.
            Category mainStore = caterogyList.Where(x => x.ID == categoryID).FirstOrDefault();
            if (mainStore == null)
                return null;

            // 挑出Layer=1的節點.
            List<int> layer1StoreIDs = caterogyList.Where(x => x.ParentID == categoryID).Select(x => x.ID).ToList();

            // 找出Layer=2的節點.
            List<Category> layer2StoreList = this._iStoreRepoAdapter.Category_GetAll().Where(x =>
                layer1StoreIDs.Contains(x.ParentID) && x.Layer == 2 && x.ShowAll == ConstShowAll.Show).OrderBy(x => x.Showorder).ToList();
            
            MStoreInfo retStore = new MStoreInfo()
            {
                Title = mainStore.Description,
                BannerList = GetMobileBannerList(categoryID),
                StoreTabList = ConvertToStoreTabList(layer2StoreList, categoryID, ref subCategoryID),
            };
            
            // 判空處理, 避免無意義的調用.
            if (retStore.StoreTabList != null && retStore.StoreTabList.Any())
            {
                // 填入確認過合法的指定ID.
                retStore.SubCategoryID = subCategoryID;
                retStore.ItemList = GetMobileStoreItems(categoryID, subCategoryID);
            }

            return retStore;
        }

        public List<MStoreItemCell> GetMobileStoreItems(int categoryID, int subCategoryID)
        {
            //A.若 categoryID == HOMESTORE_CATEGORYID ,且subCategoryID != 0 , 則抓首頁的....不用做
            //B.若 categoryID != HOMESTORE_CATEGORYID ,且subCategoryID == 0 , 則抓 categoryID 此類的精選商品的10項商品
            //C.若 categoryID != HOMESTORE_CATEGORYID ,且subCategoryID != 0 , 則抓 subCategoryID 此子類(第三層的分類)的前40項商品
            //  用CategoryItemService
      
            List<MStoreItemCell> result = new List<MStoreItemCell>();
            if (categoryID != 0 && subCategoryID == 0)
            {
                //精選商品
                List<int> lCategoryItems = this._iMobileStoreRepoAdapter.ItemForChoice_GetAll()
                .Where(t => t.Showall == ConstShowAll.Show && t.CategoryID == categoryID)
                .OrderBy(t => t.Showorder).Select(t => t.ItemID).Take(10).ToList();
                result = ConvertToMobileStoreItems(lCategoryItems);
            }
            else if (subCategoryID != 0)
            {
                //分類頁商品
                //ICategoryItemService _icategoryItemService = AutofacConfig.Container.Resolve<ICategoryItemService>();
                List<ItemInfo> lCategoryItems = _icategoryItemService.GetCategoryItemsTopRows(new CategoryItemConditions { CategoryID = subCategoryID }, 40);//.AsQueryable().Select(t => t.ItemBase.ID).ToList();
                result = ConvertToMobileStoreItems(lCategoryItems);
            }
            

            return result;
        }

        #endregion Service Interface

        private List<StoreBanner> GetMobileBannerList(int categoryID)
        {
            //IStoreService _StoreService = AutofacConfig.Container.Resolve<IStoreService>();
            return _StoreService.GetBanner(categoryID, ConstBannerType.Mobile);
        }

        private List<MStoreTab> ConvertToStoreTabList(List<Category> storeList, int categoryID, ref int subCategoryID)
        {
            List<MStoreTab> retList = new List<MStoreTab>();
            
            // 檢查是否要加入精選商品的分類(判定依據: 精選商品數>0).
            AddFeaturedCategoryTab(retList, categoryID);

            // 決定剩餘分類的條件：1.是否為PM指定 2.該分類商品數>0
            
            // 1.目前暫時做法，只取前10個(之後要改成讀取PM指定的類別)
            //List<Category>  filterStores = storeList.Take(10).ToList();
            List<Category> filterStores = storeList.Where(x => x.IsMobile == 1).OrderBy(x => x.MobileOrder).ToList();

            // 2.該分類商品數>0
            filterStores = FilterStoreWithItem(filterStores);
            filterStores = filterStores.Take(10).ToList();

            // 進行Convert.
            foreach (Category store in filterStores)
            {
                MStoreTab tab = new MStoreTab()
                {
                    ID = store.ID,
                    Title = store.Description
                };

                retList.Add(tab);
            }

            // 找出目前清單中的ID集合.
            List<int> tabIDs = retList.Select(t => t.ID).ToList();
            
            //決定最終要顯示哪個subCategoryStore.
            if (!tabIDs.Contains(subCategoryID))
            {
                if (tabIDs.Any())
                {
                    // 如果發現指定的ID不在所有類別中, 就用第一個取代.
                    subCategoryID = tabIDs[0];
                }
                else
                {
                    // 如果連精選商品也沒有, 就回傳null,並且不調用後續的item service.
                    return null;
                }
            }

            return retList;
        }

        /// <summary>
        /// 將分類中沒有商品的類別剔除.
        /// </summary>
        /// <param name="filterStores"></param>
        /// <returns></returns>
        private List<Category> FilterStoreWithItem(List<Category> filterStores)
        {
            List<int> categoryIDs = filterStores.Select(t => t.ID).ToList();

            // 代表至少在item表中有一個項目的category.
            List<int> itemCateogryIDs = _iItemRepoAdapter.GetAll().Where(x => categoryIDs.Contains(x.CategoryID))
                .Select(x => x.CategoryID).Distinct().ToList();

            // 用這個資料來過濾原本的store.
            return filterStores.Where(t => itemCateogryIDs.Contains(t.ID)).ToList();
        }

        private void AddFeaturedCategoryTab(List<MStoreTab> retList, int categoryID)
        {
            // 檢查該商品的類別是否有精選商品.
            int featuredItemCount = _iMobileStoreRepoAdapter.ItemForChoice_GetAll()
                .Where(t => t.CategoryID == categoryID && t.Showall == ConstShowAll.Show).Count();

            // 有才加入 "精選商品" 的分類Tab.
            if (featuredItemCount > 0)
            {
                retList.Add(new MStoreTab()
                {
                    ID = FEATURED_CATEGORYID,
                    Title = FEATURED_CATEGORY_TITLE
                });
            }
        }

        private string GenerateItemUrl(int itemID)
        {
            return string.Format("/item?itemid={0}", itemID);
        }

        private string GenerateLogoImageUrl(int manufactureID, int imageSize)
        {
            return string.Format("/pic/manufacture/{0}/{1}_1_{2}.jpg",
                (manufactureID / 10000).ToString("0000"),
                (manufactureID % 10000).ToString("0000"),
                imageSize);
        }

        private Dictionary<int, List<ImageUrlReferenceDM>> GetItemImageUrlDict(List<int> itemIDs)
        {
            //IItemImageUrlService _itemImageUrlService = AutofacConfig.Container.Resolve<IItemImageUrlService>();
            Dictionary<int, List<ImageUrlReferenceDM>> images = _itemImageUrlService.GetItemImagePath(itemIDs);

            return images;
        }

        private List<MStoreItemCell> ConvertToMobileStoreItems(List<ItemInfo> items)
        {
            int imageSize = 300;
            int order = 1;
            List<int> itemIDs = items.Select(x => x.ItemBase.ID).ToList();
            Dictionary<int, List<ImageUrlReferenceDM>> images = this.GetItemImageUrlDict(itemIDs);

            // 取商品的促銷資訊：商品的特價、折數、美蛋閃購區(icon:美蛋同步)、獨家販售(icon:獨家)
            // 因GroupBuyService無法用ItemID查詢，故抓全部的資料(目前先取5000筆)
            List<GroupBuyViewInfo> lGroupBuyItems = _igroupBuyService.QueryViewInfo(new GroupBuyQueryCondition { PageNumber = 1, PageSize = 5000 });

            // 因GroupBuyService回傳的資料無ItemID，故另外取ItemID和GroupBuyID
            var dGroupBuyItemID = this._iMobileStoreRepoAdapter.GroupBuy_GetAll().Where(t => itemIDs.Contains(t.ItemID))
                .Select(t => new {t.ItemID, t.ID}).OrderByDescending(t => new { t.ItemID, t.ID} ).ToList();
            
            int i = 0;
            List<MStoreItemCell> retList = new List<MStoreItemCell>();
            TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty objItemSellingQty = null;


            // 取得總價化後的商品顯示價格
            Dictionary<int, ItemPrice> priceDictionary = _itemDisplayPriceService.GetItemDisplayPrice(itemIDs);
            foreach (ItemInfo x in items)
            {
                int itemID = x.ItemBase.ID;
                decimal displayPrice = priceDictionary[x.ItemBase.ID] == null ? x.ItemBase.PriceCash : priceDictionary[x.ItemBase.ID].DisplayPrice;
                ImageUrlReferenceDM img = images[itemID].Where(y => y.Size == imageSize && y.SizeIndex == order).FirstOrDefault();

                decimal dPercent = 0, dPrice = 0;
                bool isFlashUS = false, isExclusiveSale = false, isSoldOut = false;

                // 取Item在GroupBuy的資料(促銷資訊)
                GroupBuyViewInfo groupBuyItem = lGroupBuyItems.Where(t => dGroupBuyItemID.Where(a => a.ItemID == itemID).Select(a=>a.ID).ToList().Contains(t.ID)).FirstOrDefault();
                if (groupBuyItem != null)
                {
                    dPrice = Convert.ToDecimal(groupBuyItem.GroupBuyPrice);
                    decimal sum = Math.Round(dPrice / Convert.ToDecimal(groupBuyItem.OriginalPrice), 2) * 100;
                    dPercent = sum;
                    isFlashUS = groupBuyItem.IsShowNeweggUSASync;
                    isExclusiveSale = groupBuyItem.IsShowExclusive;
                    isSoldOut = groupBuyItem.IsSoldOut;
                }
                else
                {
                    // 如果沒有SoldOut資訊, 就要自己取一下Qty來判斷.
                    objItemSellingQty = this._ItemStockServices.GetItemSellingQtyByItemId(itemID);
                    int iQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                    isSoldOut = iQty <= 0;
                }

                MStoreItemCell opItem = new MStoreItemCell()
                {
                    ItemID = itemID,
                    Title = x.ItemBase.Name,
                    ItemImage = img != null ? img.ImageUrl : "",
                    LogoImage = GenerateLogoImageUrl(x.ItemBase.ManufactureID, imageSize),
                    UnitPrice = displayPrice,
                    OriginalPrice = x.ItemBase.MarketPrice == null ? 0 : x.ItemBase.MarketPrice.Value,
                    Url = GenerateItemUrl(itemID),

                    PromoInfo = groupBuyItem == null ? null : new ItemPromoInfo()
                    {
                        PromoPercent = dPercent,
                        PromoPrice = dPrice,
                        IsFlashUS = isFlashUS,
                        IsExclusiveSale = isExclusiveSale,
                    },
                    //IsTrack = isTrackItem,//追蹤
                    IsSoldOut = isSoldOut,
                };

                retList.Add(opItem);
                i++;
            }

            return retList;
        }

        private List<MStoreItemCell> ConvertToMobileStoreItems(List<int> itemIDs)
        {
            TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty objItemSellingQty;
            List<MStoreItemCell> results = new List<MStoreItemCell>();
            if (itemIDs == null || itemIDs.Count == 0) return results;
            //IItemDetailService _itemDetailService = AutofacConfig.Container.Resolve<IItemDetailService>();

            // 取商品的促銷資訊：商品的特價、折數、美蛋閃購區(icon:美蛋同步)、獨家販售(icon:獨家)
            // 因GroupBuyService無法用ItemID查詢，故抓全部的資料(目前先取5000筆)
            //IGroupBuyService _igroupBuyService = AutofacConfig.Container.Resolve<IGroupBuyService>();
            List<GroupBuyViewInfo> lGroupBuyItems = _igroupBuyService.QueryViewInfo(new GroupBuyQueryCondition { PageNumber = 1, PageSize = 5000 });

            // 因GroupBuyService回傳的資料無ItemID，故另外取ItemID和GroupBuyID
            var dGroupBuyItemID = this._iMobileStoreRepoAdapter.GroupBuy_GetAll().Where(t => itemIDs.Contains(t.ItemID))
                .Select(t => new { t.ItemID, t.ID }).OrderByDescending(t => new { t.ItemID, t.ID }).ToList();

            // 取Item資訊
            //IItemRepoAdapter _itemRepoAdapter = AutofacConfig.Container.Resolve<IItemRepoAdapter>();
            List<MStoreItemCell> rtn = new List<MStoreItemCell>();
            Dictionary<int, Models.DBModels.TWSQLDB.Item> itemlist = _iItemRepoAdapter.GetItemList(itemIDs);
            //IItemImageUrlService _itemImageUrlService = AutofacConfig.Container.Resolve<IItemImageUrlService>();
            //IItemDisplayPriceService _itemDisplayPriceService = AutofacConfig.Container.Resolve<IItemDisplayPriceService>();
            Dictionary<int, ItemPrice> priceDictionary = _itemDisplayPriceService.GetItemDisplayPrice(itemIDs);
            int size = 300;
            int order = 1;
            int i = 0;
            Dictionary<int, List<ImageUrlReferenceDM>> images = _itemImageUrlService.GetItemImagePath(itemIDs);
            foreach (Models.DBModels.TWSQLDB.Item item in itemlist.Values)
            {
                string sPath = "";
                if (priceDictionary.ContainsKey(item.ID) == false) continue;
                ImageUrlReferenceDM img = images[item.ID].Where(x => x.Size == size && x.SizeIndex == order).FirstOrDefault();
                if (img != null) sPath = img.ImageUrl;
                else continue;//item 不存在或不能用?

                decimal dPercent = 0, dPrice = 0;
                bool isFlashUS = false, isExclusiveSale = false, isSoldOut = false;

                // 取Item在GroupBuy的資料(促銷資訊)
                GroupBuyViewInfo groupBuyItem = lGroupBuyItems.Where(t => dGroupBuyItemID.Where(a => a.ItemID == item.ID).Select(a => a.ID).ToList().Contains(t.ID)).FirstOrDefault();
                if (groupBuyItem != null)
                {
                    dPrice = Convert.ToDecimal(groupBuyItem.GroupBuyPrice);
                    decimal sum = Math.Round(dPrice / Convert.ToDecimal(groupBuyItem.OriginalPrice), 2) * 100;
                    dPercent = sum;
                    isFlashUS = groupBuyItem.IsShowNeweggUSASync;
                    isExclusiveSale = groupBuyItem.IsShowExclusive;
                    isSoldOut = groupBuyItem.IsSoldOut;
                }
                else
                {
                    // 如果沒有SoldOut資訊, 就要自己取一下Qty來判斷.
                    objItemSellingQty = this._ItemStockServices.GetItemSellingQtyByItemId(item.ID);
                    int iQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                    isSoldOut = iQty <= 0;
                }

                MStoreItemCell storeitem = new MStoreItemCell()
                {
                    ItemID = item.ID,
                    Title = item.Name,
                    ItemImage = sPath,
                    LogoImage = GenerateLogoImageUrl(item.ManufactureID, size),
                    UnitPrice = priceDictionary[item.ID].DisplayPrice,
                    OriginalPrice = item.MarketPrice == null ? 0 : item.MarketPrice.Value,
                    Url = GenerateItemUrl(item.ID),
                    PromoInfo = groupBuyItem == null ? null : new ItemPromoInfo()
                    {
                        PromoPercent = dPercent,
                        PromoPrice = dPrice,
                        IsFlashUS = isFlashUS,
                        IsExclusiveSale = isExclusiveSale,
                    },
                    //IsTrack = isTrackItem,//追蹤
                    IsSoldOut = isSoldOut,
                };
                rtn.Add(storeitem);
                i++;
            }
            foreach (var orderItem in itemIDs)
            {
                if (rtn.Where(x => x.ItemID == orderItem).FirstOrDefault() != null)
                {
                    results.Add(rtn.Where(x => x.ItemID == orderItem).FirstOrDefault());
                }
            }
            return results;
        }
    }
}
