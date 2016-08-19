using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 提供給Store頁面的主要資料型態.
    /// </summary>
    public class StoreInfo
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Store的標題.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 中央顯示輪播的Banner
        /// </summary>
        public List<StoreBanner> PullDownBannerList{ get; set; }
        
        /// <summary>
        /// 右邊小廣告的Banner,有可能為空值.
        /// </summary>
        public List<StoreBanner> PullDownAdvList { get; set; }
        
        /// <summary>
        /// 中間顯示的單一Banner
        /// </summary>
        public StoreBanner SingleBanner { get; set; }
        
        /// <summary>
        /// 電梯區,顯示的文字內容要與櫥窗的標題一致.
        /// </summary>
        public List<ElevatorItem> Elevator { get; set; }

        /// <summary>
        /// 該Store由PM配置的各個櫥窗內容,最多10個.
        /// </summary>
        public List<ShopWindow> ShopWindowList { get; set; }

        /// <summary>
        /// Store頁面的總櫥窗數(因為ShopWindowList可能不會全抓)
        /// </summary>
        public int ShopWindowCount { get; set; }
    }
}
