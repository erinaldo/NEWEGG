using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters
{
    public class ItemcategoryRepoAdapter : IItemcategoryRepoAdapter
    {
        private IRepository<ItemCategory> _itemCategoryRepo;
        public ItemcategoryRepoAdapter(IRepository<ItemCategory> itemCategoryRepo)
        {
            this._itemCategoryRepo = itemCategoryRepo;
        }

        public IQueryable<ItemCategory> GetAll()
        {
            return _itemCategoryRepo.GetAll();
        }

        public void DeteleMany(List<ItemCategory> model)
        {
            foreach (var item in model)
            {
                this._itemCategoryRepo.Delete(item);
            }
        }

        public void CreateMany(List<ItemCategory> model)
        {
            this._itemCategoryRepo.CreateMany(model);
        }
    }
}
