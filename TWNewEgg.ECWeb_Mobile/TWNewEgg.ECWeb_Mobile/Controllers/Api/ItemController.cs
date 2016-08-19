using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.ECWeb_Mobile.Utility;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.ViewModels.Item;

namespace TWNewEgg.ECWeb_Mobile.Controllers.Api
{
#if DEBUG
    [EnableCors(origins: "*", headers: "*", methods: "*")]
#endif
    public class ItemController : ApiController
    {
        // 滿額贈折價狀態設定，是否啟用正式機狀態的開關閥，正式機on、測試機off
        //private string promotionGiftStatusTurnON = System.Configuration.ConfigurationManager.AppSettings["PromotionGiftStatusTurnON"];
        private string promotionGiftStatusTurnON = "on";
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        public TWNewEgg.Models.ViewModels.Item.ItemBasic GetItemDetailByItemId(int ItemId)
        {
            TWNewEgg.Models.ViewModels.Item.ItemBasic objItemBasic = null;
            Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>> dictItemMarketGroup = null;

            objItemBasic = Processor.Request<ItemBasic, ItemDetail>("ItemDetailService", "GetItemDetail", ItemId, promotionGiftStatusTurnON).results;
            if (objItemBasic == null)
            {
                objItemBasic = new ItemBasic();
                return objItemBasic;
            }
            Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", new List<int> { ItemId }).results;

            var listImgUrl = new List<string>();
            foreach (ItemUrl singleImgUrl in itemUrlDictionary[ItemId].Where(x => x.Size == 640))
            {
                if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                {
                    listImgUrl.Add(singleImgUrl.ImageUrl);
                }
                else
                {
                    listImgUrl.Add(ImageUtility.GetImagePath(singleImgUrl.ImageUrl));
                }
            }
            objItemBasic.ImgUrlList = listImgUrl;

            //取得ItemGroup
            dictItemMarketGroup = Processor.Request<Dictionary<int, List<TWNewEgg.Models.ViewModels.Item.ItemMarketGroup>>, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ItemMarketGroup>>>("ItemGroupService", "GetRelativeItemMarketGroupByItemId", ItemId).results;
            if (dictItemMarketGroup != null)
            {
                objItemBasic.DictItemMarketGroup = dictItemMarketGroup;
            }

            return objItemBasic;
        }

        [AllowNonSecures]
        [AllowAnonymous]
        [HttpPost]
        public List<TWNewEgg.Models.ViewModels.Item.ItemBasic> GetItemDetailByItemIds(List<int> ItemIds)
        {
            List<TWNewEgg.Models.ViewModels.Item.ItemBasic> objItemBasic = null;
            if (ItemIds == null || ItemIds.Count == 0)
            {
                return objItemBasic;
            }
            ItemIds = ItemIds.Take(100).ToList();
            objItemBasic = Processor.Request<List<ItemBasic>, List<ItemDetail>>("ItemDetailService", "GetItemDetails", ItemIds, promotionGiftStatusTurnON).results;
            if (objItemBasic == null)
            {
                objItemBasic = new List<ItemBasic>();
                return objItemBasic;
            }

            Dictionary<int, List<ItemUrl>> itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", ItemIds).results;

            for (int i = 0; i < objItemBasic.Count; i++)
            {
                var listImgUrl = new List<string>();
                foreach (ItemUrl singleImgUrl in itemUrlDictionary[objItemBasic[i].Id].Where(x => x.Size == 640))
                {
                    if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                    {
                        listImgUrl.Add(singleImgUrl.ImageUrl);
                    }
                    else
                    {
                        listImgUrl.Add(ImageUtility.GetImagePath(singleImgUrl.ImageUrl));
                    }
                }
                objItemBasic[i].ImgUrlList = listImgUrl;
            }

            return objItemBasic;
        }

        /// <summary>
        /// 根據CategoryId取得麵包屑及所有的Parent
        /// </summary>
        /// <param name="CategoryId">Category Id</param>
        /// <returns></returns>
        [AllowNonSecures]
        [AllowAnonymous]
        [HttpGet]
        public TWNewEgg.Models.ViewModels.Store.Breadcrumbs GetItemParentCategories(int CategoryId)
        {
            bool boolSearchResult = false;
            int i = 0;
            //List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> resultTreeItem = null;
            TWNewEgg.Models.ViewModels.Store.Breadcrumbs objBreadcrumbs = null;
            //List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listTreeItem = null;

            objBreadcrumbs = Processor.Request<TWNewEgg.Models.ViewModels.Store.Breadcrumbs, TWNewEgg.Models.DomainModels.Store.Breadcrumbs>("CategoryItemService", "GetBreadCrumbs", CategoryId).results;


            //ECService finish it, do mark code below...2016/04/15 bw52
            //if (objBreadcrumbs != null)
            //{
            //    //取得全站的TreeItem
            //    listTreeItem = Processor.Request<List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem>>("Service.BaseService", "XML2Tree", "XML\\Category_Menu_ALL.XML").results;

            //    //取得該CategoryId的TreeItem
            //    resultTreeItem = new List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem>();
            //    for (i = 0; i < listTreeItem.Count && !boolSearchResult; i++)
            //    {
            //        GetItemCategoryPath(listTreeItem[i], ref resultTreeItem, CategoryId, 0, ref boolSearchResult);
            //    }

            //    if (resultTreeItem.Count > 0)
            //    {
            //        objBreadcrumbs.ListParentCategories = resultTreeItem;
            //    }

            //}
            return objBreadcrumbs;
        }

        /// <summary>
        /// 利用Stack的概念組合TreeItem路徑,此函式為遞迴
        /// </summary>
        /// <param name="argTreeItem">該節點</param>
        /// <param name="resultTreeItem">查詢結果, 以List<Tree_Item></Tree_Item></param>
        /// <param name="argSearchCategoryId">搜尋的CategoryId</param>
        /// <param name="argSearchLayer">遞迴層數控制, 初次呼叫請輸入0</param>
        /// <param name="boolSearchResult">搜尋結果, 初次呼叫請輸入false</param>
        private void GetItemCategoryPath(TWNewEgg.Models.ViewModels.Category.Category_TreeItem argTreeItem, ref List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> resultTreeItem, int argSearchCategoryId, int argSearchLayer, ref bool boolSearchResult)
        {
            //搜尋層數超過10層不合理, 中斷遞迴
            if (argSearchLayer > 10)
            {
                return;
            }

            int numSearchLayer = argSearchLayer + 1;
            List<TWNewEgg.Models.ViewModels.Category.Category_TreeItem> listChildTreeItem = null;

            listChildTreeItem = argTreeItem.Nodes;
            if (listChildTreeItem == null || listChildTreeItem.Count <= 0)
            {
                return;
            }

            foreach (TWNewEgg.Models.ViewModels.Category.Category_TreeItem objSubItem in listChildTreeItem)
            {
                //先將該節點加入結果集裡
                resultTreeItem.Add(objSubItem);
                if (objSubItem.category_id == argSearchCategoryId)
                {
                    boolSearchResult = true;
                    break;
                }
                else
                {
                    GetItemCategoryPath(objSubItem, ref resultTreeItem, argSearchCategoryId, numSearchLayer, ref boolSearchResult);
                    //若該遞迴沒有被中斷跳出迴圈, 表示該節點並沒有搜尋的CategoryId, 就刪除掉
                    if (boolSearchResult)
                    {
                        break;
                    }
                    else
                    {
                        resultTreeItem.Remove(objSubItem);
                    }
                }

            }

        }

    }
}
