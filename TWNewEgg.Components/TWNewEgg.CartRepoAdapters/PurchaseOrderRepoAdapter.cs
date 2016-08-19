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
    public class PurchaseOrderRepoAdapter : IPurchaseOrderRepoAdapter
    {
        private IRepository<PurchaseOrderTWSQLDB> _purchaseOrderTWSQLDB;

        public PurchaseOrderRepoAdapter(IRepository<PurchaseOrderTWSQLDB> purchaseOrderTWSQLDB)
        {
            this._purchaseOrderTWSQLDB = purchaseOrderTWSQLDB;
        }

        public IQueryable<PurchaseOrderTWSQLDB> GetPurchaseOrder(List<string> salesOrderCodes)
        {
            var PurchaseOrderList = this._purchaseOrderTWSQLDB.GetAll().Where(x => salesOrderCodes.Contains(x.SalesorderCode));
            return PurchaseOrderList;
        }
    }
}
