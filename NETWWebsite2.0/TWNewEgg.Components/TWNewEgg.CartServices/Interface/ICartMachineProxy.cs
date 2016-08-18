using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.CartServices.Interface
{
    public interface ICartMachineProxy
    {
        /// <summary>
        /// 檢查付款狀態
        /// </summary>
        void CheckPayment(int soGroupId);

        /// <summary>
        /// 取消付款
        /// </summary>
        void Cancel(int soGroupId);

        /// <summary>
        /// 訂單成立，到後台建立訂單
        /// </summary>
        void TransactToBackend(int soGroupId);

        /// <summary>
        /// 結帳
        /// </summary>
        void Pay(int soGroupId);

        /// <summary>
        /// 付款完成
        /// </summary>
        void PayComplete(int soGroupId);
    }
}
