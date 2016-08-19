using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Home
{
    /// <summary>
    /// 首頁主Banner下方要顯示的動態資訊.
    /// </summary>
    public class HomeStoreInfo
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        // 閃購區(可能不用做了).

        // 各館推薦商品區(可能不用做了).

        /// <summary>
        /// 各櫥窗的資料集合.
        /// </summary>
        public List<HomeShopWindow> ShopWindowList { get; set; }
    }
}
