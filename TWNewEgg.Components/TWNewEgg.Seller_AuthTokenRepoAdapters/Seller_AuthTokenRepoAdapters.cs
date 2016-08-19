using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_AuthTokenRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_AuthTokenRepoAdapters
{
    public class Seller_AuthTokenRepoAdapters : ISeller_AuthTokenRepoAdapters
    {
        ITWSELLERPORTALDBRepository<Seller_AuthToken> _iseller_AuthToken;

        public Seller_AuthTokenRepoAdapters(ITWSELLERPORTALDBRepository<Seller_AuthToken> seller_AuthToken)
        {
            this._iseller_AuthToken = seller_AuthToken;
        }

        public void Create(Seller_AuthToken model)
        {
            this.Create(model);
        }

        public IQueryable<Seller_AuthToken> GetAll()
        {
            return this._iseller_AuthToken.GetAll();
        }

        public void Update(Seller_AuthToken model)
        {
            this._iseller_AuthToken.Update(model);
        }
    }
}
