using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.CartPayment;

namespace TWNewEgg.NECartPaymentRepoAdapters.Interface
{
    public interface INECartPaymentRepoAdapters
    {
        DM_CartPayment GetPayTerms(int accountId, int cartType);
    }
}
