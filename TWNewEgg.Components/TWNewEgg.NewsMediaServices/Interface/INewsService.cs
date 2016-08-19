using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.News;

namespace TWNewEgg.NewsMediaServices.Interface
{
    public interface INewsService
    {
        /// <summary>
        /// 取得新聞中心-主頁中，某頁所有的資訊
        /// </summary>
        /// <param name="Page">頁碼</param>
        /// <returns>回傳新聞中心的主頁資訊</returns>
        NewsListInfo GetNewsList(int Page);

        /// <summary>
        /// 取得新聞中心的新聞全部資料
        /// </summary>
        /// <param name="iNewsID">新聞編號</param>
        /// <returns>回傳新聞中心的內頁資訊</returns>
        NewsInfo GetNewsInfo(int iNewsID);
    }
}
