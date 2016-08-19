using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.CartRepoAdapters
{
    public class SORepoAdapter : ISORepoAdapter
    {
        private IRepository<SalesOrder> _salesOrder;
        private IRepository<SalesOrderItem> _soItem;
        private IRepository<SalesOrderGroup> _soGroup;
        public SORepoAdapter(IRepository<SalesOrder> salesOrder, IRepository<SalesOrderItem> soItem, IRepository<SalesOrderGroup> soGroup)
        {   
            this._salesOrder = salesOrder;
            this._soItem = soItem;
            this._soGroup = soGroup;
        }
        public IQueryable<SalesOrderItem> GetSOItems(string SOCode)
        {
            var SoItemIQeury = this._soItem.GetAll().Where(x => x.SalesorderCode == SOCode);
            return SoItemIQeury;
        }
        public IQueryable<SalesOrderItem> GetSOItemsByCodes(IEnumerable<string> salesOrderCodes)
        {
            return this._soItem.GetAll().Where(x => salesOrderCodes.Contains(x.SalesorderCode));
        }
        public IQueryable<SalesOrderItem> GetSOItemsByCodeList(List<string> salesOrderCodes)
        {
            return this._soItem.GetAll().Where(x => salesOrderCodes.Contains(x.SalesorderCode));
        }
        public SalesOrder GetSO(string SOCode)
        {
            var SoIQeury = this._salesOrder.Get(x => x.Code == SOCode);
            return SoIQeury;
        }

        public IQueryable<SalesOrder> GetSOs(IEnumerable<string> soCodes)
        {
            var salesOrders = this._salesOrder.GetAll().Where(x => soCodes.Contains(x.Code));
            return salesOrders;
        }

        public IQueryable<SalesOrder> GetSOs(int soGroupId)
        {
            var salesOrders = this._salesOrder.GetAll().Where(x => x.SalesOrderGroupID == soGroupId);
            return salesOrders;
        }

        public SalesOrderGroup GetSOGroup(int soGroupId)
        {
            var soGroup = this._soGroup.Get(x => x.ID == soGroupId);
            return soGroup;
        }

        public void UpdateSO(SalesOrder SO)
        {
            this._salesOrder.Update(SO);
        }

        public bool UpdateRangeSOItem(List<SalesOrderItem> argListSoItem)
        {
            if (argListSoItem == null || argListSoItem.Count <= 0)
            {
                return false;
            }

            List<SalesOrderItem> listUpdateSoItem = null;
            bool boolExec = false;
            List<string> listSoItemCode = null;

            listSoItemCode = argListSoItem.Select(x=> x.Code).ToList();

            listUpdateSoItem = this._soItem.GetAll().Where(x => listSoItemCode.Contains(x.Code)).ToList();

            if (listUpdateSoItem != null && listUpdateSoItem.Count > 0)
            {
                try
                {
                    ModelConverter.ConvertTo<List<Models.DBModels.TWSQLDB.SalesOrderItem>, List<Models.DBModels.TWSQLDB.SalesOrderItem>>(argListSoItem, listUpdateSoItem);
                    this._soItem.UpdateRange(listUpdateSoItem);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return boolExec;
        }

        public IQueryable<SalesOrder> GetSOAllData()
        {
            return this._salesOrder.GetAll();
        }
        public IQueryable<SalesOrderItem> GetAllForSaleOrderItem()
        {
            return this._soItem.GetAll();
        }
    }
}
