using AllPay.Payment.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.PaymentGateway.Interface
{
    public interface IAllPay
    {
        /// <summary>
        /// 檢查是否付款成功
        /// </summary>
        /// <param name="Id">SOGroup 或 SO ID</param>
        /// <returns>是否成功</returns>
        bool IsPayed(int Id);

        /// <summary>
        /// 付款
        /// </summary>
        /// <param name="allPayment">付款資訊</param>
        /// <param name="paymentType">付款方式</param>
        string Pay(AllInOne allPayment, int paymentType);

        /// <summary>
        /// 取消付款
        /// </summary>
        /// <param name="Id">SOGroup 或 SO ID</param>
        void Cancel(int Id);

        /// <summary>
        /// 完成付款
        /// </summary>
        /// <param name="Id">SOGroup 或 SO ID</param>
        void Complete(int Id);
    }
}
