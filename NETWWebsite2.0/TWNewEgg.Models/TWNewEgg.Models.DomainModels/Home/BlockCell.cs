using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.Home
{
    /// <summary>
    /// 各櫥窗區塊的內容.
    /// </summary>
    public class BlockCell
    {
        /// <summary>
        /// 定義 block cell type，參考 Type 上的註解 
        /// </summary>
        public enum CellType
        {
            SingleImage,
            GroupList,
            LogoList,
            TextOverImage,
            Item,
            Text
        }

        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Cell內容的型態,依此來決定這個Cell要呈現何種資料型態.
        /// 目前值可能為 "SingleImage"、"GroupList"、"LogoList"、"TextOverImage"、"Item"、"Text"
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 存放單張圖檔連結、多張圖檔連結、或是多張Logo的連結.
        /// </summary>
        public List<StoreBanner> ImageList { get; set; }

        /// <summary>
        /// 推薦的連結項目.
        /// </summary>
        public List<GroupLink> GroupLinkList { get; set; }

        /// <summary>
        /// 文字型態的內容(是否為Html格式需與BSA確認)
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 單一商品的內容
        /// </summary>
        public StoreItemCell Item { get; set; }

        /// <summary>
        /// 文字疊圖類型.
        /// </summary>
        public TextOverImage TextOverImage { get; set; }
    }
}
