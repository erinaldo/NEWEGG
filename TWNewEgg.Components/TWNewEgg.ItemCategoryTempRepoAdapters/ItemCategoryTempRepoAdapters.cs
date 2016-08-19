using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemCategoryTempRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemCategoryTempRepoAdapters
{
    public class ItemCategoryTempRepoAdapters : IItemCategoryTempRepoAdapters
    {
        private IRepository<ItemCategoryTemp> _itemCategoryTemp;
        public ItemCategoryTempRepoAdapters(IRepository<ItemCategoryTemp> itemCategoryTemp)
        {
            this._itemCategoryTemp = itemCategoryTemp;
        }
        public IQueryable<ItemCategoryTemp> GetAll()
        {
            return this._itemCategoryTemp.GetAll();
        }
        public void DeleteMany(List<ItemCategoryTemp> model)
        {
            foreach (var item in model)
            {
                this._itemCategoryTemp.Delete(item);
            }
        }

        public void CreateMany(List<ItemCategoryTemp> model)
        {
            this._itemCategoryTemp.CreateMany(model);
        }


        public void Delete(ItemCategoryTemp argItemCategoryTemp)
        {
            this._itemCategoryTemp.Delete(argItemCategoryTemp);
        }


        public void Create(ItemCategoryTemp argItemCategoryTemp)
        {
            this._itemCategoryTemp.Create(argItemCategoryTemp);
        }
    }
}
