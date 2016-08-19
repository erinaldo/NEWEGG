using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IDbItemInfoRepoAdapter
    {
        IQueryable<DbItemInfo> GetDbItemInfos();
    }
}
