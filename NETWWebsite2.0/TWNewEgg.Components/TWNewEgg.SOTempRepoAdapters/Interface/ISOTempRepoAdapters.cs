using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.SOTempRepoAdapters.Interface
{
    public interface ISOTempRepoAdapters
    {
        void insertToSalesOrderGroupTemp(SalesOrderGroupTemp model);
        void insertToSalesOrderListTemp(List<SalesOrderTemp> model);
        void insertToSalesOrderItemListTemp(List<SalesOrderItemTemp> model);
        IQueryable<SalesOrderTemp> GetAllSalesOrderTempData();
        IQueryable<SalesOrderItemTemp> GetAllSalesOrderItemTempData();
        IQueryable<SalesOrderGroupTemp> GetAllSalesOrderGroupTemp();
    }
}
