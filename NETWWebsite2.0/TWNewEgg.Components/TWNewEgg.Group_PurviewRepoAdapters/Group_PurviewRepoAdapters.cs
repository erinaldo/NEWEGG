using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Group_PurviewRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Group_PurviewRepoAdapter
{
    public class Group_PurviewRepoAdapters : IGroup_PurviewRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Group_Purview> _group_Purview;

        public Group_PurviewRepoAdapters(ITWSELLERPORTALDBRepository<Group_Purview> group_Purview)
        {
            this._group_Purview = group_Purview;
        }

        public void Create(Group_Purview model)
        {
            this._group_Purview.Create(model);
        }

        public IQueryable<Group_Purview> GetAll()
        {
            return this._group_Purview.GetAll();
        }

        public void Update(Group_Purview model)
        {
            this._group_Purview.Update(model);
        }
    }
}
