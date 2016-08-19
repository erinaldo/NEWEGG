using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.CartPayment;

namespace TWNewEgg.CartServices.Interface
{
    public interface IItemPayTypeGetService
    {
        List<TWNewEgg.Models.DomainModels.CartPayment.ItemPayType> GetPayTypesByItems(List<int> itemIds);
    }
}
