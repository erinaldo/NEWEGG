using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.NCCCRepoAdapters.Interface
{
    public interface INCCCRepoAdapter
    {
        NCCCTrans Create(NCCCTrans trans);
        NCCCTrans Update(NCCCTrans trans);
        NCCCTrans Get(string orderId);
    }
}
