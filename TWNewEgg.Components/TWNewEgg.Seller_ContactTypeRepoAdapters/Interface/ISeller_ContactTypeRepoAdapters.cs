using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_ContactTypeRepoAdapters.Interface
{
    public interface ISeller_ContactTypeRepoAdapters
    {
        void Create(Seller_ContactType model);
        IQueryable<Seller_ContactType> GetAll();
        void Update(Seller_ContactType model);
    }
}
