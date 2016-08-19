using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using System.Data;
using TWNewEgg.API.Models;
using System.Web;
using log4net;
using log4net.Config;
using System.Transactions;

namespace TWNewEgg.API.Service
{
    /// <summary>
    /// 查詢 編輯 刪除商品
    /// </summary>
    public class HiLifePaymentCollectionService
    {
        /// <summary>
        /// 查詢訂單資料
        /// </summary>
        /// <param name="OrderNum">訂單編號</param>
        /// <returns>HiLife user account and order amount</returns>
        public TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.HiLifePaymentInfo> queryPaymentInfo(string OrderNum)
        {
            TWNewEgg.API.Models.ActionResponse<TWNewEgg.API.Models.HiLifePaymentInfo> paymentresult = new ActionResponse<HiLifePaymentInfo>();
            TWNewEgg.DB.TWBackendDBContext backendDB = new DB.TWBackendDBContext();
            TWNewEgg.DB.TWSqlDBContext frontDB = new DB.TWSqlDBContext();
            TWNewEgg.API.Models.HiLifePaymentInfo result = new HiLifePaymentInfo();

            var userCartInfo = backendDB.Cart.Where(x => x.ID == OrderNum).FirstOrDefault();

            // 先檢查是否有此訂單資料
            if (userCartInfo != null)
            {
                int userID = Convert.ToInt32(userCartInfo.UserID);
                var userinfo = frontDB.Account.Where(x => x.ID == userID).Select(y => new { y.Email, y.Name }).FirstOrDefault();
                var processGroup = backendDB.Process.Where(x => x.CartID == OrderNum).ToList();

                #region 取得消費者資料

                result.UserAccountEmail = userinfo.Email;
                result.UserName = userinfo.Name;

                #endregion

                #region 取得總金額

                decimal prices = processGroup.Sum(x => x.Price).Value;
                decimal pricecoupons = processGroup.Sum(x => x.Pricecoupon).Value;
                decimal apportioneamounts = processGroup.Sum(x => x.ApportionedAmount);
                decimal shippingExpenses = processGroup.Sum(x => x.ShippingExpense).Value;
                decimal serviceExpenses = processGroup.Sum(x => x.ServiceExpense).Value;
                decimal installmentFees = processGroup.Sum(x => x.InstallmentFee);

                result.TotalAmount = (prices - pricecoupons - apportioneamounts) + shippingExpenses + serviceExpenses + installmentFees;

                #endregion

                paymentresult.Finish(true, (int)ResponseCode.Success, string.Empty, result);
            }
            else
            {
                paymentresult.Finish(false, (int)ResponseCode.Error, "查無此訂單資料", null);
            }

            return paymentresult;
        }
    }
}
