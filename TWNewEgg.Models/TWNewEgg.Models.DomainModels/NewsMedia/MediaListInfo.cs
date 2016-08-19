using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.Models.DomainModels.Media
{
    /// <summary>
    /// 提供媒體報導主頁面的資料
    /// </summary>
    public class MediaListInfo
    {
        /// <summary>
        /// 媒體報導的總頁數
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 媒體報導的目前顯示的頁碼
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 媒體報導的媒體清單資料
        /// </summary>
        public List<MediaInfo> MediaList { get; set; }
    }
}
