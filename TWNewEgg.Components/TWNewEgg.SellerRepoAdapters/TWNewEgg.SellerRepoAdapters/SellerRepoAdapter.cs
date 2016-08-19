using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.SellerRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.SellerRepoAdapters
{
    public class SellerRepoAdapter : ISellerRepoAdapter
    {
        private IRepository<Seller> _sellerRepo;
        private IRepository<Country> _country;
        private IRepository<Currency> _currency;

        public SellerRepoAdapter(IRepository<Seller> sellerRepo, IRepository<Country> country, IRepository<Currency> currency)
        {
            this._sellerRepo = sellerRepo;
            this._country = country;
            this._currency = currency;
        }

        public IQueryable<Seller> GetAll()
        {
            return this._sellerRepo.GetAll();
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.Seller> GetSellerList(List<int> SellerID)
        {
            var Sellerresult = this._sellerRepo.GetAll().Where(x => SellerID.Contains(x.ID));
            return Sellerresult;
        }

        public IQueryable<Country> GetAllCountry()
        {
            return this._country.GetAll();
        }

        public IQueryable<Currency> GetAllCurrency()
        {
            return this._currency.GetAll();
        }

        public Currency UpdateCurrency(Currency newData)
        {
            _currency.Update(newData);
            return newData;
        }

        public Currency GetCurrency(string Year, string Month, int CountryID = 1)
        {
            if(Year == ""){
                Year = DateTime.UtcNow.AddHours(8).Year.ToString();
            }
            if(Month == ""){
                Year = DateTime.UtcNow.AddHours(8).Month.ToString();
            }
            IQueryable<Currency> CurrencyAll = this._currency.GetAll();
            var Currencyresult = CurrencyAll.Where(x => x.CountryID == CountryID && x.Year == Year && x.Month == Month).FirstOrDefault();
            if (Currencyresult == null) {
                Currencyresult = CurrencyAll.Where(x => x.CountryID == CountryID).OrderBy(x => x.Year).LastOrDefault();
                Currencyresult = CurrencyAll.Where(x => x.CountryID == CountryID && x.Year == Year).OrderBy(x => x.Month).LastOrDefault();
                if (Currencyresult == null)
                {
                    Currencyresult = new Currency();
                    Currencyresult.AverageexchangeRate = 1;
                }
            }
            return Currencyresult;
        }
    }
}
