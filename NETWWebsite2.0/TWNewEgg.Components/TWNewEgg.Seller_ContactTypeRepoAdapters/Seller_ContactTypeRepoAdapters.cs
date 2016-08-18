using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ContactTypeRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ContactTypeRepoAdapters
{
    public class Seller_ContactTypeRepoAdapters : ISeller_ContactTypeRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ContactType> _seller_ContactType;
        public Seller_ContactTypeRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ContactType> seller_ContactType)
        {
            this._seller_ContactType = seller_ContactType;
        }
        public void Create(Seller_ContactType model)
        {
            this._seller_ContactType.Create(model);
        }
        public IQueryable<Seller_ContactType> GetAll()
        {
            return this._seller_ContactType.GetAll();
        }
        public void Update(Seller_ContactType model)
        {
            this._seller_ContactType.Update(model);
        }
    }
}
