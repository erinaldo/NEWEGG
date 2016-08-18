using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemCategoryTempRepoAdapters.Interface
{
    public interface IItemCategoryTempRepoAdapters
    {
        IQueryable<ItemCategoryTemp> GetAll();
        void DeleteMany(List<ItemCategoryTemp> model);
        void CreateMany(List<ItemCategoryTemp> model);
        void Delete(ItemCategoryTemp argItemCategoryTemp);
        void Create(ItemCategoryTemp argItemCategoryTemp);
    }
}
