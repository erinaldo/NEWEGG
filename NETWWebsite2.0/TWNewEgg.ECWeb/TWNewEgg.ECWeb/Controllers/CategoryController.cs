using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TWNewEgg.Framework.BaseController;
using TWNewEgg.Framework.AOP;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.ViewModels;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.Models.ViewModels.Category;
using TWNewEgg.Models.ViewModels.Track;
using TWNewEgg.ECWeb.Auth;
using System.Configuration;
using TWNewEgg.ECWeb.Utility;
using System.Diagnostics;
using TWNewEgg.Models.DomainModels.Category;
using log4net;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class CategoryController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(CategoryController));
        private const int CHOOSEANY_PAGE_ITEM_COUNT = 36;

        private string imageDomain = System.Configuration.ConfigurationManager.AppSettings["ECWebHttpImgDomain"];
        private string shoppingCartCookieName = "sc";
        private string chooseListCookieName = "twcl";

        public ActionResult Index(int? CategoryID)
        {
            //檢查輸入
            if (CategoryID == null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }

            bool isChooseAny = false;

            var isChooseAnyResult = Processor.Request<bool, bool>("StoreService", "IsChooseAny", CategoryID);

            if (string.IsNullOrEmpty(isChooseAnyResult.error))
            {
                isChooseAny = isChooseAnyResult.results;
            }

            if (isChooseAny)
            {
                return RedirectToAction("ChooseAny", new { CategoryID = CategoryID });
            }

            //Breadcrumbs result = Processor.Request<Breadcrumbs, Breadcrumbs>("CategoryItemService", "GetBreadCrumbs", CategoryID.Value).results;

            Api.ItemController itemApiController = new Api.ItemController();
            TWNewEgg.Models.ViewModels.Store.Breadcrumbs result = itemApiController.GetItemParentCategories(CategoryID.Value);
            
            //if (result == null)
            //{
            //    return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            //}
            //if (result.DropDownItems.Count == 0) {
            //    return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            //}
            ViewBag.CategoryID = CategoryID;
            return View(result);
        }

        //呼叫CategoryItemService
        public TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View GetCategoryItemInfoMain_ViewList(string OrderBy, CategoryItemForm CategoryItemForm, int ItemsQty = 36, int Page = 1)
        {
            CategoryItemConditions conditions = ModelConverter.ConvertTo<CategoryItemConditions>(CategoryItemForm);
            var CategoryAreaInfotemp = Processor.Request<TWNewEgg.Models.DomainModels.Category.CategoryAreaInfo, TWNewEgg.Models.DomainModels.Category.CategoryAreaInfo>("CategoryItemService", "GetCategoryAreaInfo", conditions);
            TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View CategoryItemInfoMain_ViewList = new TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View();
            AutoMapper.Mapper.Map(CategoryAreaInfotemp.results.ItemBaseList, CategoryItemInfoMain_ViewList.CategoryItemInfo_ViewList);
            AutoMapper.Mapper.Map(CategoryAreaInfotemp.results.PriceWithQtyList, CategoryItemInfoMain_ViewList.PriceWithQty_ViewList);

            switch (CategoryItemForm.orderBy)
            {
                case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.CreatDate:
                    CategoryItemInfoMain_ViewList.OrderBy = "CreatDate";
                    break;
                case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.PopularityIndex:
                    CategoryItemInfoMain_ViewList.OrderBy = "PopularityIndex";
                    break;
                case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.Recommended:
                    CategoryItemInfoMain_ViewList.OrderBy = "Recommended";
                    break;
                case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.HighPrice:
                    CategoryItemInfoMain_ViewList.OrderBy = "HighPrice";
                    break;
                case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.LowPrice:
                    CategoryItemInfoMain_ViewList.OrderBy = "LowPrice";
                    break;
                default:
                    CategoryItemInfoMain_ViewList.OrderBy = "CreatDate";
                    break;
            }

            if (CategoryItemInfoMain_ViewList.CategoryItemInfo_ViewList == null)
            {
                CategoryItemInfoMain_ViewList.CategoryItemInfo_ViewList = new List<CategoryItemInfo_View>();
            }
            if (CategoryItemInfoMain_ViewList.PriceWithQty_ViewList == null)
            {
                CategoryItemInfoMain_ViewList.PriceWithQty_ViewList = new List<TWNewEgg.Models.ViewModels.Property.PriceWithQty_View>();
            }

            //int itemNumber = 1;
            //foreach (var item in CategoryItemInfoMain_ViewList.CategoryItemInfo_View)
            //{
            //    item.Page = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(itemNumber) / ItemsQty));
            //    itemNumber++;
            //}

            int TotalPage = 0;
            List<int> itemIDsForCategory = new List<int>();
            if (CategoryAreaInfotemp.results != null && CategoryAreaInfotemp.results.ItemBaseList.Count > 0)
            {
                TotalPage = CategoryAreaInfotemp.results.TotalPage;
                itemIDsForCategory = CategoryAreaInfotemp.results.ItemBaseList.Select(x => x.ID).ToList();
            }


            //Call Cateogry Service for SEO URL
            Api.CategoryParentController categoryService = new Api.CategoryParentController();
            Services.Category.FindCategoryForSEO categoryFinder = new Services.Category.FindCategoryForSEO();
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> allCategories = categoryService.Post(itemIDsForCategory);

            for (int i = 0; i < itemIDsForCategory.Count; i++)
            {
                var currentItem = CategoryAreaInfotemp.results.ItemBaseList.Where(x => x.ID == itemIDsForCategory[i]).FirstOrDefault();

                TWNewEgg.Models.ViewModels.Category.Category_TreeItem category = allCategories.Where(x => x.category_id == currentItem.CategoryID).FirstOrDefault();
                int categoryID = categoryFinder.FindCategoryForURL(category, 2);
                int storeID = categoryFinder.FindCategoryForURL(category, 0);

                var item = CategoryItemInfoMain_ViewList.CategoryItemInfo_ViewList.Where(x => x.ID == CategoryAreaInfotemp.results.ItemBaseList[i].ID).FirstOrDefault();
                if (item != null)
                {
                    item.CategoryID = categoryID;
                    item.StoreID = storeID;
                }
            }

            CategoryItemInfoMain_ViewList.Page = Page;
            CategoryItemInfoMain_ViewList.TotalPage = TotalPage;

            TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
            CategoryItemInfoMain_ViewList.ShowPageList = CalculationsPage.getShowPages(CategoryItemInfoMain_ViewList.TotalPage ?? 0, CategoryItemInfoMain_ViewList.Page ?? 1, 3);
            return CategoryItemInfoMain_ViewList;
        }

        public List<CategoryTopBanner_View> CategoryNewBanner(int categoryID)
        {
            List<CategoryTopBanner_View> topBanners = new List<CategoryTopBanner_View>();
      
            AdLayer3DM searchCondition = new AdLayer3DM();
            CategoryDM categoryData = new CategoryDM();
            
            Dictionary<int, List<ItemUrl>> itemUrlDictionary = new Dictionary<int, List<ItemUrl>>();
            Dictionary<int, ItemPrice> itemDisplayPriceresult = new Dictionary<int, ItemPrice>();
            Dictionary<int, ItemInfo> itemData = new Dictionary<int, ItemInfo>();
            //Call Cateogry Service for SEO URL
            Api.CategoryParentController categoryService = new Api.CategoryParentController();
            Services.Category.FindCategoryForSEO categoryFinder = new Services.Category.FindCategoryForSEO();
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> allCategories = new List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>();

            if (categoryID == 0)
            {
                topBanners = null;
            }
            else
            {
                searchCondition.CategoryID = categoryID;
                searchCondition.ShowAll = (int)AdLayer3DM.showAll.顯示;
            }

            //類別基本資料-副標題
            categoryData = Processor.Request<CategoryDM, CategoryDM>("CategoryNewServices", "GetCategoryByCategoryID", searchCondition.CategoryID).results;
            ViewBag.SubTitle = !string.IsNullOrEmpty(categoryData.SubTitle) ? categoryData.SubTitle.ToString() : "";

            //Ad layer3 data：banner and items
            var Result = Processor.Request<List<CategoryTopBanner_View>, List<AdLayer3DM>>("AdLayer3Services", "GetAdLayer3List", searchCondition);

            if (string.IsNullOrEmpty(Result.error))
            {
                topBanners = Result.results;

                foreach (CategoryTopBanner_View Ad in topBanners)
                {   
                        //判斷廣告是否為item，才開始填寫item的圖片及價格
                    if (Ad.AdType == (int)AdLayer3DM.adType.item)
                    {
                        if (Ad.ItemList != null)
                        {  
                           
                            List<int> itemIDs = new List<int>();
                            List<int> exitItemList = new List<int>();
                            exitItemList.AddRange(Ad.ItemList.Select(x => x.ItemID).ToList());
                            //在load一次有效ItemID
                            
                                                        
                            
                            itemIDs = Processor.Request<List<int>, List<int>>("AdLayer3Services", "GetAvailableAndVisibleItemID", exitItemList).results;
                            itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", itemIDs).results;
                            itemDisplayPriceresult = Processor.Request<Dictionary<int, ItemPrice>, Dictionary<int, ItemPrice>>("ItemDisplayPriceService", "GetItemDisplayPrice", itemIDs).results;
                            itemData = Processor.Request<Dictionary<int, ItemInfo>, Dictionary<int, ItemInfo>>("ItemInfoService", "GetItemInfoList", itemIDs).results;
                            allCategories = categoryService.Post(itemIDs);
                            //foreach (CategoryTopBanner_ItemView items in Ad.ItemList)
                            //{
                            //    items.ItemImage = (itemUrlDictionary[items.ItemID].Where(x => x.Size == 300).FirstOrDefault() != null) ? itemUrlDictionary[items.ItemID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl : "";
                            //    items.UnitPrice = itemDisplayPriceresult[items.ItemID].DisplayPrice;
                            //    items.Title = itemData[items.ItemID].ItemBase.Name;
                            //}

                            List<CategoryTopBanner_ItemView> showerOederList = new List<CategoryTopBanner_ItemView>();
                            foreach(var item in Ad.ItemList)
                            {
                                showerOederList.Add(new CategoryTopBanner_ItemView()
                                { 
                                    ItemID = item.ItemID,
                                    Showorder = item.Showorder
                                });
                            }
                            
                            Ad.ItemList = new List<CategoryTopBanner_ItemView>();
                            //itemlist放有效的item data
                            foreach (int itemId in itemIDs)
                            {
                                TWNewEgg.Models.ViewModels.Category.Category_TreeItem category = allCategories.Where(x => x.category_id == itemData[itemId].ItemBase.CategoryID).FirstOrDefault();
                                CategoryTopBanner_ItemView itemView = new CategoryTopBanner_ItemView();
                                itemView.AdLayer3ID = Ad.ID;
                                itemView.ItemID = itemId;
                                itemView.Showorder = showerOederList.First(x=>x.ItemID == itemId).Showorder;
                                itemView.ItemImage = (itemUrlDictionary[itemId].Where(x => x.Size == 300).FirstOrDefault() != null) ? itemUrlDictionary[itemId].Where(x => x.Size == 300).FirstOrDefault().ImageUrl : "";
                                itemView.UnitPrice = itemDisplayPriceresult[itemId].DisplayPrice;
                                itemView.Title = itemData[itemId].ItemBase.Name;
                                itemView.MarketPrice = itemData[itemId].ItemBase.MarketPrice.GetValueOrDefault();
                                itemView.CategoryID = categoryFinder.FindCategoryForURL(category, 2);
                                itemView.StoreID = categoryFinder.FindCategoryForURL(category, 0);
                                Ad.ItemList.Add(itemView);
                            }

                            Ad.ItemList = Ad.ItemList.OrderBy(x => x.Showorder).ToList();
                        }
                    }
                }
            }
            else
            {
                log.Error(Result.error);
            }

            return topBanners;
        }

        public List<MainZone> CategoryOldBanner(int categoryID)
        {
            List<MainZone> topBanners = new List<MainZone>();
            Dictionary<int, List<ItemUrl>> itemUrlDictionary = new Dictionary<int, List<ItemUrl>>();
            Dictionary<int, ItemPrice> itemDisplayPriceresult = new Dictionary<int, ItemPrice>();

            if (categoryID == 0)
            {
                topBanners = null;
            }
            else
            {
                topBanners = Processor.Request<List<MainZone>, List<MainZone>>("CategoryItemService", "GetCategoryBanner", categoryID).results;

                List<int> itemIDs = new List<int>();
                foreach (var singleMainZone in topBanners)
                {
                    if (singleMainZone.ItemList != null && singleMainZone.ItemList.Count > 0)
                    {
                        itemIDs.AddRange(singleMainZone.ItemList.Select(x => x.ItemID).ToList());
                    }
                }

                if (itemIDs.Count > 0)
                {
                    itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", itemIDs).results;
                    itemDisplayPriceresult = Processor.Request<Dictionary<int, ItemPrice>, Dictionary<int, ItemPrice>>("ItemDisplayPriceService", "GetItemDisplayPrice", itemIDs).results;
                }

                foreach (var singleMainZone in topBanners)
                {
                    if (singleMainZone.ItemList != null)
                    {
                        foreach (var singleItem in singleMainZone.ItemList)
                        {
                            try
                            {
                                if (itemUrlDictionary.ContainsKey(singleItem.ItemID))
                                {
                                    string imageUrl = (itemUrlDictionary[singleItem.ItemID].Where(x => x.Size == 300).FirstOrDefault() != null) ? itemUrlDictionary[singleItem.ItemID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl : "";
                                    if (imageUrl.IndexOf("newegg.com/") >= 0)
                                    {
                                        singleItem.ItemImage = imageUrl;
                                    }
                                    else
                                    {
                                        singleItem.ItemImage = ImageUtility.GetImagePath(imageUrl);
                                    }
                                }
                                if (itemDisplayPriceresult.ContainsKey(singleItem.ItemID))
                                {
                                    singleItem.UnitPrice = itemDisplayPriceresult[singleItem.ItemID].DisplayPrice;
                                }
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                    else
                    {
                        foreach (var singleItem in singleMainZone.LogoList)
                        {
                            if (!string.IsNullOrEmpty(singleItem.Image))
                            {
                                if (singleItem.Image.ToLower().IndexOf("http://") < 0 || singleItem.Image.ToLower().IndexOf("https://") < 0)
                                {
                                    singleItem.Image = string.Format("{0}{1}", "https://ssl-images.newegg.com.tw", singleItem.Image);
                                }
                            }
                        }
                    }
                }
            }
            return topBanners;
        }

        public ActionResult CategoryBanner(int categoryID)
        {
            CategoryTopBanner_MasterView topBanners = new CategoryTopBanner_MasterView();
            List<CategoryTopBanner_View> NewBanner = new List<CategoryTopBanner_View>();
            List<MainZone> OldBanner = new List<MainZone>();
            
            //讀取PM設定的第三層分類banner
            NewBanner = CategoryNewBanner(categoryID);
            //OldBanner = CategoryOldBanner(categoryID);
            OldBanner = CategoryTopItems(categoryID);
            topBanners.NewCategoryTopBanner = NewBanner;
            topBanners.OldCategoryTopBanner = OldBanner;

            //PM無設定，保持原來的
            //if ((NewBanner.Count == 0) || (NewBanner == null))
            //{
            //    OldBanner = CategoryTopItems(categoryID);
            //    topBanners.NewCategoryTopBanner = null;
            //    topBanners.OldCategoryTopBanner = OldBanner;
            //}
            //else
            //{
            //    topBanners.NewCategoryTopBanner = NewBanner;
            //    topBanners.OldCategoryTopBanner = null;
            //}
            
            return PartialView("Partial_CategoryTopBanner", topBanners);
        }

        //獲取CategoryBanner的最新4筆Items
        public List<MainZone> CategoryTopItems(int categoryID)
        {
            List<MainZone> topBanners = new List<MainZone>();
            List<StoreItemCell> bannerItem = new List<StoreItemCell>();
            List<ItemInfo> itemInfo = new List<ItemInfo>();
            Dictionary<int, List<ItemUrl>> itemUrlDictionary = new Dictionary<int, List<ItemUrl>>();
            Dictionary<int, ItemPrice> itemDisplayPriceresult = new Dictionary<int, ItemPrice>();
            //Call Cateogry Service for SEO URL
            Api.CategoryParentController categoryService = new Api.CategoryParentController();
            Services.Category.FindCategoryForSEO categoryFinder = new Services.Category.FindCategoryForSEO();
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> allCategories = new List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>();
            if (categoryID == 0)
            {
                itemInfo = null;
            }
            else
            {
                CategoryItemConditions condition = new CategoryItemConditions() { CategoryID = categoryID };
                itemInfo = Processor.Request<List<ItemInfo>, List<ItemInfo>>("CategoryItemService", "GetCategoryItemsTopRows", condition, 4).results;
                List<int> itemIDs = itemInfo.Select(x=>x.ItemBase.ID).ToList();
                if (itemInfo.Count > 0)
                {
                    itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", itemIDs).results;
                    itemDisplayPriceresult = Processor.Request<Dictionary<int, ItemPrice>, Dictionary<int, ItemPrice>>("ItemDisplayPriceService", "GetItemDisplayPrice", itemIDs).results;
                    allCategories = categoryService.Post(itemIDs);
                }
            }

            foreach (var Item in itemInfo)
            {
                TWNewEgg.Models.ViewModels.Category.Category_TreeItem category = allCategories.Where(x => x.category_id == Item.ItemBase.CategoryID).FirstOrDefault();
                string imageUrl = (itemUrlDictionary[Item.ItemBase.ID].Where(x => x.Size == 300).FirstOrDefault() != null) ? itemUrlDictionary[Item.ItemBase.ID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl : "";
                if (itemDisplayPriceresult.ContainsKey(Item.ItemBase.ID) == false)
                {
                }
                else
                {
                    bannerItem.Add(new StoreItemCell()
                    {
                        ItemID = Item.ItemBase.ID,
                        Title = Item.ItemBase.Name,
                        ItemImage = imageUrl,
                        UnitPrice = itemDisplayPriceresult[Item.ItemBase.ID].DisplayPrice,
                        MarketPrice = Item.ItemBase.MarketPrice.GetValueOrDefault(),
                        CategoryID = categoryFinder.FindCategoryForURL(category, 2),
                        StoreID = categoryFinder.FindCategoryForURL(category, 0)
                    });
                }
            }

            topBanners.Add(new MainZone()
            {
                ItemList = bannerItem
            });

            return topBanners;
        }

        public ActionResult Top10ItemsBanner(int CategoryID) 
        {
            List<CategoryTopItemVM> topItems = new List<CategoryTopItemVM>();
            List<CategoryTopItemVM> searchResult = new List<CategoryTopItemVM>();
            CategoryTopItemDM searchCondition =new CategoryTopItemDM();

            Dictionary<int, List<ItemUrl>> itemUrlDictionary = new Dictionary<int, List<ItemUrl>>();
            Dictionary<int, ItemPrice> itemDisplayPriceresult = new Dictionary<int, ItemPrice>();
            Dictionary<int, ItemInfo> itemData = new Dictionary<int, ItemInfo>();
            Dictionary<int, int> itemStock = new Dictionary<int, int>();
            if (CategoryID == 0)
            {
                topItems = null;
            }
            else 
            {
                searchCondition.CategoryID = CategoryID;
                searchCondition.ShowAll = (int)CategoryTopItemDM.showAll.顯示;
            }
            //load top10 Items
            var Result = Processor.Request<List<CategoryTopItemVM>, List<CategoryTopItemDM>>("CategoryTopItemService", "GetCategoryTopItem", searchCondition);

            if (string.IsNullOrEmpty(Result.error))
            {
                searchResult = Result.results;

                if (searchResult != null)
                    foreach (CategoryTopItemVM items in searchResult)
                {
                    if (items.ItemType == (int)CategoryTopItemDM.itemType.銷售TOP )
                    {
                        List<int> itemIDs = new List<int>();
                        List<int> existItemList = new List<int>();
                        //Call Cateogry Service for SEO URL
                        List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> allCategories = new List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>();
                        existItemList.Add(items.ItemID);
                        //load 有效的items
                        itemIDs = Processor.Request<List<int>, List<int>>("AdLayer3Services", "GetAvailableAndVisibleItemID", existItemList).results;
                        //Call Cateogry Service for SEO URL
                        Api.CategoryParentController categoryService = new Api.CategoryParentController();
                        Services.Category.FindCategoryForSEO categoryFinder = new Services.Category.FindCategoryForSEO();
                        if (itemIDs.Count > 0)
                        {
                            itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", itemIDs).results;
                            itemDisplayPriceresult = Processor.Request<Dictionary<int, ItemPrice>, Dictionary<int, ItemPrice>>("ItemDisplayPriceService", "GetItemDisplayPrice", itemIDs).results;
                            itemData = Processor.Request<Dictionary<int, ItemInfo>, Dictionary<int, ItemInfo>>("ItemInfoService", "GetItemInfoList", itemIDs).results;
                            itemStock = Processor.Request<Dictionary<int, int>, Dictionary<int, int>>("ItemStockService", "GetSellingQtyByItemList", itemIDs).results;
                            //Call Cateogry Service for SEO URL
                            allCategories = categoryService.Post(itemIDs);
                        }
                        
                        foreach (int itemId in itemIDs)
                        {
                            string imageUrl = (itemUrlDictionary[itemId].Where(x => x.Size == 300).FirstOrDefault() != null) ? itemUrlDictionary[itemId].Where(x => x.Size == 300).FirstOrDefault().ImageUrl : "";
                            TWNewEgg.Models.ViewModels.Category.Category_TreeItem category = allCategories.Where(x => x.category_id == items.CategoryID).FirstOrDefault();
                            CategoryTopItemVM itemView = new CategoryTopItemVM();
                            itemView.CategoryID = items.CategoryID;
                            itemView.StoreID = categoryFinder.FindCategoryForURL(category, 0);
                            itemView.ItemID = itemId;
                            itemView.Title = itemData[itemId].ItemBase.Name;
                            itemView.ItemImage = imageUrl;
                            itemView.UnitPrice = itemDisplayPriceresult[itemId].DisplayPrice;
                            itemView.SellingQty = itemStock[itemId];
                            itemView.MarketPrice = itemData[itemId].ItemBase.MarketPrice.GetValueOrDefault();
                            topItems.Add(itemView);                         
                        }
                    }     
                }
            }
            else
            {
                log.Error(Result.error);
            }
            return PartialView("Partial_CategoeryTopItem", topItems);
        }
       
        /// <summary>
        /// 初始分類內容
        /// </summary>
        /// <param name="PVID"></param>
        /// <param name="ItemsQty"></param>
        /// <param name="Page"></param>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public ActionResult InitialItemListMenu(string PVID, int ItemsQty = 36, int Page = 1, int CategoryID = 286)
        {
            CategoryItemForm CategoryItemForm = new CategoryItemForm();
            CategoryItemForm.CategoryID = CategoryID;
            CategoryItemForm.FilterID = PVID;
            CategoryItemForm.orderBy = 0;
            CategoryItemForm.OnePageItemsQty = ItemsQty;
            CategoryItemForm.NowPage = Page;

            TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View CategoryItemInfoMain_ViewList = new TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View();
            CategoryItemInfoMain_ViewList = GetCategoryItemInfoMain_ViewList(null, CategoryItemForm, ItemsQty, Page);

            return PartialView("SubItemListMenu", CategoryItemInfoMain_ViewList);
        }

        /// <summary>
        /// 分類內容換頁
        /// </summary>
        /// <param name="PVID"></param>
        /// <param name="OrderBy"></param>
        /// <param name="ItemsQty"></param>
        /// <param name="Page"></param>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public ActionResult NextPageItemListMenu(int? maxPrice, int? minPrice, string PVID, string OrderBy, int ItemsQty = 36, int Page = 1, int CategoryID = 286)
        {
            CategoryItemForm CategoryItemForm = new CategoryItemForm();
            CategoryItemForm.CategoryID = CategoryID;
            CategoryItemForm.FilterID = PVID;
            CategoryItemForm.orderBy = 0;
            CategoryItemForm.maxPrice = maxPrice;
            CategoryItemForm.minPrice = minPrice;
            CategoryItemForm.OnePageItemsQty = ItemsQty;
            CategoryItemForm.NowPage = Page;
            CategoryItemForm.IsFirstTime = 1;

            switch (OrderBy)
            {
                case "CreatDate":
                    CategoryItemForm.orderBy = (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.CreatDate;
                    break;
                case "PopularityIndex":
                    CategoryItemForm.orderBy = (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.PopularityIndex;
                    break;
                case "Recommended":
                    CategoryItemForm.orderBy = (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.Recommended;
                    break;
                case "HighPrice":
                    CategoryItemForm.orderBy = (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.HighPrice;
                    break;
                case "LowPrice":
                    CategoryItemForm.orderBy = (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.LowPrice;
                    break;
                default:
                    CategoryItemForm.orderBy = (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.CreatDate;
                    break;
            }

            TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View CategoryItemInfoMain_ViewList = new TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View();
            CategoryItemInfoMain_ViewList = GetCategoryItemInfoMain_ViewList(OrderBy, CategoryItemForm, ItemsQty, Page);

            string resultString = "";
            using (StringWriter sw = new StringWriter())
            {
                ViewData.Model = CategoryItemInfoMain_ViewList;
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, "SubItemListMenu");
                ViewContext viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                resultString = sw.GetStringBuilder().ToString();
            }
            if (Request.IsAjaxRequest())
            {
                return Json(Regex.Replace(resultString, @"[\r\n]+\s{0,}[\r\n]+", " "), JsonRequestBehavior.AllowGet);
            }
            return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
        }

        /// <summary>
        /// CategoryArea
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public ActionResult GetCategoryArea(int? CategoryID, string breadString, int ItemsQty = 36, int Page = 1)
        {
            if (CategoryID == null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }

            TWNewEgg.Models.ViewModels.Category.CategoryArea_View CategoryArea_View = new TWNewEgg.Models.ViewModels.Category.CategoryArea_View();
            CategoryArea_View.BreadString = breadString;

            var Leftbannerresult = Processor.Request<List<StoreBanner>, List<StoreBanner>>("StoreService", "GetBanner", CategoryID, 5);  // 5 為左下方廣告
            List<StoreBanner> leftBanner = new List<StoreBanner>();
            AutoMapper.Mapper.Map(Leftbannerresult.results, leftBanner);
            CategoryArea_View.PullDownAdvList = leftBanner;
            var PropertyServiceresult = Processor.Request<List<PropertyGroup>, List<PropertyGroup>>("PropertyService", "GetPropertyGroups", CategoryID);
            List<TWNewEgg.Models.ViewModels.Property.PropertyGroup_View> PropertyGroupList = new List<TWNewEgg.Models.ViewModels.Property.PropertyGroup_View>();
            AutoMapper.Mapper.Map(PropertyServiceresult.results, PropertyGroupList);
            CategoryArea_View.PropertyGroup_ViewList = PropertyGroupList;

            CategoryItemForm CategoryItemForm = new CategoryItemForm();
            CategoryItemForm.CategoryID = CategoryID ?? 286;
            CategoryItemForm.FilterID = "";
            CategoryItemForm.orderBy = 0;
            CategoryItemForm.OnePageItemsQty = ItemsQty;
            CategoryItemForm.NowPage = Page;
            CategoryItemForm.IsFirstTime = 0;

            TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View CategoryItemInfoMain_ViewList = new TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View();
            CategoryArea_View.CategoryItemInfoMain_View = GetCategoryItemInfoMain_ViewList(null, CategoryItemForm, ItemsQty, Page);

            return PartialView("Partial_CategoryArea", CategoryArea_View);
        }

        /// <summary>
        /// 屬性清單
        /// </summary>
        /// <param name="CategoryID"></param>
        /// <returns></returns>
        public ActionResult GetPropertyMenu(Nullable<int> CategoryID)
        {
            if (CategoryID == null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }
            var result = Processor.Request<List<PropertyGroup>, List<PropertyGroup>>("PropertyService", "GetPropertyGroups", CategoryID);
            List<TWNewEgg.Models.ViewModels.Property.PropertyGroup_View> PropertyGroupList = new List<TWNewEgg.Models.ViewModels.Property.PropertyGroup_View>();
            AutoMapper.Mapper.Map(result.results, PropertyGroupList);
            return PartialView("PropertyListMenu", PropertyGroupList);
        }

        /// <summary>
        /// 任選館功能.
        /// </summary>
        /// <param name="CategoryID">任選館序號.</param>
        /// <returns>任選館頁面</returns>
        [NoCache]
        public ActionResult ChooseAny(Nullable<int> CategoryID)
        {
            if (CategoryID == null)
            {
                return RedirectToAction("Index", "Home", new { returnUrl = "/Home" });
            }

            bool isChooseAny = false;

            var isChooseAnyResult = Processor.Request<bool, bool>("StoreService", "IsChooseAny", CategoryID);

            if (string.IsNullOrEmpty(isChooseAnyResult.error))
            {
                isChooseAny = isChooseAnyResult.results;
            }

            if (!isChooseAny)
            {
                return RedirectToAction("Index", "Category", new { CategoryID = CategoryID });
            }

            ViewBag.CategoryID = CategoryID;

            OptionStoreInfo viewModel = Processor.Request<OptionStoreInfo, OptionStoreInfo>("StoreService", "GetOptionStoreInfo",
                CategoryID, 1, CHOOSEANY_PAGE_ITEM_COUNT, "", "").results;

            if (viewModel == null)
            {
                return RedirectToAction("Index", "Category", new { CategoryID = CategoryID });
            }

            Api.ItemController itemApiController = new Api.ItemController();
            viewModel.Breadcurmbs = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Store.Breadcrumbs>(itemApiController.GetItemParentCategories(CategoryID.Value));
            
            ViewData["ListPageIndex"] = 1;
            return View(viewModel);
        }

        /// <summary>
        /// 任選館已選取的商品統計.
        /// </summary>
        /// <param name="CategoryID">任選館序號.</param>
        /// <param name="LimitAmount">任選館金額下線.</param>
        /// <param name="SubPageIndex">用戶顯示的當前子分頁.</param>
        /// <param name="CategoryTitle">任選館活動主題.</param>
        /// <returns></returns>
        public ActionResult GetChooseArea(int CategoryID, decimal LimitAmount, int SubPageIndex, string CategoryTitle = "")
        {
            ViewData["Choose-Any-Limit-Amount"] = LimitAmount;
            ViewData["Choose-Any-Sub-Index"] = SubPageIndex;
            ViewData["Choose-Any-Category-Title"] = CategoryTitle;
            List<OptionStoreItemCell_View> viewData = NEUser.IsAuthticated ? this.GetChoosedItemsInfoFromDataBase(CategoryID) : this.GetChoosedItemsInfoFromCookie(CategoryID);

            return PartialView("Partial_ChooseArea", viewData);
        }

        /// <summary>
        /// 載入任選館產品清單.
        /// </summary>
        /// <param name="CategoryID">任選館序號.</param>
        /// <param name="pageIndex">指定的頁面.</param>
        /// <param name="pageItemCount">每頁顯示筆數.</param>
        /// <param name="filters">屬性規格過濾器.</param>
        /// <param name="sortValue">排序方式.</param>
        /// <returns></returns>
        public ActionResult GetChooseListArea(int CategoryID = -1, int pageIndex = 1, int pageItemCount = CHOOSEANY_PAGE_ITEM_COUNT, string filters = "", string sortValue = "")
        {
            OptionStoreListZone viewModel = new OptionStoreListZone();
            viewModel = Processor.Request<OptionStoreListZone, OptionStoreListZone>("StoreService", "GetOptionStoreListZone", CategoryID, pageIndex, pageItemCount, filters, sortValue).results;
            ViewData["ListPageIndex"] = pageIndex;

            string chCookieValue = TWNewEgg.CookiesUtilities.CookiesUtility.ReadCookies(this.chooseListCookieName);
            List<CartTrack> chooseItems = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<List<CartTrack>>(chCookieValue).Where(t => t.cid == CategoryID).ToList();
            if (chooseItems.Count > 0)
            {
                foreach (CartTrack item in chooseItems)
                {
                    viewModel.ItemList.Where(t => t.ItemID == item.iid).ToList().ForEach(i => { i.IsChoose = true; i.PickQty = item.qty; });
                }
            }

            return PartialView("Partial_ChooseListArea", viewModel);
        }

        /// <summary>
        /// 讀取Cookie當中任選館商品資料.
        /// </summary>
        /// <param name="CategoryID">任選館的序號</param>
        /// <returns>產品清單, 包含每個商品已選的數量.</returns>
        private List<OptionStoreItemCell_View> GetChoosedItemsInfoFromCookie(int CategoryID)
        {
            /*將購物車清單序列化*/
            string scCookieValue = TWNewEgg.CookiesUtilities.CookiesUtility.ReadCookies(this.shoppingCartCookieName);
            List<CartTrack> cartItems = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<List<CartTrack>>(scCookieValue);
            if (cartItems == null)
            {
                cartItems = new List<CartTrack>();
            }

            /*將任選館清單序列化*/
            string chCookieValue = TWNewEgg.CookiesUtilities.CookiesUtility.ReadCookies(this.chooseListCookieName);
            List<CartTrack> chooseItems = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<List<CartTrack>>(chCookieValue);
            if (chooseItems == null)
            {
                chooseItems = new List<CartTrack>();
            }
            
            foreach (var cartItem in cartItems)
            {
                if (!chooseItems.Any(m => m.cid.Equals(CategoryID) && m.iid.Equals(cartItem.iid)) && cartItem.stu.Equals(0))
                {
                    CartTrack updateItem = chooseItems.FirstOrDefault(m => m.iid.Equals(cartItem.iid));
                    //updateItem.qty = cartItem.qty;
                }
                else if (chooseItems.Any(m => m.cid.Equals(CategoryID) && m.iid.Equals(cartItem.iid)) && cartItem.stu.Equals(0))
                {
                    chooseItems.Add(cartItem);
                }
            }
            
            TWNewEgg.CookiesUtilities.CookiesUtility.RemoveMainCookie(this.chooseListCookieName, ConfigurationManager.AppSettings["ECWebDomain"]);
            TWNewEgg.CookiesUtilities.CookiesUtility.CreateCookie(this.chooseListCookieName, TWNewEgg.Framework.Common.JSONSerialization.Serializer(chooseItems), "/", ConfigurationManager.AppSettings["ECWebDomain"], null);

            List<OptionStoreItemCell_View> itemList = new List<OptionStoreItemCell_View>();
            List<int> cartIDs = new List<int>();
            List<OptionStoreItemCell> modelList = new List<OptionStoreItemCell>();
            foreach (CartTrack cartItem in chooseItems.Where(m => m.cid.Equals(CategoryID)))
            {
                cartIDs.Add(cartItem.iid);
            }

            modelList = Processor.Request<List<OptionStoreItemCell>, List<OptionStoreItemCell>>("StoreService", "GetOptionStoreItems", cartIDs.Distinct().ToList()).results;

            if (modelList != null && modelList.Any())
            {
                foreach (var modelItem in modelList)
                {
                    itemList.Add(new OptionStoreItemCell_View(modelItem)
                    {
                        PickQty = chooseItems.Where(m => m.iid.Equals(modelItem.ItemID)).Max(n =>
                        {
                            return n.qty > 10 ? 10 : n.qty;
                        })
                    });
                }
            }

            return itemList;
        }

        /// <summary>
        /// 從資料庫讀取任選館商品清單.
        /// </summary>
        /// <param name="CategoryID">任選館的序號</param>
        /// <returns>產品清單, 包含每個商品已選的數量.</returns>
        private List<OptionStoreItemCell_View> GetChoosedItemsInfoFromDataBase(int CategoryID)
        {
            List<OptionStoreItemCell_View> itemList = new List<OptionStoreItemCell_View>();
            List<CartTrack> cartTrackItems = new List<CartTrack>();
            /*購物車所有資訊*/
            var GetCartAllList = Processor.Request<List<TWNewEgg.Models.ViewModels.Cart.ShoppingCart_View>, List<TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM>>("ShoppingCartService", "GetCartAllList", NEUser.ID);
            if (GetCartAllList != null && GetCartAllList.results != null && GetCartAllList.results.Any(m => m.ID.Equals(3)))
            {
                /*任選館購物車*/
                var chooseCartItems = GetCartAllList.results.FirstOrDefault(m => m.ID.Equals(3));
                if (chooseCartItems != null && chooseCartItems.CartItemClassList != null && chooseCartItems.CartItemClassList.Any() && chooseCartItems.CartItemClassList.Any(m => m.CategoryID.Equals(CategoryID)))
                {
                    List<int> ids = new List<int>();
                    /*DB當中前任選館的商品清單*/
                    var chooseItemsFromDb = chooseCartItems.CartItemClassList.FirstOrDefault(m => m.CategoryID != null && m.CategoryID.Equals(CategoryID));
                    foreach (var item in chooseItemsFromDb.CartItemList)
                    {
                        /*當前任選館的商品ID清單*/
                        ids.Add(item.ItemID);
                        cartTrackItems.Add(new CartTrack() { iid = item.ItemID, qty = item.Qty, stu = 0, cid = CategoryID, });
                    }

                    /*Cookie當中全部商品清單*/
                    string chCookieValue = TWNewEgg.CookiesUtilities.CookiesUtility.ReadCookies(this.chooseListCookieName);
                    if (!string.IsNullOrEmpty(chCookieValue))
                    {
                        List<CartTrack> chooseItemsFromCookie = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<List<CartTrack>>(chCookieValue);
                        foreach (CartTrack chooseItem in chooseItemsFromCookie)
                        {
                            /*當前任選館, 還沒有加入的商品*/
                            if (chooseItem.cid.Equals(CategoryID))
                            {
                                if (!ids.Any(n => n.Equals(chooseItem.iid)))
                                {
                                    ids.Add(chooseItem.iid);
                                    cartTrackItems.Add(new CartTrack()
                                    {
                                        iid = chooseItem.iid,
                                        qty = chooseItem.qty,
                                        stu = 0,
                                        cid = chooseItem.cid,
                                    });
                                }
                                else
                                {
                                    var cartItem = cartTrackItems.Where(t => t.iid == chooseItem.iid).FirstOrDefault();
                                    if (cartItem != null)
                                    {
                                        cartItem.qty = chooseItem.qty;
                                    }
                                }
                            }
                            if (!ids.Any(n => n.Equals(chooseItem.iid)))
                            {
                                cartTrackItems.Add(new CartTrack()
                                {
                                    iid = chooseItem.iid,
                                    qty = chooseItem.qty,
                                    stu = 0,
                                    cid = chooseItem.cid,
                                });
                            }
                        }
                    }

                    var items = Processor.Request<List<OptionStoreItemCell>, List<OptionStoreItemCell>>("StoreService", "GetOptionStoreItems", ids.Distinct().ToList()).results;
                    if (items != null && items.Any())
                    {
                        foreach (var modelItem in items)
                        {
                            var pickQty = cartTrackItems.FirstOrDefault(m => m.iid.Equals(modelItem.ItemID)).qty;
                            /*DB當中的商品, 加入清單給ViewMode 使用*/
                            itemList.Add(new OptionStoreItemCell_View(modelItem) { PickQty = pickQty });
                        }
                    }

                    TWNewEgg.CookiesUtilities.CookiesUtility.RemoveMainCookie(this.chooseListCookieName, ConfigurationManager.AppSettings["ECWebDomain"]);
                    TWNewEgg.CookiesUtilities.CookiesUtility.CreateCookie(this.chooseListCookieName, TWNewEgg.Framework.Common.JSONSerialization.Serializer(cartTrackItems), "/", ConfigurationManager.AppSettings["ECWebDomain"], null);
                }
                else
                {
                    return this.GetChoosedItemsInfoFromCookie(CategoryID);
                }
            }

            return itemList;
        }
    }
}
