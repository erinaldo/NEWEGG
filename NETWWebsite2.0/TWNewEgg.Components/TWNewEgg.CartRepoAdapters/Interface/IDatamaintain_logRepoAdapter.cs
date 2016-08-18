using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface IDatamaintain_logRepoAdapter
    {
        Datamaintain_log Create(Datamaintain_log Datamaintain_log);
    }
}
