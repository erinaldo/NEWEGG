using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IItemcategoryRepoAdapter
    {
        IQueryable<ItemCategory> GetAll();
        void DeteleMany(List<ItemCategory> model);
        void CreateMany(List<ItemCategory> model);
    }
}
