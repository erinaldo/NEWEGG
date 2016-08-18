using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Advertising
{

    public class AdSet
    {
        public enum TypeOption
        {
            /// <summary>
            /// 僅顯示圖片
            /// </summary>
            OnlyImage = 1,
            /// <summary>
            /// 僅顯示文字
            /// </summary>
            OnlyCharacter = 2,
            /// <summary>
            /// 顯示圖片及文字
            /// </summary>
            ImageAndCharacter = 3
        }
        public AdSet()
        {
        }

        /// <summary>
        /// 廣告代號
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 版位CSS名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 說明
        /// </summary>
        public string AdsetDesc { get; set; }

        /// <summary>
        /// 最多字數
        /// </summary>
        public int TxtMax { get; set; }

        /// <summary>
        /// 最少字數
        /// </summary>
        public int TxtMin { get; set; }

        /// <summary>
        /// 寬度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 版面型式
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 快取分鐘數
        /// </summary>
        public int CacheMins { get; set; }

        /// <summary>
        /// 版面樣版Html
        /// </summary>
        public string Tmpl { get; set; }

        /// <summary>
        /// 預覽網址
        /// </summary>
        public string PreviewUrl { get; set; }

        /// <summary>
        /// 圖片大小限制KB
        /// </summary>
        public int ImgMaxSize { get; set; }

        /// <summary>
        /// 小標題最多字數
        /// </summary>
        public int StxtMax { get; set; }

        /// <summary>
        /// 小圖片大小限制KB
        /// </summary>
        public int SimgMaxSize { get; set; }

        /// <summary>
        /// 廣告內文最多字數
        /// </summary>
        public int ContMax { get; set; }

        /// <summary>
        /// CSS ID
        /// </summary>
        public string CssId { get; set; }
    }
}