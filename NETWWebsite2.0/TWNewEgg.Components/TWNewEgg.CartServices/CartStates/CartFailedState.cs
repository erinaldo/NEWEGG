using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;

namespace TWNewEgg.CartServices.CartStates
{
    public class CartFailedState : ICartState
    {
        public void Init(CartMachines.OPCCartMachine cartMachine)
        {
            throw new NotImplementedException();
        }

        public void Pay(int orderStatus)
        {
            throw new NotImplementedException();
        }

        public void CheckPayment()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void TransactToBackend()
        {
            throw new NotImplementedException();
        }

        public void PayComplete()
        {
            throw new NotImplementedException();
        }
    }
}
