using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_SettlementsRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_SettlementsRepoAdapter
{
    public class Seller_SettlementsRepoAdapters : ISeller_SettlementsRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_Settlements> _seller_Settlements;

        public Seller_SettlementsRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Settlements> seller_Settlements)
        {
            this._seller_Settlements = seller_Settlements;
        }

        public void Create(Seller_Settlements model)
        {
            this._seller_Settlements.Create(model);
        }

        public IQueryable<Seller_Settlements> GetAll()
        {
            return this._seller_Settlements.GetAll();
        }

        public void Update(Seller_Settlements model)
        {
            this._seller_Settlements.Update(model);
        }
    }
}
