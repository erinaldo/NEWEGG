using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Cart;
using TWNewEgg.Models.DomainModels.Message;

namespace TWNewEgg.CartServices.Interface
{
    public interface ICartTempService
    {
        /// <summary>
        /// 產生此筆交易的Serial Number
        /// </summary>
        /// <param name="accountId">Account.ID</param>
        /// <param name="cartType">購物車類型</param>
        /// <returns>Serial Number</returns>
        string GenerateSerialNumber(int accountId, int cartType);

        /// <summary>
        /// 建立CartTemp、CartItemTemp
        /// </summary>
        /// <param name="cartTempDM">CartTemp DomainModel</param>
        /// <returns>建立的CartTempDM</returns>
        CartTempDM CreateCartTemp(CartTempDM cartTempDM);

        /// <summary>
        /// 更新CartTemp、CartItemTemp、CartCouponTemp
        /// </summary>
        /// <param name="cartTempDM">CartTemp DomainModel</param>
        /// <returns>更新後的CartTempDM</returns>
        ResponseMessage<CartTempDM> UpdateCartTemp(CartTempDM cartTempDM);

        /// <summary>
        /// 更新CartTemp狀態
        /// </summary>
        /// <param name="serialNumber">SerialNumber</param>
        /// <param name="status">所要更新狀態</param>
        /// <returns>更新後的CartTempDM</returns>
        CartTempDM UpdateCartTempStatus(string serialNumber, int status);

        /// <summary>
        /// 更新CartTemp.Status為訂單完成
        /// </summary>
        /// <param name="serialNumber">SerialNumber</param>
        /// <param name="salesOrderGroupID">SalesOrderGroup.ID</param>
        /// <returns>返回更新結果</returns>
        CartTempDM CartTempComplete(string serialNumber, int salesOrderGroupID);

        /// <summary>
        /// 透過SerialNumber取得CartTempDM
        /// </summary>
        /// <param name="serialNumber">SerialNumber</param>
        /// <returns>返回CartTemp DomainModel</returns>
        CartTempDM GetCartTempBySN(string serialNumber);

        /// <summary>
        /// 透過Account.ID與CartType取得CartTempDM
        /// </summary>
        /// <param name="accountId">Account.ID</param>
        /// <param name="cartType">購物車類型</param>
        /// <returns>返回CartTemp DomainModel</returns>
        CartTempDM GetCartTemp(int accountId, int cartType);

        /// <summary>
        /// 解密Serial Number
        /// </summary>
        /// <param name="serialNumber">SerialNumber</param>
        /// <returns>Serial Number包含的內容</returns>
        CartTempSNInfo Decrypt(string serialNumber);

        /// <summary>
        /// 刪除過時沒有結帳的CartTemp
        /// </summary>
        void RemoveTimeoutCartTemps(int cartTempLimitedTimeOfMinute, int cartTempLimitedTimeOfMonth);

        /// <summary>
        /// 刪除特定CartTemp
        /// </summary>
        /// <param name="accountId">Account.ID</param>
        /// <param name="cartType">購物車類型</param>
        void RemoveCartTemp(int accountId, int cartType);
    }
}
