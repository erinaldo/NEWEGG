using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Media;
using TWNewEgg.NewsMediaRepoAdapters;
using TWNewEgg.NewsMediaRepoAdapters.Interface;
using TWNewEgg.NewsMediaServices.Interface;

namespace TWNewEgg.NewsMediaServices
{
    public class MediaService : IMediaService
    {
        private INewsMediaRepoAdapter _newsMediaRepoAdapter;
        int pageSize = 6;
        public MediaService(INewsMediaRepoAdapter newsMediaRepoAdapter)
        {
            this._newsMediaRepoAdapter = newsMediaRepoAdapter;
        }
        public MediaListInfo GetMediaList(int Page)
        {
            if (Page <= 0) Page = 1;
            var query = this._newsMediaRepoAdapter.Media_GetAll().Where(x => x.ShowAll == 1);
            var page = query.OrderByDescending(x => x.LaunchDate)
                       .Select(x => new MediaInfo
                        {
                            MediaID = x.ID,
                            Title = x.Title,
                            Date = x.LaunchDate,
                            LinkUrl = x.Displaypath,
                            Type = x.Displaytype,
                            SnapshotPath = x.Snapshotpath,
                            Path = x.Clickpath,
                            NextID = 0,
                            PrevID = 0
                        })
                       .Skip((Page - 1) * pageSize).Take(pageSize)
                       .GroupBy(x=>new {Total=query.Count()})
                       .FirstOrDefault();
            return new MediaListInfo
            {
                PageCount = (page.Key.Total - 1) / pageSize + 1,
                CurrentPage = Page,
                MediaList = page.Select(x=>x).ToList()
            };
        }
        public MediaInfo GetMediaInfo(int iMediaID)
        {
            var rtn = this._newsMediaRepoAdapter.Media_GetAll().Where(x => x.ID == iMediaID).FirstOrDefault();
            if (rtn == null)
            {
                return null;
            }
            List<int> IDList = this._newsMediaRepoAdapter.Media_GetAll().Where(x => x.ShowAll == 1).OrderByDescending(x => x.LaunchDate).Select(x => x.ID).ToList();
            int index = IDList.IndexOf(rtn.ID);
            int iNext = index + 1;
            int iPrev = index - 1;
            if (iNext > IDList.Count -1) iNext = -1;
            if (iPrev < 0) iPrev = -1;
            return new MediaInfo
            {
                MediaID = iMediaID,
                Title = rtn.Title,
                Date = rtn.LaunchDate,
                LinkUrl = rtn.Displaypath,
                Type = rtn.Displaytype,
                SnapshotPath = rtn.Snapshotpath,
                Path = rtn.Clickpath,
                NextID = iNext >= 0 ? IDList[iNext] : -1,
                PrevID = iPrev >= 0 ? IDList[iPrev] : -1
            };

        }
    }
}
