using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.BankInstallmentRepoAdapters.Interface
{
    /// <summary>
    /// 銀行與分期
    /// </summary>
    public interface IBankInstallmentRepoAdapter
    {
        IQueryable<BankInstallment> GetAll();

        /// <summary>
        /// 取得所有啟用的信用卡分期
        /// </summary>
        /// <returns>IQueryable Installment</returns>
        IQueryable<Installment> GetAvailableInstallments();

        /// <summary>
        /// 取得分期期數的 PayType
        /// </summary>
        /// <param name="installments">分期期數清單</param>
        /// <returns>IQueryable PayType</returns>
        IQueryable<PayType> GetPayTypesByInstallments(IEnumerable<int> installments);

        /// <summary>
        /// 取得所有啟用的分期 PayType
        /// </summary>
        /// <returns>IQueryable PayType</returns>
        IQueryable<PayType> GetAllAvailableInstallmentPaytypes();
    }
}
