using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ChargeRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ChargeRepoAdapters
{
    public class Seller_ChargeRepoAdapters : ISeller_ChargeRepoAdapters
    {
        ITWSELLERPORTALDBRepository<Seller_Charge> _seller_Charge;
        public Seller_ChargeRepoAdapters(ITWSELLERPORTALDBRepository<Seller_Charge> seller_Charge)
        {
            this._seller_Charge = seller_Charge;
        }
        public void Create(Seller_Charge model)
        {
            this._seller_Charge.Create(model);
        }

        public IQueryable<Seller_Charge> GetAll()
        {
            return this._seller_Charge.GetAll();
        }

        public void Update(Seller_Charge model)
        {
            this._seller_Charge.Update(model);
        }

    }
}
