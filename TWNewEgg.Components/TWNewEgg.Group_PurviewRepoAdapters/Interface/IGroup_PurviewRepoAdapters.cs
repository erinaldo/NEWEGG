using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Group_PurviewRepoAdapters.Interface
{
    public interface IGroup_PurviewRepoAdapters
    {
        void Create(Group_Purview model);
        IQueryable<Group_Purview> GetAll();
        void Update(Group_Purview model);
    }
}
