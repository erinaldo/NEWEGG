using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.MobileStoreRepoAdapters.Interface
{
    public interface IMobileStoreRepoAdapter
    {
        IQueryable<ItemForChoice> ItemForChoice_GetAll();
        IQueryable<GroupBuy> GroupBuy_GetAll();
    }
}
