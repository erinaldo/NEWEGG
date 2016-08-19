using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface ICartTempRepoAdapter
    {
        /// <summary>
        /// 創建CartTemp
        /// </summary>
        /// <param name="cartTemp"></param>
        /// <returns></returns>
        CartTemp CreateCartTemp(CartTemp cartTemp);

        /// <summary>
        /// 更新CartTemp
        /// </summary>
        /// <param name="cartTemp"></param>
        /// <returns></returns>
        CartTemp UpdateCartTemp(CartTemp cartTemp);

        /// <summary>
        /// 創建CartItemTemp
        /// </summary>
        /// <param name="cartItemTemp"></param>
        /// <returns></returns>
        CartItemTemp CreateCartItemTemp(CartItemTemp cartItemTemp);

        /// <summary>
        /// 更新CartItemTemp
        /// </summary>
        /// <param name="cartItemTemp"></param>
        /// <returns></returns>
        CartItemTemp UpdateCartItemTemp(CartItemTemp cartItemTemp);

        /// <summary>
        /// 創建CartCouponTemp
        /// </summary>
        /// <param name="cartCouponTemp"></param>
        /// <returns></returns>
        CartCouponTemp CreateCartCouponTemp(CartCouponTemp cartCouponTemp);

        /// <summary>
        /// 更新CartCouponTemp
        /// </summary>
        /// <param name="cartCouponTemp"></param>
        /// <returns></returns>
        CartCouponTemp UpdateCartCouponTemp(CartCouponTemp cartCouponTemp);

        /// <summary>
        /// 取得所有CartTemp
        /// </summary>
        /// <returns></returns>
        IQueryable<CartTemp> GetAllCartTemp();

        /// <summary>
        /// 透過SerialNumber取得CartTemp
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        IQueryable<CartTemp> GetCartTemp(string serialNumber);

        /// <summary>
        /// 透過AccountID與CartType取得CartTemp
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="cartType"></param>
        /// <returns></returns>
        IQueryable<CartTemp> GetCartTemp(int accountId, int cartType);

        /// <summary>
        /// 透過CartTemp.ID清單取得CartTemp清單
        /// </summary>
        /// <param name="cartTempIDs"></param>
        /// <returns></returns>
        IQueryable<CartTemp> GetCartTemp(List<int> cartTempIDs);

        /// <summary>
        /// 透過CartTemp.ID取得CartItemTemp清單
        /// </summary>
        /// <param name="cartTempID"></param>
        /// <returns></returns>
        IQueryable<CartItemTemp> GetCartItemTempList(int cartTempID);

        /// <summary>
        /// 透過CartTemp.ID取得CartCouponTemp清單
        /// </summary>
        /// <param name="cartTempID"></param>
        /// <returns></returns>
        IQueryable<CartCouponTemp> GetCartCouponTempList(int cartTempID);

        /// <summary>
        /// 透過CartTemp.ID刪除CartTemp
        /// </summary>
        /// <param name="cartTempID"></param>
        void DeleteCartTemp(int cartTempID);

        /// <summary>
        /// 透過CartTemp.ID清單刪除CartTemp
        /// </summary>
        /// <param name="cartTempIDs"></param>
        void DeleteCartTemp(List<int> cartTempIDs);

        /// <summary>
        /// 透過CartItemTemp.ID刪除CartItemTemp
        /// </summary>
        /// <param name="cartItemTempID"></param>
        void DeleteCartItemTempByID(int cartItemTempID);

        /// <summary>
        /// 透過CartItemTemp.ID清單刪除CartItemTemp
        /// </summary>
        /// <param name="cartItemTempIDs"></param>
        void DeleteCartItemTempByIDs(List<int> cartItemTempIDs);

        /// <summary>
        /// 透過CartTemp.ID刪除CartItemTemp
        /// </summary>
        /// <param name="cartTempID"></param>
        void DeleteCartItemTempByCartTempID(int cartTempID);

        /// <summary>
        /// 透過CartTemp.ID清單刪除CartItemTemp
        /// </summary>
        /// <param name="cartTempIDs"></param>
        void DeleteCartItemTempByCartTempIDs(List<int> cartTempIDs);

        /// <summary>
        /// 透過CartCouponTemp.ID刪除CartCouponTemp
        /// </summary>
        /// <param name="cartCouponTempID"></param>
        void DeleteCartCouponTempByID(int cartCouponTempID);

        /// <summary>
        /// 透過CartCouponTemp.ID清單刪除CartCouponTemp
        /// </summary>
        /// <param name="cartCouponTempIDs"></param>
        void DeleteCartCouponTempByIDs(List<int> cartCouponTempIDs);

        /// <summary>
        /// 透過CartTemp.ID刪除CartCouponTemp
        /// </summary>
        /// <param name="cartTempID"></param>
        void DeleteCartCouponTempByCartTempID(int cartTempID);

        /// <summary>
        /// 透過CartTemp.ID清單刪除CartCouponTemp
        /// </summary>
        /// <param name="cartTempIDs"></param>
        void DeleteCartCouponTempByCartTempIDs(List<int> cartTempIDs);

        /// <summary>
        /// 移除超過時間的CartTemp與CartItemTemp
        /// </summary>
        /// <param name="cartTempLimitedTimeOfMinute">未結帳保留期限(預設20分鐘)</param>
        /// <param name="cartTempLimitedTimeOfMonth">已結帳保留期限(預設3個月)</param>
        void RemoveTimeoutCartTemps(int cartTempLimitedTimeOfMinute, int cartTempLimitedTimeOfMonth);
    }
}
