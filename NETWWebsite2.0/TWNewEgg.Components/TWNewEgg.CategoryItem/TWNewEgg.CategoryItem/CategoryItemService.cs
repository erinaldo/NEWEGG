using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TWNewEgg.CategoryItem.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.DAL;
using TWNewEgg.Framework.Common;
using TWNewEgg.PropertyRepoAdapters.Interface;
using TWNewEgg.GroupBuyServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using LinqKit;
using TWNewEgg.CategoryRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.StoreRepoAdapters.Interface;
using TWNewEgg.CategoryItem.Const;
using TWNewEgg.StarProductRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.PropertyServices.Interface;
using TWNewEgg.Models.DomainModels.Category;
//using TWNewEgg.CategoryServices.Interface;

namespace TWNewEgg.CategoryItem
{
    public class CategoryItemService : ICategoryItemService
    {
        private IItemRepoAdapter _itemRepoAdapter;
        private ICategoryRepoAdapter _categoryRepoAdapter;
        private IPropertyRepoAdapter _propertyRepoAdapter;
        private IStoreRepoAdapter _storeRepoAdapter;
        private IStarProductRepoAdapter _starRepoAdapter;
        private IItemDisplayPriceRepoAdapter _displayPriceRepoAdapter;
        private IItemImageUrlService _itemImageUrlService;
        private IPropertyService _propertyService;
        private IItemGroupRepoAdapter _ItemGroupRepo;
        private IItemGroupService _ItemGroupService;
        private IItemStockRepoAdapter _itemStockRepoAdapter;
        //private ICategoryServices _categoryService;

        public CategoryItemService(IItemRepoAdapter itemRepoAdapter,
            IPropertyRepoAdapter propertyRepoAdapter,
            ICategoryRepoAdapter categoryRepoAdapter,
            IStoreRepoAdapter storeRepoAdapter,
            IStarProductRepoAdapter starRepoAdapter,
            IItemDisplayPriceRepoAdapter displayPriceRepoAdapter,
            IItemImageUrlService itemImageUrlService,
            IPropertyService propertyService,
            IItemGroupRepoAdapter argItemGroupRepo,
            IItemGroupService argItemGroupService,
            IItemStockRepoAdapter itemStockRepoAdapter//,
            //ICategoryServices categoryService
            )
        {
            this._itemRepoAdapter = itemRepoAdapter;
            this._propertyRepoAdapter = propertyRepoAdapter;
            this._categoryRepoAdapter = categoryRepoAdapter;
            this._storeRepoAdapter = storeRepoAdapter;
            this._starRepoAdapter = starRepoAdapter;
            this._displayPriceRepoAdapter = displayPriceRepoAdapter;
            this._itemImageUrlService = itemImageUrlService;
            this._propertyService = propertyService;
            this._ItemGroupRepo = argItemGroupRepo;
            this._ItemGroupService = argItemGroupService;
            this._itemStockRepoAdapter = itemStockRepoAdapter;
            //this._categoryService = categoryService;
        }

        public CategoryAreaInfo GetCategoryAreaInfo(CategoryItemConditions conditions)
        {
            CategoryAreaInfo CategoryAreaInforesult = new CategoryAreaInfo();
            IEnumerable<ItemPriceInfoSimplify> totalCategoryItems = null;
            List<ItemPriceInfoSimplify> allItems = new List<ItemPriceInfoSimplify>();
            List<int> allPropertyIDs = new List<int>();
            if (conditions.FilterID != null)
            {
                allPropertyIDs = conditions.FilterID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f => int.Parse(f)).ToList();
            }
            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listTempSellingQty = new List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty>();
            Dictionary<int, int> dictTempSellingQty = null;
            Dictionary<int, List<ItemMarketGroup>> dictGroup = null;
            DateTime dateNow = DateTime.UtcNow.AddHours(8);
            int maxItemFromDB = 10800;
            //declare the transaction options
            var transactionOptions = new System.Transactions.TransactionOptions();
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.Item> allCategoryItems = this._itemRepoAdapter.GetAvailableAndVisible(conditions.CategoryID);
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.Item> crossCategoryItems = this._itemRepoAdapter.GetCrossCategoryAvailableAndVisible(conditions.CategoryID);
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> queryViewItemSellingQty = this._itemStockRepoAdapter.GetAllViewQty();
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> itemDisplayPriceList = this._displayPriceRepoAdapter.GetAll().Where(x => x.StartDate <= dateNow && x.EndDate > dateNow);
            //IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> AllGroupQuery = this._ItemGroupRepo.GetAllItemGroupDetailProperty().Where(x => x.ItemID != null);
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> AllGroupQuery = this._ItemGroupRepo.GetAllItemGroupDetailProperty().Where(x => x.ItemID > 0);
            //Basic Category Item
            var fullCategoryItems = allCategoryItems.Union(crossCategoryItems)
                .Select(x => new 
                { 
                    ID = x.ID,
                    Name = x.Name,
                    ManufactureID = x.ManufactureID,
                    ProductID = x.ProductID,
                    CreateDate = x.CreateDate,
                    QtyReg = x.QtyReg,
                    RedmfdbckRate = x.RedmfdbckRate,
                    PriceCash = x.PriceCash,
                    MarketPrice = x.MarketPrice,
                    CategoryID = x.CategoryID
                });

            
            //All Category Item's Price
            var itemPriceInfo = fullCategoryItems.Join(itemDisplayPriceList,
                ii => ii.ID,
                idp => idp.ItemID,
                (ii, idp) => new 
                {
                    ItemID = ii.ID,
                    ItemName = ii.Name,
                    ManufactureID = ii.ManufactureID,
                    ProductID = ii.ProductID,
                    CreateDate = ii.CreateDate,
                    QtyReg = ii.QtyReg,
                    RedmfdbckRate = ii.RedmfdbckRate,
                    MarketPrice = ii.MarketPrice,
                    PriceCash = idp.DisplayPrice,
                    PriceType = idp.PriceType,
                    CategoryID = ii.CategoryID
                 });
            itemPriceInfo = itemPriceInfo.GroupBy(x => x.ItemID,
                (key, xs) => xs.OrderByDescending(y => y.PriceType).FirstOrDefault());
            
            //All Category Item's selling qty
            var itemSellingQty = itemPriceInfo.Join(queryViewItemSellingQty,
                    ii => ii.ItemID,
                    isq => isq.ID,
                    (ii, isq) => new 
                    {
                        ItemID = ii.ItemID,
                        ItemName = ii.ItemName,
                        ManufactureID = ii.ManufactureID,
                        ProductID = ii.ProductID,
                        CreateDate = ii.CreateDate,
                        QtyReg = ii.QtyReg,
                        RedmfdbckRate = ii.RedmfdbckRate,
                        MarketPrice = ii.MarketPrice,
                        PriceCash = ii.PriceCash,
                        PriceType = ii.PriceType,
                        SellingQty = isq.SellingQty,
                        SortSellingQty = isq.SellingQty == 0 ? 0 : 1,
                        CategoryID = ii.CategoryID
                    });

            //All category Item's group info
            var itemGroupInfo = itemSellingQty.GroupJoin(AllGroupQuery,
                ii => ii.ItemID,
                igdp => igdp.ItemID,
                (ii, igdp) => new 
                { ii = ii, igdp = igdp })
                        .SelectMany(
                            x => x.igdp.DefaultIfEmpty(),
                            (x, y) => new 
                            {
                                ItemID = x.ii.ItemID,
                                ItemName = x.ii.ItemName,
                                ManufactureID = x.ii.ManufactureID,
                                ProductID = x.ii.ProductID,
                                CreateDate = x.ii.CreateDate,
                                QtyReg = x.ii.QtyReg,
                                RedmfdbckRate = x.ii.RedmfdbckRate,
                                GroupID = y == null ? -(x.ii.ItemID) : y.GroupID,
                                MasterPropertyID = y == null ? -(x.ii.ItemID) : y.MasterPropertyID,
                                GroupValueID = y == null ? -(x.ii.ItemID) : y.GroupValueID,
                                MarketPrice = x.ii.MarketPrice,
                                PriceCash = x.ii.PriceCash,
                                SellingQty = x.ii.SellingQty,
                                SortSellingQty = x.ii.SortSellingQty,
                                CategoryID = x.ii.CategoryID
                            });

            //var singleCategoryItem = itemGroupInfo.Where(x => x.GroupID == null);
            //var groupCategoryItem = itemGroupInfo.Where(x => x.GroupID != null).GroupBy(x => new { x.GroupID, x.MasterPropertyID, x.GroupValueID },
            //    (key, xs) => xs.OrderByDescending(y => y.SellingQty).FirstOrDefault());
            //set it to read uncommited
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
            //create the transaction scope, passing our options in
            using (var transactionScope = new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Required,
                transactionOptions))
            {
                var testItem = this._itemRepoAdapter.GetIfAvailable(1);
                transactionScope.Complete();
            }
            this._categoryRepoAdapter.SetContextTimeOut(75);
            //create the transaction scope, passing our options in
            using (var transactionScope = new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Required,
                transactionOptions))
            {
                var categoryItemDBData = itemGroupInfo.Take(maxItemFromDB).ToList();
                transactionScope.Complete();
                var groupCategoryItem = categoryItemDBData.Where(x => x.GroupID != null).GroupBy(x => new { x.GroupID, x.MasterPropertyID, x.GroupValueID },
                (key, xs) => xs.OrderByDescending(y => y.SellingQty).FirstOrDefault());
                totalCategoryItems = groupCategoryItem
                    .Select(x => new ItemPriceInfoSimplify
                    {
                        ID = x.ItemID,
                        ProductID = x.ProductID,
                        Name = x.ItemName,
                        QtyReg = x.QtyReg,
                        //可銷售數量
                        SellingQty = x.SellingQty ?? 0,
                        //可銷售數量排序,主要是確定有數量即可, 故有量就設為1, 無數量暫設為0 ,以免影響其他排序條件
                        SortSellingQty = x.SortSellingQty,
                        ManufactureID = x.ManufactureID,
                        CreateDate = x.CreateDate,
                        RedmfdbckRate = x.RedmfdbckRate,
                        PriceCash = x.PriceCash,
                        MarketPrice = x.MarketPrice,
                        DisplayPrice = x.PriceCash,
                        CategoryID = x.CategoryID
                    });
            }
            //set it to read uncommited
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            //create the transaction scope, passing our options in
            using (var transactionScope = new System.Transactions.TransactionScope(
                System.Transactions.TransactionScopeOption.Required,
                transactionOptions))
            {
                var testItem = this._itemRepoAdapter.GetIfAvailable(1);
                transactionScope.Complete();
            }
            //var totalCategoryItems = singleCategoryItem.Union(groupCategoryItem)
            //    .Select(x => new ItemPriceInfoSimplify
            //        {
            //            ID = x.ItemID,
            //            ProductID = x.ProductID,
            //            Name = x.ItemName,
            //            QtyReg = x.QtyReg,
            //            //可銷售數量
            //            SellingQty = x.SellingQty ?? 0,
            //            //可銷售數量排序,主要是確定有數量即可, 故有量就設為1, 無數量暫設為0 ,以免影響其他排序條件
            //            SortSellingQty = x.SortSellingQty,
            //            ManufactureID = x.ManufactureID,
            //            CreateDate = x.CreateDate,
            //            RedmfdbckRate = x.RedmfdbckRate,
            //            PriceCash = x.PriceCash,
            //            MarketPrice = x.MarketPrice,
            //            DisplayPrice = x.PriceCash
            //        });

            var predicate = PredicateBuilder.True<ItemPriceInfoSimplify>();
            bool ispredicate = false;
            if (conditions.BrandID.HasValue)
            {
                ispredicate = true;
                predicate = predicate.And(f => f.ManufactureID == conditions.BrandID);
            }

            if (conditions.minPrice != null && conditions.maxPrice != null)
            {
                ispredicate = true;
                predicate = predicate.And(f => f.DisplayPrice >= conditions.minPrice && f.DisplayPrice <= conditions.maxPrice);
            }

            if (allPropertyIDs.Count > 0)
            {
                ispredicate = true;
                List<int> productIds = totalCategoryItems.Select(x => x.ProductID).Distinct().ToList();
                productIds = this._propertyRepoAdapter.FilterProductIds(productIds, conditions.CategoryID, allPropertyIDs);
                predicate = predicate.And(f => productIds.Contains(f.ProductID));
            }

            if (conditions.orderBy != null)
            {
                switch (conditions.orderBy)
                {
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.CreatDate:
                        totalCategoryItems = totalCategoryItems.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.CreateDate);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.PopularityIndex:
                        totalCategoryItems = totalCategoryItems.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.QtyReg);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.Recommended:
                        totalCategoryItems = totalCategoryItems.OrderByDescending(x => x.SortSellingQty).ThenBy(x => x.RedmfdbckRate);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.HighPrice:
                        totalCategoryItems = totalCategoryItems.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.DisplayPrice);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.LowPrice:
                        totalCategoryItems = totalCategoryItems.OrderByDescending(x => x.SortSellingQty).ThenBy(x => x.DisplayPrice);
                        break;
                    default:
                        totalCategoryItems = totalCategoryItems.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.CreateDate);
                        break;
                }
            }
            
            
                
            var allItemstemp = totalCategoryItems.AsQueryable().AsExpandable();
            if (ispredicate == true)
            {
                allItemstemp = allItemstemp.Where(predicate);
            }

            /* 先撈出數量節省計算時間 */
            int takeQty = 100 * (conditions.OnePageItemsQty ?? 36);
            allItems = allItemstemp.Take(takeQty).ToList();
            if (conditions.IsFirstTime == 0)
            {
                List<decimal> PriceList = allItems.Select(x => x.DisplayPrice).ToList();
                if (PriceList.Count() != 0)
                {
                    List<TWNewEgg.Models.DomainModels.Property.PriceWithQty> PriceWithQtyList = this._propertyService.GetPropertyPriceWithQty(PriceList);
                    CategoryAreaInforesult.PriceWithQtyList = PriceWithQtyList;
                }
            }
           
            
            CategoryAreaInforesult.NowPage = conditions.NowPage ?? 1;
            CategoryAreaInforesult.Qty = allItems.Count();
            CategoryAreaInforesult.TotalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(CategoryAreaInforesult.Qty) / (conditions.OnePageItemsQty ?? 36)));

            int SkipQty = ((conditions.NowPage ?? 1) - 1) * (conditions.OnePageItemsQty ?? 36);
            int TakeQty = (conditions.OnePageItemsQty ?? 36);

            // 因可以先決定數量因此搬到算頁數之前
            allItems = allItems.Skip(SkipQty).Take(TakeQty).ToList();

            if (allItems == null || allItems.Count == 0)
            {
                allItems = new List<ItemPriceInfoSimplify>();
            }

            List<ItemBase> ItemBaseList = ModelConverter.ConvertTo<List<ItemBase>>(allItems);
            if (conditions.NowPage != null && conditions.NowPage != 0)
            {
                List<int> itemIds = ItemBaseList.Select(x => x.ID).ToList();
                Dictionary<int, List<ImageUrlReferenceDM>> ImageUrlReferenceDMList = this._itemImageUrlService.GetItemImagePath(itemIds);

                //取得內容物是否有規格品
                dictGroup = this._ItemGroupService.GetItemMarketGroupByItemId(itemIds, false);
                //取得規格品相關的庫存資訊
                dictTempSellingQty = ItemBaseList.Select(x => new { Key = x.ID, Value = 0 }).ToDictionary(x => x.Key, x => x.Value);
                if (ItemBaseList != null && ItemBaseList.Count > 0)
                {
                    listTempSellingQty = this._itemStockRepoAdapter.GetSellingQtyByItemList(ItemBaseList.Select(x => x.ID).ToList()).ToList();
                }
                if (listTempSellingQty != null && listTempSellingQty.Count > 0)
                {
                    foreach (TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objTempItemSellingQty in listTempSellingQty)
                    {
                        dictTempSellingQty[objTempItemSellingQty.ID] = objTempItemSellingQty.SellingQty ?? 0;
                    }
                }
                foreach (var temp in ItemBaseList)
                {
                    temp.Qty = dictTempSellingQty[temp.ID];
                    try
                    {
                        //修改規格品的名稱
                        if (dictGroup != null && dictGroup.ContainsKey(temp.ID) && dictGroup[temp.ID] != null)
                        {
                            temp.Name = temp.Name + "-" + dictGroup[temp.ID].First().MasterPropertyValueDisplay;
                        }

                        if (ImageUrlReferenceDMList.Count > 0 && ImageUrlReferenceDMList.Where(x => x.Key == temp.ID).FirstOrDefault().Value.Count != 0)
                        {
                            if (ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 300).FirstOrDefault() != null)
                            {
                                temp.imgPath = ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                            }
                            else
                            {
                                if (ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                {
                                    temp.imgPath = ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                }
                                else
                                {
                                    if (ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 60).FirstOrDefault() != null)
                                    {
                                        temp.imgPath = ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                                    }
                                }
                            }

                            if (temp.imgPath.IndexOf("newegg.com/") >= 0)
                            {
                            }
                            else
                            {
                                string hostString = ConfigurationManager.AppSettings.Get("ECWebHttpsImgDomain");
                                temp.imgPath = string.Format("{0}{1}", hostString, temp.imgPath);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            CategoryAreaInforesult.ItemBaseList = ItemBaseList;
            return CategoryAreaInforesult;
        }

        /* Old Version but new version not running yet...
        public CategoryAreaInfo GetCategoryAreaInfoV2(CategoryItemConditions conditions)
        {
            CategoryAreaInfo CategoryAreaInforesult = new CategoryAreaInfo();
            List<int> allPropertyIDs = new List<int>();
            if (conditions.FilterID != null)
            {
                allPropertyIDs = conditions.FilterID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f => int.Parse(f)).ToList();
            }

            IQueryable<Models.DBModels.TWSQLDB.Item> allCategoryItems = this._itemRepoAdapter.GetAvailableAndVisible(conditions.CategoryID);
            IQueryable<Models.DBModels.TWSQLDB.Item> crossCategoryItems = this._itemRepoAdapter.GetCrossCategoryAvailableAndVisible(conditions.CategoryID);
            allCategoryItems = allCategoryItems.Union(crossCategoryItems);
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> itemDisplayPriceList = this._displayPriceRepoAdapter.GetAll().Where(x => x.PriceType == 1 && x.MinNumber == 1 && x.MaxNumber == 1);

            //取得可銷售數量及存貨量
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> queryViewItemSellingQty = this._itemStockRepoAdapter.GetAllViewQty();
            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listTempSellingQty = new List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty>();
            Dictionary<int, int> dictTempSellingQty = null;


            #region 取得要排除的規格品Query
            //取得規格品的指令
            var AllGroupQuery = this._ItemGroupRepo.GetAllItemGroupDetailProperty().Where(x => x.ItemID != null);
            var ShowGroupQuery = (from x in AllGroupQuery
                                  join y in allCategoryItems on x.ItemID equals y.ID
                                  group x by new { x.GroupID, x.MasterPropertyID, x.GroupValueID, y.Status } into g
                                  select new
                                  {
                                      GroupID = g.Key.GroupID,
                                      MasterPropertyID = g.Key.MasterPropertyID,
                                      GroupValueID = g.Key.GroupValueID,
                                      ItemID = (from res in g where res.GroupID == g.Key.GroupID && res.MasterPropertyID == g.Key.MasterPropertyID && res.GroupValueID == g.Key.GroupValueID select res.ItemID).Max()
                                  }

                );
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> queryExceptGroupDetail = (from x in AllGroupQuery
                                                                                                           join p in ShowGroupQuery on new { x.GroupID, x.MasterPropertyID, x.ItemID } equals new { p.GroupID, p.MasterPropertyID, p.ItemID } into temp
                                                                                                           from y in temp.DefaultIfEmpty()
                                                                                                           where y.ItemID == null
                                                                                                           select x
                                );

            Dictionary<int, List<ItemMarketGroup>> dictGroup = null;
            #endregion

            var ItemPriceInfo = (from itemQueryable in allCategoryItems
                                 join itemDisplayPriceQueryable in itemDisplayPriceList on itemQueryable.ID equals itemDisplayPriceQueryable.ItemID into Queryabletemp
                                 //加入可銷售數量
                                 join viewItemSellingQty in queryViewItemSellingQty on itemQueryable.ID equals viewItemSellingQty.ID into ViewQty
                                 //加入排除名單的選項
                                 join itemExcept in queryExceptGroupDetail on itemQueryable.ID equals itemExcept.ItemID into groupResult
                                 from itemDisplayPriceQueryable in Queryabletemp.DefaultIfEmpty()
                                 from viewItemSellingQty in ViewQty.DefaultIfEmpty()
                                 from dataGroup in groupResult.DefaultIfEmpty()
                                 where dataGroup.ItemID == null
                                 select new ItemPriceInfoSimplify
                                 {
                                     ID = itemQueryable.ID,
                                     ProductID = itemQueryable.ProductID,
                                     Name = itemQueryable.Name,
                                     QtyReg = itemQueryable.QtyReg,
                                     //可銷售數量
                                     SellingQty = viewItemSellingQty.SellingQty ?? 0,
                                     //可銷售數量排序,主要是確定有數量即可, 故有量就設為1, 無數量暫設為0 ,以免影響其他排序條件
                                     SortSellingQty = viewItemSellingQty.SellingQty > 0 ? 1 : 0,
                                     ManufactureID = itemQueryable.ManufactureID,
                                     CreateDate = itemQueryable.CreateDate,
                                     RedmfdbckRate = itemQueryable.RedmfdbckRate,
                                     PriceCash = (itemDisplayPriceQueryable == null ? itemQueryable.PriceCash : itemDisplayPriceQueryable.DisplayPrice),
                                     MarketPrice = itemQueryable.MarketPrice,
                                     DisplayPrice = (itemDisplayPriceQueryable == null ? itemQueryable.PriceCash : itemDisplayPriceQueryable.DisplayPrice)
                                 });

            var predicate = PredicateBuilder.True<ItemPriceInfoSimplify>();
            bool ispredicate = false;
            if (conditions.BrandID.HasValue)
            {
                ispredicate = true;
                predicate = predicate.And(f => f.ManufactureID == conditions.BrandID);
            }

            if (conditions.minPrice != null && conditions.maxPrice != null)
            {
                ispredicate = true;
                predicate = predicate.And(f => f.DisplayPrice >= conditions.minPrice && f.DisplayPrice <= conditions.maxPrice);
            }

            if (allPropertyIDs.Count > 0)
            {
                ispredicate = true;
                List<int> productIds = ItemPriceInfo.Select(x => x.ProductID).Distinct().ToList();
                productIds = this._propertyRepoAdapter.FilterProductIds(productIds, conditions.CategoryID, allPropertyIDs);
                predicate = predicate.And(f => productIds.Contains(f.ProductID));
            }

            if (conditions.orderBy != null)
            {
                switch (conditions.orderBy)
                {
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.CreatDate:
                        ItemPriceInfo = ItemPriceInfo.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.CreateDate);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.PopularityIndex:
                        ItemPriceInfo = ItemPriceInfo.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.QtyReg);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.Recommended:
                        ItemPriceInfo = ItemPriceInfo.OrderByDescending(x => x.SortSellingQty).ThenBy(x => x.RedmfdbckRate);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.HighPrice:
                        ItemPriceInfo = ItemPriceInfo.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.DisplayPrice);
                        break;
                    case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.LowPrice:
                        ItemPriceInfo = ItemPriceInfo.OrderByDescending(x => x.SortSellingQty).ThenBy(x => x.DisplayPrice);
                        break;
                    default:
                        ItemPriceInfo = ItemPriceInfo.OrderByDescending(x => x.SortSellingQty).ThenByDescending(x => x.CreateDate);
                        break;
                }
            }

            if (conditions.IsFirstTime == 0)
            {
                List<decimal> PriceList = ItemPriceInfo.Where(x => x.DisplayPrice != null && x.DisplayPrice != 0).Select(x => x.DisplayPrice).ToList();
                if (PriceList.Count() != 0)
                {
                    List<TWNewEgg.Models.DomainModels.Property.PriceWithQty> PriceWithQtyList = this._propertyService.GetPropertyPriceWithQty(PriceList);
                    CategoryAreaInforesult.PriceWithQtyList = PriceWithQtyList;
                }
            }

            var allItemstemp = ItemPriceInfo.AsExpandable();
            if (ispredicate == true)
            {
                allItemstemp = allItemstemp.Where(predicate);
            }

            // 先撈出數量節省計算時間
            int takeQty = 100 * (conditions.OnePageItemsQty ?? 36);
            List<ItemPriceInfoSimplify> allItems = allItemstemp.Take(takeQty).ToList();

            CategoryAreaInforesult.NowPage = conditions.NowPage ?? 1;
            CategoryAreaInforesult.Qty = allItems.Count();
            CategoryAreaInforesult.TotalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(CategoryAreaInforesult.Qty) / (conditions.OnePageItemsQty ?? 36)));

            int SkipQty = ((conditions.NowPage ?? 1) - 1) * (conditions.OnePageItemsQty ?? 36);
            int TakeQty = (conditions.OnePageItemsQty ?? 36);

            // 因可以先決定數量因此搬到算頁數之前
            allItems = allItems.Skip(SkipQty).Take(TakeQty).ToList();

            if (allItems == null || allItems.Count == 0)
            {
                allItems = new List<ItemPriceInfoSimplify>();
            }

            List<ItemBase> ItemBaseList = ModelConverter.ConvertTo<List<ItemBase>>(allItems);

            if (conditions.NowPage != null && conditions.NowPage != 0)
            {
                List<int> itemIds = ItemBaseList.Select(x => x.ID).ToList();
                Dictionary<int, List<ImageUrlReferenceDM>> ImageUrlReferenceDMList = this._itemImageUrlService.GetItemImagePath(itemIds);

                //取得內容物是否有規格品
                dictGroup = this._ItemGroupService.GetItemMarketGroupByItemId(itemIds, false);
                //取得規格品相關的庫存資訊
                dictTempSellingQty = ItemBaseList.Select(x => new { Key = x.ID, Value = 0 }).ToDictionary(x => x.Key, x => x.Value);
                if (ItemBaseList != null && ItemBaseList.Count > 0)
                {
                    listTempSellingQty = this._itemStockRepoAdapter.GetSellingQtyByItemList(ItemBaseList.Select(x => x.ID).ToList()).ToList();
                }
                if (listTempSellingQty != null && listTempSellingQty.Count > 0)
                {
                    foreach (TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objTempItemSellingQty in listTempSellingQty)
                    {
                        dictTempSellingQty[objTempItemSellingQty.ID] = objTempItemSellingQty.SellingQty ?? 0;
                    }
                }
                foreach (var temp in ItemBaseList)
                {
                    temp.Qty = dictTempSellingQty[temp.ID];
                    try
                    {
                        //修改規格品的名稱
                        if (dictGroup != null && dictGroup.ContainsKey(temp.ID) && dictGroup[temp.ID] != null)
                        {
                            temp.Name = temp.Name + "-" + dictGroup[temp.ID].First().MasterPropertyValueDisplay;
                        }

                        if (ImageUrlReferenceDMList.Count > 0 && ImageUrlReferenceDMList.Where(x => x.Key == temp.ID).FirstOrDefault().Value.Count != 0)
                        {
                            if (ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 300).FirstOrDefault() != null)
                            {
                                temp.imgPath = ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                            }
                            else
                            {
                                if (ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                {
                                    temp.imgPath = ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                }
                                else
                                {
                                    if (ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 60).FirstOrDefault() != null)
                                    {
                                        temp.imgPath = ImageUrlReferenceDMList[temp.ID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                                    }
                                }
                            }

                            if (temp.imgPath.IndexOf("newegg.com/") >= 0)
                            {
                            }
                            else
                            {
                                string hostString = ConfigurationManager.AppSettings.Get("ECWebHttpsImgDomain");
                                temp.imgPath = string.Format("{0}{1}", hostString, temp.imgPath);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            CategoryAreaInforesult.ItemBaseList = ItemBaseList;
            return CategoryAreaInforesult;
        }
        */

        public List<ItemInfo> GetCategoryItems(CategoryItemConditions conditions)
        {
            List<int> allPropertyIDs = new List<int>();
            if (conditions.FilterID != null)
            {
                allPropertyIDs = conditions.FilterID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f => int.Parse(f)).ToList();
            }

            IQueryable<Models.DBModels.TWSQLDB.Item> allCategoryItems = this._itemRepoAdapter.GetAvailableAndVisible(conditions.CategoryID);
            IQueryable<Models.DBModels.TWSQLDB.Item> crossCategoryItems = this._itemRepoAdapter.GetCrossCategoryAvailableAndVisible(conditions.CategoryID);
            allCategoryItems = allCategoryItems.Union(crossCategoryItems);

            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> itemDisplayPriceList = this._displayPriceRepoAdapter.GetAll();

            var ItemPriceInfo = (from itemQueryable in allCategoryItems
                                 join itemDisplayPriceQueryable in itemDisplayPriceList on itemQueryable.ID equals itemDisplayPriceQueryable.ItemID into Queryabletemp
                                 from itemDisplayPriceQueryable in Queryabletemp.DefaultIfEmpty()
                                 select new ItemPriceInfo
                                 {
                                     item = itemQueryable,
                                     itemDisplayPrice = itemDisplayPriceQueryable
                                 });

            var predicate = PredicateBuilder.True<ItemPriceInfo>();

            if (conditions.BrandID.HasValue)
            {
                predicate = predicate.And(f => f.item.ManufactureID == conditions.BrandID);
            }

            if (conditions.minPrice != null && conditions.maxPrice != null)
            {
                predicate = predicate.And(f => f.itemDisplayPrice.DisplayPrice >= conditions.minPrice && f.itemDisplayPrice.DisplayPrice <= conditions.maxPrice);
            }

            if (allPropertyIDs.Count > 0)
            {
                List<int> productIds = ItemPriceInfo.Where(predicate).Select(x => x.item.ProductID).Distinct().ToList();
                productIds = this._propertyRepoAdapter.FilterProductIds(productIds, conditions.CategoryID, allPropertyIDs);
                predicate = predicate.And(f => productIds.Contains(f.item.ProductID));
            }

            List<ItemPriceInfo> allItems = ItemPriceInfo.AsExpandable().Where(predicate).ToList();

            if (allItems != null && allItems.Count > 0)
            {
                if (conditions.orderBy != null)
                {
                    switch (conditions.orderBy)
                    {
                        case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.CreatDate:
                            allItems = allItems.OrderByDescending(x => x.item.CreateDate).ToList();
                            break;
                        case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.PopularityIndex:
                            allItems = allItems.OrderByDescending(x => x.item.QtyReg).ToList();
                            break;
                        case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.Recommended:
                            allItems = allItems.OrderBy(x => x.item.RedmfdbckRate).ToList();
                            break;
                        case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.HighPrice:
                            allItems = allItems.OrderByDescending(x => x.itemDisplayPrice.DisplayPrice).ToList();
                            break;
                        case (int)TWNewEgg.Models.DomainModels.Item.CategoryItemConditions.OrderByCondition.LowPrice:
                            allItems = allItems.OrderBy(x => x.itemDisplayPrice.DisplayPrice).ToList();
                            break;
                        default:
                            allItems = allItems.OrderByDescending(x => x.item.CreateDate).ToList();
                            break;
                    }
                }
            }
            else
            {
                allItems = new List<ItemPriceInfo>();
            }

            int itemNumber = 1;
            foreach (var temp in allItems)
            {
                temp.NowPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(itemNumber) / (conditions.OnePageItemsQty ?? 8)));
                temp.Qty = allItems.Count();
                temp.TotalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(allItems.Count()) / (conditions.OnePageItemsQty ?? 8)));
                itemNumber++;
            }

            if (conditions.NowPage != null && conditions.NowPage != 0)
            {
                allItems = allItems.Where(x => x.NowPage == conditions.NowPage).ToList();
            }

            List<ItemInfo> result = ModelConverter.ConvertTo<List<ItemInfo>>(allItems);
            List<int> itemIds = result.Select(x => x.ItemBase.ID).ToList();

            if (conditions.NowPage != null && conditions.NowPage != 0)
            {
                Dictionary<int, List<ImageUrlReferenceDM>> ImageUrlReferenceDMList = this._itemImageUrlService.GetItemImagePath(itemIds);
                List<ItemDisplayPrice> ItemDisplayPriceList = allItems.Where(x => x.itemDisplayPrice != null).Select(x => x.itemDisplayPrice).ToList();

                foreach (var temp in result)
                {
                    temp.NowPage = conditions.NowPage ?? 1;
                    temp.Qty = allItems.FirstOrDefault().Qty;
                    temp.TotalPage = allItems.FirstOrDefault().TotalPage;

                    if (ItemDisplayPriceList != null)
                    {
                        if (ItemDisplayPriceList.Where(x => x.ItemID == temp.ItemBase.ID).Count() > 0)
                        {
                            if (ItemDisplayPriceList.Where(x => x.ItemID == temp.ItemBase.ID).FirstOrDefault() != null)
                                if (ItemDisplayPriceList.Where(x => x.ItemID == temp.ItemBase.ID).FirstOrDefault().DisplayPrice != 0)
                                {
                                    temp.ItemBase.PriceCash = ItemDisplayPriceList.Where(x => x.ItemID == temp.ItemBase.ID).FirstOrDefault().DisplayPrice;
                                }
                        }
                    }

                    try
                    {
                        if (ImageUrlReferenceDMList.Count > 0 && ImageUrlReferenceDMList.Where(x => x.Key == temp.ItemBase.ID).FirstOrDefault().Value.Count != 0)
                        {
                            if (ImageUrlReferenceDMList[temp.ItemBase.ID].Where(x => x.Size == 300).FirstOrDefault() != null)
                            {
                                temp.ItemBase.imgPath = ImageUrlReferenceDMList[temp.ItemBase.ID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                            }
                            else
                            {
                                if (ImageUrlReferenceDMList[temp.ItemBase.ID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                {
                                    temp.ItemBase.imgPath = ImageUrlReferenceDMList[temp.ItemBase.ID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                }
                                else
                                {
                                    if (ImageUrlReferenceDMList[temp.ItemBase.ID].Where(x => x.Size == 60).FirstOrDefault() != null)
                                    {
                                        temp.ItemBase.imgPath = ImageUrlReferenceDMList[temp.ItemBase.ID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                                    }
                                }
                            }

                            if (temp.ItemBase.imgPath.IndexOf("newegg.com/") >= 0)
                            {
                            }
                            else
                            {
                                string hostString = ConfigurationManager.AppSettings.Get("ECWebHttpsImgDomain");
                                temp.ItemBase.imgPath = string.Format("{0}{1}", hostString, temp.ItemBase.imgPath);
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
            return result;
        }

        public List<ItemInfo> GetCategoryItemsTopRows(CategoryItemConditions conditions, int num)
        {
            List<int> allPropertyIDs = new List<int>();
            if (conditions.FilterID != null)
            {
                allPropertyIDs = conditions.FilterID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(f => int.Parse(f)).ToList();
            }

            IQueryable<Models.DBModels.TWSQLDB.Item> allCategoryItems = this._itemRepoAdapter.GetAvailableAndVisible(conditions.CategoryID);
            IQueryable<Models.DBModels.TWSQLDB.Item> crossCategoryItems = this._itemRepoAdapter.GetCrossCategoryAvailableAndVisible(conditions.CategoryID);
            allCategoryItems = allCategoryItems.Union(crossCategoryItems);

            var predicate = PredicateBuilder.True<Models.DBModels.TWSQLDB.Item>();
            if (conditions.BrandID.HasValue)
            {
                predicate = predicate.And(f => f.ManufactureID == conditions.BrandID);
            }

            if (conditions.minPrice != null && conditions.maxPrice != null)
            {
                predicate = predicate.And(f => f.PriceCash >= conditions.minPrice && f.PriceCash <= conditions.maxPrice);
            }

            if (allPropertyIDs.Count > 0)
            {
                List<int> productIds = allCategoryItems.Where(predicate).Select(x => x.ProductID).Distinct().ToList();
                productIds = this._propertyRepoAdapter.FilterProductIds(productIds, conditions.CategoryID, allPropertyIDs);
                predicate = predicate.And(f => productIds.Contains(f.ProductID));
            }

            List<Models.DBModels.TWSQLDB.Item> allItems = num > 0 ? allCategoryItems.AsExpandable().Where(predicate).OrderByDescending(x => x.CreateDate).Take(num).ToList() : allCategoryItems.AsExpandable().Where(predicate).OrderByDescending(x => x.CreateDate).ToList();
            List<ItemInfo> result = ModelConverter.ConvertTo<List<ItemInfo>>(allItems);

            return result;
        }

        public List<MainZone> GetCategoryBanner(int categoryID)
        {
            List<MainZone> categoryTopBanner = new List<MainZone>();
            List<StoreBanner> categoryBanners = ConvertToStoreBannerList(this._storeRepoAdapter.Advertising_GetAll()
                .Where(x => x.CategoryID == categoryID && x.BannerType == ConstBannerType.Rotate && x.ShowAll == ConstShowAll.Show)
                .OrderBy(x => x.Showorder).ToList());

            if (categoryBanners != null && categoryBanners.Count > 0 && categoryBanners.FirstOrDefault() != null)
            {
                foreach (var singleCategoryBanner in categoryBanners)
                {
                    categoryTopBanner.Add(ConvertTopBannerToMainZone(singleCategoryBanner));
                }
            }
            else
            {
                List<int> itemIDs = GetStarProductItemID(categoryID);
                if (itemIDs != null && itemIDs.Count > 0)
                {
                    List<Models.DBModels.TWSQLDB.Item> starItems = this._itemRepoAdapter.GetAvailableAndVisibleItemList(itemIDs).ToList();
                    MainZone mainZone = new MainZone();
                    mainZone = ConvertItemToMainZone(starItems, itemIDs);
                    mainZone.Title = GetStarProductTitle(categoryID);
                    categoryTopBanner.Add(mainZone);
                }
            }

            return categoryTopBanner;
        }

        /// <summary>
        /// 獲取同一層級，加上父節點的麵包屑選單.
        /// </summary>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public Breadcrumbs GetBreadCrumbs(int categoryID)
        {
            // 先找出目前Store的父節點是誰.
            Category currentStore = this._categoryRepoAdapter.Category_GetAll().Where(x =>
                x.ID == categoryID && x.ShowAll == 1).OrderBy(x => x.Showorder).FirstOrDefault();

            // 如果節點不存在.
            if (currentStore == null)
                return null;

            // 把父節點和同層的節點撈出來.
            List<Category> caterogyList = this._categoryRepoAdapter.Category_GetAll().Where(x =>
                (x.ParentID == currentStore.ParentID || x.ID == currentStore.ParentID) && x.ShowAll == 1).OrderBy(x =>x.Showorder).ToList();

            Category father = caterogyList.Where(x => x.ID == currentStore.ParentID).FirstOrDefault();
            List<Category> brothers = caterogyList.Where(x => x.ParentID == currentStore.ParentID).ToList();

            // 建立父節點.
            Breadcrumbs retBreadCrumbs = new Breadcrumbs();
            retBreadCrumbs.ListParentCategories = new List<Category_TreeItem>();
            if (father != null)
            {
                Category grandFather = this._categoryRepoAdapter.Category_GetAll().Where(x => x.ID == father.ParentID).FirstOrDefault();
                retBreadCrumbs.Title = father.Description;
                retBreadCrumbs.CategoryID = grandFather == null ? father.ID : grandFather.ID;
                if (grandFather != null)
                {
                    retBreadCrumbs.ListParentCategories.Add(ConvertToCategoryTreeItem(grandFather));
                }
                retBreadCrumbs.ListParentCategories.Add(ConvertToCategoryTreeItem(father));
            }
            retBreadCrumbs.ListParentCategories.Add(ConvertToCategoryTreeItem(currentStore));
            // 建立兄弟姊妹的結點.
            retBreadCrumbs.DropDownItems = new List<BreadcrumbItem>();
            if (brothers.Any())
            {
                // 找出兄弟姊妹是否在任選館Table當中.
                List<int> brotherCategoryIDs = brothers.Select(x => x.ID).ToList();

                foreach (var x in brothers)
                {

                    //Should Speed up... bw52
                    if (_itemRepoAdapter.GetAll().Where(y => y.CategoryID == x.ID && y.Status == 0).Count() < 1)
                    {
                        continue;
                    }
                    BreadcrumbItem item = new BreadcrumbItem()
                    {
                        Title = x.Description,
                        CategoryID = x.ID,
                        IsOptionStore = false, // 如果是任選館, 就會在清單內可找到.
                    };

                    retBreadCrumbs.DropDownItems.Add(item);
                }
            }

            return retBreadCrumbs;
        }

        #region private function
        private Category_TreeItem ConvertToCategoryTreeItem(Category category)
        {
            Category_TreeItem result = new Category_TreeItem();
            result.category_categoryfromwsid = category.CategoryfromwsID;
            result.category_createdate = category.CreateDate;
            result.category_createuser = category.CreateUser;
            result.category_description = category.Description;
            result.category_deviceid = category.DeviceID;
            result.category_id = category.ID;
            result.category_layer = category.Layer;
            result.category_parentid = category.ParentID;
            result.category_sellerid = category.SellerID;
            result.category_showall = category.ShowAll;
            result.category_showorder = category.Showorder;
            result.category_title = category.Title;
            result.ClassName = category.ClassName;
            result.ImageHref = category.ImageHref;
            result.ImagePath = category.ImagePath;
            return result;
        }

        private List<StoreBanner> ConvertToStoreBannerList(List<Advertising> advs)
        {
            List<StoreBanner> rtn = new List<StoreBanner>();
            foreach (Advertising adv in advs)
                rtn.Add(ConvertToStoreBanner(adv));

            return rtn;
        }
        private StoreBanner ConvertToStoreBanner(Advertising adv)
        {
            if (adv == null)
                return null;

            StoreBanner retBanner = new StoreBanner()
            {
                Title = adv.Description,
                Image = adv.Imagepath,
                Url = adv.Clickpath
            };

            return retBanner;
        }
        private MainZone ConvertTopBannerToMainZone(StoreBanner singleCategoryBanner)
        {
            MainZone mainZone = new MainZone();
            mainZone.Title = singleCategoryBanner.Title;
            mainZone.LogoList = new List<StoreBanner>();
            mainZone.LogoList.Add(singleCategoryBanner);
            return mainZone;
        }
        private string GetStarProductTitle(int categoryID)
        {
            string title = string.Empty;
            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
            var starItems = this._starRepoAdapter.GetAll().Where(x => x.CategoryID == categoryID &&
                x.DateStart <= datetimeNow &&
                x.DateEnd > datetimeNow &&
                x.ManufactureID == 0).
                OrderBy(x => x.StarProductOrder).FirstOrDefault();
            if (starItems != null)
            {
                title = string.IsNullOrEmpty(starItems.StarTitle) == true ? "" : starItems.StarTitle;
            }
            return title;
        }
        private List<int> GetStarProductItemID(int categoryID)
        {
            List<int> itemIDs = new List<int>();
            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
            var starItems = this._starRepoAdapter.GetAll().Where(x => x.CategoryID == categoryID &&
                x.DateStart <= datetimeNow &&
                x.DateEnd > datetimeNow &&
                x.ManufactureID == 0).
                OrderBy(x => x.StarProductOrder).ToList();
            if (starItems.Count > 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    var topOrder = starItems.Where(x => x.ShowType == 1 && x.StarProductOrder == (i + 1)).FirstOrDefault();
                    if (topOrder != null)
                    {
                        itemIDs.Add(topOrder.ItemID);
                    }
                    else
                    {
                        var defaultOrder = starItems.Where(x => x.ShowType == 0 && x.StarProductOrder == (i + 1)).FirstOrDefault();
                        if (defaultOrder != null)
                        {
                            itemIDs.Add(defaultOrder.ItemID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            else
            {
                itemIDs.AddRange(starItems.Select(x => x.ItemID));
            }

            return itemIDs;
        }
        private MainZone ConvertItemToMainZone(List<Models.DBModels.TWSQLDB.Item> starItems, List<int> orderItems)
        {
            MainZone mainZone = new MainZone();

            mainZone.ItemList = new List<StoreItemCell>();
            //foreach (var singleItem in starItems)
            //{
            foreach (var orderItem in orderItems)
            {
                Models.DBModels.TWSQLDB.Item singleItem = starItems.Where(x => x.ID == orderItem).FirstOrDefault();
                if (singleItem != null)
                {
                    mainZone.ItemList.Add(ConvertItemToStoreItemCell(singleItem));
                }
                if (mainZone.ItemList.Count > 3)
                {
                    break;
                }
            }

            //}

            return mainZone;
        }
        private StoreItemCell ConvertItemToStoreItemCell(Models.DBModels.TWSQLDB.Item singleItem)
        {
            StoreItemCell storeItemCell = new StoreItemCell();
            storeItemCell.ItemID = singleItem.ID;
            storeItemCell.Title = singleItem.Name;
            storeItemCell.UnitPrice = singleItem.PriceCash;
            return storeItemCell;
        }
        #endregion
    }
}
