using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemStockTempRepoAdapters.Interface
{
    public interface IItemStockTempRepoAdapters
    {
        IQueryable<ItemStocktemp> GetAll();
        void Update(ItemStocktemp model);
    }
}
