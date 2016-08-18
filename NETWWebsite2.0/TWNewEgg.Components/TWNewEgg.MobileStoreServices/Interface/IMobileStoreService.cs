using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.MobileStore;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.MobileStoreServices.Interface
{
    public interface IMobileStoreService
    {
        /// <summary>
        /// 獲取手機分類頁的所有資料
        /// </summary>
        /// <param name="categoryID">目前分類的ID</param>
        /// <param name="subCategoryID">指定要顯示哪個子分類的商品, 如果給0, 就取精選商品或第一個子分類</param>
        /// <returns>回傳手機分類頁的所有資料(如果CategoryID不存在或查不到資料, 就返回null)</returns>
        MStoreInfo GetMobileStoreInfo(int categoryID, int subCategoryID);

        /// <summary>
        /// 獲取手機子分類的商品集合
        /// </summary>
        /// <param name="categoryID">商品主分類ID</param>
        /// <param name="subCategoryID">想獲取商品的子分類頁ID, 如果為0就代表取精選商品</param>
        /// <returns>回傳商品的集合(如果該分類無資料,或ID不存在,就返回空串列)</returns>
        List<MStoreItemCell> GetMobileStoreItems(int categoryID, int subCategoryID);

        //List<ShopWindow> GetShopWindows(int mainCategoryID, int selCategoryID, int? accountID = null);
    }
}
