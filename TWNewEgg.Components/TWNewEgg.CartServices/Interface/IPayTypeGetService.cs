using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;

namespace TWNewEgg.CartServices.Interface
{
    public interface IPayTypeGetService
    {
        IEnumerable<DM_PayType> GetPayTypes(DM_ShoppingCart shoppingCart);
        IQueryable<DM_PayType> GetAllPayTypes();
    }
}
