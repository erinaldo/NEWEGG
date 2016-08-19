using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface IRetgoodRepoAdapter
    {
        IQueryable<Retgood> GetAll();
        Retgood GetRetgood(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM);
        Retgood GetRetgood(string Code);
        Retgood GetRetgoodbyProcessID(string ProcessId);
        Retgood Update(Retgood retgood);
    }
}
