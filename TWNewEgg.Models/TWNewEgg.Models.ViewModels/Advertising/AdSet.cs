using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Advertising
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

        [DisplayName("廣告代號")]
        public string Code { get; set; }
        [DisplayName("版位CSS名稱")]
        public string Name { get; set; }
        [DisplayName("說明")]
        public string AdsetDesc { get; set; }
        [DisplayName("最多字數")]
        public int TxtMax { get; set; }
        [DisplayName("最少字數")]
        public int TxtMin { get; set; }
        [DisplayName("寬度")]
        public int Width { get; set; }
        [DisplayName("高度")]
        public int Height { get; set; }
        [DisplayName("版面型式")]
        public int Type { get; set; }
        [DisplayName("快取分鐘數")]
        public int CacheMins { get; set; }
        [DisplayName("版面樣版Html")]
        public string Tmpl { get; set; }
        [DisplayName("預覽網址")]
        public string PreviewUrl { get; set; }
        [DisplayName("圖片大小限制KB")]
        public int ImgMaxSize { get; set; }
        [DisplayName("小標題最多字數")]
        public int StxtMax { get; set; }
        [DisplayName("小圖片大小限制KB")]
        public int SimgMaxSize { get; set; }
        [DisplayName("廣告內文最多字數")]
        public int ContMax { get; set; }
        [DisplayName("CSS ID")]
        public string CssId { get; set; }
    }
}