using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.NewsMediaRepoAdapters.Interface;

namespace TWNewEgg.NewsMediaRepoAdapters
{
    public class NewsMediaRepoAdapter : INewsMediaRepoAdapter
    {
        private IRepository<NewsInfo> _newsdata;
        private IRepository<MediaInfo> _mediadata;
        public NewsMediaRepoAdapter(IRepository<NewsInfo> newsdata, IRepository<MediaInfo> mediadata)
        {
            this._newsdata = newsdata;
            this._mediadata = mediadata;
        }

        public IQueryable<NewsInfo> News_GetAll()
        {
            return this._newsdata.GetAll();
        }

        public IQueryable<MediaInfo> Media_GetAll()
        {
            return this._mediadata.GetAll();
        }
    }
}
