using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.NeweggUSARequestServices.Interface
{
    public interface INeweggRequest
    {
        T Get<T>(string url);
        T Post<T>(string url, object body);
        TWNewEgg.NeweggUSARequestServices.Models.ProductDetail GetProductDetail(string itemNumber);
        Dictionary<string, Models.Pricing.ItemInfo> GetPrice(List<string> itemNumbers);
    }
}
