using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.ShoppingCartServices.Interface
{
    public interface IShoppingCartService
    {
        List<ShoppingCartDM> GetCartAllList(int account);
        List<ViewTracksCartItems> NoDiscountsGoods(int accountID);
        List<ViewTracksCartItems> GetIncreasePurchaseItemList(int accountID, int cartTypeID);
    }
}
