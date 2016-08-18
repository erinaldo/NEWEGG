using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.StarProductRepoAdapters.Interface;

namespace TWNewEgg.StarProductRepoAdapters
{
    public class StarProductRepoAdapter : IStarProductRepoAdapter
    {
        private IRepository<StarProduct> _starProductRepo;
        public StarProductRepoAdapter(IRepository<StarProduct> starProductRepo)
        {
            this._starProductRepo = starProductRepo;
        }

        public IQueryable<StarProduct> GetAll()
        {
            return this._starProductRepo.GetAll();
        }
    }
}
