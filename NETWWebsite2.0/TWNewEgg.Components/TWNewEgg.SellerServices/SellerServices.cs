using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.SellerServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.SellerRepoAdapters.Interface;

namespace TWNewEgg.SellerServices
{
    public class SellerServices : ISellerServices
    {
        private ISellerRepoAdapter _iSellerRepoAdapter;

        public SellerServices(ISellerRepoAdapter iSellerRepoAdapter)
        {
            this._iSellerRepoAdapter = iSellerRepoAdapter;
        }

        /// <summary>
        /// 商家訊息家國別
        /// </summary>
        /// <param name="SellerIDList"></param>
        /// <returns></returns>
        public Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> GetSellerWithCountryList(List<int> SellerIDList) {
            Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> SellerBaseList = new Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase>();
            var SellerList = this._iSellerRepoAdapter.GetSellerList(SellerIDList).ToList();
            var CountryList = this._iSellerRepoAdapter.GetAllCountry().ToList();

            foreach (var item in SellerList) {
                TWNewEgg.Models.DomainModels.Seller.SellerBase SellerBase = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Seller.SellerBase>(item);
                TWNewEgg.Models.DBModels.TWSQLDB.Country Country = CountryList.Where(x => x.ID == item.CountryID).FirstOrDefault();
                if (Country == null)
                {
                    Country = CountryList.Where(x => x.ID == 1).FirstOrDefault();
                }

                SellerBase.CountryName = Country.Name;
                SellerBase.CountryNameCHT = Enum.Parse(typeof(TWNewEgg.Models.DBModels.TWSQLDB.Country.countryNameTW), Country.ID.ToString()).ToString();
                SellerBase.UsageCurrency = Country.UsageCurrency;

                SellerBaseList.Add(SellerBase.ID, SellerBase);
            }
            return SellerBaseList;
        }
    }
}
