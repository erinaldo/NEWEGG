using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.SellerRepoAdapters.Interface
{
    public interface ISellerRepoAdapter
    {
        IQueryable<Seller> GetAll();
        IQueryable<Seller> GetSellerList(List<int> SellerID);
        IQueryable<Country> GetAllCountry();
        IQueryable<Currency> GetAllCurrency();
        Currency UpdateCurrency(Currency newData);
        Currency GetCurrency(string Year, string Month, int CountryID = 1);
    }
}
