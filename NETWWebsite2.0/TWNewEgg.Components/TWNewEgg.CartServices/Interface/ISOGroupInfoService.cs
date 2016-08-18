using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartServices.Interface
{
    public interface ISOGroupInfoService
    {
        /// <summary>
        /// 取得SOGroupInfo
        /// </summary>
        /// <param name="soGroupId">SO Group ID</param>
        /// <returns>SOGroupInfo</returns>
        SOGroupInfo GetSOGroupInfo(int soGroupId);

        /// <summary>
        /// 更新實體ATM付款資訊
        /// </summary>
        /// <param name="soGroupId">soGroupId</param>
        /// <param name="bankCode">銀行代碼</param>
        /// <param name="cardNo">匯款帳號</param>
        /// <param name="expireDate">繳費期限</param>
        void UpdateATMPayment(int soGroupId, string bankCode, string vAccount, DateTime expireDate);
    }
}
