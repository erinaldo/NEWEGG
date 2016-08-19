using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;
using TWNewEgg.StoreRepoAdapters;
using TWNewEgg.StoreRepoAdapters.Interface;




namespace TWNewEgg.StoreServices.Interface
{
    public interface IStoreService
    {
        /// <summary>
        /// 獲取第一次進入Store頁面的所有必要資訊
        /// </summary>
        /// <param name="categoryID">Store的分類ID</param>
        /// <param name="shopWindowIndexList">指定要抓取的ShopWindow索引,例如 {0,1,2}, 如果給空串列或null, 代表取全部</param>
        /// <returns>回傳Store頁面的所有資訊</returns>
        StoreInfo GetStoreInfo(int categoryID, List<int> shopWindowIndexList);
        
        /// <summary>
        /// 獲取某個Store底下的指定ShopWindow資料集合.
        /// </summary>
        /// <param name="categoryID">Store的分類ID</param>
        /// <param name="shopWindowIndexList">指定的ShopWindow索引,例如 {0,3,4}, 如果給空串列或null, 代表取全部</param>
        /// <returns>回傳ShopWindow的集合</returns>
        List<ShopWindow> GetShopWindows(int categoryID, List<int> shopWindowIndexList);

        /// <summary>
        /// 獲取任選館的頁面全部資訊.
        /// </summary>
        /// <param name="categoryID">任選館的分類ID</param>
        /// <param name="pageIndex">要求顯示第幾頁的資料</param>
        /// <param name="pageItemCount">單頁商品的數量(給0的話就用Service配置的預設值32)</param>
        /// <param name="filterIDs">篩選條件,內容是把屬性ID用逗號分隔的字串,例如 "1001,1002,1003"</param>
        /// <param name="sortValue">排序的方法, 如果給的Value不在清單中，就使用"人氣排行榜"進行排序</param>
        /// <returns>回傳該頁面的全部資訊</returns>
        OptionStoreInfo GetOptionStoreInfo(int categoryID, int pageIndex, int pageItemCount, string filterIDs, string sortValue);

        /// <summary>
        /// 獲取任選館的商品列表顯示區.
        /// </summary>
        /// <param name="categoryID">任選館的分類ID</param>
        /// <param name="pageIndex">要求顯示第幾頁的資料</param>
        /// <param name="pageItemCount">單頁商品的數量(給0的話就用Service配置的預設值32)</param>
        /// <param name="filterIDs">篩選條件,內容是把屬性ID用逗號分隔的字串,例如 "1001,1002,1003"</param>
        /// <param name="sortValue">排序的方法, 如果給的Value不在清單中，就使用"人氣排行榜"進行排序</param>
        /// <returns>回傳商品列表顯示區(內含商品集合與總頁數)</returns>
        OptionStoreListZone GetOptionStoreListZone(int categoryID, int pageIndex, int pageItemCount, string filterIDs, string sortValue);

        /// <summary>
        /// 獲取任選館指定的特定Item集合.
        /// </summary>
        /// <param name="itemIDs">傳入需要商品的ItemID集合</param>
        /// <returns>回傳商品的集合</returns>
        List<OptionStoreItemCell> GetOptionStoreItems(List<int> itemIDs);

        /// <summary>
        /// 獲取廣告.
        /// </summary>
        /// <param name="categoryID">傳入分類id</param>
        /// <param name="bannerType">傳入要取得的廣告類型</param>
        /// <returns>回傳廣告的集合</returns>
        List<StoreBanner> GetBanner(int categoryID, int bannerType);

        /// <summary>
        /// 獲取該分類下全部的廣告.
        /// </summary>
        /// <param name="categoryID">傳入分類id</param>
        /// <returns>回傳廣告的集合</returns>
        Dictionary<int, List<StoreBanner>> GetAllBanner(int categoryID);

        /// <summary>
        /// 新增任選館商品
        /// </summary>
        /// <param name="argObjSubCategory_OptionStore"></param>
        int Create(TWNewEgg.Models.DomainModels.Store.SubCategory_OptionStore_DM argObjSubCategory_OptionStore);
        
        /// <summary>
        /// 更新任選館商品
        /// </summary>
        /// <param name="argObjSubCategory_OptionStore"></param>
        /// <returns></returns>
        bool Update(TWNewEgg.Models.DomainModels.Store.SubCategory_OptionStore_DM argObjSubCategory_OptionStore);
        /// <summary>
        /// 搜尋任選館資料
        /// </summary>
        /// <param name="argNumId"></param>
        /// <returns></returns>
        Models.DomainModels.Store.SubCategory_OptionStore_DM GetById(int CategoryId);
    }
}
