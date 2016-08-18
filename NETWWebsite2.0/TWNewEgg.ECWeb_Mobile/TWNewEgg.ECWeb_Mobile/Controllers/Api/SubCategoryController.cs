using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    [AllowNonSecures]
    [AllowAnonymous]
    public class SubCategoryController : ApiController
    {
        // GET api/subcategory/5
        public CategoryItemInfoMain_View Get(int? maxPrice, int? minPrice, string PVID, string OrderBy, int ItemsQty = 36, int Page = 1, int CategoryID = 286)
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
            CategoryItemInfoMain_ViewList = FindCategoryItems(OrderBy, CategoryItemForm, ItemsQty, Page);

            return CategoryItemInfoMain_ViewList;
        }

        // POST api/subcategory
        public void Post([FromBody]string value)
        {
        }

        // PUT api/subcategory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/subcategory/5
        public void Delete(int id)
        {
        }


        private CategoryItemInfoMain_View FindCategoryItems(string OrderBy, CategoryItemForm CategoryItemForm, int ItemsQty = 36, int Page = 1)
        {
            CategoryItemConditions conditions = ModelConverter.ConvertTo<CategoryItemConditions>(CategoryItemForm);
            var CategoryAreaInfotemp = Processor.Request<TWNewEgg.Models.DomainModels.Category.CategoryAreaInfo, TWNewEgg.Models.DomainModels.Category.CategoryAreaInfo>("CategoryItemService", "GetCategoryAreaInfo", conditions);
            TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View CategoryItemInfoMain_ViewList = new TWNewEgg.Models.ViewModels.Item.CategoryItemInfoMain_View();
            if (!string.IsNullOrEmpty(CategoryAreaInfotemp.error))
            {
                return CategoryItemInfoMain_ViewList;
            }
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

            // Mobile Site don't need it.
            //TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
            CategoryItemInfoMain_ViewList.ShowPageList = new List<Models.ViewModels.Page.ShowPage>();
            return CategoryItemInfoMain_ViewList;
        }
    }
}
