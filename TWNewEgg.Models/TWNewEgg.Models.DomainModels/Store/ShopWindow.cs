using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// Store頁面的櫥窗區塊
    /// </summary>
    public class ShopWindow
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 代表在該Store的顯示位置，例如0代表是第一個要顯示的ShopWindow
        /// </summary>
        public int Index { get; set; }
        
        /// <summary>
        /// 如果為 "UsaDirect" 代表是直購館Layout; 否則預設值為 "Normal" 代表一般分類頁面Layout.
        /// </summary>
        public string LayoutType { get; set; }

        /// <summary>
        /// 中央主顯示區.
        /// </summary>
        public MainZone MainZone { get; set; }
        
        /// <summary>
        /// 商品列表區.
        /// </summary>
        public ListZone ListZone { get; set; }
    }
}
