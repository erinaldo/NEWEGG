using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemDeliverWhiteRepoAdapters.Interface
{
    public interface IItemDeliverWhiteRepoAdapters
    {
        void Create(ItemDeliverWhite model);
        IQueryable<ItemDeliverWhite> GetAll();
    }
}
