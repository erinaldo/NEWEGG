using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.SalesOrderItemRepoAdapters.Interface;

namespace TWNewEgg.SalesOrderItemRepoAdapters
{
    public class SalesOrderItemRepoAdapters : ISalesOrderItemRepoAdapters
    {
        IRepository<SalesOrderItem> _salesOrderItem;
        public SalesOrderItemRepoAdapters(IRepository<SalesOrderItem> salesOrderItem)
        {
            this._salesOrderItem = salesOrderItem;
        }

        #region  建立
        public void CreateSalesOrderItem(SalesOrderItem saleOrderItemInsertModel)
        {
            this._salesOrderItem.Create(saleOrderItemInsertModel);
        }
        #endregion
        #region 更新
        public void UpdateSalesOrderItem(SalesOrderItem saleOrderItemUpdateModel)
        {
            this._salesOrderItem.Update(saleOrderItemUpdateModel);
        }
        #endregion
        #region 刪除
        public void DeleteSalesOrderItem(SalesOrderItem saleOrderItemDeleteModel)
        {
            this._salesOrderItem.Delete(saleOrderItemDeleteModel);
        }
        #endregion
        #region 根據各種條件做查詢
        public IQueryable<SalesOrderItem> GetAll()
        {
            return this._salesOrderItem.GetAll();
        }
        public IQueryable<SalesOrderItem> GetSalesOrderItemListBySalesOrderCode(List<string> salesOrderCode)
        {
            return this._salesOrderItem.GetAll().Where(x => salesOrderCode.Contains(x.SalesorderCode));
        }
        public SalesOrderItem GetSalesOrderItemByCode(string code)
        {
            return this._salesOrderItem.Get(p => p.Code == code);
        }
        #endregion
        public void CreateSalesOrderItems(IEnumerable<SalesOrderItem> saleOrderItemInsertModel)
        {
            this._salesOrderItem.CreateMany(saleOrderItemInsertModel);
        }
    }
}
