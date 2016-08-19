using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.SalesOrderRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.SalesOrderRepoAdapters
{
    public class SalesOrderRepoAdapters : ISalesOrderRepoAdapters
    {
        IRepository<SalesOrder> _salesOrder;
        public SalesOrderRepoAdapters(IRepository<SalesOrder> salesOrder)
        {
            this._salesOrder = salesOrder;
        }
        #region 建立
        public void CreateSalesOrder(SalesOrder _salesOrderInsertModel)
        {
            this._salesOrder.Create(_salesOrderInsertModel);
        }
        #endregion
        #region 更新
        public void UpdateSalesOder(SalesOrder _salesOrderUpdateModel)
        {
            this._salesOrder.Update(_salesOrderUpdateModel);
        }
        #endregion
        #region 刪除
        public void DeleteSalesOrder(SalesOrder _salesOrderDeleteModel)
        {
            this._salesOrder.Delete(_salesOrderDeleteModel);
        }
        #endregion
        #region 根據某些條件做查詢
        public IQueryable<SalesOrder> GetAll()
        {
            return this._salesOrder.GetAll();
        }

        public SalesOrder GetSpecificSoBySoCode(string soCode)
        {
            var soItemiQuery = this._salesOrder.Get(p => p.Code == soCode);
            return soItemiQuery;
        }

        public IQueryable<SalesOrder> GetSoByListSoCode(IEnumerable<string> soCode)
        {
            var salesOrders = this._salesOrder.GetAll().Where(x => soCode.Contains(x.Code));
            return salesOrders;
        }

        public IQueryable<SalesOrder> GetSoBySalesOrderGroupId(int salesOrderGroupId)
        {
            var salesOrders = this._salesOrder.GetAll().Where(x => x.SalesOrderGroupID == salesOrderGroupId);
            return salesOrders;
        }
        #endregion


        public void CreateSalesOrders(IEnumerable<SalesOrder> _salesOrderInsertModel)
        {
            this._salesOrder.CreateMany(_salesOrderInsertModel);
        }
    }
}
