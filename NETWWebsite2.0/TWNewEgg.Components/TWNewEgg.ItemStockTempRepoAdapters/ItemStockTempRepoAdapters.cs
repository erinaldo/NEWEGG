using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemStockTempRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemStockTempRepoAdapters
{
    public class ItemStockTempRepoAdapters : IItemStockTempRepoAdapters
    {
        private IRepository<ItemStocktemp> _itemStocktemp;

        public ItemStockTempRepoAdapters(IRepository<ItemStocktemp> itemStocktemp)
        {
            this._itemStocktemp = itemStocktemp;
        }

        public IQueryable<ItemStocktemp> GetAll()
        {
            return this._itemStocktemp.GetAll();
        }
        public void Update(ItemStocktemp model)
        {
            this._itemStocktemp.Update(model);
        }
    }
}
