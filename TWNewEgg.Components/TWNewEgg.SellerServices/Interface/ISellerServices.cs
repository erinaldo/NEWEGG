using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.SellerServices.Interface
{
    public interface ISellerServices
    {
        Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> GetSellerWithCountryList(List<int> SellerList);
    }
}
