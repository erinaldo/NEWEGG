using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.CartPayment;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;

namespace TWNewEgg.CartServices.Interface
{
    public interface IPaymentTermsGetService
    {
        IEnumerable<DM_PaymentTerm> GetPaymentTerms(DM_ShoppingCart shoppingCart);
    }
}
