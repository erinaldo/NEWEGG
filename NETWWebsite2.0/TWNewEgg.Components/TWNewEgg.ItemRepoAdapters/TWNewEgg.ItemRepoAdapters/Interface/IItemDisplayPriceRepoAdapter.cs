using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IItemDisplayPriceRepoAdapter
    {
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> GetItemDisplayPriceList(List<int> itemIDList);
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemDisplayPrice> GetAll();
    }
}
