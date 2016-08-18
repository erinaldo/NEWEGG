using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.SalesOrderItemRepoAdapters.Interface
{
    public interface ISalesOrderItemRepoAdapters
    {
        void CreateSalesOrderItems(IEnumerable<SalesOrderItem> saleOrderItemInsertModel);
        void CreateSalesOrderItem(SalesOrderItem saleOrderItemInsertModel);
        void UpdateSalesOrderItem(SalesOrderItem saleOrderItemUpdateModel);
        void DeleteSalesOrderItem(SalesOrderItem saleOrderItemDeleteModel);
        IQueryable<SalesOrderItem> GetAll();
        IQueryable<SalesOrderItem> GetSalesOrderItemListBySalesOrderCode(List<string> salesOrderCode);
        SalesOrderItem GetSalesOrderItemByCode(string code);
    }
}
