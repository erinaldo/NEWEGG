using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ContactInfoRepoAdapters.Interface
{
    public interface ISeller_ContactInfoRepoAdapters
    {
        void Create(Seller_ContactInfo model);
        IQueryable<Seller_ContactInfo> GetAll();
        void Update(Seller_ContactInfo model);
    }
}
