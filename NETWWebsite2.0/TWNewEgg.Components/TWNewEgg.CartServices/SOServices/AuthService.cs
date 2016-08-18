using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Auth;

namespace TWNewEgg.CartServices.SOServices
{
    public class AuthService : IAuthService
    {
        private IAuthRepoAdapter  _AuthRepository;

        public AuthService(IAuthRepoAdapter AuthRepository)
        {
            this._AuthRepository = AuthRepository;
        }
        public List<AuthDM> GetAuthBySalesOrderGroupID(int OrderGroupID)
        {
            List<AuthDM> result = new List<AuthDM>();
            var resultTemp = _AuthRepository.GetAuthBySalesOrderGroupID(OrderGroupID).ToList();
            result = ModelConverter.ConvertTo<List<AuthDM>>(resultTemp);
            return result;
        }
    }
}
