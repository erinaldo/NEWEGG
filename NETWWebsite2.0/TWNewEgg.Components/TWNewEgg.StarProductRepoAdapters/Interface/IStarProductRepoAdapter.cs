using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.StarProductRepoAdapters.Interface
{
    public interface IStarProductRepoAdapter
    {
        IQueryable<StarProduct> GetAll();
    }
}
