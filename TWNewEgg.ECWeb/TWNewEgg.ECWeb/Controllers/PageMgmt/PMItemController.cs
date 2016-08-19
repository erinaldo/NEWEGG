using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.PageMgmt;

namespace TWNewEgg.ECWeb.Controllers.PageMgmt
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class PMItemController : Controller
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        // 滿額贈折價狀態設定，是否啟用正式機狀態的開關閥，正式機on、測試機off
        private string promotionGiftStatusTurnON = System.Configuration.ConfigurationManager.AppSettings["PromotionGiftStatusTurnON"];

        [HttpPost]
        public ActionResult Type1(int? id)
        {
            try
            {
                int ID = id ?? 0;
                var itemServiceResult = Processor.Request<ItemDetail, ItemDetail>("ItemDetailService", "GetItemDetail", id, promotionGiftStatusTurnON);
                if (itemServiceResult.error != null)
                {
                    throw new Exception(itemServiceResult.error);
                }
                
                var imageServiceResult = Processor.Request<List<ImageUrlReferenceDM>, List<ImageUrlReferenceDM>>("ItemImageUrlService", "GetSingleItemImagePath", id);
                if (imageServiceResult.error != null)
                {
                    throw new Exception(imageServiceResult.error);
                }

                int storeID = 0;
                Api.CategoryParentController categoryService = new Api.CategoryParentController();
                Services.Category.FindCategoryForSEO categoryFinder = new Services.Category.FindCategoryForSEO();
                var categories = categoryService.Post(new List<int> { itemServiceResult.results.Main.ItemBase.ID });
                TWNewEgg.Models.ViewModels.Category.Category_TreeItem category = categories.Where(x => x.category_id == itemServiceResult.results.Main.ItemBase.CategoryID).FirstOrDefault();
                if (category != null)
                {
                    storeID = categoryFinder.FindCategoryForURL(category, 0);
                }

                ItemDetail item = itemServiceResult.results;
                List<ImageUrlReferenceDM> imgUrls = imageServiceResult.results;
                ImageUrlReferenceDM image = imgUrls.Where(x => x.Size >= 300).FirstOrDefault();
                string imgUrl = image != null ? image.ImageUrl : imgUrls[0].ImageUrl;

                Type1PMItem pmItem = new Type1PMItem
                {
                    ID = item.Main.ItemBase.ID,
                    Title = item.Main.ItemBase.Name,
                    SubTitle = item.Main.ItemBase.Spechead,
                    DisplayPrice = item.Price.DisplayPrice.ToString("N0"),
                    MarketPrice = item.Main.ItemBase.MarketPrice.HasValue ? item.Main.ItemBase.MarketPrice.Value.ToString("N0") : item.Price.DisplayPrice.ToString("N0"),
                    ImageUrl = TWNewEgg.ECWeb.Utility.ImageUtility.GetImagePath(imgUrl),
                    SellingQty = item.SellingQty,
                    CategoryID = item.Main.ItemBase.CategoryID,
                    StoreID = storeID
                };

                return Json(pmItem);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                return Json(null);
            }
        }
    }
}
