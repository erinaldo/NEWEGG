using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartService.Interface;
using TWNewEgg.CategoryServices.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.SellerServices.Interface;
using TWNewEgg.ShoppingCartServices.Interface;
using TWNewEgg.StoredProceduresRepoAdapters.Interface;
using TWNewEgg.StoreRepoAdapters.Interface;
using TWNewEgg.TrackRepoAdapters.Interface;

namespace TWNewEgg.ShoppingCartServices
{
    public class ShoppingCartService : IShoppingCartService
    {
        private IStoredProceduresRepoAdapter _iStoredProceduresRepoAdapter;
        private IStoreRepoAdapter _iStoreRepoAdapter;
        private ISellerServices _iSellerServices;
        private IItemImageUrlService _iItemImageUrlService;
        private ICarts _carts;
        private IItemGroupService _ItemGroupService;
        private IItemStockService _ItemStockService;
        private ITrackRepoAdapter _trackRepoAdapter;
        private ICategoryServices _categoryService;

        // 依據 BSATW-173 廢四機需求增加
        // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160517
        private IItemInfoService _IItemInfoService;

        public ShoppingCartService(IStoredProceduresRepoAdapter iStoredProceduresRepoAdapter, IStoreRepoAdapter iStoreRepoAdapter, ISellerServices iSellerServices, IItemImageUrlService iItemImageUrlService, ICarts carts, IItemGroupService argItemGroupService, IItemStockService argItemStockService, ITrackRepoAdapter trackRepoAdapter
            , ICategoryServices categoryService,IItemInfoService iitemInfoService )
        {
            this._iStoredProceduresRepoAdapter = iStoredProceduresRepoAdapter;
            this._iStoreRepoAdapter = iStoreRepoAdapter;
            this._iSellerServices = iSellerServices;
            this._iItemImageUrlService = iItemImageUrlService;
            this._carts = carts;
            this._ItemGroupService = argItemGroupService;
            this._ItemStockService = argItemStockService;
            this._trackRepoAdapter = trackRepoAdapter;
            this._categoryService = categoryService;

            // 依據 BSATW-173 廢四機需求增加
            // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160517
            this._IItemInfoService = iitemInfoService;

        }

        /// <summary>
        /// 計算購物車各車數量與明細
        /// </summary>
        /// <param name="CartItemList"></param>
        /// <param name="Sellerresult"></param>
        /// <param name="itemUrlDictionary"></param>
        /// <returns></returns>
        public List<ShoppingCartDM> GetCartAllList(int account)
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

            Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> Sellerresult = this._iSellerServices.GetSellerWithCountryList(SellerIDList);
            Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>> itemUrlDictionary = this._iItemImageUrlService.GetItemImagePath(ItemIDList);
            List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem> itemCategory = this._categoryService.GetAllParentCategoriesByItemIDs(ItemIDList);

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

                            if (Cartadditional.Count != 0) {
                                try
                                {
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional);
                                    foreach (var temp in CartItemListadditional)
                                    {
                                        temp.CategoryID = -999;
                                    }
                                    CartItemList.AddRange(CartItemListadditional);
                                }
                                catch(Exception e){
                                
                                }
                            }

                            List<CartItemClass> CartItemClassList = new List<CartItemClass>();
                            CartItemClass CartItemClass = new CartItemClass();
                            CartItemClass.FreeCost = null;
                            CartItemClass.Title = TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車.ToString();
                            CartItemClass.CartItemList = BulidCartItemList(CartItemList, Sellerresult, itemUrlDictionary, itemCategory);

                            foreach (var CartItemListtemp in CartItemClass.CartItemList)
                            {
                                objItemSellingQty = this._ItemStockService.GetItemSellingQtyByItemId(CartItemListtemp.ItemID);
                                CartItemListtemp.MaxQty = objItemSellingQty != null ? objItemSellingQty.SellingQty ?? 0 : 0;
                                if (CartItemListtemp.MaxQty > 10)
                                {
                                    CartItemListtemp.MaxQty = 10;
                                }
                                if (CartItemListtemp.Qty > CartItemListtemp.MaxQty)
                                {
                                    CartItemListtemp.Qty = CartItemListtemp.MaxQty;
                                }
                            }
                            CartItemClassList.Add(CartItemClass);

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
                                    ClearTheNumber0cart(account, TrackStatus);
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
                                CartItemClass.CartItemList = BulidCartItemList(CartItemList2.Where(x => x.CategoryID == CartItemListGrouptemp.Key).ToList(), Sellerresult, itemUrlDictionary, itemCategory);

                                decimal CartItemClassSumPrice = CartItemClass.CartItemList.Sum(x => x.Qty * x.NTPrice);

                                if (CartItemClassSumPrice < SubCategory_OptionStore.FreeCost)
                                {
                                    CartItemClass.IsCheckout = false;
                                }

                                dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(CartItemClass.CartItemList.Select(x => x.ItemID).ToList());
                                foreach (var CartItemListtemp in CartItemClass.CartItemList)
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
                                CartItemClassList.Add(CartItemClass);
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
                                    ClearTheNumber0cart(account, TrackStatus);
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

                            if (CartItemList.Where(x => x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem).Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList().Count > 0) {
                                dictItemSellingQty = this._ItemStockService.GetSellingQtyByItemList(CartItemList.Where(x => x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForCart || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AddtionalItemForItem || x.ShowOrder == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.ShowOrderStatus.AdditionalItem).Select(x => x.ItemID).GroupBy(x => x).Select(x => x.Key).ToList());
                            }
                            
                            foreach (var CartItemListtemp in CartItemList)
                            {
                                if (CartItemListtemp.ItemDelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.三角 || CartItemListtemp.ItemDelvType == (int)TWNewEgg.Models.DBModels.TWSQLDB.Item.tradestatus.海外切貨)
                                {
                                    CartItemListtemp.Qty = 1;
                                    CartItemListtemp.MaxQty = 1;
                                }
                                else {
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
                            CartItemClass.CartItemList = BulidCartItemList(CartItemList, Sellerresult, itemUrlDictionary, itemCategory);
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
                                    ClearTheNumber0cart(account, TrackStatus);
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
                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault().CartItemClassList.FirstOrDefault().CartItemList.AddRange(BulidCartItemList(CartItemList, Sellerresult, itemUrlDictionary, itemCategory));
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

        /// <summary>
        /// 計算購物車各車數量
        /// </summary>
        /// <param name="CartItemList"></param>
        /// <param name="Sellerresult"></param>
        /// <param name="itemUrlDictionary"></param>
        /// <returns></returns>
        public List<ShoppingCartDM> GetCartAllListQty(int account)
        {
            DateTime datetimeNow = DateTime.UtcNow.AddHours(8);
            Dictionary<string, List<ViewTracksCartItems>> CartAllList = this._iStoredProceduresRepoAdapter.GetShoppingAllCart(account);
            List<SubCategory_OptionStore> SubCategory_OptionStoreList = this._iStoreRepoAdapter.OptionStore_GetAll().Where(x => x.ShowAll == (int)SubCategory_OptionStore.ConstShowAll.Show && (x.DateEnd > datetimeNow || x.DateEnd == null) && (x.DateStart < datetimeNow || x.DateStart == null)).ToList();
            List<int> SubCategory_OptionStoreID = SubCategory_OptionStoreList.Select(x => x.CategoryID ?? 0).ToList();

            List<ShoppingCartDM> ShoppingCartDMList = new List<ShoppingCartDM>();

            foreach (int item in Enum.GetValues(typeof(TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType)))
            {
                ShoppingCartDM ShoppingCartDMtemp = new ShoppingCartDM();
                ShoppingCartDMtemp.ID = item;
                ShoppingCartDMtemp.Name = Enum.GetName(typeof(TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType), item).ToString();
                ShoppingCartDMtemp.CartItemClassList = new List<CartItemClass>();
                ShoppingCartDMtemp.Qty = 0;
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
                            if (CartItemList.Count != 0)
                            {
                                try
                                {
                                    List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.國內購物車加價商品).ToString()].ToList();
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional);
                                    CartItemList.AddRange(CartItemListadditional);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.新蛋購物車).FirstOrDefault().Qty = CartItemList.Count();
                        }
                    }
                    if (cart2 != null)
                    {
                        if (cart2.Count != 0)
                        {
                            var CartItemList2 = ModelConverter.ConvertTo<List<CartItem>>(cart2);
                            if (CartItemList2.Count != 0)
                            {
                                try
                                {
                                    List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.任選館購物車加價商品).ToString()].ToList();
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional);
                                    CartItemList2.AddRange(CartItemListadditional);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.任選館購物車).FirstOrDefault().Qty = CartItemList2.Count();

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
                            if (CartItemList.Count != 0)
                            {
                                try
                                {
                                    List<ViewTracksCartItems> Cartadditional = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), (int)TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType.海外購物車加價商品).ToString()].ToList();
                                    var CartItemListadditional = ModelConverter.ConvertTo<List<CartItem>>(Cartadditional);
                                    CartItemList.AddRange(CartItemListadditional);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.海外購物車).FirstOrDefault().Qty = CartItemList.Count();
                        }
                    }
                }
                else
                {
                    List<ViewTracksCartItems> cart1 = CartAllList[Enum.GetName(typeof(TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems.ViewTracksType), item).ToString()].ToList();
                    if (cart1 != null)
                    {
                        if (cart1.Count != 0)
                        {
                            var CartItemList = ModelConverter.ConvertTo<List<CartItem>>(cart1);
                            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault().Qty = ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.追蹤清單購物車).FirstOrDefault().Qty + CartItemList.Count();
                        }
                    }
                }
            }

            ShoppingCartDMList.Where(x => x.ID == (int)TWNewEgg.Models.DomainModels.Cart.ShoppingCartDM.CartType.全部).FirstOrDefault().Qty = CartAllList.Sum(x => x.Value.Count);

            return ShoppingCartDMList;
        }

        /// <summary>
        /// 建立購物車明細資料
        /// </summary>
        /// <param name="CartItemList"></param>
        /// <param name="Sellerresult"></param>
        /// <param name="itemUrlDictionary"></param>
        /// <returns></returns>
        public List<CartItem> BulidCartItemList(List<CartItem> CartItemList, Dictionary<int, TWNewEgg.Models.DomainModels.Seller.SellerBase> Sellerresult, Dictionary<int, List<TWNewEgg.Models.DomainModels.Item.ImageUrlReferenceDM>> itemUrlDictionary, List<TWNewEgg.Models.DomainModels.Category.Category_TreeItem> itemCategory)
        {
            foreach (var CartItemtemp in CartItemList)
            {
                if (Sellerresult != null && Sellerresult.Where(x => x.Key == CartItemtemp.SellerID).ToList().Count != 0)
                {
                    CartItemtemp.CountryofOriginID = Sellerresult[CartItemtemp.SellerID].CountryID;
                    CartItemtemp.CountryofOrigin = Sellerresult[CartItemtemp.SellerID].CountryNameCHT;
                }

                // 依據 BSATW-173 廢四機需求增加
                // 廢四機賣場商品, Y=是癈四機 ---------------add by bruce 20160517
                CartItemtemp.Discard4 = string.Empty;
                TWNewEgg.Models.DomainModels.Item.ItemInfo domain_item = null;
                domain_item = this._IItemInfoService.GetItemInfo(CartItemtemp.ItemID);
                CartItemtemp.Discard4 = (domain_item.ItemBase.Discard4 != null) ? domain_item.ItemBase.Discard4 : string.Empty;


                string strItemGroupName = string.Empty;
                strItemGroupName = this._ItemGroupService.GetItemMarketGroupNameByItemId(CartItemtemp.ItemID);
                if (!string.IsNullOrEmpty(strItemGroupName))
                {
                    CartItemtemp.ItemPropertyName = (strItemGroupName.StartsWith("-")) ? strItemGroupName.Substring(1) : strItemGroupName;
                }


                TWNewEgg.Models.DomainModels.Category.Category_TreeItem currentCategory = itemCategory.Where(x => x.category_id == CartItemtemp.CategoryID).FirstOrDefault();
                CartItemtemp.StoreID = 0;
                if (currentCategory != null)
                {
                    CartItemtemp.CategoryID = FindCategoryForURL(currentCategory, 2);
                    CartItemtemp.StoreID = FindCategoryForURL(currentCategory, 0);
                }

                try
                {
                    if (itemUrlDictionary != null)
                    {
                        if (itemUrlDictionary.Count > 0 && itemUrlDictionary.Where(x => x.Key == CartItemtemp.ItemID).FirstOrDefault().Value.Count != 0)
                        {
                            CartItemtemp.ImagePath = itemUrlDictionary[CartItemtemp.ItemID].FirstOrDefault().ImageUrl;
                            if (itemUrlDictionary[CartItemtemp.ItemID].Where(x => x.Size == 60).FirstOrDefault() != null)
                            {
                                CartItemtemp.ImagePath = itemUrlDictionary[CartItemtemp.ItemID].Where(x => x.Size == 60).FirstOrDefault().ImageUrl;
                            }
                            else
                            {
                                if (itemUrlDictionary[CartItemtemp.ItemID].Where(x => x.Size == 125).FirstOrDefault() != null)
                                {
                                    CartItemtemp.ImagePath = itemUrlDictionary[CartItemtemp.ItemID].Where(x => x.Size == 125).FirstOrDefault().ImageUrl;
                                }
                                else
                                {
                                    if (itemUrlDictionary[CartItemtemp.ItemID].Where(x => x.Size == 300).FirstOrDefault() != null)
                                    {
                                        CartItemtemp.ImagePath = itemUrlDictionary[CartItemtemp.ItemID].Where(x => x.Size == 300).FirstOrDefault().ImageUrl;
                                    }
                                }
                            }

                            if (CartItemtemp.ImagePath.IndexOf("newegg.com/") >= 0)
                            {
                            }
                            else
                            {
                                string hostString = ConfigurationManager.AppSettings.Get("ECWebHttpsImgDomain");
                                CartItemtemp.ImagePath = string.Format("{0}{1}", hostString, CartItemtemp.ImagePath);
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
            return CartItemList;
        }

        /// <summary>
        /// 不參與優惠活動的Item清單
        /// </summary>
        /// <param name="accountID">客戶ID</param>
        /// <returns>返回搜尋結果</returns>
        public List<ViewTracksCartItems> NoDiscountsGoods(int accountID)
        {
            // 不參與優惠活動的Item清單
            List<string> notToParticipateNameList =
                new List<string>() {
                    ViewTracksCartItems.ViewTracksType.國內購物車加價商品.ToString(),
                    ViewTracksCartItems.ViewTracksType.海外購物車加價商品.ToString(),
                    ViewTracksCartItems.ViewTracksType.任選館購物車加價商品.ToString()
                };
            Dictionary<string, List<ViewTracksCartItems>> getShoppingAllCart = new Dictionary<string, List<ViewTracksCartItems>>();
            List<ViewTracksCartItems> getNotToParticipateList = new List<ViewTracksCartItems>();
            // 不參與活動的itemID
            List<int> result = new List<int>();
            getShoppingAllCart = this._iStoredProceduresRepoAdapter.GetShoppingAllCart(accountID);
            if (getShoppingAllCart != null && getShoppingAllCart.Count > 0)
            {
                List<List<ViewTracksCartItems>> getShoppingAllCartValue = getShoppingAllCart.Where(x => notToParticipateNameList.Contains(x.Key)).Select(x => x.Value).ToList();
                if (getShoppingAllCartValue != null)
                {
                    getShoppingAllCartValue.ForEach(s =>
                    {
                        getNotToParticipateList.AddRange(s);
                    });
                }
            }

            return getNotToParticipateList;
        }

        /// <summary>
        /// 取得加價購商品清單
        /// </summary>
        /// <param name="accountID">客戶ID</param>
        /// <param name="cartTypeID">購物車類型ID</param>
        /// <returns>返回搜尋結果</returns>
        public List<ViewTracksCartItems> GetIncreasePurchaseItemList(int accountID, int cartTypeID)
        {
            Dictionary<string, List<ViewTracksCartItems>> getShoppingAllCart = new Dictionary<string, List<ViewTracksCartItems>>();
            List<ViewTracksCartItems> getShoppingAllCartValue = new List<ViewTracksCartItems>();
            getShoppingAllCart = this._iStoredProceduresRepoAdapter.GetShoppingAllCart(accountID);
            if (getShoppingAllCart != null && getShoppingAllCart.Count > 0)
            {
                getShoppingAllCartValue = getShoppingAllCart.Where(x => x.Key == ReturnViewTracksType(cartTypeID)).Select(x => x.Value).FirstOrDefault();
            }

            return getShoppingAllCartValue;
        }

        /// <summary>
        /// Find current category ID by layer
        /// </summary>
        /// <param name="category"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        private int FindCategoryForURL(TWNewEgg.Models.DomainModels.Category.Category_TreeItem category, int layer)
        {
            if (category == null)
            {
                return 0;
            }
            if (category.Parents == null)
            {
                return category.category_id;
            }
            if (category.category_layer == layer)
            {
                return category.category_id;
            }
            return FindCategoryForURL(category.Parents, layer++);
        }

        /// <summary>
        /// 返回購物車類型列舉名稱
        /// </summary>
        /// <param name="cartTypeID"></param>
        /// <returns></returns>
        private string ReturnViewTracksType(int cartTypeID)
        {
            switch (cartTypeID)
            {
                case 1:
                    return ViewTracksCartItems.ViewTracksType.國內購物車加價商品.ToString();
                    break;
                case 2:
                    return ViewTracksCartItems.ViewTracksType.海外購物車加價商品.ToString();
                    break;
                case 3:
                    return ViewTracksCartItems.ViewTracksType.任選館購物車加價商品.ToString();
                    break;
                default:
                    throw new Exception("購物車類型傳送錯誤");
                    break;
            }
        }

        /// <summary>
        /// 清除購物車加購商品
        /// </summary>
        /// <param name="cartTypeID"></param>
        /// <returns></returns>
        private bool ClearTheNumber0cart(int account, int cartTypeID)
        {
            bool result = true;
            try
            {
                List<Track> deleteTracks = new List<Track>();
                deleteTracks = this._trackRepoAdapter.GetAll().Where(x => x.ACCID == account && x.Status == cartTypeID).ToList();
                if (deleteTracks.Count > 0)
                {
                    this._trackRepoAdapter.DeleteTracks(deleteTracks);
                }
            }
            catch (Exception e) {
                result = false;
            }
            return result;
        }
    }
}
