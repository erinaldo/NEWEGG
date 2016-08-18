using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.PaymentGateway;

namespace TWNewEgg.PaymentGatewayAdapter.Interface
{
    public interface IHiTrustProxy
    {
        string Pay(SOGroupInfo soGroupInfo);
        HiTrustQueryData CheckPayResult(int id);
    }
}
