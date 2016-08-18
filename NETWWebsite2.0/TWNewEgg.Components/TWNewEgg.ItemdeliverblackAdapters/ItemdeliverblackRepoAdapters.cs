using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemdeliverblackAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemdeliverblackAdapters
{
    public class ItemdeliverblackRepoAdapters : IItemdeliverblackRepoAdapters
    {
        private IRepository<ItemDeliverBlack> _iItemDeliverBlack;
        public ItemdeliverblackRepoAdapters(IRepository<ItemDeliverBlack> iItemDeliverBlack)
        {
            this._iItemDeliverBlack = iItemDeliverBlack;
        }
        public void Create(ItemDeliverBlack model)
        {
            this._iItemDeliverBlack.Create(model);
        }

        public IQueryable<ItemDeliverBlack> GetAll()
        {
            return this._iItemDeliverBlack.GetAll();
        }

        public void Update(ItemDeliverBlack model)
        {
            this._iItemDeliverBlack.Update(model);
        }
    }
}
