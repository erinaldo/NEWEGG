using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_ContactInfoRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ContactInfoRepoAdapters
{
    public class Seller_ContactInfoRepoAdapters : ISeller_ContactInfoRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_ContactInfo> _seller_ContactInfo;
        public Seller_ContactInfoRepoAdapters(ITWSELLERPORTALDBRepository<Seller_ContactInfo> seller_ContactInfo)
        {
            this._seller_ContactInfo = seller_ContactInfo;
        }
        public void Create(Seller_ContactInfo model)
        {
            this._seller_ContactInfo.Create(model);
        }
        public IQueryable<Seller_ContactInfo> GetAll()
        {
            return this._seller_ContactInfo.GetAll();
        }
        public void Update(Seller_ContactInfo model)
        {
            this._seller_ContactInfo.Update(model);
        }
    }
}
