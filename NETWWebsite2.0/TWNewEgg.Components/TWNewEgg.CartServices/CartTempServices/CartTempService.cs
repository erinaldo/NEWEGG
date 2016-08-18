using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.AccountEnprypt.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.DAL.Interface;
using System.Web.Script.Serialization;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.Models.DomainModels.Redeem;
using TWNewEgg.ItemService.Service;
using TWNewEgg.CartService.Interface;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.ShoppingCartServices.Interface;

namespace TWNewEgg.CartServices.CartTempServices
{
    public class CartTempService : ICartTempService
    {
        private static object syncRoot = new Object();
        private IAesService _aesEnc;
        private ICartTempRepoAdapter _cartTempRepoAdapter;
        private IItemInfoService _itemInfoService;
        private IItemDisplayPriceService _itemDisplayPricePrice;
        private IShoppingCartService _IShoppingCartService = null;
        int cartTempLimitedTimeOfMinute = -20;
        int cartTempLimitedTimeOfMonth = -3;

        public CartTempService(
            IAesService aesEnc,
            ICartTempRepoAdapter cartTempRepoAdapter,
            IItemInfoService itemInfoService,
            IItemDisplayPriceService itemDisplayPriceService,
            IShoppingCartService shoppingCartService
            )
        {
            this._aesEnc = aesEnc;
            this._cartTempRepoAdapter = cartTempRepoAdapter;
            this._itemInfoService = itemInfoService;
            this._itemDisplayPricePrice = itemDisplayPriceService;
            this._IShoppingCartService = shoppingCartService;
            string strCartTempLimitedTimeOfMinute = System.Configuration.ConfigurationManager.AppSettings["CartTempLimitedTimeOfMinute"];
            string strCartTempLimitedTimeOfMonth = System.Configuration.ConfigurationManager.AppSettings["CartTempLimitedTimeOfMonth"];
            int.TryParse(strCartTempLimitedTimeOfMinute, out cartTempLimitedTimeOfMinute);
            int.TryParse(strCartTempLimitedTimeOfMonth, out cartTempLimitedTimeOfMonth);
        }

        /// <summary>
        /// 取得SerialNumber
        /// </summary>
        /// <param name="accountId">會員ID</param>
        /// <param name="cartType">購物車模式</param>
        /// <returns>返回SerialNumber</returns>
        public string GenerateSerialNumber(int accountId, int cartType)
        {
            string resultSNO = string.Empty;
            if (accountId > 0 && cartType > 0)
            {
                resultSNO = this.GetSerialNumber(accountId, cartType);
            }

            return resultSNO;
        }

        /// <summary>
        /// 取得GetSerialNumber
        /// </summary>
        /// <param name="accountID">會員ID</param>
        /// <param name="cartType">購物車模式</param>
        /// <returns>返回SerialNumber</returns>
        private string GetSerialNumber(int accountID, int cartType)
        {
            string generateSNO = string.Empty;
            CartTempSNInfo newCartTempSNInfo = new CartTempSNInfo();
            newCartTempSNInfo.AccountID = accountID;
            newCartTempSNInfo.CartTypeID = cartType;
            newCartTempSNInfo.CreateDate = DateTime.UtcNow.AddHours(8);
            JavaScriptSerializer objSerializer = new JavaScriptSerializer();
            try
            {
                string tempSNO = objSerializer.Serialize(newCartTempSNInfo);
                generateSNO = this._aesEnc.Enprypt(tempSNO);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return generateSNO;
        }

        /// <summary>
        /// 建立CartTemp、CartItemTemp List與CartCouponTemp List
        /// </summary>
        /// <param name="cartTempDM">CartTempDomainModel</param>
        /// <returns>返回建立結果</returns>
        public CartTempDM CreateCartTemp(CartTempDM cartTempDM)
        {
            if (cartTempDM == null)
            {
                throw new Exception("無資訊可供處理");
            }

            #region 取得該Account所有未結帳的CartTemp資訊
            List<CartTemp> getCartTempList = new List<CartTemp>();
            try
            {
                // 取得該Account所有未結帳的CartTemp資訊
                getCartTempList = this._cartTempRepoAdapter
                     .GetCartTemp(cartTempDM.AccountID, (int)cartTempDM.CartTypeID)
                     .Where(x => x.Status != (int)CartTempDM.StatusEnum.SOCreated).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CartTemp資料查詢失敗 [AccountID] " + cartTempDM.AccountID + " [SerialNumber] " + cartTempDM.SerialNumber + " [ErrorMessage] " + ex.ToString());
            }
            #endregion

            #region 若該CartTemp清單存在DB中，則刪除
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    // 若該CartTemp清單存在，則刪除
                    getCartTempList.ForEach(x =>
                    {
                        int cartTempID = x.ID;
                        this._cartTempRepoAdapter.DeleteCartTemp(cartTempID);
                        this._cartTempRepoAdapter.DeleteCartItemTempByCartTempID(cartTempID);
                        this._cartTempRepoAdapter.DeleteCartCouponTempByCartTempID(cartTempID);
                    });

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("CartTemp資料刪除失敗 [AccountID] " + cartTempDM.AccountID + " [SerialNumber] " + cartTempDM.SerialNumber + " [ErrorMessage] " + ex.ToString());
                }
            }
            #endregion

            #region 建立新的未結帳CartTemp資訊
            try
            {
                // 建立新的未結帳CartTemp資訊
                cartTempDM = this.InsertCartTempToDB(cartTempDM);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            #endregion

            return cartTempDM;
        }

        /// <summary>
        /// 創建新的CartTemp相關資訊制DB中
        /// </summary>
        /// <param name="cartTempDM">新的CartTEMP資訊</param>
        /// <returns>返回執行結果</returns>
        private CartTempDM InsertCartTempToDB(CartTempDM cartTempDM)
        {
            CartTempDM outputCartTempDM = new CartTempDM();
            CartTempSNInfo cartTempSNInfo = new CartTempSNInfo();
            CartTemp cartTemp = new CartTemp();
            CartTemp newCartTemp = new CartTemp();
            List<CartItemTemp> getCartItemTempList = new List<CartItemTemp>();
            try
            {
                cartTempSNInfo = this.Decrypt(cartTempDM.SerialNumber);
                cartTemp = ModelConverter.ConvertTo<CartTemp>(cartTempDM);
                cartTemp.CreateDate = cartTempSNInfo.CreateDate;
                cartTemp.CreateUser = "system";
                cartTemp.Updated = 0;
                getCartItemTempList = this.CartItemTempInfoCombination(cartTempDM.ID, cartTempDM.CartItemTempDMs, cartTempSNInfo.CreateDate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #region 創建CartTemp與CartItemTemp List
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    #region Create CartTemp
                    newCartTemp = this._cartTempRepoAdapter.CreateCartTemp(cartTemp);
                    #endregion

                    #region Create CartItemTemp
                    getCartItemTempList.ForEach(x => {
                        // Create CartItemTemp
                        x.CartTempID = newCartTemp.ID;
                        this._cartTempRepoAdapter.CreateCartItemTemp(x);
                    });
                    #endregion
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("建立新的未結帳CartTemp資訊失敗 [AccountID] " + cartTempDM.AccountID + " [SerialNumber] " + cartTempDM.SerialNumber + " [ErrorMessage] " + ex.ToString());
                }
            }
            #endregion
            try
            {
                outputCartTempDM = ModelConverter.ConvertTo<CartTempDM>(newCartTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outputCartTempDM;
        }

        /// <summary>
        /// CartItemTemp資料組合
        /// </summary>
        /// <param name="cartTempID">CartTemp.ID</param>
        /// <param name="cartItemTempDMs">CartItemTemp資料清單</param>
        /// <param name="createDate">CartTemp創建日期</param>
        /// <returns>返回組合結果</returns>
        private List<CartItemTemp> CartItemTempInfoCombination(int cartTempID, List<CartItemTempDM> cartItemTempDMs, DateTime createDate)
        {
            List<int> itemList = new List<int>();
            List<CartItemTemp> outputCartItemTemp = new List<CartItemTemp>();
            if (cartItemTempDMs == null || cartItemTempDMs.Count == 0)
            {
                return outputCartItemTemp;
            }

            cartItemTempDMs.ForEach(x =>
            {
                itemList.Add(x.ItemID);
            });
            itemList = itemList.Distinct().ToList();
            try
            {
                // 取得Item相關資訊
                Dictionary<int, ItemInfo> itemDetails = this._itemInfoService.GetItemInfoList(itemList);
                // 取得Item金額資訊
                Dictionary<int, ItemPrice> itemDisplayPrices = this._itemDisplayPricePrice.GetItemDisplayPrice(itemList);
                foreach (CartItemTempDM cartItemTempDM in cartItemTempDMs)
                {
                    CartItemTemp subCartItemTemp = new CartItemTemp();
                    ItemInfo itemInfo = itemDetails[cartItemTempDM.ItemID];
                    ItemPrice itemDisplayPrice = itemDisplayPrices[cartItemTempDM.ItemID];
                    subCartItemTemp.CartTempID = cartTempID;
                    subCartItemTemp.ItemID = cartItemTempDM.ItemID;
                    subCartItemTemp.Qty = cartItemTempDM.Qty;
                    subCartItemTemp.Category = itemInfo.ItemBase.CategoryID;
                    #region Price
                    subCartItemTemp.DisplayPrice = itemDisplayPrice.DisplayPrice;
                    subCartItemTemp.PriceCash = itemInfo.ItemBase.PriceCash;
                    subCartItemTemp.ServicePrice = itemInfo.ItemBase.ServicePrice;
                    subCartItemTemp.ShippingPrice = itemDisplayPrice.DisplayShipping - itemInfo.ItemBase.ServicePrice;
                    subCartItemTemp.Tax = itemDisplayPrice.DisplayTax;
                    #endregion
                    subCartItemTemp.CreateDate = createDate;
                    subCartItemTemp.CreateUser = "system";
                    subCartItemTemp.Updated = 0;
                    outputCartItemTemp.Add(subCartItemTemp);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return outputCartItemTemp;
        }

        /// <summary>
        /// 更新CartTemp、CartItemTemp、CartCouponTemp等相關資訊
        /// </summary>
        /// <param name="cartTempDM">購物車中的CartTemp資訊</param>
        /// <returns>返回執行結果</returns>
        public ResponseMessage<CartTempDM> UpdateCartTemp(CartTempDM cartTempDM)
        {
            ResponseMessage<CartTempDM> updateResult = new ResponseMessage<CartTempDM>();
            try
            {
                #region 資料取得與驗證
                // 取得資料庫中，CartTemp的相關資訊
                CartTemp cartTemp = this._cartTempRepoAdapter.GetCartTemp(cartTempDM.SerialNumber).FirstOrDefault();
                if (cartTemp == null)
                {
                    throw new Exception("此筆資料不存在");
                }

                CartTempSNInfo cartTempSNInfo = this.Decrypt(cartTempDM.SerialNumber);
                updateResult = this.DataValidation(cartTemp, cartTempDM, cartTempSNInfo);
                if (updateResult.IsSuccess == false)
                {
                    return updateResult;
                }

                cartTemp.PayType = cartTempDM.PayType;
                cartTemp.BankID = cartTempDM.BankID;
                cartTemp.PayTypeGroupID = cartTempDM.PayTypeGroupID;
                cartTemp.SalesOrderGroupID = cartTempDM.SalesOrderGroupID;
                cartTemp.Status = cartTempDM.Status;
                cartTemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                cartTemp.UpdateUser = "system";
                cartTemp.Updated = cartTemp.Updated == null ? 1 : cartTemp.Updated++;
                #endregion
                // 取得CartTempID底下的CartItemTemp List
                List<CartItemTemp> cartItemTemps = this._cartTempRepoAdapter.GetCartItemTempList(cartTemp.ID).ToList();
                #region 刪除DB中存在，但DM中不存在的ItemTemp資訊
                if (cartTempDM.CartItemTempDMs != null && cartItemTemps != null)
                {
                    // 刪除DB中存在，但DM中不存在的ItemTemp資訊
                    this.RemoveDBCartTemp(cartTempDM.CartItemTempDMs.Select(x => x.ItemID).ToList(), cartItemTemps);
                }
                #endregion
                // 呼叫加價購Service-其回傳加價購商品的ItemID,Qty,Price(假資料-等Bill的Service完成之後再做修改)
                //List<IncreasePurchaseItem> increasePurchaseItemList = new List<IncreasePurchaseItem>();
                List<TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems> increasePurchaseItemList = new List<Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems>();
                increasePurchaseItemList = this._IShoppingCartService.GetIncreasePurchaseItemList(cartTemp.AccountID, (int)cartTempDM.CartTypeID);
                // 取得CartItemTemp中，需要更新與新增的資訊
                UpdateCartItemTempInfoList getCartItemTempInfo = this.GetUpdateAndCreateCartItemTempInfo(cartTempDM.CartItemTempDMs, cartItemTemps, cartTempSNInfo, increasePurchaseItemList);
                // 取得CartCouponTemp中，需要更新的資訊
                List<CartCouponTemp> cartCouponTemps = this.TransformCartCouponTempList(cartTemp.ID, cartTempDM.CartCouponTempDMs, increasePurchaseItemList);
                #region 資料創建與更新置資料庫中
                using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
                {
                    try
                    {
                        #region Update CartTemp
                        this._cartTempRepoAdapter.UpdateCartTemp(cartTemp);
                        #endregion

                        #region Update & Create CartItemTemp
                        // 更新CartItemTemp
                        getCartItemTempInfo.UpdateCartItemTemps.ForEach(x =>
                        {
                            this._cartTempRepoAdapter.UpdateCartItemTemp(x);
                        });
                        // 新增CartItemTemp
                        getCartItemTempInfo.AddCartItemTemps.ForEach(x =>
                        {
                            this._cartTempRepoAdapter.CreateCartItemTemp(x);
                        });
                        #endregion

                        #region Update CartCouponTemp
                        // 移除原Coupon紀錄
                        this._cartTempRepoAdapter.DeleteCartCouponTempByCartTempID(cartTemp.ID);
                        cartCouponTemps.ForEach(x =>
                        {
                            // Create CartCouponTemp
                            this._cartTempRepoAdapter.CreateCartCouponTemp(x);
                        });
                        #endregion

                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                }
                #endregion
                // 重新取得更新後結果
                cartTempDM = this.GetCartTempBySN(cartTemp.SerialNumber);
            }
            catch (Exception ex)
            {
                updateResult.IsSuccess = false;
                updateResult.Error = new ErrorMessage();
                updateResult.Error.Code = (int)CartTempDM.UpdateResultErrorCode.系統錯誤;
                updateResult.Error.Detail = ex.ToString();
                return updateResult;
            }

            updateResult.IsSuccess = true;
            updateResult.Data = cartTempDM;
            return updateResult;
        }

        #region CartTemp資料驗證
        /// <summary>
        /// CartTemp資料驗證
        /// </summary>
        /// <param name="cartTemp">DB購物車資料暫存</param>
        /// <param name="cartTempDM">使用者購物車資料暫存</param>
        /// <param name="cartTempSNInfo">SN解析後的詳細資訊</param>
        /// <returns>返回驗證結果</returns>
        private ResponseMessage<CartTempDM> DataValidation(CartTemp cartTemp, CartTempDM cartTempDM, CartTempSNInfo cartTempSNInfo)
        {
            ResponseMessage<CartTempDM> updateResult = new ResponseMessage<CartTempDM>();
            updateResult.IsSuccess = true;
            #region CartTemp資料驗證
            if (cartTempDM == null || string.IsNullOrEmpty(cartTempDM.SerialNumber))
            {
                updateResult.IsSuccess = false;
                updateResult.Error = new ErrorMessage();
                updateResult.Error.Code = (int)CartTempDM.UpdateResultErrorCode.資料錯誤;
                updateResult.Error.Detail = "資料缺漏";
                return updateResult;
            }

            if (cartTemp == null)
            {
                updateResult.IsSuccess = false;
                updateResult.Error = new ErrorMessage();
                updateResult.Error.Code = (int)CartTempDM.UpdateResultErrorCode.連線逾時;
                updateResult.Error.Detail = "購物車連線逾時，系統將自動導回購物車頁";
                return updateResult;
            }

            int cartTempStatus = this.CartTempStatusCheck(cartTemp.Status, cartTempDM.Status);
            if (cartTempStatus != (int)CartTempDM.UpdateResultErrorCode.無異常)
            {
                updateResult.IsSuccess = false;
                updateResult.Error = new ErrorMessage();
                updateResult.Error.Code = cartTempStatus;
                return updateResult;
            }

            DateTime negativeMinutes = DateTime.UtcNow.AddHours(8).AddMinutes(cartTempLimitedTimeOfMinute);
            if (DateTime.Compare(cartTemp.CreateDate.Value, negativeMinutes) <= 0)
            {
                updateResult.IsSuccess = false;
                updateResult.Error = new ErrorMessage();
                updateResult.Error.Code = (int)CartTempDM.UpdateResultErrorCode.連線逾時;
                updateResult.Error.Detail = "購物車連線逾時，系統將自動導回購物車頁";
                return updateResult;
            }
            
            if (cartTempDM.CartItemTempDMs == null)
            {
                updateResult.IsSuccess = false;
                updateResult.Error = new ErrorMessage();
                updateResult.Error.Code = (int)CartTempDM.UpdateResultErrorCode.連線逾時;
                updateResult.Error.Detail = "購物車商品訂購數量不得為零，系統將自動導回購物車頁!";
                return updateResult;
            }
            #endregion
            return updateResult;
        }
        #endregion

        /// <summary>
        /// 刪除DB中存在，但DM中不存在的ItemTemp資訊
        /// </summary>
        /// <param name="dmItemIDList"></param>
        /// <param name="cartItemTemps"></param>
        private void RemoveDBCartTemp(List<int> dmItemIDList, List<CartItemTemp> cartItemTemps)
        {
            try
            {
                // 刪除DB中存在，但DM中不存在的ItemTemp資訊
                List<CartItemTemp> deleteCartItemTemps = cartItemTemps.Where(x => !dmItemIDList.Contains(x.ItemID)).ToList();
                if (deleteCartItemTemps != null)
                {
                    deleteCartItemTemps.ForEach(y =>
                    {
                        // 若該商品已被刪除則移除CartItemTemp中，此商品的資訊
                        this._cartTempRepoAdapter.DeleteCartItemTempByID(y.ID);
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新CartItemTemp.Qty與建立新增的CartItemTemp項目
        /// </summary>
        /// <param name="cartItemTempDMs">購物車中CartItemTemp清單</param>
        /// <param name="cartItemTemps">資料庫中CartItemTemp清單</param>
        /// <param name="cartTempSNInfo">購物車訂購人AccountID與購物車類型</param>
        /// <param name="increasePurchaseItemList">加價購商品資訊</param>
        /// <returns>返回資料收集結果</returns>
        private UpdateCartItemTempInfoList GetUpdateAndCreateCartItemTempInfo(List<CartItemTempDM> cartItemTempDMs, List<CartItemTemp> cartItemTemps, CartTempSNInfo cartTempSNInfo, List<TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems> increasePurchaseItemList)
        {
            UpdateCartItemTempInfoList updateCartTempInfoList = new UpdateCartItemTempInfoList();
            List<CartItemTempDM> addCartItemTemps = new List<CartItemTempDM>();
            string errorMessage = string.Empty;
            if (cartItemTempDMs == null)
            {
                errorMessage += "購物車中CartItemTemp清單缺漏; ";
            }

            if (cartItemTemps == null)
            {
                errorMessage += "資料庫中CartItemTemp清單缺漏;";
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception("AccountID [" + cartTempSNInfo.AccountID + "] CartTypeID [" + cartTempSNInfo.CartTypeID + "] " + errorMessage);
            }

            try
            {
                foreach (CartItemTempDM cartItemTempDM in cartItemTempDMs)
                {
                    CartItemTemp cartItemTemp = cartItemTemps.Where(x => x.ItemID == cartItemTempDM.ItemID).FirstOrDefault();
                    if (cartItemTemp != null)
                    {
                        // 若該Item存在，則更新其訂購數量
                        cartItemTemp.Qty = cartItemTempDM.Qty;
                        //cartItemTemp
                        cartItemTemp.Updated = cartItemTemp.Updated == null ? 1 : cartItemTemp.Updated++;
                        cartItemTemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                        cartItemTemp.UpdateUser = "system";
                        updateCartTempInfoList.UpdateCartItemTemps.Add(cartItemTemp);
                    }
                    else
                    {
                        // 若該Item不存在，則新增該Item
                        // 驗證是否為加價購商品，一般商品在進入購物車時，皆已產生，故在此新增商品非一般賣場商品
                        TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems getIncreasePurchaseItem = increasePurchaseItemList.Where(x => x.ItemID == cartItemTempDM.ItemID).FirstOrDefault();
                        if (getIncreasePurchaseItem != null)
                        {
                            cartItemTempDM.Qty = getIncreasePurchaseItem.TrackQty ?? 0;
                            addCartItemTemps.Add(cartItemTempDM);
                        }
                        else
                        {
                            throw new Exception("資料錯誤，AccountID [" + cartTempSNInfo.AccountID + "] CartTypeID [" + cartTempSNInfo.CartTypeID + "] CartTempID [" + cartItemTempDM.CartTempID + "] 無ItemID為 [" + cartItemTempDM.ItemID + "] 的加價購商品");
                        }
                    }
                }

                // 若該Item不存在，則新增該Item
                List<CartItemTemp> createNewCartItemTemps = this.CartItemTempInfoCombination(cartItemTemps.FirstOrDefault().CartTempID, addCartItemTemps, cartTempSNInfo.CreateDate);
                updateCartTempInfoList.AddCartItemTemps.AddRange(createNewCartItemTemps);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return updateCartTempInfoList;
        }

        public class UpdateCartItemTempInfoList
        {
            public UpdateCartItemTempInfoList()
            {
                this.UpdateCartItemTemps = new List<CartItemTemp>();
                this.AddCartItemTemps = new List<CartItemTemp>();
            }

            public List<CartItemTemp> UpdateCartItemTemps { get; set; }
            public List<CartItemTemp> AddCartItemTemps { get; set; }
        }

        /// <summary>
        /// 轉換所需新增的CartCouponTemp清單
        /// </summary>
        /// <param name="cartTempID">CartTemp.ID</param>
        /// <param name="cartCouponTempDMs">所需轉換的CartCouponTemp清單</param>
        /// <param name="increasePurchaseItemList">加價購商品資訊</param>
        /// <returns>返回轉換後結果</returns>
        public List<CartCouponTemp> TransformCartCouponTempList(int cartTempID, List<CartCouponTempDM> cartCouponTempDMs, List<TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems> increasePurchaseItemList)
        {
            List<CartCouponTemp> result = new List<CartCouponTemp>();
            if (cartCouponTempDMs == null && cartTempID == 0)
            {
                return result;
            }

            try
            {
                foreach (CartCouponTempDM cartCouponTempDM in cartCouponTempDMs)
                {
                    // 僅儲存有使用Coupon的Item的Coupon相關資訊
                    if (cartCouponTempDM.ItemID > 0)
                    {
                        TWNewEgg.Models.DBModels.TWSQLDBExtModels.ViewTracksCartItems increasePurchaseItem = increasePurchaseItemList.Where(x => x.ItemID == cartCouponTempDM.ItemID).FirstOrDefault();
                        if (increasePurchaseItem == null)
                        {
                            CartCouponTemp cartCouponTemp = ModelConverter.ConvertTo<CartCouponTemp>(cartCouponTempDM);
                            cartCouponTemp.CartTempID = cartTempID;
                            //cartItemTemp
                            cartCouponTemp.Updated = cartCouponTemp.Updated == null ? 1 : cartCouponTemp.Updated++;
                            cartCouponTemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                            cartCouponTemp.UpdateUser = "system";
                            // Create CartCouponTemp
                            result.Add(cartCouponTemp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 驗證CartTemp可執行狀態
        /// </summary>
        /// <param name="dbStatus">資料庫目前狀態</param>
        /// <param name="wantToChange">所要更新的狀態</param>
        /// <returns>返回執行結果</returns>
        private int CartTempStatusCheck(int dbStatus, int wantToChange)
        {
            int result = (int)CartTempDM.UpdateResultErrorCode.無異常;

            if (dbStatus == wantToChange)
            {
                return (int)CartTempDM.UpdateResultErrorCode.處理中;
            }

            switch (dbStatus)
            {
                case (int)CartTempDM.StatusEnum.Initial:
                    switch (wantToChange)
                    {
                        case (int)CartTempDM.StatusEnum.Step2:
                            break;
                        case (int)CartTempDM.StatusEnum.Step3:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                        case (int)CartTempDM.StatusEnum.SOCreated:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                        default:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                    }
                    break;
                case (int)CartTempDM.StatusEnum.Step2:
                    switch (wantToChange)
                    {
                        case (int)CartTempDM.StatusEnum.Initial:
                            break;
                        case (int)CartTempDM.StatusEnum.Step3:
                            break;
                        case (int)CartTempDM.StatusEnum.SOCreated:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                        default:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                    }
                    break;
                case (int)CartTempDM.StatusEnum.Step3:
                    switch (wantToChange)
                    {
                        case (int)CartTempDM.StatusEnum.Initial:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                        case (int)CartTempDM.StatusEnum.Step2:
                            break;
                        case (int)CartTempDM.StatusEnum.SOCreated:
                            break;
                        default:
                            return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                            break;
                    }
                    break;
                default:
                    return (int)CartTempDM.UpdateResultErrorCode.系統流程錯誤;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 更新CartTemp狀態
        /// </summary>
        /// <param name="serialNumber">serialNumber序號</param>
        /// <param name="status">所要更新的狀態</param>
        /// <returns>返回更新結果</returns>
        public CartTempDM UpdateCartTempStatus(string serialNumber, int status)
        {
            CartTempDM cartTempDM = new CartTempDM();
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new Exception("購物車連線逾時，系統將自動導回購物車頁");
            }

            CartTemp cartTemp = this._cartTempRepoAdapter.GetCartTemp(serialNumber).FirstOrDefault();
            if (cartTemp == null)
            {
                throw new Exception("購物車連線逾時，系統將自動導回購物車頁");
            }

            DateTime negativeMinutes = DateTime.UtcNow.AddHours(8).AddMinutes(cartTempLimitedTimeOfMinute);
            if (DateTime.Compare(cartTemp.CreateDate.Value, negativeMinutes) <= 0)
            {
                throw new Exception("購物車連線逾時，系統將自動導回購物車頁");
            }

            //List<CartItemTemp> cartItemTemps = this._cartTempRepoAdapter.GetCartItemTempList(cartTemp.ID).ToList();
            #region 更新CartTemp
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    cartTemp.Status = status;
                    cartTemp.UpdateDate = DateTime.UtcNow.AddHours(8);
                    cartTemp.UpdateUser = "system";
                    cartTemp.Updated = cartTemp.Updated == null ? 1 : cartTemp.Updated++;
                    this._cartTempRepoAdapter.UpdateCartTemp(cartTemp);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            #endregion
            // 重新取得更新後結果
            try
            {
                cartTempDM = this.GetCartTempBySN(cartTemp.SerialNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartTempDM;
        }

        /// <summary>
        /// 將CartTemp更改至訂單完成狀態
        /// </summary>
        /// <param name="serialNumber">serialNumber序號</param>
        /// <param name="salesOrderGroupID">購物車ID</param>
        /// <returns>返回更新結果</returns>
        public CartTempDM CartTempComplete(string serialNumber, int salesOrderGroupID)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new Exception("購物車連線逾時，系統將自動導回購物車頁");
            }

            CartTempDM cartTempDM = new CartTempDM();
            CartTemp cartTemp = this._cartTempRepoAdapter.GetCartTemp(serialNumber).FirstOrDefault();
            if (cartTemp == null)
            {
                throw new Exception("購物車連線逾時，系統將自動導回購物車頁");
            }

            DateTime negativeMinutes = DateTime.UtcNow.AddHours(8).AddMinutes(cartTempLimitedTimeOfMinute);
            if (DateTime.Compare(cartTemp.CreateDate.Value, negativeMinutes) <= 0)
            {
                throw new Exception("購物車連線逾時，系統將自動導回購物車頁");
            }

            cartTemp.Status = (int)CartTempDM.StatusEnum.SOCreated;
            cartTemp.SalesOrderGroupID = salesOrderGroupID;
            cartTemp.Updated = cartTemp.Updated == null ? 1 : cartTemp.Updated++;
            cartTemp.UpdateDate = DateTime.UtcNow.AddHours(8);
            cartTemp.UpdateUser = "system";
            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope())
            {
                try
                {
                    this._cartTempRepoAdapter.UpdateCartTemp(cartTemp);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            // 重新取得更新後結果
            try
            {
                cartTempDM = this.GetCartTempBySN(cartTemp.SerialNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cartTempDM;
        }

        /// <summary>
        /// 透過serialNumber取得CartTemp資訊
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        public CartTempDM GetCartTempBySN(string serialNumber)
        {
            CartTempDM result = new CartTempDM();
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new Exception("SerialNumber不得為空");
            }

            CartTemp getCartTemp = null;
            try
            {
                getCartTemp = this._cartTempRepoAdapter.GetCartTemp(serialNumber).FirstOrDefault();
                result = this.GetCartTempSubset(getCartTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 取得CartTemp資訊
        /// </summary>
        /// <param name="accountId">客戶ID</param>
        /// <param name="cartType">購物車編號</param>
        /// <returns>返回查詢結果</returns>
        public CartTempDM GetCartTemp(int accountId, int cartType)
        {
            CartTempDM result = new CartTempDM();
            if (accountId <= 0 && cartType <= 0)
            {
                throw new Exception("accountId與cartType需大於0不得為空");
            }

            CartTemp getCartTemp = null;
            try
            {
                getCartTemp = this._cartTempRepoAdapter.GetCartTemp(accountId, cartType).Where(x => x.Status == 0).FirstOrDefault();
                result = this.GetCartTempSubset(getCartTemp);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 查詢CartTemp相關資訊
        /// </summary>
        /// <param name="cartTemp">CartTemp資訊</param>
        /// <returns>返回查詢結果</returns>
        private CartTempDM GetCartTempSubset(CartTemp cartTemp)
        {
            CartTempDM result = new CartTempDM();
            List<CartItemTemp> getCartItemTempList = new List<CartItemTemp>();
            List<CartItemTempDM> getCartItemTempDMList = new List<CartItemTempDM>();
            List<CartCouponTemp> getCartCouponTempList = new List<CartCouponTemp>();
            List<CartCouponTempDM> getCartCouponTempDMList = new List<CartCouponTempDM>();
            if (cartTemp == null)
            {
                throw new Exception("無可供查詢資訊");
            }

            try
            {
                getCartItemTempList = this._cartTempRepoAdapter.GetCartItemTempList(cartTemp.ID).ToList();
                getCartCouponTempList = this._cartTempRepoAdapter.GetCartCouponTempList(cartTemp.ID).ToList();

                result = ModelConverter.ConvertTo<CartTempDM>(cartTemp);
                if (getCartItemTempList.Count > 0)
                {
                    getCartItemTempDMList = ModelConverter.ConvertTo<List<CartItemTempDM>>(getCartItemTempList);
                    result.CartItemTempDMs.AddRange(getCartItemTempDMList);
                }

                if (getCartCouponTempList.Count > 0)
                {
                    getCartCouponTempDMList = ModelConverter.ConvertTo<List<CartCouponTempDM>>(getCartCouponTempList);
                    result.CartCouponTempDMs.AddRange(getCartCouponTempDMList);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// serialNumber序號解密
        /// </summary>
        /// <param name="serialNumber">serialNumber序號</param>
        /// <returns>返回執行結果</returns>
        public CartTempSNInfo Decrypt(string serialNumber)
        {
            CartTempSNInfo getCartTempSNInfo = new CartTempSNInfo();
            JavaScriptSerializer objSerializer = new JavaScriptSerializer();
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new Exception("無可供查詢資訊");
            }

            try
            {
                string decryptSerialNumber = this._aesEnc.Decrypt(serialNumber);
                getCartTempSNInfo = objSerializer.Deserialize<CartTempSNInfo>(decryptSerialNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return getCartTempSNInfo;
        }

        /// <summary>
        /// 移除逾期CartTemp資訊
        /// </summary>
        /// <param name="cartTempLimitedTimeOfMinute">限定時間:分</param>
        /// <param name="cartTempLimitedTimeOfMonth">限定時間:月</param>
        public void RemoveTimeoutCartTemps(int cartTempLimitedTimeOfMinute, int cartTempLimitedTimeOfMonth)
        {
            try
            {
                this._cartTempRepoAdapter.RemoveTimeoutCartTemps(cartTempLimitedTimeOfMinute, cartTempLimitedTimeOfMonth);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 移除CartTemp
        /// </summary>
        /// <param name="accountId">客戶ID</param>
        /// <param name="cartType">購物車編號</param>
        public void RemoveCartTemp(int accountId, int cartType)
        {
            try
            {
                List<CartTemp> getCartTemps = this._cartTempRepoAdapter.GetCartTemp(accountId, cartType).ToList();
                if (getCartTemps != null && getCartTemps.Count > 0)
                {
                    List<int> getCartTempIDs = getCartTemps.Select(x => x.ID).ToList();
                    this._cartTempRepoAdapter.DeleteCartTemp(getCartTempIDs);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
