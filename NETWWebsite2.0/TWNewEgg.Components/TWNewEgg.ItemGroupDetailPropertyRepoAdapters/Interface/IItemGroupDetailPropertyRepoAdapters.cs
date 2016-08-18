using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemGroupDetailPropertyRepoAdapters.Interface
{
    public interface IItemGroupDetailPropertyRepoAdapters
    {
        IQueryable<ItemGroupDetailProperty> GetAll();
    }
}
