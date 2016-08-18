using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.SalesOrderItemRepoAdapters.Interface;

namespace TWNewEgg.SalesOrderItemRepoAdapters
{
    public class SalesOrderItemTempRepoAdapters : ISalesOrderItemRepoAdapters
    {
        private IRepository<SalesOrderItem> _salesOrderItem;
        private IRepository<SalesOrderItemTemp> _salesOrderItemTemp;
        public SalesOrderItemTempRepoAdapters(IRepository<SalesOrderItem> salesOrderItem, IRepository<SalesOrderItemTemp> salesOrderItemTemp)
        {
            this._salesOrderItem = salesOrderItem;
            this._salesOrderItemTemp = salesOrderItemTemp;
        }

        #region  建立
        public void CreateSalesOrderItem(SalesOrderItem saleOrderItemInsertModel)
        {
            SalesOrderItemTemp temp = ModelConverter.ConvertTo<SalesOrderItemTemp>(saleOrderItemInsertModel);
            this._salesOrderItemTemp.Create(temp);
        }
        #endregion
        #region 更新
        public void UpdateSalesOrderItem(SalesOrderItem saleOrderItemUpdateModel)
        {
            SalesOrderItemTemp temp = ModelConverter.ConvertTo<SalesOrderItemTemp>(saleOrderItemUpdateModel);
            this._salesOrderItemTemp.Update(temp);
        }
        #endregion
        #region 刪除
        public void DeleteSalesOrderItem(SalesOrderItem soItemTempsaleOrderItemDeleteModel)
        {
            SalesOrderItemTemp temp = ModelConverter.ConvertTo<SalesOrderItemTemp>(soItemTempsaleOrderItemDeleteModel);
            this._salesOrderItemTemp.Delete(temp);
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
            SalesOrderItemTemp temp = this._salesOrderItemTemp.Get(x => x.Code == code);
            SalesOrderItem item = ModelConverter.ConvertTo<SalesOrderItem>(temp);
            return item;
        }
        #endregion

        public void CreateSalesOrderItems(IEnumerable<SalesOrderItem> saleOrderItemInsertModel)
        {
            IEnumerable<SalesOrderItemTemp> temp = ModelConverter.ConvertTo<IEnumerable<SalesOrderItemTemp>>(saleOrderItemInsertModel);
            this._salesOrderItemTemp.CreateMany(temp);
        }
    }
}
