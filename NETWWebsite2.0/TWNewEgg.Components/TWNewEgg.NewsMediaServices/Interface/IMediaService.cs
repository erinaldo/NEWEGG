using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Media;

namespace TWNewEgg.NewsMediaServices.Interface
{
    public interface IMediaService
    {
        /// <summary>
        /// 取得媒體報導-主頁中，某頁所有的資訊
        /// </summary>
        /// <param name="Page">頁碼</param>
        /// <returns>回傳媒體報導的主頁資訊</returns>
        MediaListInfo GetMediaList(int Page);

        /// <summary>
        /// 取得媒體報導的媒體全部資料
        /// </summary>
        /// <param name="iMediaID">媒體編號</param>
        /// <returns>回傳媒體報導的內頁資訊</returns>
        MediaInfo GetMediaInfo(int iMediaID);
    }
}
