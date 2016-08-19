using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Category;
using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    [AllowNonSecures]
    [AllowAnonymous]
    public class CategoryTopTenItemsController : ApiController
    {

        // GET api/categorytoptenitems/5
        public List<CategoryTopItemVM> Get(int categoryID)
        {
            List<CategoryTopItemVM> topItems = new List<CategoryTopItemVM>();
            List<CategoryTopItemVM> searchResult = new List<CategoryTopItemVM>();
            CategoryTopItemDM searchCondition = new CategoryTopItemDM();

            Dictionary<int, List<ItemUrl>> itemUrlDictionary = new Dictionary<int, List<ItemUrl>>();
            Dictionary<int, ItemPrice> itemDisplayPriceresult = new Dictionary<int, ItemPrice>();
            Dictionary<int, ItemInfo> itemData = new Dictionary<int, ItemInfo>();
            Dictionary<int, int> itemStock = new Dictionary<int, int>();
            if (categoryID == 0)
            {
                topItems = null;
            }
            else
            {
                searchCondition.CategoryID = categoryID;
                searchCondition.ShowAll = (int)CategoryTopItemDM.showAll.顯示;
            }
            //load top10 Items
            var Result = Processor.Request<List<CategoryTopItemVM>, List<CategoryTopItemDM>>("CategoryTopItemService", "GetCategoryTopItem", searchCondition);

            if (!string.IsNullOrEmpty(Result.error))
            {
                return topItems;
            }
            searchResult = Result.results;

            if (searchResult != null)
                foreach (CategoryTopItemVM items in searchResult)
                {
                    if (items.ItemType == (int)CategoryTopItemDM.itemType.銷售TOP)
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
                            TWNewEgg.Models.ViewModels.Category.Category_TreeItem category = allCategories.Where(x => x.category_id == itemData[itemId].ItemBase.CategoryID).FirstOrDefault();
                            CategoryTopItemVM itemView = new CategoryTopItemVM();
                            itemView.CategoryID = itemData[itemId].ItemBase.CategoryID;
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

            return topItems;
        }

        // POST api/categorytoptenitems
        public void Post([FromBody]string value)
        {
        }

        // PUT api/categorytoptenitems/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/categorytoptenitems/5
        public void Delete(int id)
        {
        }
    }
}
