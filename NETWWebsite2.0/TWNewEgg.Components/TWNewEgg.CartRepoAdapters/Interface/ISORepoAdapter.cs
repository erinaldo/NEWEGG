using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface ISORepoAdapter
    {
        SalesOrderGroup GetSOGroup(int soGroupId);
        IQueryable<SalesOrderItem> GetSOItems(string SOCode);
        SalesOrder GetSO(string SOCode);
        IQueryable<SalesOrder> GetSOs(IEnumerable<string> soCodes);
        IQueryable<SalesOrder> GetSOs(int soGroupId);
        IQueryable<SalesOrderItem> GetSOItemsByCodes(IEnumerable<string> salesOrderCodes);
        IQueryable<SalesOrderItem> GetSOItemsByCodeList(List<string> salesOrderCodes);
        void UpdateSO(SalesOrder SO);
        bool UpdateRangeSOItem(List<SalesOrderItem> argListSoItem);
        IQueryable<SalesOrder> GetSOAllData();
        IQueryable<SalesOrderItem> GetAllForSaleOrderItem();
    }
}
