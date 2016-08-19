using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ActionRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ActionRepoAdapters
{
    public class Seller_ActionRepoAdapters : ISeller_ActionRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Action> _seller_Action;

        public Seller_ActionRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Action> seller_Action)
        {
            this._seller_Action = seller_Action;
        }

        public IQueryable<Seller_Action> GetAll()
        {
            return this._seller_Action.GetAll().AsQueryable();
        }

        public void Create(Seller_Action model)
        {
            this._seller_Action.Create(model);
        }

        public void Update(Seller_Action model)
        {
            this._seller_Action.Update(model);
        }

    }
}
