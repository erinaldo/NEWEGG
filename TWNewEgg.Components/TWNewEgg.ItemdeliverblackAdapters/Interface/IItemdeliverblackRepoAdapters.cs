using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemdeliverblackAdapters.Interface
{
    public interface IItemdeliverblackRepoAdapters
    {
        void Create(ItemDeliverBlack model);
        IQueryable<ItemDeliverBlack> GetAll();
        void Update(ItemDeliverBlack model);
    }
}
