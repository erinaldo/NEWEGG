using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartServices.Interface
{
    public interface ISalesOrderInfoService
    {
        List<DemainInsertSalesOrdersBySellerOutput> CreateSoInfo(DomainInsertSOGroupOrderItem _data, string testCreateSO);
    }
}
