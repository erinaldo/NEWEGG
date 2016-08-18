using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.MobileStore
{
    /// <summary>
    /// 手機版 - 提供給Store頁面的主要資料型態.
    /// </summary>
    public class MStoreInfo
    {
        /// <summary>
        /// Store的標題.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 中央顯示輪播的Banner
        /// </summary>
        public List<StoreBanner> BannerList { get; set; }

        /// <summary>
        /// 頁籤區塊(顯示精選商品、其他子分類的名稱資料)
        /// </summary>
        public List<MStoreTab> StoreTabList { get; set; }

        /// <summary>
        /// 目前顯示的子分類頁ID.
        /// </summary>
        public int SubCategoryID { get; set; }
        
        /// <summary>
        /// 子分類頁產品的集合.
        /// </summary>
        public List<MStoreItemCell> ItemList { get; set; }
    }
}
