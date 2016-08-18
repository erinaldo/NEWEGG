using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.PaymentGatewayAdapter.Interface
{
    public interface IAllPayProxy
    {
        string Pay(SOGroupInfo soGroupInfo);
    }
}
