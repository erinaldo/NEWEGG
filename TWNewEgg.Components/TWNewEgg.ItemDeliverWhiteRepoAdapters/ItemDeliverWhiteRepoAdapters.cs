using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemDeliverWhiteRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemDeliverWhiteRepoAdapters
{
    public class ItemDeliverWhiteRepoAdapters : IItemDeliverWhiteRepoAdapters
    {
        private IRepository<ItemDeliverWhite> _itemDeliverWhite;

        public ItemDeliverWhiteRepoAdapters(IRepository<ItemDeliverWhite> itemDeliverWhite)
        {
            this._itemDeliverWhite = itemDeliverWhite;
        }

        public void Create(ItemDeliverWhite model)
        {
            this._itemDeliverWhite.Create(model);
        }

        public IQueryable<ItemDeliverWhite> GetAll()
        {
            return this._itemDeliverWhite.GetAll();
        }
    }
}
