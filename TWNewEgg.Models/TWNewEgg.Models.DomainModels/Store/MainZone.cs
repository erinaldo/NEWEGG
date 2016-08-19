using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 分類頁面、直購館的櫥窗的中央顯示區塊
    /// </summary>
    public class MainZone
    {
        /// <summary>
        /// 櫥窗的標題.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 如果是直購館的話，會有形象圖片.
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// 推薦的品牌連結.
        /// </summary>
        public List<StoreBanner> LogoList { get; set; }
        
        /// <summary>
        /// 中央顯示的產品集合.
        /// </summary>
        public List<StoreItemCell> ItemList { get; set; }
        
        /// <summary>
        /// 推薦的連結項目.
        /// </summary>
        public List<GroupLink> GroupLinkList { get; set; }
    }
}
