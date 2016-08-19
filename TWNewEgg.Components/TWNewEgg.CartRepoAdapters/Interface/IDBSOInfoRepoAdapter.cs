using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface IDBSOInfoRepoAdapter
    {
        DbSOInfo GetDBSOInfo(string SOCode);
    }
}
