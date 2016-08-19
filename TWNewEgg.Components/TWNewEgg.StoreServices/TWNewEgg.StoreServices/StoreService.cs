using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CategoryItem.Interface;
using TWNewEgg.CategoryServices.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.ItemRepoAdapters;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.ItemServices;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.PropertyServices;
using TWNewEgg.PropertyServices.Interface;
using TWNewEgg.StoreRepoAdapters;
using TWNewEgg.StoreRepoAdapters.Interface;
using TWNewEgg.StoreServices.Const;
using TWNewEgg.StoreServices.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.StoreServices
{
    public class StoreService : IStoreService, IDisposable
    {
        private IStoreRepoAdapter _iStoreRepoAdapter;
        private const int DEFAULT_PAGE_ITEMCOUNT = 8;
        private ILifetimeScope scope = AutofacConfig.Container.BeginLifetimeScope();
        private IItemRepoAdapter _itemRepoAdapter;
        private IItemImageUrlService _itemImageUrlService;
        private IItemDisplayPriceService _itemDisplayPriceService;
        private IPropertyService _propertyService;
        private ICategoryItemService _iCategoryItemService;
        private IItemStockService _ItemStockService;
        private ICategoryServices _iCategoryServices;

        public StoreService(IStoreRepoAdapter iStoreRepoAdapter)
        {
            this._iStoreRepoAdapter = iStoreRepoAdapter;
            _itemRepoAdapter = scope.Resolve<IItemRepoAdapter>();
            _itemImageUrlService = scope.Resolve<IItemImageUrlService>();
            _itemDisplayPriceService = scope.Resolve<IItemDisplayPriceService>();
            _propertyService = scope.Resolve<IPropertyService>();
            _iCategoryItemService = scope.Resolve<ICategoryItemService>();
            _ItemStockService = scope.Resolve<IItemStockService>();
            _iCategoryServices = scope.Resolve<ICategoryServices>();
        }

        public StoreInfo GetStoreInfo(int categoryID, List<int> shopWindowIndexList)        
        {
            StoreInfo mainStore = new StoreInfo();
            mainStore.Title = this._iStoreRepoAdapter.Category_GetAll().Where(x => x.ID == categoryID).Select(x => x.Description).FirstOrDefault();
            
            // 取得櫥窗資料
            mainStore.ID = categoryID.ToString();
            mainStore.ShopWindowList = GetShopWindows(categoryID, null);
            mainStore.ShopWindowCount = mainStore.ShopWindowList.Count;
            
            // 取得廣告部分
            mainStore.PullDownBannerList = ConvertToStoreBannerList(this._iStoreRepoAdapter.Advertising_GetAll()
                .Where(x => x.CategoryID == categoryID && x.BannerType == ConstBannerType.Rotate && x.ShowAll == ConstShowAll.Show)
                .OrderBy(x => x.Showorder).ToList());
            mainStore.PullDownAdvList = ConvertToStoreBannerList(this._iStoreRepoAdapter.Advertising_GetAll()
                .Where(x => x.CategoryID == categoryID && x.BannerType == ConstBannerType.PullDown && x.ShowAll == ConstShowAll.Show)
                .OrderBy(x => x.Showorder).ToList());
            mainStore.SingleBanner = ConvertToStoreBanner(this._iStoreRepoAdapter.Advertising_GetAll()
                .Where(x => x.CategoryID == categoryID && x.BannerType == ConstBannerType.Single && x.ShowAll == ConstShowAll.Show)
                .OrderBy(x => x.Showorder).FirstOrDefault());
            
            // 取得電梯部分
            mainStore.Elevator = new List<ElevatorItem>();
            foreach (ShopWindow win in mainStore.ShopWindowList)
            {
                mainStore.Elevator.Add(new ElevatorItem
                {
                    Title = win.MainZone.Title                    
                });
            }
            
            return mainStore;
        }

        public List<ShopWindow> GetShopWindows(int categoryID, List<int> shopWindowIndexList)
        {
            List<SubCategory_NormalStore> storelist = this._iStoreRepoAdapter.NormalStore_GetAll()
                .Where(t => t.ShowAll == ConstShowAll.Show && t.CategoryID == categoryID)
                .OrderBy(t => t.Showorder).ToList();

            int iStoreindex = 0;
            List<ShopWindow> ret = new List<ShopWindow>();
            foreach (SubCategory_NormalStore store in storelist)
            {
                if (shopWindowIndexList == null || shopWindowIndexList.Contains(iStoreindex))
                {
                    ret.Add(new ShopWindow()
                    {
                        ID = store.ID.ToString(),
                        Index = iStoreindex,
                        LayoutType = store.StoreClass == ConstStoreClass.UsaDirect ? ConstLayoutType.UsaDirect : ConstLayoutType.Normal,
                        MainZone = new MainZone
                        {
                            Title = store.Title,
                            Image = store.StoreImageURL,
                            LogoList = GetLogoList(store.ID),
                            ItemList = GetZoneItemList(store.ID, ConstZone.Main),
                            GroupLinkList = GetGroupLinkList(store.ID),
                        },
                        ListZone = new ListZone { ItemList = GetZoneItemList(store.ID, ConstZone.List) },
                    });
                }
                
                iStoreindex++;
            }
            
            return ret;
        }

        public bool IsChooseAny(int categoryID)
        {
            bool isOptionStore = false;

            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);

            SubCategory_OptionStore optionStore = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x =>
                x.CategoryID == categoryID && x.ShowAll == ConstShowAll.Show && (x.DateEnd > datetimeNow || x.DateEnd == null) && (x.DateStart < datetimeNow || x.DateStart == null)).OrderBy(x => x.Showorder).FirstOrDefault();
            
            if (optionStore != null)
            {
                isOptionStore = true;
            }

            return isOptionStore;
        }

        public OptionStoreInfo GetOptionStoreInfo(int categoryID, int pageIndex, int pageItemCount, string filterIDs, string sortValue)
        {
            // 獲取任選館的基本資訊.
            SubCategory_OptionStore optionStore = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x =>
                x.CategoryID == categoryID && x.ShowAll == ConstShowAll.Show).OrderBy(x => x.Showorder).FirstOrDefault();

            // 如果該館不是任選館,回傳null.
            if (optionStore == null)
                return null;

            // 填入標題等基本資訊.
            OptionStoreInfo retStore = ConvertToOptionStoreInfo(optionStore);

            // 排序清單.
            retStore.SortOptionList = this.GetSortOptionList();
            
            // 獲取Banner.
            List<Advertising> banners = this._iStoreRepoAdapter.Advertising_GetAll().Where(x => x.CategoryID == categoryID &&
                x.ShowAll == ConstShowAll.Show).ToList();
            retStore.SingleBanner = this.ConvertToStoreBanner(banners.Where(x => x.BannerType == ConstBannerType.Single)
                .OrderBy(x => x.Showorder).FirstOrDefault());
            retStore.PullDownAdvList = this.ConvertToStoreBannerList(banners.Where(x => x.BannerType == ConstBannerType.LeftAdv)
                .OrderBy(x => x.Showorder).ToList());

            // 麵包屑選單. 
            // The function is older, so don't use it.
            //retStore.Breadcurmbs = this.GetBreadCrumbs(categoryID);
            
            
            // 屬性過濾區.
            List<PropertyGroup> propertyFilter = GetPropertyGroup(categoryID);

            // 產品列表區域.
            retStore.ListZone = this.GetOptionStoreListZoneEx(categoryID, pageIndex, pageItemCount, filterIDs, sortValue, optionStore.IsFormat);
            return retStore;
        }

        public OptionStoreListZone GetOptionStoreListZone(int categoryID, int pageIndex, int pageItemCount, string filterIDs, string sortValue)
        {
            // 檢查是否為任選館.
            SubCategory_OptionStore optionStore = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x =>
                x.CategoryID == categoryID && x.ShowAll == ConstShowAll.Show).OrderBy(x => x.Showorder).FirstOrDefault();

            // 如果該館不是任選館,回傳null.
            if (optionStore == null)
                return null;

            return this.GetOptionStoreListZoneEx(categoryID, pageIndex, pageItemCount, filterIDs, sortValue, optionStore.IsFormat);
        }

        public List<StoreItemCell> GetStoreItems(List<int> itemIDs)
        {
            List<StoreItemCell> rtn = new List<StoreItemCell>();
            if (itemIDs == null || itemIDs.Count == 0) return rtn;
            //IItemDetailService _itemDetailService = AutofacConfig.Container.Resolve<IItemDetailService>();
            //IItemImageUrlService _itemImageUrlService = AutofacConfig.Container.Resolve<IItemImageUrlService>();            
            //int size = 300;
            //int order = 1;
            //Dictionary<int,List<ImageUrlReferenceDM>> images = _itemImageUrlService.GetItemImagePath(itemIDs);
            //foreach (int itemID in itemIDs)
            //{
            //    ItemDetail item = _itemDetailService.GetItemDetail(itemID);
            //    string sPath = "";
            //    ImageUrlReferenceDM img = images[itemID].Where(x => x.Size == size && x.SizeIndex == order).FirstOrDefault();
            //    if (img != null) sPath = img.ImageUrl;
            //    StoreItemCell storeitem = new StoreItemCell()
            //    {
            //        ItemID = itemID,
            //        LogoImage = string.Format("/pic/manufacture/{0}/{1}_1_{2}.jpg", (item.Main.ItemBase.ManufactureID / 10000).ToString("0000"), (item.Main.ItemBase.ManufactureID % 10000).ToString("00"), size),
            //        Title = item.Main.ItemBase.Name,
            //        UnitPrice = item.Price.DisplayPrice,
            //        Url = "/item?itemid=" + itemID,
            //        ItemImage = sPath
            //    };
            //    rtn.Add(storeitem);
            //}
            
            // 自己取資料,不用ItemDetailService的寫法
            //IItemRepoAdapter _itemRepoAdapter = AutofacConfig.Container.Resolve<IItemRepoAdapter>();
            Dictionary<int, Models.DBModels.TWSQLDB.Item> itemlist = _itemRepoAdapter.GetItemList(itemIDs)
                .Where(x => x.Value.Status == 0 && x.Value.ShowOrder >= 0)
                .ToDictionary(x => x.Key, x => x.Value);
            //IItemImageUrlService _itemImageUrlService = AutofacConfig.Container.Resolve<IItemImageUrlService>();
            //IItemDisplayPriceService _itemDisplayPriceService = AutofacConfig.Container.Resolve<IItemDisplayPriceService>();
            Dictionary<int, ItemPrice> priceDictionary = _itemDisplayPriceService.GetItemDisplayPrice(itemIDs);
            int size = 300;
            int order = 1;
            Dictionary<int, List<ImageUrlReferenceDM>> images = _itemImageUrlService.GetItemImagePath(itemIDs);
            List<Category_TreeItem> allCategories = _iCategoryServices.GetAllParentCategoriesByItemIDs(itemIDs);
            //SellingQty
            Dictionary<int, int> dictItemSellingQty = new Dictionary<int,int>();
            //取得SellingQty
            dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(itemIDs);
            foreach (Models.DBModels.TWSQLDB.Item item in itemlist.Values)
            {
                string sPath = "";
                if (priceDictionary.ContainsKey(item.ID) == false) continue;
                ImageUrlReferenceDM img = images[item.ID].Where(x => x.Size == size && x.SizeIndex == order).FirstOrDefault();
                Category_TreeItem category = allCategories.Where(x => x.category_id == item.CategoryID).FirstOrDefault();
                if (img != null) sPath = img.ImageUrl;
                else continue;//item 不存在或不能用?
                StoreItemCell storeitem = new StoreItemCell()
                {
                    ItemID = item.ID,
                    LogoImage = GenerateLogoImageUrl(item.ManufactureID, size),
                    Title = item.Name,
                    //副標
                    SubTitle = item.Spechead,
                    //原價
                    MarketPrice = item.MarketPrice ?? 0,
                    UnitPrice = priceDictionary[item.ID].DisplayPrice,
                    //UnitPrice = item.PriceCash,
                    Url = GenerateItemUrl(item.ID, category),
                    ItemImage = sPath,
                    SellingQty = dictItemSellingQty[item.ID]
                };
                rtn.Add(storeitem);
            }

            return rtn;
        }
        
        public List<OptionStoreItemCell> GetOptionStoreItems(List<int> itemIDs)
        {
            List<OptionStoreItemCell> rtn = new List<OptionStoreItemCell>();
            if (itemIDs == null || itemIDs.Count==0) return rtn;
            Dictionary<int, Models.DBModels.TWSQLDB.Item> itemlist = _itemRepoAdapter.GetItemList(itemIDs);

            Dictionary<int, ItemPrice> priceDictionary = _itemDisplayPriceService.GetItemDisplayPrice(itemIDs);
            Dictionary<int, List<ImageUrlReferenceDM>> images = _itemImageUrlService.GetItemImagePath(itemIDs);
            List<Category_TreeItem> allCategories = _iCategoryServices.GetAllParentCategoriesByItemIDs(itemIDs);
            //SellingQty
            Dictionary<int, int> dictItemSellingQty = null;
            int size = 300;
            int order = 1;

            //取得SellingQty
            dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(itemIDs);
            foreach (Models.DBModels.TWSQLDB.Item item in itemlist.Values)
            {
                if (item.ID <= 0) continue;
                if (priceDictionary.ContainsKey(item.ID) == false) continue;
                string sPath = "";
                ImageUrlReferenceDM img = images[item.ID].Where(x => x.Size == size && x.SizeIndex == order).FirstOrDefault();
                Category_TreeItem category = allCategories.Where(x => x.category_id == item.CategoryID).FirstOrDefault();
                if (img != null) sPath = img.ImageUrl;
                int iOty = dictItemSellingQty[item.ID];
                OptionStoreItemCell optionitem = new OptionStoreItemCell()
                {
                    ItemID = item.ID,
                    LogoImage = GenerateLogoImageUrl(item.ManufactureID, size),
                    Title = item.Name,
                    UnitPrice = priceDictionary[item.ID].DisplayPrice,
                    Url = GenerateItemUrl(item.ID, category),
                    ItemImage = sPath,
                    SellingQty = iOty,
                    IsOutOfStock = iOty > 0 ? false : true,
                    FormatList = new List<FormatInfo>()
                };
                
                rtn.Add(optionitem);
            }
            
            return rtn;            
        }

        public List<StoreBanner> GetBanner(int categoryID, int bannerType)
        {
            return ConvertToStoreBannerList(this._iStoreRepoAdapter.Advertising_GetAll()
                .Where(x => x.CategoryID == categoryID && x.BannerType == bannerType && x.ShowAll == ConstShowAll.Show)
                .OrderBy(x => x.Showorder).ToList());
        }
        
        public Dictionary<int, List<StoreBanner>> GetAllBanner(int categoryID)
        {
            int[] iBannerTypes = new int[] { ConstBannerType.Rotate, ConstBannerType.PullDown, ConstBannerType.Single };
            Dictionary<int, List<StoreBanner>> rtn = new Dictionary<int, List<StoreBanner>>();
            foreach (int iBanner in iBannerTypes)
                rtn.Add(iBanner, GetBanner(categoryID, iBanner));
            
            return rtn;
        }

        public List<StoreBanner> GetLogoList(int iSubCategoryID)
        {
            int logoSize = 100;  // Logo圖檔的Size
            // Logo
            List<Subcategorylogo> SubcategoryLogoList = this._iStoreRepoAdapter.Subcategorylogo_GetAll()
                .Where(t => t.ShowAll == ConstShowAll.Show && t.SubCategoryID == iSubCategoryID).OrderBy(t => t.Showorder).ToList();
            List<StoreBanner> StoreLogoList = new List<StoreBanner>();
            foreach (Subcategorylogo logo in SubcategoryLogoList)
            {
                StoreLogoList.Add(new StoreBanner()
                {              
                    ID = logo.SubCategoryID+"_"+logo.ManufactureID,
                    Title = logo.ManufactureID.ToString(),
                    Image = logo.ImageUrl ?? GenerateLogoImageUrl(logo.ManufactureID, logoSize),
                    Url = logo.Clickpath
                });
            }

            return StoreLogoList;
        }

        public List<GroupLink> GetGroupLinkList(int iSubCategoryID)
        {
            // 群組
            List<Subcategorygroup> SubcategoryGroupLinkList = this._iStoreRepoAdapter.Subcategorygroup_GetAll()
                .Where(t => t.ShowAll == ConstShowAll.Show && t.SubCategoryID == iSubCategoryID).OrderBy(t => t.Showorder).ToList();
            List<GroupLink> StoreGroupLinkList = new List<GroupLink>();
            foreach (Subcategorygroup link in SubcategoryGroupLinkList)
            {
                StoreGroupLinkList.Add(new GroupLink()
                {
                    ID = link.ID.ToString(),
                    Title = link.Description,
                    Url = link.Clickpath,
                });
            }

            return StoreGroupLinkList;
        }
        
        #region Private functions

        private string GenerateItemUrl(int itemID, Category_TreeItem category)
        {
            string categoryID = FindCategoryForURL(category, 2);
            string storeID = FindCategoryForURL(category, 0);

            return string.Format("/item?itemid={0}&categoryid={1}&StoreID={2}", itemID, categoryID, storeID);
        }

        /// <summary>
        /// Find current category ID by layer
        /// </summary>
        /// <param name="category"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        private string FindCategoryForURL(Category_TreeItem category, int layer)
        {
            if (category == null)
            {
                return "0";
            }
            if (category.Parents == null)
            {
                return category.category_id.ToString();
            }
            if (category.category_layer == layer)
            {
                return category.category_id.ToString();
            }
            return FindCategoryForURL(category.Parents, layer++);
        }

        private string GenerateLogoImageUrl(int manufactureID, int imageSize)
        {
            return string.Format("/pic/manufacture/{0}/{1}_1_{2}.jpg",
                (manufactureID / 10000).ToString("0000"),
                (manufactureID % 10000).ToString("0000"),
                imageSize);
        }

        private List<PropertyGroup> GetPropertyGroup(int categoryID)
        {
            // 抓任選館的左方的過濾清單
            //IPropertyService _propertyService = AutofacConfig.Container.Resolve<IPropertyService>();
            return _propertyService.GetPropertyGroups(categoryID);
        }

        private OptionStoreListZone GetOptionStoreListZoneEx(int categoryID, int pageIndex, int pageItemCount, string filterIDs, string sortValue, bool? isFormat)
        {
            // 取得屬於這個類別的Items
            //ICategoryItemService _iCategoryItemService = AutofacConfig.Container.Resolve<ICategoryItemService>();
            
            // 傳入搜尋條件(排序也在這邊做).
            CategoryItemConditions conditions = new CategoryItemConditions()
            {
                CategoryID = categoryID,
                FilterID = filterIDs,
                orderBy = ConvertOrderByValue(sortValue)
            };
            
            List<ItemInfo> items = _iCategoryItemService.GetCategoryItems(conditions);

            //必須要過濾剩下開賣期間的商品.
            DateTime nowTime = DateTime.Now;
            items = items.Where(t => t.ItemBase.DateStart < nowTime && t.ItemBase.DateEnd > nowTime).ToList();

            //List<int> itemIDs = items.Select(x => x.ItemBase.ID).ToList();
            //List<ItemDetail> itemDetails = this.GetItemDetailList(itemIDs);

            // 把相同規格的從Detail中移除,另外存放.
            
            // 剩下的進行排序.
            //items = this.SortItemInfoList(items, sortValue);

            // 分頁.
            int pageCount;
            List<ItemInfo> pageItems = this.PagingItemInfoList(items, ref pageIndex, ref pageItemCount, out pageCount);

            // 最後再做轉換.
            OptionStoreListZone retZone = new OptionStoreListZone()
            {
                PageIndex = pageIndex,
                PageItemCount = pageItemCount,
                TotalPageCount = pageCount,
                ItemList = ConvertToOptionStoreItems(pageItems)
            };

            return retZone;
        }

        /// <summary>
        /// 轉換排序值到對應的排序編號
        /// </summary>
        /// <param name="sortValue"></param>
        /// <returns></returns>
        private int ConvertOrderByValue(string sortValue)
        {
            int orderValue = 0;
            switch (sortValue)
            {
                case ConstSortValue.Latest: // 最新上架.
                    orderValue = (int)CategoryItemConditions.OrderByCondition.CreatDate;
                    break;
                case ConstSortValue.Recommended: // 推薦排行.
                    orderValue = (int)CategoryItemConditions.OrderByCondition.Recommended;
                    break;
                case ConstSortValue.PriceDesc: // 金額高.
                    orderValue = (int)CategoryItemConditions.OrderByCondition.HighPrice;
                    break;
                case ConstSortValue.PriceAsc: // 金額低.
                    orderValue = (int)CategoryItemConditions.OrderByCondition.LowPrice;
                    break;
                default: // 人氣排行(預設排序)
                    orderValue = (int)CategoryItemConditions.OrderByCondition.PopularityIndex;
                    break;
            }

            return orderValue;
        }

        private List<ItemInfo> PagingItemInfoList(List<ItemInfo> itemInfoList, ref int pageIndex, ref int pageItemCount, out int pageCount)
        {
            // 錯誤的輸入參數修正.
            if (pageItemCount < 1)
                pageItemCount = DEFAULT_PAGE_ITEMCOUNT;

            pageCount = (int)Math.Ceiling((double)itemInfoList.Count / pageItemCount);

            // 下邊界修正.
            if (pageIndex < 1)
                pageIndex = 1;

            // 上邊界修正.
            if (pageIndex > pageCount)
                pageIndex = pageCount;

            return itemInfoList.Skip((pageIndex - 1) * pageItemCount).Take(pageItemCount).ToList();
        }

        ////private List<ItemInfo> SortItemInfoList(List<ItemInfo> itemInfoList, string sortValue)
        ////{
        ////    List<ItemInfo> retList = null;
        ////    switch (sortValue)
        ////    {
        ////        case ConstSortValue.Latest: // 最新上架.
        ////            retList = itemInfoList.OrderByDescending(x => x.ItemBase.CreateDate).ToList();
        ////            break;
        ////        case ConstSortValue.PriceDesc: // 金額高.
        ////            retList = itemInfoList.OrderByDescending(x => x.ItemBase.PriceCash).ToList();
        ////            break;
        ////        case ConstSortValue.PriceAsc: // 金額低.
        ////            retList = itemInfoList.OrderBy(x => x.ItemBase.PriceCash).ToList();
        ////            break;
        ////        default: // 人氣排行(預設排序).
        ////            retList = itemInfoList.OrderByDescending(x => x.ItemBase.QtyReg).ToList();
        ////            break;
        ////    }

        ////    return retList;
        ////}

        //private List<ItemDetail> GetItemDetailList(List<int> itemIDs)
        //{
        //    IItemDetailService _itemDetailService = AutofacConfig.Container.Resolve<IItemDetailService>();
        //    List<ItemDetail> retDetails = new List<ItemDetail>();

        //    foreach (int itemID in itemIDs)
        //    {
        //        ItemDetail item = _itemDetailService.GetItemDetail(itemID);
        //        if (item != null)
        //            retDetails.Add(item);
        //    }

        //    return retDetails;
        //}

        /// <summary>
        /// 獲取同一層級，加上父節點的麵包屑選單.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        private Breadcrumbs GetBreadCrumbs(int categoryID)
        {
            // 先找出目前Store的父節點是誰.
            Category currentStore = this._iStoreRepoAdapter.Category_GetAll().Where(x =>
                x.ID == categoryID && x.ShowAll == ConstShowAll.Show).OrderBy(x => x.Showorder).FirstOrDefault();

            // 如果節點不存在.
            if (currentStore == null)
                return null;

            // 把父節點和同層的節點撈出來.
            List<Category> caterogyList = this._iStoreRepoAdapter.Category_GetAll().Where(x =>
                (x.ParentID == currentStore.ParentID || x.ID == currentStore.ParentID) && x.ShowAll == ConstShowAll.Show).ToList();

            Category father = caterogyList.Where(x => x.ID == currentStore.ParentID).FirstOrDefault();
            List<Category> brothers = caterogyList.Where(x => x.ParentID == currentStore.ParentID).ToList();

            // 建立父節點.
            Breadcrumbs retBreadCrumbs = new Breadcrumbs();
            if (father != null)
            {
                retBreadCrumbs.Title = father.Description;
                retBreadCrumbs.CategoryID = father.ID;
            }

            // 建立兄弟姊妹的結點.
            retBreadCrumbs.DropDownItems = new List<BreadcrumbItem>();
            if (brothers.Any())
            {
                // 找出兄弟姊妹是否在任選館Table當中.
                List<int> brotherCategoryIDs = brothers.Select(x => x.ID).ToList();
                List<SubCategory_OptionStore> storeList = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x =>
                    x.CategoryID != null && brotherCategoryIDs.Contains(x.CategoryID.Value) &&
                    x.ShowAll == ConstShowAll.Show).ToList();

                foreach (var x in brothers)
                {
                    SubCategory_OptionStore optionStore = storeList.Where(y => y.CategoryID == x.ID).FirstOrDefault();
                    BreadcrumbItem item = new BreadcrumbItem()
                    {
                        Title = x.Description,
                        CategoryID = x.ID,
                        IsOptionStore = optionStore != null, // 如果是任選館, 就會在清單內可找到.
                    };

                    retBreadCrumbs.DropDownItems.Add(item);
                }
            }

            return retBreadCrumbs;
        }

        private Dictionary<int, List<ImageUrlReferenceDM>> GetItemImageUrlDict(List<int> itemIDs)
        {
            //IItemImageUrlService _itemImageUrlService = AutofacConfig.Container.Resolve<IItemImageUrlService>();
            Dictionary<int, List<ImageUrlReferenceDM>> images = _itemImageUrlService.GetItemImagePath(itemIDs);

            return images;
        }

        /// <summary>
        /// 目前為寫死的固定值, 之後再改為讀取配置檔.
        /// </summary>
        /// <returns></returns>
        private List<SortOption> GetSortOptionList()
        {
            string[] SortTitleArray = { "最新上架", "人氣排行榜", "推薦排行", "金額高", "金額低" };
            string[] SortValueArray = { ConstSortValue.Latest, ConstSortValue.MostBuy, ConstSortValue.Recommended, ConstSortValue.PriceDesc, ConstSortValue.PriceAsc };
            List<SortOption> retList = new List<SortOption>();

            for (int i = 0; i < SortTitleArray.Count(); i++)
            {
                retList.Add(new SortOption()
                {
                    Title = SortTitleArray[i],
                    SortValue = SortValueArray[i]
                });
            }

            return retList;
        }

        private List<OptionStoreItemCell> ConvertToOptionStoreItems(List<ItemInfo> items)
        {
            int imageSize = 300;
            int order = 1;
            List<int> itemIDs = items.Select(x => x.ItemBase.ID).ToList();
            Dictionary<int, List<ImageUrlReferenceDM>> images = this.GetItemImageUrlDict(itemIDs);
            List<Category_TreeItem> allCategories = _iCategoryServices.GetAllParentCategoriesByItemIDs(itemIDs);
            //IItemStockRepoAdapter _itemStockRepoAdapter = AutofacConfig.Container.Resolve<IItemStockRepoAdapter>();
            TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty objItemSellingQty = null;
            List<OptionStoreItemCell> retList = new List<OptionStoreItemCell>();
            foreach (ItemInfo x in items)
            {
                int itemID = x.ItemBase.ID;
                decimal displayPrice = x.ItemBase.PriceCash;
                //取得SellingQty
                objItemSellingQty = this._ItemStockService.GetItemSellingQtyByItemId(itemID);
                Category_TreeItem category = allCategories.Where(y => y.category_id == x.ItemBase.CategoryID).FirstOrDefault();
                int sellingQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                OptionStoreItemCell opItem = new OptionStoreItemCell()
                {
                    ItemID = itemID,
                    LogoImage = GenerateLogoImageUrl(x.ItemBase.ManufactureID, imageSize),
                    Title = x.ItemBase.Name,
                    Url = GenerateItemUrl(itemID, category),
                    UnitPrice = displayPrice,
                    SellingQty = sellingQty,
                    IsOutOfStock = sellingQty > 0 ? false : true,
                    OriginalPrice = x.ItemBase.MarketPrice == null ? displayPrice : x.ItemBase.MarketPrice.Value,
                    //FormatList // 功能尚未完成.
                };
                
                ImageUrlReferenceDM img = images[itemID].Where(y => y.Size == imageSize && y.SizeIndex == order).FirstOrDefault();
                if (img != null)
                {
                    opItem.ItemImage = img.ImageUrl;
                }

                retList.Add(opItem);
            }

            return retList;
        }

        private OptionStoreInfo ConvertToOptionStoreInfo(SubCategory_OptionStore dbOptionStore)
        {
            OptionStoreInfo retStore = new OptionStoreInfo()
            {
                LimitAmount = dbOptionStore.FreeCost == null ? 0 : dbOptionStore.FreeCost.Value,
                Title = dbOptionStore.Title
            };

            return retStore;
        }

        private StoreBanner ConvertToStoreBanner(Advertising adv)
        {
            if (adv == null)
                return null;

            StoreBanner retBanner = new StoreBanner()
            {
                ID = adv.ID.ToString(),
                Title = adv.Description,
                Image = adv.Imagepath,
                Url = adv.Clickpath
            };

            return retBanner;
        }
        
        private List<StoreBanner> ConvertToStoreBannerList(List<Advertising> advs)
        {
            List<StoreBanner> rtn = new List<StoreBanner>();
            foreach (Advertising adv in advs)
                rtn.Add(ConvertToStoreBanner(adv));
            
            return rtn;
        }

        private List<StoreItemCell> GetZoneItemList(int iSubCategoryID, int iZone)
        {
            // Item
            List<ItemAndSubCategoryMapping_NormalStore> SubcategoryItemList =
                this._iStoreRepoAdapter.NormalStoreItem_GetAll().Where(t => t.SubCategoryID == iSubCategoryID && t.Zone == iZone)
                .OrderBy(t => t.Showorder).ToList();
            List<StoreItemCell> ItemInfolist = GetStoreItems(SubcategoryItemList.Where(t => t.ItemID > 0).Select(t => t.ItemID)
                .Distinct().ToList());

            List<StoreItemCell> ShopWindowsitem = new List<StoreItemCell>();
            foreach (ItemAndSubCategoryMapping_NormalStore item in SubcategoryItemList)
            {
                if (item.ItemID > 0)
                {
                    // 商品
                    StoreItemCell ItemInfo = ItemInfolist.Where(t => t.ItemID == item.ItemID).FirstOrDefault();                    
                    if (ItemInfo != null)
                    {
                        ItemInfo.ID = item.ID.ToString();
                        if (!String.IsNullOrEmpty(item.ImageURL))
                            ItemInfo.ItemImage = item.ImageURL;
                        ShopWindowsitem.Add(ItemInfo);
                    }
                    else if (!String.IsNullOrEmpty(item.ImageURL))
                        ShopWindowsitem.Add(ConvertToActivityItem(item));
                }
                else
                {
                    // 活動
                    ShopWindowsitem.Add(ConvertToActivityItem(item));
                }
            }

            return ShopWindowsitem;
        }

        private StoreItemCell ConvertToActivityItem(ItemAndSubCategoryMapping_NormalStore item)
        {
            StoreItemCell ActivityItem = new StoreItemCell()
            {
                ID = item.ID.ToString(),
                ItemID = 0,
                ItemImage = item.ImageURL,
                Url = item.LinkURL,
            };
            return ActivityItem;
        }

        #endregion Private function

        
        //任選館 新增
        /// <summary>
        /// create SubCategory_OptionStore
        /// </summary>
        /// <param name="argObjSubCategory_OptionStore"></param>
        /// <returns>create success return id, else return 0</returns>
        public int Create(SubCategory_OptionStore_DM argObjSubCategory_OptionStore)
        {
            Models.DBModels.TWSQLDB.SubCategory_OptionStore objDbOptionStore = null;
            Models.DomainModels.Store.SubCategory_OptionStore_DM objResult = argObjSubCategory_OptionStore;
            int numOptionStoreId = 0;

            try
            {
                if (objResult != null)
                {
                    //translater
                    objDbOptionStore = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.SubCategory_OptionStore>(objResult);

                    this._iStoreRepoAdapter.Create(objDbOptionStore);
                    numOptionStoreId = objDbOptionStore.ID;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return numOptionStoreId;
            
        }
        //任選館 更新
        public bool Update(SubCategory_OptionStore_DM argObjSubCategory_OptionStore)
        {
            Models.DBModels.TWSQLDB.SubCategory_OptionStore objDbOptionStore = null;
            Models.DomainModels.Store.SubCategory_OptionStore_DM objResult = argObjSubCategory_OptionStore;
            bool boolExec = false;

            try
            {
                if (objResult != null)
                {
                    //translater
                    objDbOptionStore = ModelConverter.ConvertTo<Models.DBModels.TWSQLDB.SubCategory_OptionStore>(objResult);

                    this._iStoreRepoAdapter.Update(objDbOptionStore);
                    boolExec = true;
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }

            return boolExec;
        }
        //任選館取得CategoryId
        public SubCategory_OptionStore_DM GetById(int CategoryId)
        {
            IQueryable<Models.DBModels.TWSQLDB.SubCategory_OptionStore> querySearch = null;
            Models.DBModels.TWSQLDB.SubCategory_OptionStore objDbSearch = null;
            Models.DomainModels.Store.SubCategory_OptionStore_DM objResult = null;

            try
            {
                querySearch = this._iStoreRepoAdapter.GetById(CategoryId);    
                objDbSearch = querySearch.FirstOrDefault();
                if (objDbSearch != null)
                {
                    objResult = ModelConverter.ConvertTo<Models.DomainModels.Store.SubCategory_OptionStore_DM>(objDbSearch);
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message, ex);
            }
            return objResult;    
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool dispose)
        {
            if (dispose && scope != null)
            {
                scope.Dispose();
            }
        }
    }
}
