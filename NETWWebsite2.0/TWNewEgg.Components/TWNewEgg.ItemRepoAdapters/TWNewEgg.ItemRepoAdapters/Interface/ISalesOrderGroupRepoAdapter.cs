using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface ISalesOrderGroupRepoAdapter
    {
        SalesOrderGroup Get(int salesOrderGroupId);
    }
}
