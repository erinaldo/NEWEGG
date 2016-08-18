using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.NewEggUSGateway.Models;


namespace TWNewEgg.NewEggUSGateway.Interface
{
    public interface INeweggUSAAPI
    {
        ActionResponse<string> SendToNeweggUSA(SendToNeweggUSAApiModel SendToNeweggUSAApiModel);
        Newegg.Mobile.MvcApplication.Models.UIProductItemDetailAllInfo GetProductDetail(string itemNumber);
        TWNewEgg.NewEggService.Models.ProductDetail GetProductDetailV2(string itemNumber);
        T Get<T>(string url);
        T Post<T>(string url, object body);
        string Serialize(object o);
    }
}
