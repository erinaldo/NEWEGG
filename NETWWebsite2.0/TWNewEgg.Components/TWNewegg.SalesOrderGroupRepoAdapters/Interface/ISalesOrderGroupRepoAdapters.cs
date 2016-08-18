using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace salesordergroup.Interface
{
    public interface ISalesOrderGroupRepoAdapters
    {
        void insertSalesordergroup(SalesOrderGroup _salesOrderGroupModel);
        IQueryable<SalesOrderGroup> GetAll();

        bool isConnected();
    }
}
