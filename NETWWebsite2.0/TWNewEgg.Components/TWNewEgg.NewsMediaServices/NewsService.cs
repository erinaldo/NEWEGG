using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.News;
using TWNewEgg.NewsMediaRepoAdapters;
using TWNewEgg.NewsMediaRepoAdapters.Interface;
using TWNewEgg.NewsMediaServices.Const;
using TWNewEgg.NewsMediaServices.Interface;

namespace TWNewEgg.NewsMediaServices
{
    public class NewsService : INewsService
    {
        private INewsMediaRepoAdapter _newsMediaRepoAdapter;
        private int pageSize = 10;

        public NewsService(INewsMediaRepoAdapter iNewsMediaRepoAdapter)
        {
            this._newsMediaRepoAdapter = iNewsMediaRepoAdapter;
        }

        public NewsListInfo GetNewsList(int Page)
        {
            if (Page <= 0) Page = 1;
            String patHtml = @"<.*?>";
            List<TWNewEgg.Models.DBModels.TWSQLDB.NewsInfo> dbNewsList = this._newsMediaRepoAdapter.News_GetAll().Where(t => t.ShowAll == 1).OrderByDescending(x => x.LaunchDate).ThenBy(x => x.Showorder).ToList();
            List<NewsInfo> lNewsList = new List<NewsInfo>();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.NewsInfo x in dbNewsList.Skip((Page - 1) * pageSize).Take(pageSize))
            {
                lNewsList.Add(new NewsInfo {
                    NewsID = x.ID,
                    Title = x.Title,
                    Date = x.LaunchDate,
                    Extract = Regex.Replace(x.Contents, patHtml, string.Empty, RegexOptions.Multiline),
                    StyleType = x.Showorder == 0 ? ConstStyleType.ImageRight : ConstStyleType.ImageLeft,
                    ImagePath = x.Imagepath,
                    NextID = 0,
                    PrevID = 0
                });
            }
            
            NewsListInfo retNews = new NewsListInfo
            {
                PageCount = (dbNewsList.Count() - 1) / pageSize + 1,
                CurrentPage = Page,
                NewsList = lNewsList,
            };
            return retNews;
        }

        public NewsInfo GetNewsInfo(int iNewsID)
        {
            var rtn = this._newsMediaRepoAdapter.News_GetAll().Where(x => x.ID == iNewsID).FirstOrDefault();
            if (rtn == null)
            {
                return null;
            }
            List<int> IDList = this._newsMediaRepoAdapter.News_GetAll().Where(x => x.ShowAll == 1).OrderByDescending(x => x.LaunchDate).Select(x => x.ID).ToList();
            int index = IDList.IndexOf(rtn.ID);
            int iNext = index + 1;
            int iPrev = index - 1;
            if (iNext > IDList.Count - 1) iNext = -1;
            if (iPrev < 0) iPrev = -1;
            return new NewsInfo
            {
                NewsID = iNewsID,
                Title = rtn.Title,
                Date = rtn.LaunchDate,
                Contents= rtn.Contents,
                StyleType = rtn.Showorder == 0 ? ConstStyleType.ImageRight : ConstStyleType.ImageLeft,
                ImagePath = rtn.Imagepath,
                NextID = iNext >= 0 ? IDList[iNext] : -1,
                PrevID = iPrev >= 0 ? IDList[iPrev] : -1
            };

        }

    }
}
