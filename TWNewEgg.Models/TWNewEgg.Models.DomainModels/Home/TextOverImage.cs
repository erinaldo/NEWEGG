using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.Home
{
    /// <summary>
    /// 文字疊圖, 因為未來可能會再擴充, 故資料結構設計的較為彈性.
    /// </summary>
    public class TextOverImage
    {
        /// <summary>
        /// 決定型態, 要套哪個對應CSS的模組.
        /// </summary>
        public int LayoutNumber { get; set; }

        /// <summary>
        /// 可存放多張圖檔的連結.
        /// </summary>
        public List<StoreBanner> ImageList { get; set; }

        /// <summary>
        /// 可存放多個文字連結.
        /// </summary>
        public List<GroupLink> TextLinkList { get; set; }
    }
}
