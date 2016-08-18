using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface IPurchaseOrderRepoAdapter
    {
        IQueryable<PurchaseOrderTWSQLDB> GetPurchaseOrder(List<string> salesOrderCodes);
    }
}
