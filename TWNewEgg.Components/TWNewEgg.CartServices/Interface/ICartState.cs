using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartServices.CartMachines;

namespace TWNewEgg.CartServices.Interface
{
    public interface ICartState
    {
        /// <summary>
        /// 初始化Cart狀態
        /// </summary>
        /// <param name="cartMachine">ICartMachine</param>
        void Init(OPCCartMachine cartMachine);

        /// <summary>
        /// 結帳
        /// </summary>
        void Pay(int orderStatus);

        /// <summary>
        /// 檢查付款狀態
        /// </summary>
        void CheckPayment();                                                                                                                                                    

        /// <summary>
        /// 取消付款
        /// </summary>
        void Cancel();

        /// <summary>
        /// 訂單成立，到後台建立訂單
        /// </summary>
        void TransactToBackend();

        /// <summary>
        /// 完成付款
        /// </summary>
        void PayComplete();
    }
}
