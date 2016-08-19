using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.ItemService.Models;


namespace TWNewEgg.CartService.Interface
{
    public interface ICarts
    {
        //Check account ID and loginstatus
        bool SetTrackAll(int accID, string dateTime);
        int CheckAccount(int accID, int loginStatus);
        //Get Stored procudure and return 
        IEnumerable<ShoppingCartItems> GetShoppingCart(int sortCode, string isOverSea);
        List<ShoppingCartItems> GetBuyingCart(int accountID, int sortCode, List<BuyingItems> BuyingCart, string isOverSea);
        IEnumerable<CartItems> GetTrackAll(int opCode, int sortCode);

        //Modify dbo.Track table
        string AddTrack(List<int> itemIDs, List<int> itemlistIDs, int trackStatus);
        string RemoveTrack(List<int> itemIDs);
        string UpdateTrack(List<int> itemIDs, int trackStatus, bool updateTime);

        //Modify dbo.Trackitem table
        Dictionary<int, List<int>> GetTrackItemAll();
        string AddTrackItem(int itemID, List<int> itemlistIDs, int trackStatus, int trackID);
        string AddTrackItem(int itemID, int itemlistID);
        string RemoveTrackItem(int itemID);
        string RemoveTrackItem(int itemID, int itemlistID);
        string RemoveTrackItem(int itemID, List<int> itemlistID);
        string UpdateTrackItem(int trackID, int trackStatus);

        //Aes Decode 
        string[] Decoder(string fromBody, bool uriDecode);
        string[] DecoderIE(string fromBody, bool uriDecode);
        
        //Calculate Shipping Cost
        Dictionary<string, decimal> ShippingCosts(string buyingCartItem, string returnCategory, string shoptype = "shoppingcart");
        ShipTaxService ShippingCosts(List<BuyingItems> PostData, string shoptype = "shoppingcart");
        ShipTaxService ShippingCosts(List<BuyingItems> PostData, string isOverSea, string shoptype = "shoppingcart");
        Dictionary<string, decimal> GetCartNumber(string trackStatus);
        List<BuyingItems> GetBuyingCartItems(string postData);
        decimal getTotalWeight(List<BuyingItems> PostData);

        bool CheckDateTimeOut(int itemID, string tableName);
        void CheckTrackCreateDate();

        //Get Categories by itemids
        Dictionary<string, string> GetRootCategorybyItemIDs(List<int> itemIDs);
    }
}