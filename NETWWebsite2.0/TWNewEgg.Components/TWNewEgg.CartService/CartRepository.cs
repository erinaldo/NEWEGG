using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Text;
using System.IO;
using TWNewEgg.DB;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.DB.TWBACKENDDB.Models;
using TWNewEgg.CategoryService.Models;
using TWNewEgg.ItemService.Models;
using TWNewEgg.ItemService.Service;
using TWNewEgg.CartService.Interface;
using TWNewEgg.AccountEnprypt.Interface;
using TWNewEgg.AccountEnprypt;
using TWNewEgg.CategoryService.Service;
using TWNewEgg.CategoryService.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels;
using System.Web.Script.Serialization;

namespace TWNewEgg.CartService
{
    public class CartsRepository : ICarts, IDisposable
    {
        private int accountId;
        private IAesService _aesEnc;
        private ICategoryService _categoryApiService;
        private IItemPrice _itemPrice;
        private IItemStockRepoAdapter _ItemStockRepo;

        public CartsRepository(IAesService aesEnc, ICategoryService categoryApiService, IItemPrice itemPrice, IItemStockRepoAdapter argItemStockRepo)
        {
            this._aesEnc = aesEnc;
            this._categoryApiService = categoryApiService;
            this._itemPrice = itemPrice;
            this._ItemStockRepo = argItemStockRepo;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _itemPrice.Dispose();
                //_itemPrice = null;
            }
        }

        public string[] Decoder(string fromBody, bool uriDecode)
        {
            string[] splitString = new string[] { "%spl=" };
            if (uriDecode)
                fromBody = HttpUtility.UrlDecode(fromBody);
            string[] splitEncode = fromBody.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitEncode.Length; i++)
            {
                splitEncode[i] = _aesEnc.Decrypt(splitEncode[i]);
                if (i == 0)
                {
                    fromBody = splitEncode[i] + "_";
                }
                else
                {
                    fromBody += splitEncode[i];
                }
            }
            splitString = new string[] { "_" };
            string[] plainText = fromBody.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            if (plainText.Length < 4)
            {
                return plainText;
            }
            /*  //The content in Accountid Cookie is "account.ID_Account.CreateDate", and LoginStatus Cookie is "account.LoginStatus_Account.CreateDate"
            if (plainText[1] == plainText[3]) //So compare second and fourth argument, if they are equal, then it's means this cookies is valid
            {
                plainText[1] = plainText[2]; //Set third argument to second argument, coz only use first and second argument after decode.
                plainText[2] = plainText[3]; //Set fouth argument to third argument, coz only use first and second argument after decode.
            }
            */
            //The content in Accountid Cookie is "account.ID_Account.CreateDate_DateTime.now.ToString("yyyymmss")", and LoginStatus Cookie is "account.LoginStatus_Account.CreateDate_DateTime.now.ToString("yyyymmss")"
            try
            {
                if (plainText[1] == plainText[4])
                {
                    plainText[1] = plainText[3]; //Set third argument to second argument, coz only use first and second argument after decode.
                    plainText[2] = plainText[4]; //Set third argument to second argument, coz only use first and second argument after decode.
                }
            }
            catch (Exception e)
            {
                return plainText;
            }
            return plainText;
        }
        public string[] DecoderIE(string fromBody, bool uriDecode)
        {
            string[] splitString = new string[] { "_" };
            if (uriDecode)
                fromBody = HttpUtility.UrlDecode(fromBody);

            string temp = _aesEnc.Decrypt(fromBody);
            string[] plainText = temp.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            return plainText;
        }
        /**
         * Opcode number
         * 1  =  get buynow table
         * 3  =  get buynext table
         * 5  =  get tracknext table
         * 7  =  get oversea buynow table
         * 8  =  get oversea buynext table
         * 9  =  get oversea tracknext table
         * 21 =  add to Shopping cart from product page
         * 23 =  add to Shopping cart from Api
         * 31 =  del track
         * 33 =  del Trackitem
         * 35 =  del Trackitem's attrid
         */
        //public cartsRepository()
        //{
        //    
        //}
        /// <summary>
        /// check login account id and Set the id to accountId variable
        /// </summary>
        /// <param name="accID"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool SetTrackAll(int accID, string dateTime) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //var loginStatus = db.account.Where(x => x.account_id == accID).Select(y => new { y.account_loginstatus, y.account_loginon }).FirstOrDefault();
            var loginStatus = db.Account.Where(x => x.ID == accID).Select(y => new { account_loginstatus = y.LoginStatus, account_createdate = y.CreateDate }).FirstOrDefault();
            //if (loginStatus != null && loginStatus.account_loginstatus != null && loginStatus.account_loginstatus.Value == 1 && loginStatus.account_loginon != null && loginStatus.account_loginon.ToString() == dateTime)
            if (loginStatus != null && loginStatus.account_loginstatus != null && loginStatus.account_loginstatus.Value == 1 && loginStatus.account_createdate != null && loginStatus.account_createdate.ToString() == dateTime)
            {
                accountId = accID;
                return true;
            }
            else
            {
                return false;
            }

            //tracks = (from A in db.track where A.track_id == accid && A.track_status == status select A).ToList();
        }
        /// <summary>
        /// check login status and return it
        /// </summary>
        /// <param name="accID"></param>
        /// <param name="loginStatus"></param>
        /// <returns></returns>
        public int CheckAccount(int accID, int loginStatus)
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            int? checkLogin = db.Account.Where(x => x.ID == accID && x.LoginStatus == loginStatus).Select(y => y.LoginStatus).FirstOrDefault();
            if (checkLogin != null)
            {
                return checkLogin.Value;
            }
            else
            {
                return 0;
            }
        }
        //public int getTracknum()
        //{
        //return tracks.Count;
        //}
        /// <summary>
        /// Get all buynow's shopping cart items from Stored procedure
        /// </summary>
        /// <param name="sortCode">if sortCode is 1 then sort , if 0 then do nothing</param>
        /// <param name="isOverSea">if isOverSea is "True" then return oversea's items , if 0 then return local's items</param>
        /// <returns></returns>
        public IEnumerable<ShoppingCartItems> GetShoppingCart(int sortCode, string isOverSea)
        {
            //TWSqlDBContext db = new TWSqlDBContext();
            using (var db = new TWNewEgg.DB.TWSqlDBContext())
            {
                List<int> allItem = new List<int>();
                try
                {
                    //Get item where track_status is 0 from dbo.track table
                    allItem = (from A in db.Track where A.ACCID == accountId && A.Status == 0 select A.ItemID).ToList();
                }
                catch (Exception e)
                {
                    return null;
                }

                string itemIDs = "";

                foreach (var item in allItem)
                {
                    itemIDs += item.ToString() + ",";

                }


                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();



                SqlParameter paramOne = new SqlParameter();
                paramOne.ParameterName = "@account_id";
                paramOne.Value = accountId;
                cmd.Parameters.Add(paramOne);

                SqlParameter paramTwo = new SqlParameter();
                paramTwo.ParameterName = "@item_ids";
                paramTwo.Value = itemIDs;
                cmd.Parameters.Add(paramTwo);

                cmd.CommandText = "[dbo].[UP_EC_ShoppingCartGetCheckoutItem] @account_id, @item_ids";

                try
                {
                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    //read oversea items from stored procedure
                    IEnumerable<ShoppingCartItems> overSeaShoppingCart = ((IObjectContextAdapter)db).ObjectContext.Translate<ShoppingCartItems>(reader, "ShoppingCartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //read local items from stored procedure
                    IEnumerable<ShoppingCartItems> shoppingCart = ((IObjectContextAdapter)db).ObjectContext.Translate<ShoppingCartItems>(reader, "ShoppingCartItems", MergeOption.NoTracking).ToList();

                    db.Database.Connection.Close();
                    overSeaShoppingCart = SumJiaMeiPrice<IEnumerable<ShoppingCartItems>>(null, overSeaShoppingCart);
                    shoppingCart = SumJiaMeiPrice<IEnumerable<ShoppingCartItems>>(null, shoppingCart);
                    switch (sortCode)
                    {
                        case 0:
                            if (isOverSea != "True")
                            {
                                return shoppingCart;
                            }
                            else
                            {
                                return overSeaShoppingCart;
                            }
                            break;
                        case 1:
                            if (isOverSea != "True")
                            {
                                return shoppingCart.OrderBy(x => x.ItemID).ThenBy(x => x.ItemListOrder).ThenBy(x => x.ItemListID).ThenBy(x => x.ItemListItemListID).ThenByDescending(x => x.ItemListType);
                            }
                            else
                            {
                                return overSeaShoppingCart.OrderBy(x => x.ItemID).ThenBy(x => x.ItemListOrder).ThenBy(x => x.ItemListID).ThenBy(x => x.ItemListItemListID).ThenByDescending(x => x.ItemListType);
                            }
                            break;
                        default:
                            if (isOverSea != "True")
                            {
                                return shoppingCart;
                            }
                            else
                            {
                                return overSeaShoppingCart;
                            }
                            break;
                    }



                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }
        /// <summary>
        /// Receve all cart items that send from user client, and call stored procedure to check where items is or is not in the stored procedure.
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="sortCode"></param>
        /// <param name="BuyingCarts">this is saved shopping cart which from user client</param>
        /// <param name="isOverSea"></param>
        /// <returns></returns>
        public List<ShoppingCartItems> GetBuyingCart(int accountID, int sortCode, List<BuyingItems> BuyingCarts, string isOverSea)
        {
            accountId = accountID; //set accountID to accountId
            List<ShoppingCartItems> AllShoppingCart = new List<ShoppingCartItems>();//save shopping cart form stored procedure
            List<ShoppingCartItems> AllBuyingCart = new List<ShoppingCartItems>();//Saved final shopping cart data that after check with sp
            if (isOverSea == "False")
            {
                AllShoppingCart = GetShoppingCart(0, "False").ToList(); //Get local's items from buynow shipping cart which called from stored procedure
            }
            else if (isOverSea == "True")
            {
                AllShoppingCart = GetShoppingCart(0, "True").ToList(); //Get local's items from buynow shipping cart which called from stored procedure
            }
            foreach (var BuyingCart in BuyingCarts)
            {
                List<ShoppingCartItems> temp = new List<ShoppingCartItems>();
                temp.AddRange(AllShoppingCart.Where(x => x.ItemID == BuyingCart.buyItemID)); //add item which had in the Allshoppingcart
                if (temp.Count != 0) //if there had data in AllshoppingCart then add it to AllBuyingCart
                {
                    int buyingItemListNumber = 0;
                    int itemListNumber = 0;
                    if (BuyingCart.item_AttrID != null) //if this item had arrtibute item find temp have or have not itemlist
                        itemListNumber = temp.Where(x => x.ItemListID == BuyingCart.item_AttrID).Count();
                    if (itemListNumber != 0)
                    {
                        buyingItemListNumber += itemListNumber; //if it had itemlist(attribute) than add to buyingItemListNumber variable
                        AllBuyingCart.AddRange(temp.Where(x => x.ItemListID == BuyingCart.item_AttrID)); //add itemlist(attribute) to AllBuyingCart
                    }
                    foreach (var buyingItemList in BuyingCart.buyItemLists) //find all itemlist in BuyingCarts
                    {
                        itemListNumber = 0;
                        itemListNumber = temp.Where(x => x.ItemListID == buyingItemList.buyItemlistID).Count(); //check there havd how many itemlists
                        if (itemListNumber != 0)
                        {
                            buyingItemListNumber += itemListNumber; //if it had itemlist than add to buyingItemListNumber variable
                            AllBuyingCart.AddRange(temp.Where(x => x.ItemListID == buyingItemList.buyItemlistID)); ////add itemlist to AllBuyingCart
                        }
                    }
                    if (buyingItemListNumber == 0) //if this item have no any itemlist then clear all itemlist fields.
                    {
                        AllBuyingCart.Add(ClearItemList(temp.First()));
                    }
                }
                temp.Clear();
                //AllBuyingCart.AddRange();
            }

            return AllBuyingCart;
        }
        /// <summary>
        /// clear itemlist, if the item haven't any attribute
        /// </summary>
        /// <param name="Item"></param>
        /// <returns></returns>
        private ShoppingCartItems ClearItemList(ShoppingCartItems Item)
        {
            /*
            ShoppingCartItems NewItemList = new ShoppingCartItems();

            NewItemList.track_status = Item.track_status;
            NewItemList.item_id = Item.item_id;
            NewItemList.item_productid = Item.item_productid;
            NewItemList.item_name = Item.item_name;
            NewItemList.item_pricecash = Item.item_pricecash;
            NewItemList.item_pricecard = Item.item_pricecard;
            NewItemList.item_saletype = Item.item_saletype;
            NewItemList.item_paytype = Item.item_paytype;
            NewItemList.item_delvtype = Item.item_delvtype;
            NewItemList.item_datestart = Item.item_datestart;
            NewItemList.item_dateend = Item.item_dateend;
            NewItemList.item_datedel = Item.item_datedel;
            NewItemList.item_coupon = Item.item_coupon;
            NewItemList.item_inst0rate = Item.item_inst0rate;
            NewItemList.item_pricecoupon = Item.item_pricecoupon;
            NewItemList.item_sellingQty = Item.item_sellingQty;
            NewItemList.item_sellerid = Item.item_sellerid;
            NewItemList.track_createdate = Item.track_createdate;
            NewItemList.item_image = Item.item_image;
            NewItemList.item_producttype = Item.item_producttype;
            NewItemList.item_attribname = Item.item_attribname;

            NewItemList.itemlist_selecttype = null;
            NewItemList.itemlist_id = null;
            NewItemList.itemlist_sellingQty = null; 
            NewItemList.itemlist_sellerid = null; 
            NewItemList.itemlist_itemlistid = null;
            NewItemList.itemlist_type = null; 
            NewItemList.itemlist_name = null; 
            NewItemList.itemlist_price = null;
            NewItemList.itemlist_order = null;



            NewItemList.item_product_length = Item.item_product_length;
            NewItemList.item_product_width = Item.item_product_width;
            NewItemList.item_product_height = Item.item_product_height;
            NewItemList.item_product_weight = Item.item_product_weight;

            NewItemList.item_pricedetail = Item.item_pricedetail;


            NewItemList.itemlist_product_length = null;
            NewItemList.itemlist_product_width = null;
            NewItemList.itemlist_product_height = null;
            NewItemList.itemlist_product_weight = null;
            
            NewItemList.itemlist_pricedetail = null;
            */

            ShoppingCartItems NewItemList = new ShoppingCartItems(Item);


            NewItemList.ItemListSelectType = null;
            NewItemList.ItemListID = null;
            NewItemList.ItemListSellingQty = null;
            NewItemList.ItemListSellerID = null;
            NewItemList.ItemListItemListID = null;
            NewItemList.ItemListType = null;
            NewItemList.ItemListName = null;
            NewItemList.ItemListPrice = null;
            NewItemList.ItemListOrder = null;
            NewItemList.ItemListProductLength = null;
            NewItemList.ItemListProductWidth = null;
            NewItemList.ItemlistProductHeight = null;
            NewItemList.ItemListProductWeight = null;
            //NewItemList.itemlist_pricedetail = null;


            return NewItemList;
        }
        /// <summary>
        /// Get all shipping cart's items include buynow, buynext, and wish list.
        /// </summary>
        /// <param name="OpCode"></param>
        /// <param name="sortCode"> if sortCode is 1 then sort , if 0 then do nothing</param>
        /// <returns></returns>
        public IEnumerable<CartItems> GetTrackAll(int OpCode, int sortCode) //Done
        {
            //TWSqlDBContext db = new TWSqlDBContext();
            using (var db = new TWNewEgg.DB.TWSqlDBContext())
            {
                //DBContext tt = new DBContext();
                //db.Dispose();
                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();
                //Dictionary<string, > para = new Dictionary<string,VarChar>();
                //para.Add("@account_id");
                /*
                DbParameter para = cmd.CreateParameter();
                para.DbType = DbType.Int32;
                para.ParameterName = "@accunt_id";
                para.Value = 7;
                para.Direction = ParameterDirection.Input;
                */

                //DbParameterCollection paramCollection = new DbParameterCollection(); 



                SqlParameter paramOne = new SqlParameter();
                paramOne.ParameterName = "@account_id";
                paramOne.Value = accountId;
                cmd.Parameters.Add(paramOne);
                //cmd.Parameters["@account_id"].Value = 50;
                cmd.CommandText = "[dbo].[UP_EC_ShoppingCartGetItem] @account_id";
                //cmd.CommandType = CommandType.StoredProcedure;
                //int account_id = 7;
                //var qaa = tt.Database.SqlQuery<List<CartItems>>("EXEC [dbo].[shoppingcart_getitem] {0}", account_id).ToList();
                //var qab = tt.Database.SqlQuery<CartTable>("EXEC [dbo].[shoppingcart_getitem] {0}", 7);
                try
                {
                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    //Get oversea buynow item
                    IEnumerable<CartItems> overSeaBuyNow = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get oversea buynext item
                    IEnumerable<CartItems> overSeaBuyNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get oversea wishlist item
                    IEnumerable<CartItems> overSeaTrackNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get local buynow item
                    IEnumerable<CartItems> buyNow = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get local buynext item
                    IEnumerable<CartItems> buyNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get local wishlist item
                    IEnumerable<CartItems> trackNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();

                    //var tracks = (from A in db.track where A.track_id == Accountid select A).ToList();
                    //return tracks;
                    db.Database.Connection.Close();
                    buyNow = SumJiaMeiPrice<IEnumerable<CartItems>>(buyNow, null);
                    buyNext = SumJiaMeiPrice<IEnumerable<CartItems>>(buyNext, null);
                    trackNext = SumJiaMeiPrice<IEnumerable<CartItems>>(trackNext, null);
                    if (OpCode == 1)
                    {
                        switch (sortCode)
                        {
                            case 0:
                                return buyNow;
                                break;
                            case 1:
                                return buyNow.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return buyNow;
                                break;

                        }
                    }
                    if (OpCode == 3)
                    {
                        switch (sortCode)
                        {
                            case 0:
                                return buyNext;
                                break;
                            case 1:
                                return buyNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return buyNext;
                                break;

                        }
                    }
                    if (OpCode == 5)
                    {
                        switch (sortCode)
                        {
                            case 0:
                                return trackNext;
                                break;
                            case 1:
                                return trackNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return trackNext;
                                break;

                        }
                    }
                    if (OpCode == 7)
                    {
                        switch (sortCode)
                        {
                            case 0:
                                return overSeaBuyNow;
                                break;
                            case 1:
                                return overSeaBuyNow.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return overSeaBuyNow;
                                break;

                        }
                    }
                    if (OpCode == 8)
                    {
                        switch (sortCode)
                        {
                            case 0:
                                return overSeaBuyNext;
                                break;
                            case 1:
                                return overSeaBuyNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return overSeaBuyNext;
                                break;

                        }
                    }
                    if (OpCode == 9)
                    {
                        switch (sortCode)
                        {
                            case 0:
                                return overSeaTrackNext;
                                break;
                            case 1:
                                return overSeaTrackNext.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return overSeaTrackNext;
                                break;

                        }
                    }
                    if (OpCode == 11)
                    {
                        List<CartItems> nowBuyList = new List<CartItems>();
                        nowBuyList.AddRange(overSeaBuyNow);
                        nowBuyList.AddRange(buyNow);
                        switch (sortCode)
                        {
                            case 0:
                                return nowBuyList;
                                break;
                            case 1:
                                return nowBuyList.OrderBy(x => x.ItemSellerID).ThenBy(x => x.ItemID).ThenByDescending(x => x.ItemListType);
                                break;
                            default:
                                return nowBuyList;
                                break;

                        }
                    }
                }
                catch (Exception e)
                {
                    return null;
                }

                return null;
            }
        }
        private T SumJiaMeiPrice<T>(IEnumerable<CartItems> jiaMeiItems, IEnumerable<ShoppingCartItems> shoppingCartItems)
        {
            //IEnumerable<CartItems> jiaMeiItems = new List<CartItems>();
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (jiaMeiItems != null && shoppingCartItems == null)
            {
                if (jiaMeiItems.Count() > 0)
                {
                    List<int> itemIDs = new List<int>();
                    foreach (var aCartItem in jiaMeiItems)
                    {
                        itemIDs.Add(aCartItem.ItemID);
                    }
                    
                    var items = db.Item.Where(x => itemIDs.Contains(x.ID)).ToList();

                    foreach (var aItem in items)
                    {
                        switch (aItem.DelvType)
                        {
                            case 4:
                                var aCartItem = jiaMeiItems.Where(x => x.ItemID == aItem.ID && x.ItemListID == null).FirstOrDefault();
                                aCartItem.ItemPriceCash = aItem.PriceCash + (aItem.Taxfee ?? 0) + aItem.PriceGlobalship;
                                break;
                            default:
                                break;
                        }
                    }
                    /*
                    foreach (var aCartItem in jiaMeiItems.Where(x => x.ItemDelvType == 4).ToList())
                    {
                        var Itemtemp = items.Where(x => x.ID == aCartItem.ItemID).FirstOrDefault();
                        aCartItem.ItemPriceCash = Itemtemp.PriceCash + (Itemtemp.Taxfee ?? 0) + Itemtemp.PriceGlobalship;
                    }
                    */ 
                }
                //var returnData = jiaMeiItems;
                return (T)jiaMeiItems;
            }
            if (jiaMeiItems == null && shoppingCartItems != null)
            {
                if (shoppingCartItems.Count() > 0)
                {
                    List<int> itemIDs = new List<int>();
                    foreach (var aCartItem in shoppingCartItems)
                    {
                        itemIDs.Add(aCartItem.ItemID);
                    }
                    
                    var items = db.Item.Where(x => itemIDs.Contains(x.ID)).ToList();

                    foreach (var aItem in items)
                    {
                        switch (aItem.DelvType)
                        {
                            case 4:
                                var aShoppingCartItem = shoppingCartItems.Where(x => x.ItemID == aItem.ID && x.ItemListID == null).FirstOrDefault();
                                aShoppingCartItem.ItemPriceCash = aItem.PriceCash + (aItem.Taxfee ?? 0) + aItem.PriceGlobalship;
                                break;
                            default:
                                break;
                        }
                    }
                }
                //var returnData = jiaMeiItems;
                return (T)shoppingCartItems;
            }
            return default(T);
        }
        /// <summary>
        /// Check all dbo.track item with accountId, if which items saved over 30 days then delete them
        /// </summary>
        public void CheckTrackCreateDate()
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            var allTracks = db.Track.Where(x => x.ACCID == accountId).ToList(); //find accountId's items from dbo.track

            foreach (var track in allTracks)
            {
                var itemDate = track.CreateDate;
                var nowDate = DateTime.Now;
                var timeDiff = itemDate.Subtract(nowDate).Duration();//compare two date
                if (timeDiff.Days > 31) //if over a month
                {
                    List<int> itemIDs = new List<int>();
                    itemIDs.Add(track.ItemID);
                    RemoveTrack(itemIDs); //delete the items 
                }
                else
                {
                    int status;
                    status = db.Item.Where(x => x.ID == track.ItemID).Select(x => x.Status).FirstOrDefault();
                    if (status != 0)
                    {
                        List<int> itemIDs = new List<int>();
                        itemIDs.Add(track.ItemID);
                        RemoveTrack(itemIDs); //delete the items 
                    }
                }
            }

            return;
        }
        private int CheckItemStatus()
        {
            return 0;
        }
        /// <summary>
        /// add items and itemlist to dbo.track
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="itemlistIDs"></param>
        /// <param name="trackStatus"></param>
        /// <returns></returns>
        public string AddTrack(List<int> itemIDs, List<int> itemlistIDs, int trackStatus)
        {
            if (trackStatus == 2)
            {
                trackStatus = 1;
            }
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (itemIDs.Count < 0)
            {
                throw new ArgumentNullException("item");
            }

            Dictionary<int, int> dictItemSellingQty = null;
            Track newtrack;

            //取得所有的Item的Qty
            dictItemSellingQty = this.GetSellingQtyByItemIdList(itemIDs);

            foreach (var itemID in itemIDs)
            {
                int sellingQty = (int)dictItemSellingQty[itemID]; //check the item which had selling qty or not
                if (sellingQty <= 0)
                {
                    trackStatus = 1;
                    if (trackStatus == 0)
                    {
                        return OutputMessage.noQty;//if no qty can buy then return error message
                    }
                    else
                    {
                    }
                }
                newtrack = new Track { ACCID = accountId, ItemID = itemID, Status = trackStatus, CreateDate = DateTime.Now }; //set a new track row data
                //tracks.Add(newtrack);
                if (trackStatus == 0 && db.Track.Where(x => x.ACCID == accountId && x.Status == trackStatus).Count() > 24) //check track is over 25 items or not
                {
                    return OutputMessage.over25; //if over 25 items then return error message
                }
                if (CheckDateTimeOut(itemID, "Item")) //check the item which is expire or not
                {
                    if (trackStatus == 0)
                    {
                        return OutputMessage.timeOut; //if expired then return error message
                    }
                    else
                    {
                    }

                }
                if (db.Track.Where(x => x.ACCID == accountId && x.ItemID == itemID).Count() == 0) //check this item is added or not
                {
                    try
                    {
                        db.Track.Add(newtrack); //add newtrack into dbo.track
                        db.SaveChanges(); //saved
                        AddTrackItem(itemID, itemlistIDs, trackStatus, newtrack.ID); //saved the itemlist int to dbo.Trackitem
                        return OutputMessage.addSuccess;
                    }
                    catch (Exception e)
                    {
                        return OutputMessage.addException;

                    }
                }
                else
                {
                    if (db.Track.Where(x => x.ACCID == accountId && x.ItemID == itemID).FirstOrDefault().Status == 0)
                    {
                        return OutputMessage.hadAlready;
                    }
                    else
                    {
                        return OutputMessage.hadAlreadyWish;
                    }
                }



            }

            return OutputMessage.doNothing;

        }

        /// <summary>
        /// //Check Item DateTime where timeout or not, return true means timeout, false means not time out yet.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool CheckDateTimeOut(int itemID, string tableName)
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            bool checkTimeStatus = new bool();
            if (tableName == "Item")
            {
                var itemDate = db.Item.Where(x => x.ID == itemID).Select(y => y.DateEnd).FirstOrDefault(); //get item date
                var nowDate = DateTime.Now; //get now
                if (DateTime.Compare(itemDate, nowDate) > 0) //compare two date, and make sure item date is bigger the now
                {
                    checkTimeStatus = false;
                }
                else
                {
                    checkTimeStatus = true;
                }
            }
            if (tableName == "ItemList")
            {
                checkTimeStatus = true;
            }

            //int sellingQty = countSellingQty(itemQty, itemQtyReg, itemQtyLimit, itemStockQty, itemStockQtyReg);

            return checkTimeStatus;
        }
        /// <summary>
        /// remove dbo.track's items
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <returns></returns>
        public string RemoveTrack(List<int> itemIDs) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (itemIDs.Count < 0)
            {
                throw new ArgumentNullException("trackid");
            }
            //int itemid;
            List<Track> updateTrack = db.Track.Where(x => itemIDs.Contains(x.ItemID) && x.ACCID == accountId).ToList();//find item from dbo.track

            if (updateTrack == null || updateTrack.Count == 0) //if no data in dbo.track
            {
                return OutputMessage.noData; //return error message
            }
            //itemid = updatetrack.track_itemid;
            try
            {
                int atrackId;
                foreach (var aUpdateTrack in updateTrack) //del each item from dbo.track
                {
                    atrackId = aUpdateTrack.ID;
                    db.Track.Remove(aUpdateTrack); //delete item
                    //Track newtrack = new Track() { track_accid = accountId, track_itemid = aUpdateTrack.track_itemid, track_status = trackStatus, track_createdate = DateTime.Now };
                    //db.Track.Add(newtrack);
                    db.SaveChanges();
                    //UpdateTrackItem(aUpdateTrack.track_id, newtrack.track_id, trackStatus);
                    RemoveTrackItem(atrackId);// delete the item's itemlist in dbo.itemlist

                }


                return OutputMessage.removeSuccess;
            }
            catch (Exception e)
            {
                return OutputMessage.removeException;
            }

        }
        /// <summary>
        /// update track's status 
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="trackStatus"></param>
        /// <returns></returns>
        public string UpdateTrack(List<int> itemIDs, int trackStatus, bool updateTime) //Done
        {
            if (trackStatus == 2)
            {
                trackStatus = 1;
            }
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            string outputMessage = OutputMessage.updateSuccess;
            if (itemIDs.Count < 0)
            {
                throw new ArgumentNullException("trackid");
            }
            //int itemid;
            List<Track> updateTrack = db.Track.Where(x => itemIDs.Contains(x.ItemID) && x.ACCID == accountId).ToList(); //find item from dbo.track
            List<int> listNumTrackItemId = null;
            Dictionary<int, int> dictItemQty = null;

            if (updateTrack == null || updateTrack.Count == 0) //if no data
            {
                return OutputMessage.noData; //error message
            }
            //itemid = updatetrack.track_itemid;
            try
            {
                //取得SellingQty
                listNumTrackItemId = updateTrack.Select(x => x.ItemID).ToList();
                dictItemQty = this.GetSellingQtyByItemIdList(listNumTrackItemId);

                foreach (var aUpdateTrack in updateTrack)
                {
                    /*
                    db.Track.Remove(aUpdateTrack);
                    Track newtrack = new Track() { track_accid = accountId, track_itemid = aUpdateTrack.track_itemid, track_status = trackStatus, track_createdate = DateTime.Now };
                    db.Track.Add(newtrack);
                    */

                    int sellingQty = (int)dictItemQty[aUpdateTrack.ItemID];
                    if (sellingQty <= 0)
                    {
                        trackStatus = 1;
                        outputMessage = OutputMessage.noQty;
                    }
                    if (trackStatus == 0 && db.Track.Where(x => x.ACCID == accountId && x.Status == trackStatus).Count() > 24) //check buynow is over 25 items or not
                    {
                        trackStatus = 1;
                        outputMessage = OutputMessage.over25;
                    }
                    if (CheckDateTimeOut(aUpdateTrack.ItemID, "Item")) //check item expired or not
                    {
                        trackStatus = 1;
                        outputMessage = OutputMessage.timeOut;
                    }
                    aUpdateTrack.Status = trackStatus; //update status
                    if (updateTime)
                    {
                        aUpdateTrack.CreateDate = DateTime.Now; //update createdate
                    }
                    db.SaveChanges();
                    UpdateTrackItem(aUpdateTrack.ID, trackStatus); //update item's itemlist in dbo.Trackitem
                }


                return outputMessage;
                //return OutputMessage.updateSuccess;
            }
            catch (Exception e)
            {
                return OutputMessage.updateException;
            }

            //int trackindex = tracks.FindIndex(x => x.track_id == trackid);
            //if (trackindex == -1)
            //{
            //    return false;
            //}
            //int itemid = tracks[trackindex].track_itemid;
            //tracks.RemoveAt(trackindex);
            //track newtrack = new track() { track_accid = Accountid, track_itemid = itemid, track_status = status, track_createdate = DateTime.Now };
            //tracks.Add(newtrack);
            //db.SaveChanges();
            //Updatetrackitem(trackid, status);


        }

        /// <summary>
        /// Get all accountId's shoppingCart choose itemlist
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<int>> GetTrackItemAll()
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            Dictionary<int, List<int>> trackItem = new Dictionary<int, List<int>>();
            List<Track> track = (from A in db.Track where A.ACCID == accountId select A).ToList(); //Get all items from dbo.track

            foreach (var aTrack in track)
            {
                List<int> trackItemlist = (from A in db.TrackItem where A.TrackID == aTrack.ID select A.ItemlistID).ToList();//find itemlist that saved in dbo.Trackitem
                trackItem.Add(aTrack.ItemID, trackItemlist);
            }

            return trackItem;
        }
        /// <summary>
        /// add itemlist into dbo.Trackitem
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemlistIDs"></param>
        /// <param name="trackStatus"></param>
        /// <param name="trackId"></param>
        /// <returns></returns>
        public string AddTrackItem(int itemID, List<int> itemlistIDs, int trackStatus, int trackId)
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            Dictionary<int, int> dictItemSellingQty = null;
            var itemlist = (from A in db.ItemList where itemlistIDs.Contains(A.ID) && A.ItemID == itemID select A.ID).ToList(); //make sure there have item in dbo.itemlist
            if (itemlist.Count > 0)
            {
                //取得SellingQty
                dictItemSellingQty = this.GetSellingQtyByItemIdList(itemlist);
                foreach (var list in itemlist)
                {
                    int sellingQty = (int)dictItemSellingQty[list];
                    if (sellingQty > 0)
                    {
                        TrackItem newtrackitem = new TrackItem() { ItemlistID = list, TrackID = trackId, Status = trackStatus };
                        try
                        {
                            db.TrackItem.Add(newtrackitem); //add itemlist into dbo.Trackitem
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    else
                    {
                        //return OMessage.noQty;
                    }

                }
                db.SaveChanges();
                return OutputMessage.addSuccess;
            }
            else
            {
                return OutputMessage.noData;
            }
        }
        /// <summary>
        /// add itemlist into dbo.Trackitem
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemlistID"></param>
        /// <returns></returns>
        public string AddTrackItem(int itemID, int itemlistID) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objItemSellingQty = null;

            if (itemID <= 0 && itemlistID <= 0)
            {
                throw new ArgumentNullException("itemID & itemlistID");
            }
            var trackId = (from A in db.Track where A.ItemID == itemID && A.ACCID == accountId select A).FirstOrDefault();//find item from dbo.track
            //var trackItems = (from A in db.TrackItem where A.trackitem_trackid == trackId.track_id select A).ToList();
            //int trackindex = trackitems.FindAll(x => x.trackitem_trackid == trackId).Count;
            if (trackId == null)
            {
                return OutputMessage.noData;
            }
            var aTrackItem = (from A in db.TrackItem where A.TrackID == trackId.ID && A.ItemlistID == itemlistID select A).FirstOrDefault();
            if (aTrackItem != null)
            {
                RemoveTrackItem(itemID, itemlistID); //check Trackitem, if there had Trackitem then remove it, if not then add it.
                return OutputMessage.removeSuccess;
            }
            TrackItem newtrackitem = new TrackItem() { TrackID = trackId.ID, Status = trackId.Status, ItemlistID = itemlistID };
            try
            {
                objItemSellingQty = this._ItemStockRepo.GetItemSellingQtyByItemId(itemlistID).FirstOrDefault();
                int sellingQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                if (sellingQty > 0)
                {
                    db.TrackItem.Add(newtrackitem); //add itemlist into Trackitem
                }
                else
                {
                    return OutputMessage.noQty;
                }
                //TrackItem newtrackitem = new TrackItem() { trackitem_trackid = newTrackId, trackitem_status = trackStatus, trackitem_itemlistid = aTrackItem.trackitem_itemlistid };
                //db.TrackItem.Add(newtrackitem);
            }
            catch (Exception e)
            {
                return OutputMessage.addException;
            }
            //int itemlistid = aTrackItem.trackitem_itemlistid;

            try
            {
                db.SaveChanges();
                return OutputMessage.addSuccess;
            }
            catch (Exception e)
            {
                return OutputMessage.dbAddFail;
            }
            return OutputMessage.doNothing;
        }
        /// <summary>
        /// remove itemlist in dbo.Trackitem
        /// </summary>
        /// <param name="trackID"></param>
        /// <returns></returns>
        public string RemoveTrackItem(int trackID) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (trackID < 0)
            {
                throw new ArgumentNullException("trackid");
            }
            //var trackId = (from A in db.Track where A.track_itemid == trackID && A.track_accid == accountId select A).FirstOrDefault();
            var trackItems = (from A in db.TrackItem where A.TrackID == trackID select A).ToList();
            //int trackindex = trackitems.FindAll(x => x.trackitem_trackid == trackId).Count;
            if (trackItems.Count <= 0)
            {
                return OutputMessage.noData;
            }

            foreach (var aTrackItem in trackItems)
            {
                try
                {
                    db.TrackItem.Remove(aTrackItem);
                    //TrackItem newtrackitem = new TrackItem() { trackitem_trackid = newTrackId, trackitem_status = trackStatus, trackitem_itemlistid = aTrackItem.trackitem_itemlistid };
                    //db.TrackItem.Add(newtrackitem);
                }
                catch (Exception e)
                {

                }
                //int itemlistid = aTrackItem.trackitem_itemlistid;
            }
            try
            {
                db.SaveChanges();
                return OutputMessage.removeSuccess;
            }
            catch (Exception e)
            {
                return OutputMessage.dbRemoveFail;
            }
            return OutputMessage.doNothing;
        }
        /// <summary>
        /// del itemlist in dbo.Trackitem
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemlistID"></param>
        /// <returns></returns>
        public string RemoveTrackItem(int itemID, int itemlistID) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (itemID <= 0 && itemlistID <= 0)
            {
                throw new ArgumentNullException("itemID & itemlistID");
            }
            var trackId = (from A in db.Track where A.ItemID == itemID && A.ACCID == accountId select A).FirstOrDefault();
            //var trackItems = (from A in db.TrackItem where A.trackitem_trackid == trackId.track_id select A).ToList();
            //int trackindex = trackitems.FindAll(x => x.trackitem_trackid == trackId).Count;
            if (trackId == null)
            {
                return OutputMessage.noData;
            }
            var aTrackItem = (from A in db.TrackItem where A.TrackID == trackId.ID && A.ItemlistID == itemlistID select A).FirstOrDefault();
            try
            {
                db.TrackItem.Remove(aTrackItem);
                //TrackItem newtrackitem = new TrackItem() { trackitem_trackid = newTrackId, trackitem_status = trackStatus, trackitem_itemlistid = aTrackItem.trackitem_itemlistid };
                //db.TrackItem.Add(newtrackitem);
            }
            catch (Exception e)
            {
                return OutputMessage.removeException;
            }
            //int itemlistid = aTrackItem.trackitem_itemlistid;

            try
            {
                db.SaveChanges();
                return OutputMessage.removeSuccess;
            }
            catch (Exception e)
            {
                return OutputMessage.dbRemoveFail;
            }
            return OutputMessage.doNothing;
        }
        /// <summary>
        /// del itemlists in dbo.Trackitem
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="itemlistID"></param>
        /// <returns></returns>
        public string RemoveTrackItem(int itemID, List<int> itemlistID) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (itemID <= 0 && itemlistID.Count == 0)
            {
                throw new ArgumentNullException("itemID & itemlistID");
            }
            var trackId = (from A in db.Track where A.ItemID == itemID && A.ACCID == accountId select A).FirstOrDefault();
            //var trackItems = (from A in db.TrackItem where A.trackitem_trackid == trackId.track_id select A).ToList();
            //int trackindex = trackitems.FindAll(x => x.trackitem_trackid == trackId).Count;
            if (trackId == null)
            {
                return OutputMessage.noData;
            }
            var aTrackItems = (from A in db.TrackItem where A.TrackID == trackId.ID && itemlistID.Contains(A.ItemlistID) select A).ToList();

            foreach (var aTrackItem in aTrackItems)
            {
                try
                {
                    db.TrackItem.Remove(aTrackItem);

                }
                catch (Exception e)
                {
                    //return "Remove Exception";
                }
                //TrackItem newtrackitem = new TrackItem() { trackitem_trackid = newTrackId, trackitem_status = trackStatus, trackitem_itemlistid = aTrackItem.trackitem_itemlistid };
                //db.TrackItem.Add(newtrackitem);
            }

            //int itemlistid = aTrackItem.trackitem_itemlistid;

            try
            {
                db.SaveChanges();
                return AddTrackItem(itemID, itemlistID[0]);  //if there don't have any Trackitem , then add it , if it has item , then remove.
            }
            catch (Exception e)
            {
                return OutputMessage.dbRemoveFail;
            }
            return OutputMessage.doNothing;
        }
        /// <summary>
        /// update itemlist in dbo.Trackitem
        /// </summary>
        /// <param name="trackID"></param>
        /// <param name="trackStatus"></param>
        /// <returns></returns>
        public string UpdateTrackItem(int trackID, int trackStatus) //Done
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            if (trackID < 0)
            {
                throw new ArgumentNullException("trackid");
            }
            var trackItems = (from A in db.TrackItem where A.TrackID == trackID select A).ToList();
            //int trackindex = trackitems.FindAll(x => x.trackitem_trackid == trackId).Count;
            if (trackItems.Count <= 0)
            {
                return OutputMessage.noData;
            }

            foreach (var aTrackItem in trackItems)
            {
                try
                {
                    /*
                    db.TrackItem.Remove(aTrackItem);
                    TrackItem newtrackitem = new TrackItem() { trackitem_trackid = newTrackID, trackitem_status = trackStatus, trackitem_itemlistid = aTrackItem.trackitem_itemlistid };
                    db.TrackItem.Add(newtrackitem);
                    */
                    aTrackItem.Status = trackStatus;
                }
                catch (Exception e)
                {

                }
                //int itemlistid = aTrackItem.trackitem_itemlistid;
            }
            try
            {
                db.SaveChanges();
                return OutputMessage.updateSuccess;
            }
            catch (Exception e)
            {
                return OutputMessage.dbUpdateFail;
            }
            return OutputMessage.doNothing;
        }
        /// <summary>
        /// Get shopping cart's number from stored procedure
        /// </summary>
        /// <param name="trackStatus"></param>
        /// <returns></returns>
        public Dictionary<string, decimal> GetCartNumber(string trackStatus)
        {

            //TWSqlDBContext db = new TWSqlDBContext();
            using (var db = new TWNewEgg.DB.TWSqlDBContext())
            {
                Dictionary<string, decimal> cartNumber = new Dictionary<string, decimal>();
                db.Database.Initialize(force: false);
                var cmd = db.Database.Connection.CreateCommand();




                SqlParameter paramOne = new SqlParameter();
                paramOne.ParameterName = "@account_id";
                paramOne.Value = accountId;
                cmd.Parameters.Add(paramOne);

                cmd.CommandText = "[dbo].[UP_EC_ShoppingCartGetItem] @account_id";

                try
                {
                    db.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    //Get oversea buynow item
                    IEnumerable<CartItems> overSeaBuyNow = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get oversea buynext item
                    IEnumerable<CartItems> overSeaBuyNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get oversea wishlist item
                    IEnumerable<CartItems> overSeaTrackNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get local buynow item
                    IEnumerable<CartItems> buyNow = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get local buynext item
                    IEnumerable<CartItems> buyNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();
                    reader.NextResult();
                    //Get local wishlist item
                    IEnumerable<CartItems> trackNext = ((IObjectContextAdapter)db).ObjectContext.Translate<CartItems>(reader, "CartItems", MergeOption.NoTracking).ToList();

                    //var tracks = (from A in db.track where A.track_id == Accountid select A).ToList();
                    //return tracks;
                    db.Database.Connection.Close();

                    UpdateTrack(overSeaBuyNow.Select(x => x.ItemID).ToList(), 0, false); //update these itemids to status 0
                    UpdateTrack(overSeaBuyNext.Select(x => x.ItemID).ToList(), 1, false);//update these itemids to status 1
                    UpdateTrack(overSeaTrackNext.Select(x => x.ItemID).ToList(), 1, false);//update these itemids to status 2
                    UpdateTrack(buyNow.Select(x => x.ItemID).ToList(), 0, false);//update these itemids to status 0
                    UpdateTrack(buyNext.Select(x => x.ItemID).ToList(), 1, false);//update these itemids to status 1
                    UpdateTrack(trackNext.Select(x => x.ItemID).ToList(), 1, false);//update these itemids to status 2

                    cartNumber.Add("cartNumber", buyNow.Count()); //this two are local buynow and oversea buynow
                    cartNumber.Add("wishNumber", buyNext.Count() + trackNext.Count()); //this four are local buynext, wishlist and oversea buynext, wishlist.
                    cartNumber.Add("overseaCartNumber", overSeaBuyNow.Count()); //this two are local buynow and oversea buynow
                    cartNumber.Add("overseaWishNumber", overSeaBuyNext.Count() + overSeaTrackNext.Count()); //this four are local buynext, wishlist and oversea buynext, wishlist.


                    cartNumber.Add("cartPrice", CountCartsPrice(buyNow));//set local cart's price 
                    cartNumber.Add("wishPrice", CountCartsPrice(buyNext) + CountCartsPrice(trackNext));// set local wish cart's price
                    cartNumber.Add("overseaCartPrice", CountCartsPrice(overSeaBuyNow));//set oversea cart's price
                    cartNumber.Add("overseaWishPrice", CountCartsPrice(overSeaBuyNext) + CountCartsPrice(overSeaTrackNext));//set oversea wish cart's price

                    return cartNumber;
                }
                catch (Exception e)
                {
                    cartNumber.Add("cartNumber", 0);
                    cartNumber.Add("wishNumber", 0);
                    cartNumber.Add("overseaCartNumber", 0);
                    cartNumber.Add("overseaWishNumber", 0);
                    cartNumber.Add("cartPrice", 0);//set local cart's price 
                    cartNumber.Add("wishPrice", 0);// set local wish cart's price
                    cartNumber.Add("overseaCartPrice", 0);//set oversea cart's price
                    cartNumber.Add("overseaWishPrice", 0);//set oversea wish cart's price
                    return cartNumber;
                }


                return null;

            }

            /*
            TWSqlDBContext db = new TWSqlDBContext();
            Dictionary<string, decimal> cartNumber = new Dictionary<string, decimal>();
            db.Database.Initialize(force: false);
            var cmd = db.Database.Connection.CreateCommand();

            SqlParameter paramOne = new SqlParameter();
            paramOne.ParameterName = "@account_id";
            paramOne.Value = accountId;
            cmd.Parameters.Add(paramOne);

            cmd.CommandText = "[dbo].[shoppingcart_getnum] @account_id";

            try
            {
                db.Database.Connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    cartNumber.Add("cartNumber", reader.GetInt32(3)); //this two are local buynow and oversea buynow
                    cartNumber.Add("wishNumber", reader.GetInt32(4) + reader.GetInt32(5)); //this four are local buynext, wishlist and oversea buynext, wishlist.
                    cartNumber.Add("overseaCartNumber", reader.GetInt32(0)); //this two are local buynow and oversea buynow
                    cartNumber.Add("overseaWishNumber", reader.GetInt32(1) + reader.GetInt32(2)); //this four are local buynext, wishlist and oversea buynext, wishlist.
                    db.Database.Connection.Close();
                    return cartNumber;
                }
                else
                {
                    cartNumber.Add("cartNumber", 0);
                    cartNumber.Add("wishNumber", 0);
                    cartNumber.Add("overseaCartNumber", 0);
                    cartNumber.Add("overseaWishNumber", 0);
                    return cartNumber;
                }
                

            }
            catch (Exception e)
            {
                cartNumber.Add("cartNumber", 0);
                cartNumber.Add("wishNumber", 0);
                cartNumber.Add("overseaCartNumber", 0);
                cartNumber.Add("overseaWishNumber", 0);
                return cartNumber;
            }

            */

        }
        /// <summary>
        /// count all cart's Items price  ********Note: this function only use for item with item attribute and itemlist, but it's not suit for item with item attribute and itemlist with itemlist attribute
        /// </summary>
        /// <param name="cartItems"></param> cart's Items
        /// <returns></returns>
        private decimal CountCartsPrice(IEnumerable<CartItems> cartItems)
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            decimal priceSum = new decimal();
            priceSum = 0;
            List<int> itemIDs = cartItems.Select(x => x.ItemID).ToList();
            List<int> trackIDs = db.Track.Where(x => itemIDs.Contains(x.ItemID)).Select(y => y.ID).ToList();
            List<TrackItem> trackItemIDs = db.TrackItem.Where(x => trackIDs.Contains(x.TrackID)).ToList();
            foreach (var cartItem in cartItems)
            {

                try
                {
                    if (cartItem.ItemListID != null && cartItem.ItemListID.Value != 0 && cartItem.ItemListItemListID != null && cartItem.ItemListItemListID.Value != 0) //make sure it's itemlist's itemlist or itemlist's attribute
                    {
                        if (trackItemIDs.Where(x => x.ItemlistID == cartItem.ItemListItemListID).FirstOrDefault() != null)//make sure user had select this itemlist's itemlist or itemlist's attribute
                        {
                            priceSum += cartItem.ItemListPrice.Value;
                        }
                        else//if no then add 0
                        {
                            priceSum += 0;
                        }

                    }
                    else if (cartItem.ItemListID != null && cartItem.ItemListID.Value != 0) //make sure it is item's itemlist 
                    {
                        if (trackItemIDs.Where(x => x.ItemlistID == cartItem.ItemListID).FirstOrDefault() != null)//make sure user had select this itemlist's itemlist or itemlist's attribute
                        {
                            if (cartItem.ItemListType == "屬性") //if this itemlist is attribute then add itemlist price
                            {
                                priceSum += cartItem.ItemPriceCash;
                            }
                            else if (cartItem.ItemListType == "贈品")// if this itemlist is gift then add 0
                            {
                                priceSum += 0;
                            }
                            else
                            {
                                priceSum += cartItem.ItemListPrice.Value; // if this is itemlist then add it
                            }
                        }
                        else
                        {
                            priceSum += 0;
                        }
                    }
                    else
                    {
                        priceSum += cartItem.ItemPriceCash;
                    }
                }
                catch (Exception e)
                {
                    priceSum += 0;
                }
            }

            return priceSum;
        }
        private void changeTrackStatus()
        {

        }
        /// <summary>
        /// Count shipping cost
        /// </summary>
        /// <param name="buyingCartItems">receve from user client</param>
        /// <param name="returnCategory">if returnCategory equal "SellerID" then return all seller's item's cost(include itemlists), if  returnCategory equal "ItemID" then return a item's cost(include itemlists)</param>
        /// <returns></returns>
        public Dictionary<string, decimal> ShippingCosts(string buyingCartItems, string returnCategory, string shoptype = "shoppingcart")
        {
            //var itemProducts;
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            List<BuyingItems> PostData = GetBuyingCartItems(buyingCartItems);
            Dictionary<string, decimal> shoppingCosts = new Dictionary<string, decimal>();
            //List<string> itemSellers = new List<string>();
            //itemSellers = db.Item.Where(y => (buyingItemslist.Select(x => x.buyItemID).ToList()).Contains(y.item_id)).Select(z => z.item_id).Distinct().ToList();
            List<int> returnIDs = new List<int>();
            if (PostData == null || PostData.Count <= 0)
            {
                return null;
            }
            var buyitemIDs = PostData.Select(x => x.buyItemID);
            if (buyitemIDs.Count() <= 0)
            {
                return null;
            }
            var itemIDSellerID = PostData.Select(z => new { item_sellerid = z.buyItemID_Seller, item_id = z.buyItemID }).ToList();
            if (itemIDSellerID.Count <= 0)
            {
                return null;
            }
            if (returnCategory == "SellerID")
            {
                var sellerIDs = itemIDSellerID.Select(x => x.item_sellerid).Distinct().ToList();
                returnIDs = sellerIDs;
                foreach (var returnID in returnIDs)
                {
                    decimal weight = new decimal();
                    weight = 0;
                    var sellerItems = itemIDSellerID.Where(x => x.item_sellerid == returnID).ToList();
                    foreach (var sellerItem in sellerItems)
                    {
                        var items = PostData.Where(x => x.buyItemID == sellerItem.item_id).FirstOrDefault();
                        int itemID;
                        if (items.item_AttrID != null && items.item_AttrID != 0)
                        {
                            itemID = items.item_AttrID.Value;
                            weight += GetItemWeight(itemID, items.buyingNumber, "Itemlist");
                        }
                        else
                        {
                            itemID = items.buyItemID;
                            weight += GetItemWeight(itemID, items.buyingNumber, "Item");
                        }

                        foreach (var itemlists in items.buyItemLists)
                        {
                            int itemlistID;
                            if (itemlists.item_AttrID != null && itemlists.item_AttrID != 0)
                            {
                                itemlistID = itemlists.item_AttrID.Value;
                            }
                            else
                            {
                                itemlistID = itemlists.buyItemlistID;
                            }
                            weight += GetItemWeight(itemlistID, itemlists.buyingNumber, "Itemlist");
                        }

                    }
                    
                    shoppingCosts.Add(returnID.ToString(), GetItemShipping(weight, returnID, shoptype));
                    //GetItemTaxDetails("0,0,0", "73,74,110", "174,174,174");///////////////////////////////////
                    //////////////////////////////////
                }
                return shoppingCosts;
            }
            else if (returnCategory == "ItemID") //No using 
            {
                var itemIDs = itemIDSellerID.Select(x => x.item_id).Distinct().ToList();
                returnIDs = itemIDs;
                foreach (var returnID in returnIDs)
                {
                    decimal weight = new decimal();
                    weight = 0;
                    var itemItems = itemIDSellerID.Where(x => x.item_id == returnID).ToList();
                    foreach (var itemItem in itemItems)
                    {
                        var items = PostData.Where(x => x.buyItemID == itemItem.item_id).FirstOrDefault();
                        int itemID;
                        if (items.item_AttrID != null && items.item_AttrID != 0)
                        {
                            itemID = items.item_AttrID.Value;
                            weight += GetItemWeight(itemID, items.buyingNumber, "Itemlist");
                        }
                        else
                        {
                            itemID = items.buyItemID;
                            weight += GetItemWeight(itemID, items.buyingNumber, "Item");
                        }

                        foreach (var itemlists in items.buyItemLists)
                        {
                            int itemlistID;
                            if (itemlists.item_AttrID != null && itemlists.item_AttrID != 0)
                            {
                                itemlistID = itemlists.item_AttrID.Value;
                            }
                            else
                            {
                                itemlistID = itemlists.buyItemlistID;
                            }
                            weight += GetItemWeight(itemlistID, itemlists.buyingNumber, "Itemlist");
                        }

                    }
                    shoppingCosts.Add(returnID.ToString(), GetItemShipping(weight, returnID, shoptype));

                }


                return shoppingCosts;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Get shipping cost and taxdetail, first is count by program, last is call from stored procedure , this is for local items
        /// </summary>
        /// <param name="buyingItemslist"></param>
        /// <returns></returns>
        public ShipTaxService ShippingCosts(List<BuyingItems> PostData, string shoptype = "shoppingcart")
        {
            return _itemPrice.GetShippingCosts(PostData, shoptype);
        }
        /// <summary>
        /// Get shipping cost and taxdetail, first is count by program, last is call from stored procedure , this is for local items
        /// </summary>
        /// <param name="buyingItemslist"></param>
        /// <returns></returns>
        private ShipTaxService ShippingCosts20140402(List<BuyingItems> PostData, string shoptype = "shoppingcart")
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //var itemProducts;
            ShipTaxService shipTaxService = new ShipTaxService();
            //List<BuyingItems> buyingItemslist = GetBuyingCartItems(buyingCartItems);
            Dictionary<string, decimal> shoppingCosts = new Dictionary<string, decimal>();
            Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail = new Dictionary<string, List<GetItemTaxDetail>>();
            //List<BuyingItems> postData = new List<BuyingItems>();
            //postData = buyingItemslist;

            //List<string> itemSellers = new List<string>();
            //itemSellers = db.Item.Where(y => (buyingItemslist.Select(x => x.buyItemID).ToList()).Contains(y.item_id)).Select(z => z.item_id).Distinct().ToList();
            List<int> returnIDs = new List<int>();
            if (PostData == null || PostData.Count <= 0)
            {
                return shipTaxService;
            }
            var buyitemIDs = PostData.Select(x => x.buyItemID);
            if (buyitemIDs.Count() <= 0)
            {
                return shipTaxService;
            }
            var itemIDSellerID = PostData.Select(z => new { item_sellerid = z.buyItemID_Seller, item_id = z.buyItemID }).ToList();
            if (itemIDSellerID.Count <= 0)
            {
                return shipTaxService;
            }
            var sellerIDs = itemIDSellerID.Select(x => x.item_sellerid).Distinct().ToList();
            returnIDs = sellerIDs;
            foreach (var returnID in returnIDs)
            {
                List<string> itemforSP = new List<string>(); //this is feeding for stored procedure
                List<string> itemlistforSP = new List<string>();//this is feeding for stored procedure
                List<string> priceforSP = new List<string>();//this is feeding for stored procedure
                List<decimal> priceforSPTemp = new List<decimal>();//this is feeding for stored procedure
                List<decimal> eachweight = new List<decimal>(); //this is stored each item or itemlist's weight
                List<int> eachItemNumber = new List<int>();
                List<decimal> eachItemShippingFee = new List<decimal>();

                decimal weight = new decimal();
                weight = 0;
                var sellerItems = itemIDSellerID.Where(x => x.item_sellerid == returnID).ToList();
                foreach (var sellerItem in sellerItems)
                {
                    var items = PostData.Where(x => x.buyItemID == sellerItem.item_id).FirstOrDefault();
                    int itemID;
                    if (items.item_AttrID != null && items.item_AttrID != 0)
                    {
                        itemID = items.item_AttrID.Value;

                        itemforSP.Add(items.buyItemID.ToString()); //set item id into string 
                        itemlistforSP.Add(items.item_AttrID.Value.ToString());//set itemlist id into string, which had attribute
                        
                        decimal tempWeight = GetItemWeight(itemID, items.buyingNumber, "Itemlist");
                        // 存partWeight
                        //BuyingItems tempPostData = postData.Where(x => x.item_AttrID == items.item_AttrID.Value).Select(x => x).FirstOrDefault();
                        //tempPostData.buyItemWeight = tempWeight;
                        
                        weight += tempWeight;
                        eachweight.Add(tempWeight); //set weight into string
                    }
                    else
                    {
                        itemID = items.buyItemID;

                        itemforSP.Add(items.buyItemID.ToString());//set item id into string
                        itemlistforSP.Add("0");//set itemlist id into string, which had no attribute

                        decimal tempWeight = GetItemWeight(itemID, items.buyingNumber, "Item");
                        // 存partWeight
                        //BuyingItems tempPostData = postData.Where(x => x.buyItemID == items.buyItemID).Select(x => x).FirstOrDefault();
                        //tempPostData.buyItemWeight = tempWeight;
                        
                        weight += tempWeight;
                        eachweight.Add(tempWeight);//set weight into string
                    }

                    foreach (var itemlists in items.buyItemLists)
                    {
                        int itemlistID;
                        if (itemlists.item_AttrID != null && itemlists.item_AttrID != 0)
                        {
                            itemlistID = itemlists.item_AttrID.Value;

                            itemlistforSP.Add(itemlists.item_AttrID.Value.ToString());//set itemlist id into string, which had attribute

                        }
                        else
                        {
                            itemlistID = itemlists.buyItemlistID;

                            itemlistforSP.Add(itemlists.buyItemlistID.ToString());//set itemlist id into string, which had no attribute
                        }

                        itemforSP.Add(items.buyItemID.ToString());//set item id into string 

                        decimal tempWeight = GetItemWeight(itemlistID, itemlists.buyingNumber, "Itemlist");
                        // 存partWeight
                        //BuyingItemList tempPostData = postData.Where(x => x.buyItemID == items.buyItemID).Select(x => x).FirstOrDefault().buyItemLists.Where(y => y.buyItemlistID == itemlistID).Select(y => y).FirstOrDefault();
                        //tempPostData.buyItemListWeight = tempWeight;

                        weight += tempWeight;
                        eachweight.Add(tempWeight);//set weight into string

                    }

                }
                decimal totalItemShipping = GetItemShipping(weight, returnID, shoptype); //count total shipping cost
                shoppingCosts.Add(returnID.ToString(), totalItemShipping);//set seller with all seller's items

                int seller_id = sellerItems[0].item_sellerid;
                int? seller_country = db.Seller.Where(x => x.ID == seller_id).Select(x => x.CountryID).FirstOrDefault();
                //string seller_currencytype = db.seller.Where(x => x.seller_id == seller_id).Select(x => x.seller_currencytype).FirstOrDefault();
                if (seller_country != 1)
                {
                    foreach (var itemweight in eachweight)
                    {
                        priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / weight), 0, MidpointRounding.AwayFromZero))); //count each item's shipping cost
                    }
                }
                else
                {
                    if (weight != 0)
                    {
                        foreach (var itemweight in eachweight)
                        {
                            priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / weight), 0, MidpointRounding.AwayFromZero))); //count each item's shipping cost
                        }
                    }
                    else
                    {
                        foreach (var itemweight in eachweight)
                        {
                            priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / 1), 0, MidpointRounding.AwayFromZero))); //count each item's shipping cost
                        }
                    }
                }
                decimal totalPriceforSP = new decimal();
                totalPriceforSP = 0;
                foreach (var eachPrice in priceforSPTemp)
                {
                    totalPriceforSP += eachPrice;
                }
                priceforSPTemp[priceforSPTemp.Count - 1] += (totalItemShipping - totalPriceforSP);
                foreach (var eachPrice in priceforSPTemp)
                {
                    priceforSP.Add(eachPrice.ToString());
                }

                var PostDatatemp = PostData.Where(x => x.buyItemID_Seller == seller_id).ToList();
                for (int i = 0, itemp = 0; i < priceforSP.Count; i++, itemp++)
                {
                    eachItemShippingFee.Add(Convert.ToDecimal(priceforSP[i]));
                    priceforSP[i] = (Convert.ToInt32(priceforSP[i]) / PostDatatemp[itemp].buyingNumber).ToString();
                    eachItemNumber.Add(PostDatatemp[itemp].buyingNumber);
                    if (PostDatatemp[itemp].buyItemLists.Count > 0)
                    {
                        int y = 1;
                        for (y = 1; y <= PostDatatemp[itemp].buyItemLists.Count; y++)
                        { priceforSP[i + y] = (Convert.ToInt32(priceforSP[i + y]) / PostDatatemp[itemp].buyItemLists[y - 1].buyingNumber).ToString(); }
                        i = i + y - 1;
                    }                 
                }

                string taxItem = "", taxItemlist = "", taxShippingPrice = "";

                for (int i = 0; i < itemforSP.Count; i++) //combine string for stroed procedure
                {
                    if (taxItem != "")
                    {
                        taxItem += ",";
                        taxItemlist += ",";
                        taxShippingPrice += ",";
                    }
                    if (itemlistforSP[i] == "0")
                    {
                        taxItem += itemforSP[i];
                    }
                    else
                    {
                        //taxItem += itemforSP[i];
                        taxItem += "0";
                    }
                    taxItemlist += itemlistforSP[i];
                    taxShippingPrice += priceforSP[i];
                }

                itemTaxDetail.Add(returnID.ToString(), GetItemTaxDetails(taxItem, taxItemlist, taxShippingPrice).ToList()); //call stored procedure and save taxdetail with seller id

                SetItemTaxDetailData(itemTaxDetail, returnID.ToString(), itemforSP, totalItemShipping, eachItemNumber, eachItemShippingFee, shoptype); //setting item taxdetail
                //GetItemTaxDetails("0,0,0", "73,74,110", "174,174,174");///////////////////////////////////
                //////////////////////////////////
            }

            shipTaxService.ShippingCost = shoppingCosts; //set total shipping cost

            shipTaxService.ShippingTaxCost = itemTaxDetail; //set item tax detail

            //shipTaxService.buyingItemslist = postData;

            return shipTaxService;


        }
        /// <summary>
        /// Get shipping cost and taxdetail, first is count by program, last is call from stored procedure , this is for oversea items
        /// </summary>
        /// <param name="buyingItemslist"></param>
        /// <param name="isOverSea"></param>
        /// <returns></returns>
        public ShipTaxService ShippingCosts(List<BuyingItems> PostData, string isOverSea, string shoptype = "shoppingcart")
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //var itemProducts;
            ShipTaxService shipTaxService = new ShipTaxService();
            //List<BuyingItems> buyingItemslist = GetBuyingCartItems(buyingCartItems);
            Dictionary<string, decimal> shoppingCosts = new Dictionary<string, decimal>();
            Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail = new Dictionary<string, List<GetItemTaxDetail>>();

            //List<string> itemSellers = new List<string>();
            //itemSellers = db.Item.Where(y => (buyingItemslist.Select(x => x.buyItemID).ToList()).Contains(y.item_id)).Select(z => z.item_id).Distinct().ToList();
            List<int> returnIDs = new List<int>();
            if (PostData == null || PostData.Count <= 0)
            {
                return shipTaxService;
            }
            var buyitemIDs = PostData.Select(x => x.buyItemID).Distinct();
            if (buyitemIDs.Count() <= 0)
            {
                return shipTaxService;
            }
            var itemIDSellerID = PostData.Select(z => new { item_sellerid = z.buyItemID_Seller, item_id = z.buyItemID }).ToList();
            if (itemIDSellerID.Count <= 0)
            {
                return shipTaxService;
            }
            returnIDs.AddRange(buyitemIDs.ToList());
            int sellerItems = new int();
            foreach (var returnID in returnIDs) //count each item, not all seller's items
            {
                List<string> itemforSP = new List<string>();//this is feeding for stored procedure
                List<string> itemlistforSP = new List<string>();//this is feeding for stored procedure
                List<string> priceforSP = new List<string>();//this is feeding for stored procedure
                List<decimal> priceforSPTemp = new List<decimal>();//this is feeding for stored procedure
                List<decimal> eachweight = new List<decimal>();//save each item and itemlists's weight
                List<int> eachItemNumber = new List<int>();
                List<decimal> eachItemShippingFee = new List<decimal>();

                decimal weight = new decimal();
                weight = 0;
                sellerItems = itemIDSellerID.Where(x => x.item_id == returnID).Select(y => y.item_sellerid).FirstOrDefault();
                var items = PostData.Where(x => x.buyItemID == returnID).FirstOrDefault();
                int itemID;
                if (items.item_AttrID != null && items.item_AttrID != 0)
                {
                    itemID = items.item_AttrID.Value;

                    itemforSP.Add(items.buyItemID.ToString());//set item id into string 
                    itemlistforSP.Add(items.item_AttrID.Value.ToString());//set itemlist id into string, if item had attribute

                    decimal tempWeight = GetItemWeight(itemID, items.buyingNumber, "Itemlist");
                    weight += tempWeight;
                    eachweight.Add(tempWeight); //set each item's weight
                }
                else
                {
                    itemID = items.buyItemID;

                    itemforSP.Add(items.buyItemID.ToString());//set item id into string 
                    itemlistforSP.Add("0");//set itemlist id into string, if item had no attribute

                    decimal tempWeight = GetItemWeight(itemID, items.buyingNumber, "Item");
                    weight += tempWeight;
                    eachweight.Add(tempWeight);//set each item's weight
                }

                foreach (var itemlists in items.buyItemLists)
                {
                    int itemlistID;
                    if (itemlists.item_AttrID != null && itemlists.item_AttrID != 0)
                    {
                        itemlistID = itemlists.item_AttrID.Value;

                        itemlistforSP.Add(itemlists.item_AttrID.Value.ToString());//set itemlist id into string, if item had no attribute

                    }
                    else
                    {
                        itemlistID = itemlists.buyItemlistID;

                        itemlistforSP.Add(itemlists.buyItemlistID.ToString());//set itemlist id into string, if item had no attribute
                    }

                    itemforSP.Add(items.buyItemID.ToString());//set item id into string

                    decimal tempWeight = GetItemWeight(itemlistID, itemlists.buyingNumber, "Itemlist");
                    weight += tempWeight;
                    eachweight.Add(tempWeight); //set itemlist's weight

                }

                decimal totalItemShipping = GetItemShipping(weight, sellerItems, shoptype);
                shoppingCosts.Add(returnID.ToString(), totalItemShipping); //count shipping cost

                foreach (var itemweight in eachweight)
                {
                    priceforSPTemp.Add(Convert.ToInt32(Math.Round(((totalItemShipping * itemweight) / weight), 0, MidpointRounding.AwayFromZero))); //count each item's shipping cost
                }
                decimal totalPriceforSP = new decimal();
                totalPriceforSP = 0;
                foreach (var eachPrice in priceforSPTemp)
                {
                    totalPriceforSP += eachPrice;
                }
                priceforSPTemp[priceforSPTemp.Count - 1] += (totalItemShipping - totalPriceforSP);
                foreach (var eachPrice in priceforSPTemp)
                {
                    priceforSP.Add(eachPrice.ToString());
                }

                for (int i = 0; i < priceforSP.Count; i++)
                {
                    eachItemShippingFee.Add(Convert.ToDecimal(priceforSP[i]));
                    priceforSP[i] = (Convert.ToInt32(priceforSP[i]) / PostData[i].buyingNumber).ToString();
                    eachItemNumber.Add(PostData[i].buyingNumber);
                }

                string taxItem = "", taxItemlist = "", taxShippingPrice = "";

                for (int i = 0; i < itemforSP.Count; i++) //set string that for stored procedure
                {
                    if (taxItem != "")
                    {
                        taxItem += ",";
                        taxItemlist += ",";
                        taxShippingPrice += ",";
                    }
                    if (itemlistforSP[i] == "0")
                    {
                        taxItem += itemforSP[i];
                    }
                    else
                    {
                        //taxItem += itemforSP[i];
                        taxItem += "0";
                    }
                    taxItemlist += itemlistforSP[i];
                    taxShippingPrice += priceforSP[i];
                }

                itemTaxDetail.Add(returnID.ToString(), GetItemTaxDetails(taxItem, taxItemlist, taxShippingPrice).ToList()); //add itemtaxdetial by item id not seller id

                SetItemTaxDetailData(itemTaxDetail, returnID.ToString(), itemforSP, totalItemShipping, eachItemNumber, eachItemShippingFee, shoptype); //set item tax detial
                //GetItemTaxDetails("0,0,0", "73,74,110", "174,174,174");///////////////////////////////////
                //////////////////////////////////
            }

            shipTaxService.ShippingCost = shoppingCosts;//set total shipping cost




            shipTaxService.ShippingTaxCost = itemTaxDetail; //set item tax detail
            return shipTaxService;


        }
        /// <summary>
        /// acturally this function was changed all by Steven.S.Chen, so... XD  
        /// </summary>
        /// <param name="itemTaxDetail"></param>
        /// <param name="sellerID"></param>
        /// <param name="itemforSP"></param>
        /// <param name="totalItemShipping"></param>
        private void SetItemTaxDetailData(Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail, string sellerID, List<string> itemforSP, decimal totalItemShipping, List<int> eachItemNumber, List<decimal> eachItemShippingFee, string shoptype = "shoppingcart")
        {
            _itemPrice.SetItemTaxDetailData(itemTaxDetail, sellerID, itemforSP, totalItemShipping, eachItemNumber, eachItemShippingFee, shoptype);
        }
        /// <summary>
        /// acturally this function was changed all by Steven.S.Chen, so... XD  
        /// </summary>
        /// <param name="itemTaxDetail"></param>
        /// <param name="sellerID"></param>
        /// <param name="itemforSP"></param>
        /// <param name="totalItemShipping"></param>
        private void SetItemTaxDetailData20140402(Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail, string sellerID, List<string> itemforSP, decimal totalItemShipping,string shoptype = "shoppingcart")
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            int i = 0;
            foreach (var itemTax in itemTaxDetail[sellerID])
            {
                itemTax.item_id = int.Parse(itemforSP[i]);
                Item itemdetail = db.Item.Where(x => x.ID == itemTax.item_id).FirstOrDefault();
                int seller_id = itemdetail.SellerID;
                int? seller_country = db.Seller.Where(x => x.ID == seller_id).Select(x => x.CountryID).FirstOrDefault();
                string seller_currencytype = db.Seller.Where(x => x.ID == seller_id).Select(x => x.CurrencyType).FirstOrDefault();

                string v0 = itemTax.pricetaxdetail.Split(',')[0];//原產地
                string v1 = itemTax.pricetaxdetail.Split(',')[1];//台幣售價
                string v2 = itemTax.pricetaxdetail.Split(',')[2];//稅賦
                string v3 = itemTax.pricetaxdetail.Split(',')[3];//服務費
                string v4 = itemTax.pricetaxdetail.Split(',')[4];//關稅
                string v5 = itemTax.pricetaxdetail.Split(',')[5];//VAT
                string v6 = itemTax.pricetaxdetail.Split(',')[6];//貨物稅
                string v7 = itemTax.pricetaxdetail.Split(',')[7];//推廣貿易服務費
                
                if (seller_country == 1)
                {
                    if (itemdetail.DelvType != 3)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 0)
                    {
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                    }
                    else if (itemdetail.DelvType == 2)
                    {
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                    }
                    else if (itemdetail.DelvType == 7)
                    {
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                    }
                    else if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString();//台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString();//itemTax.pricetaxdetail.Split(',')[2];//稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();
                        }
                    }
                    else {
                        v1 = (itemdetail.PriceCash + itemdetail.PriceGlobalship + itemdetail.Taxfee).ToString();
                        itemTax.pricetaxdetail = "\"---\"" + "," + itemdetail.PriceCash + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                    }
                }
                else if (seller_country == 2) //美國
                {
                    if (itemdetail.DelvType != 3)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString();//台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString();//itemTax.pricetaxdetail.Split(',')[2];//稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();
                        }
                    }
                    else
                    {
                        itemTax.pricetaxdetail = "\"" + seller_currencytype + " " + v0 + "\"," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                    }
                }
                else if (seller_country == 4) //中國大陸
                {
                    if (itemdetail.DelvType != 3)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString();//台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString();//itemTax.pricetaxdetail.Split(',')[2];//稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();
                        }
                    }
                    if (shoptype == "shoppingcart")
                    {
                        if (itemdetail.DelvType == 4)
                        {
                            if (itemdetail.PriceGlobalship != null)
                            {
                                v1 = ((int)(itemdetail.PriceCash + itemdetail.PriceGlobalship)).ToString();
                            }
                            if (itemdetail.Taxfee != null)
                            {
                                v1 = ((int)(itemdetail.PriceCash + itemdetail.Taxfee)).ToString();
                            }
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + "0" + "," + "0" + "," + "0" + "," + "0" + "," + "0" + "," + "0";
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                    }
                    else
                    {
                        itemTax.pricetaxdetail = "\"" + seller_currencytype + " " + v0 + "\"," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        itemTax.pricetaxdetail += "," + totalItemShipping.ToString();                   
                    }
                }               
                else
                {
                    if (itemdetail.DelvType != 3)
                    {
                        v3 = "0";
                    }
                    if (itemdetail.DelvType == 6)
                    {
                        v1 = ((int)Math.Floor((double)itemdetail.PriceCash + 0.6)).ToString();//台幣售價
                        v2 = "0";
                        if (itemdetail.Taxfee != null)
                        {
                            v2 = ((int)(itemdetail.Taxfee)).ToString();//itemTax.pricetaxdetail.Split(',')[2];//稅賦
                        }
                        else
                        {
                            v2 = "0";
                        }
                        itemTax.pricetaxdetail = "\"---\"" + "," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                        if (itemdetail.PriceGlobalship != null)
                        {
                            itemTax.pricetaxdetail += "," + ((int)Math.Floor((double)itemdetail.PriceGlobalship + 0.6)).ToString();
                        }
                        else
                        {
                            itemTax.pricetaxdetail += "," + (0).ToString();
                        }
                    }
                    itemTax.pricetaxdetail = "\"" + seller_currencytype + " " + v0 + "\"," + v1 + "," + v2 + "," + v3 + "," + v4 + "," + v5 + "," + v6 + "," + v7;
                    itemTax.pricetaxdetail += "," + totalItemShipping.ToString();
                }

                i++;
            }

        }
        /// <summary>
        /// call stored procedure to get item tax detail
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="itemlistid"></param>
        /// <param name="shippingpricelist"></param>
        /// <returns></returns>
        private IEnumerable<GetItemTaxDetail> GetItemTaxDetails(string itemid, string itemlistid, string shippingpricelist)
        {
            return _itemPrice.GetItemTaxDetails(itemid, itemlistid, shippingpricelist);
        }
        /// <summary>
        /// call stored procedure to get item tax detail
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="itemlistid"></param>
        /// <param name="shippingpricelist"></param>
        /// <returns></returns>
        private IEnumerable<GetItemTaxDetail> GetItemTaxDetails20140402(string itemid, string itemlistid, string shippingpricelist)
        {
            //List<GetItemTaxDetail> ItemTaxDetail = new List<GetItemTaxDetail>();
            //TWSqlDBContext db_SP = new TWSqlDBContext();
            using (var db_SP = new TWNewEgg.DB.TWSqlDBContext())
            {

                db_SP.Database.Initialize(force: false);
                var cmd = db_SP.Database.Connection.CreateCommand();



                SqlParameter paramOne = new SqlParameter();
                paramOne.ParameterName = "@itemid";
                paramOne.Value = itemid;
                cmd.Parameters.Add(paramOne);

                SqlParameter paramTwo = new SqlParameter();
                paramTwo.ParameterName = "@itemlistid";
                paramTwo.Value = itemlistid;
                cmd.Parameters.Add(paramTwo);

                SqlParameter paramThree = new SqlParameter();
                paramThree.ParameterName = "@shippingpricelist";
                paramThree.Value = shippingpricelist;
                cmd.Parameters.Add(paramThree);
                //shippingpricelist
                cmd.CommandText = "[dbo].[UP_EC_GetItemTaxDetail] @itemid, @itemlistid, @shippingpricelist";

                try
                {
                    db_SP.Database.Connection.Open();
                    var reader = cmd.ExecuteReader();

                    IEnumerable<GetItemTaxDetail> ItemTaxDetail = ((IObjectContextAdapter)db_SP).ObjectContext.Translate<GetItemTaxDetail>(reader, "GetItemTaxDetail", MergeOption.NoTracking).ToList();

                    return ItemTaxDetail;

                }
                catch (Exception e)
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// receve string from user client and transformer to model object
        /// </summary>
        /// <param name="postData"></param>
        /// <returns></returns>
        public List<BuyingItems> GetBuyingCartItems(string postData)
        {
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"66\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"62\",\"buyingNumber\":\"2\"}]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"76\",\"buyingNumber\":\"6\"}]}]";
            //postData = "[{\"buyItemID\":\"58\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"65\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[]},{\"buyItemID\":\"70\",\"item_AttrID\":\"72\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"74\",\"buyingNumber\":\"2\"},{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]},{\"buyItemID\":\"71\",\"item_AttrID\":\"\",\"buyingNumber\":\"1\",\"buyItemLists\":[{\"buyItemlistID\":\"75\",\"buyingNumber\":\"6\"}]}]";
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<BuyingItems> PostData = new List<BuyingItems>();
            string newPostData = postData.Replace("null", "");
            newPostData = newPostData.Replace(",\"Coupons\":}", ",\"Coupons\":{}}");
            try
            {
                PostData = serializer.Deserialize<List<BuyingItems>>(newPostData); //string to model object

                return PostData;
            }
            catch (Exception e)
            {
                PostData.Add(new BuyingItems { buyItemID = 0, buyingNumber = 0 });
                return PostData;
            }

        }

        /// <summary>
        /// count item and itemlist's length, width, and weight and compare with each weight, choose max one 
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="number"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private decimal GetItemWeight(int itemIDs, int number, string table)
        {
            return _itemPrice.GetItemWeight(itemIDs, number, table);
        }
        private decimal GetItemWeight20140402(int itemIDs, int number, string table)
        {
            decimal over5KG = Decimal.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("ProductWeightOver5KG"));
            decimal below5KG = Decimal.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("ProductWeightBelow5KG"));
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //Get itemID or itemlistID

            var itemData = db.Item.Where(x => x.ID == itemIDs).FirstOrDefault();
            if (itemData.DelvType == 6)
            {
                over5KG = 1.0M;
                below5KG = 1.0M;
            }
            int sellerid = itemData.SellerID;
            int countryID = (int)db.Seller.Where(x => x.ID == sellerid).FirstOrDefault().CountryID;

            if (countryID == 4)
            {
                if (sellerid == 5) //沒有乘上數量
                {
                    int ID = new int();
                    if (table == "Item")
                    {
                        ID = db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                    }
                    else if (table == "Itemlist")
                    {
                        ID = db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                    }
                    if (ID != null || ID != 0)
                    {
                        //Get Product's Volume
                        var volume = db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                        if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                        {
                            string path = HttpContext.Current.Server.MapPath("~/Log/Product_Value/");
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                            LogtoFileWrite(path, writeStringend);
                        }

                        //Select which one is more heavy
                        decimal weight = ((volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 6000);

                        if (weight <= volume.product_weight.Value)
                        {
                            weight = volume.product_weight.Value;
                        }

                        //Set Weight 
                        return weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("GetItemWeight ERROR");
                    }
                }
                else if (sellerid == 6)
                {
                    int ID = new int();
                    if (table == "Item")
                    {
                        ID = db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                    }
                    else if (table == "Itemlist")
                    {
                        ID = db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                    }
                    if (ID != null || ID != 0)
                    {
                        //Get Product's Volume
                        var volume = db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                        if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                        {
                            string path = HttpContext.Current.Server.MapPath("~/Log/Product_Value/");
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                            LogtoFileWrite(path, writeStringend);
                        }

                        //Select which one is more heavy
                        decimal weight = ((volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 6000);

                        if (weight <= volume.product_weight.Value)
                        {
                            weight = volume.product_weight.Value;
                        }

                        //Set Weight * Number 
                        weight = weight * number;

                        return weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("GetItemWeight ERROR");
                    }
                }
                else
                {
                    int ID = new int();
                    if (table == "Item")
                    {
                        ID = db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                    }
                    else if (table == "Itemlist")
                    {
                        ID = db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                    }
                    if (ID != null || ID != 0)
                    {
                        //Get Product's Volume
                        var volume = db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                        if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                        {
                            string path = HttpContext.Current.Server.MapPath("~/Log/Product_Value/");
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                            LogtoFileWrite(path, writeStringend);
                        }

                        //Select which one is more heavy
                        decimal weight = ((volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 6000);

                        if (weight <= volume.product_weight.Value)
                        {
                            weight = volume.product_weight.Value;
                        }

                        //Set Weight * Number 
                        weight = weight * number;

                        return weight;
                    }
                    else
                    {
                        throw new ArgumentNullException("GetItemWeight ERROR");
                    }
                }
            }
            else //if(sellerid == 1 || sellerid == 2 || sellerid == 3  )
            {         
                int ID = new int();
                if (table == "Item")
                {
                    ID = db.Item.Where(x => x.ID == itemIDs).FirstOrDefault().ProductID;
                }
                else if (table == "Itemlist")
                {
                    ID = db.ItemList.Where(x => x.ID == itemIDs).FirstOrDefault().ItemlistProductID;
                }
                if (ID != null || ID != 0)
                {
                    //Get Product's Volume
                    var volume = db.Product.Where(x => x.ID == ID).Select(y => new { product_length = y.Length, product_width = y.Width, product_height = y.Height, product_weight = y.Weight }).FirstOrDefault();

                    if (volume.product_length == null || volume.product_width == null || volume.product_height == null || volume.product_weight == null)
                    {
                        string path = HttpContext.Current.Server.MapPath("~/Log/Product_Value/");
                        string writeStringend = "";
                        writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  Item_id:{1}　Product_id:{2}　table:{3}　材積有誤!!", DateTime.Now, itemIDs, ID, table) + Environment.NewLine;
                        LogtoFileWrite(path, writeStringend);
                    }

                    //Select which one is more heavy
                    decimal weight = ((volume.product_length.Value * volume.product_width.Value * volume.product_height.Value) / 5000);

                    if (weight <= volume.product_weight.Value)
                    {
                        weight = volume.product_weight.Value;
                    }

                    //Set Weight * Number 
                    weight = weight * number;

                    //Set Weight * 2 for shipping, coz the original product didn't package, so weight may need more heavy.
                    if (weight >= 5)
                    {
                        weight = weight * over5KG;
                    }
                    else
                    {
                        weight = weight * below5KG;
                    }

                    return weight;
                }
                else
                {
                    throw new ArgumentNullException("GetItemWeight ERROR");
                }
            }

        }

        /// <summary>
        /// detemer weight and get shipping cost from db
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="sellerID"></param>
        /// <returns></returns>
        private decimal GetItemShipping(decimal weight, int sellerID, string shoptype = "shoppingcart")
        {
            return _itemPrice.GetItemShipping(weight, sellerID, shoptype);
        }
        /// <summary>
        /// detemer weight and get shipping cost from db
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="sellerID"></param>
        /// <returns></returns>
        private decimal GetItemShipping20140402(decimal weight, int sellerID, string shoptype = "shoppingcart")
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //decimal newWeight = (weight / 1000);
            decimal newWeight = weight; //The unit in DB is kg. not g.
            decimal shippingCosts = new decimal();
            decimal? fulcharge = new decimal();
            //fulcharge = db.Logistic.Where(x => x.SellerID == sellerID).Select(y => y.FulCharge).FirstOrDefault() + 1;
            int CountryID = (int)db.Seller.Where(x => x.ID == sellerID).FirstOrDefault().CountryID;
            if (fulcharge == null || fulcharge < 1)
            {
                fulcharge = 1;
            }
            if (sellerID == 1) //This is for Taiwan's shipping, if the item is in taiwan so it don't need shipping cost.
            {
                fulcharge = 0;
            }

            if (CountryID == 1)
            { 
                return 0; 
            }
            else if (sellerID == 5)
            {
                if (shoptype == "paytype")
                {
                    if (newWeight == 0)
                    {
                        shippingCosts = 0;
                        return Math.Round(shippingCosts);
                    }

                    int over1kg = 0;
                    decimal first1kg = db.Logistic.Where(x => x.SellerID == sellerID).FirstOrDefault().Expense;
                    decimal currency = db.Currency.Where(x => x.CountryID == 4).OrderByDescending(x => x.CreateDate).FirstOrDefault().AverageexchangeRate;
                    if (sellerID == 5)
                    {
                        over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("JieMaiProductWeightOverPerkg"));
                    }
                    else if (sellerID == 6)
                    {
                        over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("NeweggCNProductWeightOverPerkg"));
                    }

                    if (newWeight == 0)
                    {
                        shippingCosts = 0;
                        return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (newWeight <= 1)
                    {
                        newWeight = Math.Ceiling(newWeight);
                        shippingCosts = first1kg;

                        return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        newWeight = Math.Ceiling(newWeight);
                        shippingCosts = first1kg + (newWeight - 1) * over1kg;
                        return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {
                    return 0;
                }
            }
            else if (sellerID == 6)
            {             
                if (newWeight == 0)
                {
                    shippingCosts = 0;
                    return Math.Round(shippingCosts);
                }

                int over1kg = 0;
                decimal first1kg = db.Logistic.Where(x => x.SellerID == sellerID).FirstOrDefault().Expense;
                decimal currency = db.Currency.Where(x => x.CountryID == 4).OrderByDescending(x => x.CreateDate).FirstOrDefault().AverageexchangeRate;
                if (sellerID == 5)
                {
                    over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("JieMaiProductWeightOverPerkg"));
                }
                else if (sellerID == 6)
                {
                    over1kg = int.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("NeweggCNProductWeightOverPerkg"));
                }

                if (newWeight == 0)
                {
                    shippingCosts = 0;
                    return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                }
                else if (newWeight <= 1)
                {
                    newWeight = Math.Ceiling(newWeight);
                    shippingCosts = first1kg;

                    return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    newWeight = Math.Ceiling(newWeight);
                    shippingCosts = first1kg + (newWeight - 1) * over1kg;
                    return Math.Round(shippingCosts * currency, 0, MidpointRounding.AwayFromZero);
                }                             
            }
            else
            {
                if (newWeight == 0)
                {
                    shippingCosts = 0;
                    return Math.Round(shippingCosts * 0, 0, MidpointRounding.AwayFromZero);
                }
                else if (newWeight <= 99 && newWeight > 0) // if weight over 0kg and below 21kg then get shipping cost form db
                {
                    Logistic Logisticitem = new Logistic();
                    //Logisticitem = db.Logistic.Where(x => x.Weight > newWeight && x.Weight <= (newWeight + 0.5M) && x.SellerID == sellerID).FirstOrDefault();
                    Logisticitem = db.Logistic.Where(x => x.Weight >= newWeight && x.SellerID == sellerID).OrderBy(x => x.Weight).FirstOrDefault();
                    shippingCosts = Logisticitem.Expense;
                    fulcharge = Logisticitem.FulCharge;
                    //shippingCosts = db.Logistic.Where(x => x.Weight > newWeight && x.Weight <= (newWeight + 0.5M) && x.SellerID == sellerID).Select(y => y.Expense).FirstOrDefault();
                    if (shippingCosts == null || shippingCosts == 0) 
                    {
                        if ((newWeight + 0.5M) > 99M)
                        {
                            shippingCosts = 170 * (Math.Ceiling(newWeight));
                        }
                        else
                        {
                            shippingCosts = 0; 
                            string path = HttpContext.Current.Server.MapPath("~/Log/Product_Value/");
                            string writeStringend = "";
                            writeStringend = string.Format("{0:yyyy/MM/dd HH:mm:ss}  運費有誤!!", DateTime.Now) + Environment.NewLine;
                            LogtoFileWrite(path, writeStringend);
                        }
                    }
                    if (fulcharge == null) { fulcharge = 1; }
                    
                    //return Math.Ceiling(shippingCosts * fulcharge.Value);
                    return Math.Round(shippingCosts * (fulcharge.Value+1), 0, MidpointRounding.AwayFromZero);
                }
                /*else if (newWeight < 45 && newWeight >= 21)  // if weight over 21kg and below 45kg then each kg plus 170
                {
                    shippingCosts = 170 * (Math.Round(newWeight, 2, MidpointRounding.AwayFromZero));
                    return Math.Round(shippingCosts * fulcharge.Value, 0, MidpointRounding.AwayFromZero);
                }*/
                //else if (newWeight < 71 && newWeight >= 45) // if weight over 45kg and below 71kg then each kg plus 163
                else if (newWeight > 99) // if weight over 45kg then each kg plus 163
                {
                    shippingCosts = 163 * (Math.Ceiling(newWeight));
                    fulcharge = 1.18M;
                    return Math.Round(shippingCosts * fulcharge.Value, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    throw new Exception("Too Light");
                }
            }
        }

        public void LogtoFileWrite(string path, string writeStringendtoFile)
        {

            string filename = path + string.Format("\\{0:yyyy}\\{0:MM}\\{0:yyyy-MM-dd}.txt", DateTime.Now);
            FileInfo finfo = new FileInfo(filename);

            if (finfo.Directory.Exists == false)
            {
                finfo.Directory.Create();
            }

            System.IO.File.AppendAllText(filename, writeStringendtoFile, Encoding.Unicode);

        }

        public decimal getTotalWeight(List<BuyingItems> PostData)
        {
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //var itemProducts;
            ShipTaxService shipTaxService = new ShipTaxService();
            //List<BuyingItems> buyingItemslist = GetBuyingCartItems(buyingCartItems);
            Dictionary<string, decimal> shoppingCosts = new Dictionary<string, decimal>();
            Dictionary<string, List<GetItemTaxDetail>> itemTaxDetail = new Dictionary<string, List<GetItemTaxDetail>>();
            decimal weight = new decimal();
            weight = 0;
            //List<string> itemSellers = new List<string>();
            //itemSellers = db.Item.Where(y => (buyingItemslist.Select(x => x.buyItemID).ToList()).Contains(y.item_id)).Select(z => z.item_id).Distinct().ToList();
            List<int> returnIDs = new List<int>();
            if (PostData == null || PostData.Count <= 0)
            {
                return 0;
            }
            var buyitemIDs = PostData.Select(x => x.buyItemID).Distinct();
            if (buyitemIDs.Count() <= 0)
            {
                return 0;
            }
            var itemIDSellerID = PostData.Select(z => new { item_sellerid = z.buyItemID_Seller, item_id = z.buyItemID }).ToList();
            if (itemIDSellerID.Count <= 0)
            {
                return 0;
            }
            returnIDs.AddRange(buyitemIDs.ToList());
            int sellerItems = new int();
            foreach (var returnID in returnIDs) //count each item, not all seller's items
            {
                List<string> itemforSP = new List<string>();//this is feeding for stored procedure
                List<string> itemforSP1 = new List<string>();
                List<string> itemlistforSP = new List<string>();//this is feeding for stored procedure
                List<string> priceforSP = new List<string>();//this is feeding for stored procedure
                List<decimal> eachweight = new List<decimal>();//save each item and itemlists's weight
                List<int> count = new List<int>();
                sellerItems = itemIDSellerID.Where(x => x.item_id == returnID).Select(y => y.item_sellerid).FirstOrDefault();
                var items = PostData.Where(x => x.buyItemID == returnID).FirstOrDefault();
                int itemID;
                if (items.item_AttrID != null && items.item_AttrID != 0)
                {
                    itemID = items.item_AttrID.Value;

                    itemforSP.Add(items.buyItemID.ToString());//set item id into string 
                    itemforSP1.Add("0");
                    //itemforSP1.Add(items.pricecost.ToString());
                    itemlistforSP.Add(items.item_AttrID.Value.ToString());//set itemlist id into string, if item had attribute

                    decimal tempWeight = GetItemWeight(itemID, items.buyingNumber, "Itemlist");
                    count.Add(items.buyingNumber);
                    if (sellerItems != 1) //If Country is Taiwan then product weight equal zero.
                    {
                        weight += tempWeight;
                    }
                    else
                    {
                        weight += 0;
                    }
                    eachweight.Add(tempWeight); //set each item's weight
                }
                else
                {
                    itemID = items.buyItemID;

                    itemforSP.Add(items.buyItemID.ToString());//set item id into string 
                    itemforSP1.Add("0");
                    //itemforSP1.Add(items.pricecost.ToString());
                    itemlistforSP.Add("0");//set itemlist id into string, if item had no attribute

                    decimal tempWeight = GetItemWeight(itemID, items.buyingNumber, "Item");
                    count.Add(items.buyingNumber);
                    if (sellerItems != 1)  //If Country is Taiwan then product weight equal zero.
                    {
                        weight += tempWeight;
                    }
                    else
                    {
                        weight += 0;
                    }
                    eachweight.Add(tempWeight);//set each item's weight
                }

                if (items.buyItemLists != null)
                {
                    foreach (var itemlists in items.buyItemLists)
                    {
                        int itemlistID;
                        if (itemlists.item_AttrID != null && itemlists.item_AttrID != 0)
                        {
                            itemlistID = itemlists.item_AttrID.Value;

                            itemlistforSP.Add(itemlists.item_AttrID.Value.ToString());//set itemlist id into string, if item had no attribute

                        }
                        else
                        {
                            itemlistID = itemlists.buyItemlistID;

                            itemlistforSP.Add(itemlists.buyItemlistID.ToString());//set itemlist id into string, if item had no attribute
                        }

                        itemforSP.Add(items.buyItemID.ToString());//set item id into string

                        decimal tempWeight = GetItemWeight(itemlistID, itemlists.buyingNumber, "Itemlist");
                        if (sellerItems != 1)  //If Country is Taiwan then product weight equal zero.
                        {
                            weight += tempWeight;
                        }
                        else
                        {
                            weight += 0;
                        }
                        eachweight.Add(tempWeight); //set itemlist's weight

                    }
                }
            }
            return weight;
        }

        /// <summary>
        /// Get all parent categories by item ids.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetRootCategorybyItemIDs(List<int> itemIDs)
        {
            Dictionary<string, string> itemParentCategoet = new Dictionary<string, string>();
            TWNewEgg.DB.TWSqlDBContext db = new TWNewEgg.DB.TWSqlDBContext();
            //var allCategorystring = db.Item.Where(x => itemIDs.Contains(x.ID)).GroupJoin(db.Category, x => x.CategoryID, y => y.ID, (x, yx) => new { x, yx = yx.DefaultIfEmpty() }).SelectMany(z => z.yx.DefaultIfEmpty().Select(y => new { x = z.x, y })).ToString(); //Left Join
            var allCategory = db.Item.Where(x => itemIDs.Contains(x.ID)).GroupJoin(db.Category, x => x.CategoryID, y => y.ID, (x, yx) => new { x, yx = yx.DefaultIfEmpty() }).SelectMany(z => z.yx.DefaultIfEmpty().Select(y => new { x = z.x, y })).ToList();
            allCategory = allCategory.Where(x => x.y != null).ToList();
            
            foreach (var aCategory in allCategory)
            {
                List<MapPath> allCategories;
                allCategories = _categoryApiService.GetParents(aCategory.y.ID, aCategory.y.Layer, null, null, null).ToList();
                if (allCategories.Count > 0)
                {
                    //itemParentCategoet.Add(aCategory.x.ID.ToString(), allCategories[(allCategories.Count - 1)].category_id.ToString());
                    itemParentCategoet.Add(aCategory.x.ID.ToString(), string.Join(",", allCategories.Select(x => x.category_id).ToArray()));
                }
            }

            //db.Dispose();
            return itemParentCategoet;
        }

        /// <summary>
        /// 取得SellingQty
        /// </summary>
        /// <param name="argListItemId">list of Item Id</param>
        /// <returns>Dictionary<ItemId, SellingQty></returns>
        private Dictionary<int, int> GetSellingQtyByItemIdList(List<int> argListItemId)
        {
            Dictionary<int, int> dictItemSellingQty = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty> listItemSellingQty = null;
            TWNewEgg.Models.DBModels.TWSQLDB.View_ItemSellingQty objTempSellingQty = null;

            if (argListItemId == null || argListItemId.Count <= 0)
            {
                return null;
            }
            
            //設定Qty預設值皆為0
            dictItemSellingQty = argListItemId.Select(x => new { Key = x, Value = 0 }).ToDictionary(x => x.Key, x => x.Value);

            //取得Qty
            listItemSellingQty = this._ItemStockRepo.GetSellingQtyByItemList(argListItemId).ToList();
            if (listItemSellingQty != null && listItemSellingQty.Count > 0)
            {
                foreach(int numItemId in argListItemId)
                {
                    objTempSellingQty = listItemSellingQty.Where(x=>x.ID == numItemId).FirstOrDefault();
                    dictItemSellingQty[numItemId] = objTempSellingQty != null ? objTempSellingQty.SellingQty ?? 0 : 0;
                }
            }
            return dictItemSellingQty;
        }
    }
}