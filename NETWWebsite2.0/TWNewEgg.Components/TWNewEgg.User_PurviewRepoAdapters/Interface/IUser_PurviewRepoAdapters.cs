using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.User_PurviewRepoAdapters.Interface
{
    public interface IUser_PurviewRepoAdapters
    {
        void Create(User_Purview model);
        IQueryable<User_Purview> GetAll();
        void Update(User_Purview model);
    }
}
