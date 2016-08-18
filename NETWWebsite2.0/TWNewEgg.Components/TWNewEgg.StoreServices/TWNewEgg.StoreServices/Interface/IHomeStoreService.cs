using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Home;

namespace TWNewEgg.StoreServices.Interface
{
    public interface IHomeStoreService
    {
        /// <summary>
        /// 取得首頁包含商品櫥窗的所有資訊(目前僅有櫥窗集合,其他的別人做了)
        /// </summary>
        /// <param name="shopWindowIndexList">指定要抓取的ShopWindow索引,例如 {0,1,2}, 如果給空串列或null, 代表取全部</param>
        /// <returns>回傳首頁下方的資訊</returns>
        HomeStoreInfo GetHomeStoreInfo(List<int> shopWindowIndexList);

        /// <summary>
        /// 獲取首頁底下的櫥窗資料集合.
        /// </summary>
        /// <param name="shopWindowIndexList">指定的ShopWindow索引,例如 {0,3,4}, 如果給空串列或null, 代表取全部</param>
        /// <returns>回傳HomeShopWindow的集合</returns>
        List<HomeShopWindow> GetHomeShopWindows(List<int> shopWindowIndexList);
    }
}
