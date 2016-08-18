using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.User_GroupRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.User_GroupRepoAdapter
{
    public class User_GroupRepoAdapters : IUser_GroupRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<User_Group> _user_Group;

        public User_GroupRepoAdapters(ITWSELLERPORTALDBRepository<User_Group> user_Group)
        {
            this._user_Group = user_Group;
        }

        public void Create(User_Group model)
        {
            this._user_Group.Create(model);
        }

        public IQueryable<User_Group> GetAll()
        {
            return this._user_Group.GetAll();
        }

        public void Update(User_Group model)
        {
            this._user_Group.Update(model);
        }
    }
}
