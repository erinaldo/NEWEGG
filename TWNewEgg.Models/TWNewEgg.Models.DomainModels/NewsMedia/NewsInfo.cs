using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.Models.DomainModels.News
{
    /// <summary>
    /// 提供給新聞頁面的主要資料型態.
    /// </summary>
    public class NewsInfo
    {
        /// <summary>
        /// 新聞的編號.
        /// </summary>
        public int NewsID { get; set; }

        /// <summary>
        /// 新聞的標題.
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 新聞的日期.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 新聞的摘錄.
        /// </summary>
        public String Extract { get; set; }

        /// <summary>
        /// 新聞的內容.
        /// </summary>
        public String Contents { get; set; }

        /// <summary>
        /// 新聞的樣板 1-靠右,2-靠左
        /// </summary>
        public int DisplayType { get; set; }

        /// <summary>
        /// 新聞的樣板.
        /// </summary>
        public String StyleType { get; set; }

        /// <summary>
        /// 新聞的圖片.
        /// </summary>
        public String ImagePath { get; set; }

        /// <summary>
        /// 上一則新聞的編號.
        /// </summary>
        public int PrevID { get; set; }

        /// <summary>
        /// 下一則新聞的編號.
        /// </summary>
        public int NextID { get; set; }
    }
}
