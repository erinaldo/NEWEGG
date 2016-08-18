using System.Collections.Generic;
using System.Linq;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.SalesOrderRepoAdapters.Interface
{
    public interface ISalesOrderRepoAdapters
    {
        void CreateSalesOrder(SalesOrder _salesOrderInsertModel);
        void CreateSalesOrders(IEnumerable<SalesOrder> _salesOrderInsertModel);
        void UpdateSalesOder(SalesOrder _salesOrderUpdateModel);
        void DeleteSalesOrder(SalesOrder _salesOrderDeleteModel);
        IQueryable<SalesOrder> GetAll();
        SalesOrder GetSpecificSoBySoCode(string soCode);
        IQueryable<SalesOrder> GetSoByListSoCode(IEnumerable<string> soCode);
        IQueryable<SalesOrder> GetSoBySalesOrderGroupId(int salesOrderGroupId);
    }
}
