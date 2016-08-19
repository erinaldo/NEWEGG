using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;

namespace TWNewEgg.StoredProceduresRepoAdapters.Interface
{
    public interface IStoredProceduresRepoAdapter
    {
        Dictionary<string, List<ViewTracksCartItems>> GetShoppingAllCart(int accountId);
    }
}
