using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters
{
    public class AuthRepoAdapter : IAuthRepoAdapter
    {
        private IRepository<Auth> _authDB;
        private IRepository<SalesOrder> _salesOrder;

        public AuthRepoAdapter(IRepository<Auth> auth, IRepository<SalesOrder> salesOrder)
        {
            this._authDB = auth;
            this._salesOrder = salesOrder;
        }

        public Auth CreateAuth(Auth authTemp)
        {
            try
            {
                authTemp.CreateDate = DateTime.UtcNow.AddHours(8);
                _authDB.Create(authTemp);
                return authTemp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<Auth> GetAuthBySalesOrderGroupID(int _SalesOrderGroupID)
        {
            IQueryable<Auth> result = null;
            var salesOrderData = _salesOrder.GetAll().Where(p => p.SalesOrderGroupID == _SalesOrderGroupID).AsQueryable();
            List<string> list_salesOrderData = salesOrderData.Select(p => p.Code).ToList();
            if (salesOrderData == null)
            {
                return null;
            }
            result = _authDB.GetAll().AsQueryable().Where(p => list_salesOrderData.Contains(p.OrderNO)).AsQueryable();
            return result;
        }
    }
}
