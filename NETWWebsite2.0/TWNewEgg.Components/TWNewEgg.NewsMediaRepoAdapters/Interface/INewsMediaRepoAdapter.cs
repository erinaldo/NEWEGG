using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.NewsMediaRepoAdapters.Interface
{
    public interface INewsMediaRepoAdapter
    {
        IQueryable<NewsInfo> News_GetAll();
        IQueryable<MediaInfo> Media_GetAll();
    }
}
