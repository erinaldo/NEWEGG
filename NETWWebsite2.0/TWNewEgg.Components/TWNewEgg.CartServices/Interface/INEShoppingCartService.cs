using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;

namespace TWNewEgg.CartServices.Interface
{
    public interface INEShoppingCartService
    {
        DM_ShoppingCart GetShoppingCart(int AccountID, int CartType);
    }
}
