using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.CategoryRepoAdapters.Interface
{
    public interface IAdLayer3RepoAdapter
    {
        IQueryable<AdLayer3> AdLayer3_GetAll();

        IQueryable<AdLayer3Item> AdLayer3Item_GetAll();

        AdLayer3 UpdateAdLayer3Data(AdLayer3 newData);

        AdLayer3Item UpdateAdLayer3ItemData(AdLayer3Item newData);

        void DeleteAdLayer3Items(int ID);
    }
}
