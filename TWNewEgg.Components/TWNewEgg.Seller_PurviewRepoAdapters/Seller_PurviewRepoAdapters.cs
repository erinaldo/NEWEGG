using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_PurviewRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_PurviewRepoAdapter
{
    public class Seller_PurviewRepoAdapters : ISeller_PurviewRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Purview> _seller_Purview;

        public Seller_PurviewRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Purview> seller_Purview)
        {
            this._seller_Purview = seller_Purview;
        }

        public void Create(Seller_Purview model)
        {
            this._seller_Purview.Create(model);
        }

        public IQueryable<Seller_Purview> GetAll()
        {
            return this._seller_Purview.GetAll();
        }

        public void Update(Seller_Purview model)
        {
            this._seller_Purview.Update(model);
        }
    }
}
