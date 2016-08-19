using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Auth;

namespace TWNewEgg.CartServices.Interface
{
    public interface IAuthService
    {
        List<AuthDM> GetAuthBySalesOrderGroupID(int OrderGroupID);
    }
}
