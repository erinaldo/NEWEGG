using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.News;
using TWNewEgg.Models.DomainModels.Media;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class NewsController : Controller
    {
        /// <summary>
        /// 訪問新聞中心列表.
        /// </summary>
        /// <returns>新聞列表頁面.</returns>
        public ActionResult Index(Nullable<int> pageIndex)
        {
            NewsListInfo viewModel = null;

            try
            {
                viewModel = Processor.Request<NewsListInfo, NewsListInfo>("NewsService", "GetNewsList", pageIndex == null ? 1 : pageIndex).results;
            }
            catch (Exception)
            {
                ////Cannot fetch data from NewsService, 
            }

            return View(viewModel);
        }

        /// <summary>
        /// 訪問新聞內容.
        /// </summary>
        /// <param name="newsID">新聞訊息序號.</param>
        /// <returns>新聞詳情頁面.</returns>
        public ActionResult Content(Nullable<int> newsID = 0)
        {
            if (newsID <= 0 || newsID == null)
            {
                return RedirectToAction("Index");
            }
            ////GetNewsInfo
            NewsInfo viewModel = null;
            try
            {
                viewModel = Processor.Request<NewsInfo, NewsInfo>("NewsService", "GetNewsInfo", newsID).results;
            }
            catch (Exception)
            {
                ////Cannot fetch data from NewsService, 
            }

            if (viewModel == null)
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
        /// <summary>
        /// 訪問媒體中心.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public ActionResult Media(Nullable<int> pageIndex)
        {
            MediaListInfo viewModel = new MediaListInfo();
            viewModel = Processor.Request<MediaListInfo, MediaListInfo>("MediaService", "GetMediaList", pageIndex).results;
            #region HardCode
            /*
            MediaListInfo viewModel = new MediaListInfo();
            viewModel.CurrentPage = pageIndex == null || pageIndex < 0 ? 1 : (int)pageIndex;
            viewModel.PageCount = 5;
            viewModel.MediaList = new List<MediaInfo>();

            MediaInfo media1 = new MediaInfo();
            media1.Date = DateTime.Now;
            media1.LinkUrl = "http://google.com.tw";
            media1.MediaID = 1;
            media1.NextID = 2;
            media1.Path = "http://img.ltn.com.tw/Upload/liveNews/BigPic/600_phpdiEGEJ.jpg";
            media1.PrevID = 0;
            media1.SnapshotPath = "/Themes/img/img6.jpg";
            media1.Title = "堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善";
            media1.Type = 1;

            MediaInfo media2 = new MediaInfo();
            media2.Date = DateTime.Now;
            media2.LinkUrl = "http://yahoo.com.tw";
            media2.MediaID = 2;
            media2.NextID = 3;
            media2.Path = "http://cncdn.gospelherald.com/data/images/full/22714/facegloria-jpg.jpg?w=720";
            media2.PrevID = 1;
            media2.SnapshotPath = "/Themes/img/img6.jpg";
            media2.Title = "堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善";
            media2.Type = 1;

            MediaInfo media3 = new MediaInfo();
            media3.Date = DateTime.Now;
            media3.LinkUrl = "http://newegg.com";
            media3.MediaID = 3;
            media3.NextID = 4;
            media3.Path = "http://www.youtube.com/watch?v=09R8_2nJtjg";
            media3.PrevID = 2;
            media3.SnapshotPath = "/Themes/img/img6.jpg";
            media3.Title = "堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善土地靠智慧堅持無毒種紅豆友善";
            media3.Type = 2;


            viewModel.MediaList.Add(media1);
            viewModel.MediaList.Add(media2);
            viewModel.MediaList.Add(media3);
            viewModel.MediaList.Add(media1);
            viewModel.MediaList.Add(media2);
            viewModel.MediaList.Add(media3);
            */
            #endregion
            return View(viewModel);
        }

    }
}
