using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CartServices.Interface
{
    public interface ICartTempVerificationService
    {
        ResponseMessage<CartTempDM> Verify(CartTempDM cartTempDM);
        ResponseMessage<CartTempDM> Verify(string sn);
        ResponseMessage<CartTempDM> Verify(int accountID, int cartType);
    }
}
