using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.SalesOrderRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.SalesOrderRepoAdapters
{
    public class SalesOrderTempRepoAdapter : ISalesOrderRepoAdapters
    {
        private IRepository<SalesOrderTemp> _salesOrderTemp;
        private IRepository<SalesOrder> _salesOrder;
        public SalesOrderTempRepoAdapter(IRepository<SalesOrderTemp> salesOrderTemp, IRepository<SalesOrder> salesOrder)
        {
            this._salesOrderTemp = salesOrderTemp;
            this._salesOrder = salesOrder;
        }
        #region 建立
        public void CreateSalesOrder(SalesOrder _salesOrderInsertModel)
        {
            SalesOrderTemp temp = ModelConverter.ConvertTo<SalesOrderTemp>(_salesOrderInsertModel);
            this._salesOrderTemp.Create(temp);
        }
        #endregion
        #region 更新
        public void UpdateSalesOder(SalesOrder _salesOrderUpdateModel)
        {
            SalesOrderTemp temp = ModelConverter.ConvertTo<SalesOrderTemp>(_salesOrderUpdateModel);
            this._salesOrderTemp.Update(temp);
        }
        #endregion
        #region 刪除
        public void DeleteSalesOrder(SalesOrder _salesOrderDeleteModel)
        {
            SalesOrderTemp temp = ModelConverter.ConvertTo<SalesOrderTemp>(_salesOrderDeleteModel);
            this._salesOrderTemp.Delete(temp);
        }
        #endregion
        #region 根據某些條件做查詢
        public IQueryable<SalesOrder> GetAll()
        {
            return this._salesOrder.GetAll();
        }

        public SalesOrder GetSpecificSoBySoCode(string soCode)
        {
            SalesOrderTemp temp = this._salesOrderTemp.Get(x => x.Code == soCode);
            SalesOrder so = ModelConverter.ConvertTo<SalesOrder>(temp);
            return so;
        }

        public IQueryable<SalesOrder> GetSoByListSoCode(IEnumerable<string> soCode)
        {
            return this._salesOrder.GetAll().Where(x => soCode.Contains(x.Code));
        }

        public IQueryable<SalesOrder> GetSoBySalesOrderGroupId(int salesOrderGroupId)
        {
            return this._salesOrder.GetAll().Where(x => x.SalesOrderGroupID == salesOrderGroupId);
        }
        #endregion

        public void CreateSalesOrders(IEnumerable<SalesOrder> _salesOrderInsertModel)
        {
            IEnumerable<SalesOrderTemp> soTemps = ModelConverter.ConvertTo<IEnumerable<SalesOrderTemp>>(_salesOrderInsertModel);
            this._salesOrderTemp.CreateMany(soTemps);
        }
    }
}
