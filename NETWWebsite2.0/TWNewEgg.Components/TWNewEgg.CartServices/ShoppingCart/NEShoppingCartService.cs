using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Models.DomainModels.ShoppingCartPayType;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.TrackRepoAdapters.Interface;
using TWNewEgg.ShoppingCartServices.Interface;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.StoredProceduresRepoAdapters.Interface;
using TWNewEgg.StoreRepoAdapters.Interface;

namespace TWNewEgg.CartServices.ShoppingCart
{
    public class NEShoppingCartService : INEShoppingCartService
    {
        private ITrackRepoAdapter _trackRepoAdapter;
        private IShoppingCartService _shoppingCartService;
        private IStoredProceduresRepoAdapter _iStoredProceduresRepoAdapter;
        private IStoreRepoAdapter _iStoreRepoAdapter;

        public NEShoppingCartService(ITrackRepoAdapter trackRepoAdapter, IStoredProceduresRepoAdapter storedProceduresRepoAdapter, IStoreRepoAdapter storeRepoAdapter, IShoppingCartService shoppingCartService)
        {
            this._trackRepoAdapter = trackRepoAdapter;
            this._shoppingCartService = shoppingCartService;
            this._iStoredProceduresRepoAdapter = storedProceduresRepoAdapter;
            this._iStoreRepoAdapter = storeRepoAdapter;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="cartType"></param>
        /// <returns></returns>
        public DM_ShoppingCart GetShoppingCart(int accountID, int cartType)
        {
            DM_ShoppingCart DM_ShoppingCartModel = new DM_ShoppingCart();
            List<ShoppingCartDM> listShoppingCart = new List<ShoppingCartDM>();
            List<ShoppingCartDM> listShoppingCartNew = new List<ShoppingCartDM>();
            int itemsId ;
            List<int> listItem = new List<int>();
            ShoppingCartDM shoppingCart = new ShoppingCartDM();
            List<Track> accountIdCartItem = new List<Track>();
            string cartTypeName = string.Empty;
            //取出購買人購物車所有商品
            //accountIdCartItem = _trackRepoAdapter.ReadTracks(accountID, null, null, null).ToList();
            //carttype
            listShoppingCart = _shoppingCartService.GetCartAllList(accountID);
            //listShoppingCartNew = GetAllCartAllList(accountID);
            switch (cartType)
            { 
               
                case 1:
                    cartTypeName = DM_ShoppingCart.CartType.新蛋購物車.ToString();
                    break;
                case 2:
                    cartTypeName = DM_ShoppingCart.CartType.海外購物車.ToString();
                    break;
                case 3:
                    cartTypeName = DM_ShoppingCart.CartType.任選館購物車.ToString();
                    break;
                case 4:
                    cartTypeName = DM_ShoppingCart.CartType.追蹤清單購物車.ToString();
                    break;
                
            }

            shoppingCart = listShoppingCart.Where(x => x.Name == cartTypeName).FirstOrDefault();
            foreach (var cartItem in shoppingCart.CartItemClassList)
            {
                foreach (var cartItemList in cartItem.CartItemList)
                {
                    itemsId = cartItemList.ItemID;

                    listItem.Add(itemsId);
                }
            }
            DM_ShoppingCartModel.CarType = cartType;
            DM_ShoppingCartModel.Items = listItem;
            return DM_ShoppingCartModel;
        }

        private List<ShoppingCartDM> GetAllCartAllList(int account)
        {
            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
            Dictionary<string, List<ViewTracksCartItems>> CartAllList = this._iStoredProceduresRepoAdapter.GetShoppingAllCart(account);
            List<SubCategory_OptionStore> SubCategory_OptionStoreList = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x => x.ShowAll == (int)SubCategory_OptionStore.ConstShowAll.Show && (x.DateEnd > datetimeNow || x.DateEnd == null) && (x.DateStart < datetimeNow || x.DateStart == null)).ToList();
            List<int> SubCategory_OptionStoreID = SubCategory_OptionStoreList.Select(x => x.CategoryID ?? 0).ToList();

            //SellingQty
            TWNewEgg.Models.DomainModels.Item.View_ItemSellingQty objItemSellingQty = null;
            Dictionary<int, int> dictItemSellingQty = null;

            List<ShoppingCartDM> ShoppingCartDMList = new List<ShoppingCartDM>();
            //List<int> 
            List<List<ViewTracksCartItems>> viewTracksCartItemsListtemp = CartAllList.Where(x => x.Value.Count > 0).Select(x => x.Value).ToList();
            List<ViewTracksCartItems> viewTracksCartItemstemp = new List<ViewTracksCartItems>();
            foreach (var item in viewTracksCartItemsListtemp)
            {
                viewTracksCartItemstemp.AddRange(item);
            }
            List<int> ItemIDList = viewTracksCartItemstemp.Select(x => x.ItemID).ToList();
            List<int> SellerIDList = viewTracksCartItemstemp.Select(x => x.ItemSellerID).ToList();

            //Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> Sellerresult = this._iSellerServices.GetSellerWithCountryList(SellerIDList);
            //Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>> itemUrlDictionary = this._iItemImageUrlService.GetItemImagePath(ItemIDList);

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType)))
            {
                ShoppingCartDM ShoppingCartDMtemp = new ShoppingCartDM();
                ShoppingCartDMtemp.ID = item;
                ShoppingCartDMtemp.Name = Enum.GetName(typeof(TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType), item).ToString();
                ShoppingCartDMtemp.CartItemClassList = new List<CartItemClass>();
                ShoppingCartDMList.Add(ShoppingCartDMtemp);
            }

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType)))
            {
                if (item == (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.一般宅配)
                {
                    List<ViewTracksCartItems> cart1 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), item).ToString()].Where(x => !SubCategory_OptionStoreID.Contains(x.CategoryID)).ToList();
                    List<ViewTracksCartItems> cart2 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), item).ToString()].Where(x => SubCategory_OptionStoreID.Contains(x.CategoryID)).ToList();
                    if (cart1 != null)
                    {
                        if (cart1.Count != 0)
                        {
                            var CartItemList = ModelConverter.ConvertTo<List<CartItem>>(cart1);

                            List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.國內購物車加價商品).ToString()].ToList();

                            if (Cartadditional.Count != 0)
                            {
                                try
                                {
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional);
                                    foreach (var temp in CartItemListadditional)
                                    {
                                        temp.CategoryID = -999;
                                    }
                                    CartItemList.AddRange(CartItemListadditional);
                                }
                                catch (Exception e)
                                {

                                }
                            }

                            List<CartItemClass> CartItemClassList = new List<CartItemClass>();
                            CartItemClass CartItemClass = new CartItemClass();
                            CartItemClass.FreeCost = null;
                            CartItemClass.Title = TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車.ToString();
                            //CartItemClass.CartItemList = BulidCartItemList(CartItemList, Sellerresult, itemUrlDictionary);

                            //foreach (var CartItemListtemp in CartItemClass.CartItemList)
                            //{
                            //    objItemSellingQty = this._ItemStockService.GetItemSellingQtyByItemId(CartItemListtemp.ItemID);
                            //    CartItemListtemp.MaxQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                            //    if (CartItemListtemp.MaxQty > 10)
                            //    {
                            //        CartItemListtemp.MaxQty = 10;
                            //    }
                            //    if (CartItemListtemp.Qty > CartItemListtemp.MaxQty)
                            //    {
                            //        CartItemListtemp.Qty = CartItemListtemp.MaxQty;
                            //    }
                            //}
                            //CartItemClassList.Add(CartItemClass);

                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車).FirstOrDefault().CartItemClassList.AddRange(CartItemClassList);
                        }
                        else
                        {
                            List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.國內購物車加價商品).ToString()].ToList();
                            if (Cartadditional.Count != 0)
                            {
                                try
                                {
                                    int TrackStatus = (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.國內購物車加價商品;
                                    //ClearTheNumber0cart(account, TrackStatus);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                    if (cart2 != null)
                    {
                        if (cart2.Count != 0)
                        {
                            var CartItemList2 = ModelConverter.ConvertTo<List<CartItem>>(cart2);

                            List<ViewTracksCartItems> Cartadditional2 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.任選館購物車加價商品).ToString()].ToList();

                            if (Cartadditional2.Count != 0)
                            {
                                try
                                {
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional2);
                                    foreach (var temp in CartItemListadditional)
                                    {
                                        temp.CategoryID = -999;
                                    }
                                    CartItemList2.AddRange(CartItemListadditional);
                                }
                                catch (Exception e)
                                {

                                }
                            }

                            var ShoppingCartDMtemp = ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車).FirstOrDefault();

                            var CartItemListGroup = CartItemList2.GroupBy(x => x.CategoryID).ToList();

                            List<CartItemClass> CartItemClassList = new List<CartItemClass>();
                            foreach (var CartItemListGrouptemp in CartItemListGroup)
                            {
                                SubCategory_OptionStore SubCategory_OptionStore = SubCategory_OptionStoreList.Where(x => x.CategoryID == CartItemListGrouptemp.FirstOrDefault().CategoryID).FirstOrDefault();
                                if (SubCategory_OptionStore == null)
                                {
                                    SubCategory_OptionStore = new SubCategory_OptionStore();
                                    SubCategory_OptionStore.Title = "加購明細";
                                    SubCategory_OptionStore.FreeCost = 0;
                                    SubCategory_OptionStore.CategoryID = -999;
                                }
                                CartItemClass CartItemClass = new CartItemClass();
                                CartItemClass.Title = SubCategory_OptionStore.Title;
                                CartItemClass.FreeCost = SubCategory_OptionStore.FreeCost;
                                CartItemClass.CategoryID = SubCategory_OptionStore.CategoryID;
                                //CartItemClass.CartItemList = BulidCartItemList(CartItemList2.Where(x => x.CategoryID == CartItemListGrouptemp.Key).ToList(), Sellerresult, itemUrlDictionary);

                                decimal CartItemClassSumPrice = CartItemClass.CartItemList.Sum(x => x.Qty * x.NTPrice);

                                if (CartItemClassSumPrice < SubCategory_OptionStore.FreeCost)
                                {
                                    CartItemClass.IsCheckout = false;
                                }

                                //dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(CartItemClass.CartItemList.Select(x => x.ItemID).ToList());
                                //foreach (var CartItemListtemp in CartItemClass.CartItemList)
                                //{
                                //    CartItemListtemp.MaxQty = dictItemSellingQty[CartItemListtemp.ItemID];
                                //    if (CartItemListtemp.MaxQty > 10)
                                //    {
                                //        CartItemListtemp.MaxQty = 10;
                                //    }
                                //    if (CartItemListtemp.Qty > CartItemListtemp.MaxQty)
                                //    {
                                //        CartItemListtemp.Qty = CartItemListtemp.MaxQty;
                                //    }
                                //}
                                //CartItemClassList.Add(CartItemClass);
                            }

                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車).FirstOrDefault().CartItemClassList.AddRange(CartItemClassList);

                        }
                        else
                        {
                            List<ViewTracksCartItems> Cartadditional2 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.任選館購物車加價商品).ToString()].ToList();
                            if (Cartadditional2.Count != 0)
                            {
                                try
                                {
                                    int TrackStatus = (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.任選館購物車加價商品;
                                    //ClearTheNumber0cart(account, TrackStatus);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                }
                else if (item == (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.海外購物車)
                {
                    List<ViewTracksCartItems> cart1 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), item).ToString()].ToList();
                    if (cart1 != null)
                    {
                        if (cart1.Count != 0)
                        {
                            var CartItemList = ModelConverter.ConvertTo<List<CartItem>>(cart1);

                            List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.海外購物車加價商品).ToString()].ToList();

                            if (Cartadditional.Count != 0)
                            {
                                try
                                {
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional);
                                    foreach (var temp in CartItemListadditional)
                                    {
                                        temp.CategoryID = -999;
                                    }
                                    CartItemList.AddRange(CartItemListadditional);
                                }
                                catch (Exception e)
                                {

                                }
                            }

                            if (CartItemList.Where(x => x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem).Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList().Count > 0)
                            {
                                //dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(CartItemList.Where(x => x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem).Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList());
                            }

                            foreach (var CartItemListtemp in CartItemList)
                            {
                                if (CartItemListtemp.ItemDelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || CartItemListtemp.ItemDelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.海外切貨)
                                {
                                    CartItemListtemp.Qty = 1;
                                    CartItemListtemp.MaxQty = 1;
                                }
                                else
                                {
                                    CartItemListtemp.MaxQty = dictItemSellingQty[CartItemListtemp.ItemID];
                                    if (CartItemListtemp.MaxQty > 10)
                                    {
                                        CartItemListtemp.MaxQty = 10;
                                    }
                                    if (CartItemListtemp.Qty > CartItemListtemp.MaxQty)
                                    {
                                        CartItemListtemp.Qty = CartItemListtemp.MaxQty;
                                    }
                                }
                            }

                            List<CartItemClass> CartItemClassList = new List<CartItemClass>();
                            CartItemClass CartItemClass = new CartItemClass();
                            CartItemClass.FreeCost = null;
                            CartItemClass.Title = TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車.ToString();
                            //CartItemClass.CartItemList = BulidCartItemList(CartItemList, Sellerresult, itemUrlDictionary);
                            CartItemClassList.Add(CartItemClass);

                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車).FirstOrDefault().CartItemClassList.AddRange(CartItemClassList);
                        }
                        else
                        {
                            List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.海外購物車加價商品).ToString()].ToList();
                            if (Cartadditional.Count != 0)
                            {
                                try
                                {
                                    int TrackStatus = (int)TWNewEgg.Models.DBModels.TWSQLDB.Track.TrackStatus.海外購物車加價商品;
                                    //ClearTheNumber0cart(account, TrackStatus);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }
                    }
                }
                else if (item != (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.任選館購物車加價商品 && item != (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.海外購物車加價商品 && item != (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.國內購物車加價商品)
                {
                    List<ViewTracksCartItems> cart1 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), item).ToString()].ToList();
                    if (cart1 != null)
                    {
                        if (cart1.Count != 0)
                        {
                            var CartItemList = ModelConverter.ConvertTo<List<CartItem>>(cart1);

                            if (ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault().CartItemClassList.FirstOrDefault() == null)
                            {
                                List<CartItemClass> CartItemClassList = new List<CartItemClass>();
                                CartItemClass CartItemClass = new CartItemClass();
                                CartItemClass.FreeCost = null;
                                CartItemClass.Title = TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車.ToString();
                                CartItemClass.CartItemList = new List<CartItem>();
                                CartItemClassList.Add(CartItemClass);
                                ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault().CartItemClassList = CartItemClassList;
                            }

                            // 判斷可售數量
                            if (item == (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.一般追蹤 || item == (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.海外追蹤)
                            {
                                foreach (var CartItemListtemp in CartItemList)
                                {
                                    CartItemListtemp.CanAddtoCart = false;
                                    CartItemListtemp.MaxQty = 0;
                                    CartItemListtemp.Qty = 0;
                                }
                            }
                            else
                            {
                                foreach (var CartItemListtemp in CartItemList)
                                {
                                    CartItemListtemp.Qty = 1;
                                }
                            }
                            //ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault().CartItemClassList.FirstOrDefault().CartItemList.AddRange(BulidCartItemList(CartItemList, Sellerresult, itemUrlDictionary));
                        }
                    }
                }
            }

            foreach (var ShoppingCartDMitem in ShoppingCartDMList)
            {
                ShoppingCartDMitem.Qty = 0;
                foreach (var CartItemClassList in ShoppingCartDMitem.CartItemClassList)
                {
                    CartItemClassList.TypeQty = CartItemClassList.CartItemList.Count;
                    ShoppingCartDMitem.Qty = ShoppingCartDMitem.Qty + CartItemClassList.CartItemList.Count;
                }
            }

            return ShoppingCartDMList;
        }
    
    }
}
