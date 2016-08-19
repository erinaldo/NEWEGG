using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.Home
{
    /// <summary>
    /// 首頁的單一櫥窗
    /// </summary>
    public class HomeShopWindow
    {
        public HomeShopWindow()
        {
            BlockList = new List<WindowBlock>();
            ImageList = new List<StoreBanner>();
            BeltBanner = new StoreBanner();
        }
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 代表在首頁的顯示位置，例如0代表是第一個要顯示的ShopWindow
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 用來指定各櫥窗的圖示(內容形式還要再跟UI約定).
        /// </summary>
        public string IconType { get; set; }

        /// <summary>
        /// 用來決定各櫥窗要綁哪種CSS(可能只給編號, UI再自行對應)
        /// </summary>
        public string CssType { get; set; }

        /// <summary>
        /// 各櫥窗的標題名稱.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 腰帶區
        /// </summary>
        public StoreBanner BeltBanner { get; set; }

        /// <summary>
        /// 櫥窗區塊集合(目前最多只會有三個區塊,左-中-右)
        /// </summary>
        public List<WindowBlock> BlockList { get; set; }

        /// <summary>
        /// 存放單張圖檔連結、多張圖檔連結、或是多張Logo的連結.
        /// </summary>
        public List<StoreBanner> ImageList { get; set; }
    } 
}
