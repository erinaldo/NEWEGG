using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Bank;

namespace TWNewEgg.BankServices.Interface
{
    public interface IBankService
    {
        /// <summary>
        /// 取得 Bank 表的銀行編號
        /// </summary>
        /// <param name="bankCode">銀行代碼</param>
        /// <returns>銀行編號</returns>
        ResponseMessage<int> GetBankId(string bankCode);
    }
}
