using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.User_PurviewRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.User_PurviewRepoAdapter
{
    public class User_PurviewRepoAdapters : IUser_PurviewRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<User_Purview> _user_Purview;

        public User_PurviewRepoAdapters(ITWSELLERPORTALDBRepository<User_Purview> user_Purview)
        {
            this._user_Purview = user_Purview;
        }

        public void Create(User_Purview model)
        {
            this._user_Purview.Create(model);
        }

        public IQueryable<User_Purview> GetAll()
        {
            return this._user_Purview.GetAll();
        }

        public void Update(User_Purview model)
        {
            this._user_Purview.Update(model);
        }
    }
}
