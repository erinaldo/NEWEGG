using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.ECWeb.Services.OldCart.NeweggService
{
    public interface IPlaceOrder
    {
        string SendPlaceOrder(int salesOrderGroupID, string salesOrderCode);
    }
}
