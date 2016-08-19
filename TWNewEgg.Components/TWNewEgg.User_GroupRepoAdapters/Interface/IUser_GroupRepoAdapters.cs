using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.User_GroupRepoAdapters.Interface
{
    public interface IUser_GroupRepoAdapters
    {
        void Create(User_Group model);
        IQueryable<User_Group> GetAll();
        void Update(User_Group model);
    }
}
