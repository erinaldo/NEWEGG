using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.ItemService.Models;


namespace TWNewEgg.Website.ECWeb.Models
{
    public interface IGetInfo
    {
        Country GetCountry(int countryID); //Get all Country detail from DB.
        List<Seller> GetSeller(); //Get all seller detail from DB.
        string[] Decoder(string fromCookie, bool uriDecode); //Decode the URI which send by client.
        int CheckAccount(int accID); //Check account ID which is correct or not.
        string UrlDecode(string encodestring);//decode url 
        List<CookieCart> findShippingCart(string shippingCart);//transfer cookies data to model
        List<BuyingItems> GetBuyingCartItems(string postData);//transfer json string to model
        string ModeltoJSON(List<BuyingItems> postData); //transfer model to JSON
        Dictionary<string, List<GetItemTaxDetail>> GetProductCostByItemID(List<BuyingItems> postData, string isOverSea); //get product cost for steven
    }
}