using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemGroupDetailPropertyRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemGroupDetailPropertyRepoAdapters
{
    public class ItemGroupDetailPropertyRepoAdapters : IItemGroupDetailPropertyRepoAdapters
    {
        private IRepository<ItemGroupDetailProperty> _itemGroupDetailProperty;
        public ItemGroupDetailPropertyRepoAdapters(IRepository<ItemGroupDetailProperty> itemGroupDetailProperty)
        {
            this._itemGroupDetailProperty = itemGroupDetailProperty;
        }
        public IQueryable<ItemGroupDetailProperty> GetAll()
        {
            return this._itemGroupDetailProperty.GetAll();
        }
    }
}
