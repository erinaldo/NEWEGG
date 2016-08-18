using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.Models.DomainModels.Media
{
    /// <summary>
    /// 提供給媒體報導頁面的主要資料型態.
    /// </summary>
    public class MediaInfo
    {
        /// <summary>
        /// 媒體的編號.
        /// </summary>
        public int MediaID { get; set; }

        /// <summary>
        /// 媒體的內容類別.
        /// </summary>
        public int Type { get; set; }
        
        /// <summary>
        /// 媒體的標題.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 媒體的日期.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 媒體的快照圖片位址.
        /// </summary>
        public String SnapshotPath { get; set; }

        /// <summary>
        /// 媒體的 1.圖片位址 2.影片位址
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// 媒體的 圖片連結.
        /// </summary>
        public String LinkUrl { get; set; }

        /// <summary>
        /// 上一則媒體的編號.
        /// </summary>
        public int PrevID { get; set; }

        /// <summary>
        /// 下一則媒體的編號.
        /// </summary>
        public int NextID { get; set; }

    }
}
