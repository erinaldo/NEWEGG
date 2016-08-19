using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels;

namespace TWNewEgg.Models.DomainModels.News
{
    /// <summary>
    /// 提供新聞中心主頁面的資料
    /// </summary>
    public class NewsListInfo
    {
        /// <summary>
        /// 新聞中心的總頁數
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 新聞中心的目前顯示的頁碼
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// 新聞中心的新聞清單資料
        /// </summary>
        public List<NewsInfo> NewsList { get; set; }
    }
}
