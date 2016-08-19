using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_UserRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_UserRepoAdapters
{
    public class Seller_UserRepoAdapters : ISeller_UserRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_User> _seller_User;

        public Seller_UserRepoAdapters(ITWSELLERPORTALDBRepository<Seller_User> seller_User)
        {
            this._seller_User = seller_User;
        }

        public void Create(Seller_User model)
        {
            this._seller_User.Create(model);
        }

        public IQueryable<Seller_User> GetAllSellerUser()
        {
            return this._seller_User.GetAll();
        }

        public void Update(Seller_User model)
        {
            this._seller_User.Update(model);
        }
    }
}
