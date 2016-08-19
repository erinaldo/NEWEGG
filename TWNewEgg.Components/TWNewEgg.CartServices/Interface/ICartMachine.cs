using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Cart;

namespace TWNewEgg.CartServices.Interface
{
    public interface ICartMachine
    {
        /// <summary>
        /// 改變狀態
        /// </summary>
        void ChangeState();

        /// <summary>
        /// 根據SalesorderGroup的狀態初始化狀態機
        /// </summary>
        /// <param name="soGroupId">SalesorderGroup Id</param>
        void InitialMachine(int soGroupId);

        /// <summary>
        /// 根據SalesorderGroup的狀態初始化狀態機
        /// </summary>
        /// <param name="soGroup">SalesorderGroup</param>
        void InitialMachine(SOGroupInfo soGroup);

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
        /// 結帳
        /// </summary>
        void Pay(int orderStatus);

        /// <summary>
        /// 付款完成
        /// </summary>
        void PayComplete();

        /// <summary>
        /// 從資料庫更新SOGroupInfo
        /// </summary>
        void ChangeSOGroupInfo();

        /// <summary>
        /// 取得訂單總金額
        /// </summary>
        /// <returns></returns>
        decimal GetTotalPrice();
    }
}