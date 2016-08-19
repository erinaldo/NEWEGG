using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.SOTempRepoAdapters.Interface;

namespace TWNewEgg.SOTempRepoAdapters
{
    public class SOTempRepoAdapters : ISOTempRepoAdapters
    {
        IRepository<SalesOrderGroupTemp> _salesOrderGroupTemp;
        IRepository<SalesOrderTemp> _salesOrderTemp;
        IRepository<SalesOrderItemTemp> _salesOrderItemTemp;
        
        public SOTempRepoAdapters(IRepository<SalesOrderGroupTemp> salesOrderGroupTemp, IRepository<SalesOrderTemp> salesOrderTemp, IRepository<SalesOrderItemTemp> salesOrderItemTemp)
        {
            this._salesOrderGroupTemp = salesOrderGroupTemp;
            this._salesOrderTemp = salesOrderTemp;
            this._salesOrderItemTemp = salesOrderItemTemp;
        }

        public void insertToSalesOrderGroupTemp(SalesOrderGroupTemp model)
        {
            this._salesOrderGroupTemp.Create(model);
        }
        public IQueryable<SalesOrderTemp> GetAllSalesOrderTempData()
        {
            return this._salesOrderTemp.GetAll();
        }
        public void insertToSalesOrderListTemp(List<SalesOrderTemp> model)
        {
            this._salesOrderTemp.CreateMany(model);
        }
        public IQueryable<SalesOrderItemTemp> GetAllSalesOrderItemTempData()
        {
            return this._salesOrderItemTemp.GetAll();
        }
        public void insertToSalesOrderItemListTemp(List<SalesOrderItemTemp> model)
        {
            this._salesOrderItemTemp.CreateMany(model);
        }
        public IQueryable<SalesOrderGroupTemp> GetAllSalesOrderGroupTemp()
        {
            return this._salesOrderGroupTemp.GetAll();
        }

    }
}
