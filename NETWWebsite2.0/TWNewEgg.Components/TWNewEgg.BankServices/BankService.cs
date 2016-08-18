using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankServices.Interface;
using TWNewEgg.BankRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Bank;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;

namespace TWNewEgg.BankServices
{
    public class BankService : IBankService
    {
        private IBankRepoAdapter _IBankRepoAdapter;
        private IPayTypeRepoAdapter _iPayTypeRepoAdapter;
        private IPayTypeService _iPayTypeService;

        public enum ErrorCode
        {
            未傳入參數 = 1,
            例外發生 = 2,
        }

        public BankService(IBankRepoAdapter bankRepoAdapter, IPayTypeRepoAdapter iPayTypeRepoAdapter, IPayTypeService iPayTypeService)
        {
            this._IBankRepoAdapter = bankRepoAdapter;
            this._iPayTypeRepoAdapter = iPayTypeRepoAdapter;
            this._iPayTypeService = iPayTypeService;
        }

        /// <summary>
        /// 取得 Bank 表的銀行編號
        /// </summary>
        /// <param name="bankCode">銀行代碼</param>
        /// <returns>銀行編號</returns>
        public ResponseMessage<int> GetBankId(string bankCode)
        {
            ResponseMessage<int> result = new ResponseMessage<int>();

            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            try
            {
                // 檢查輸入參數
                if (string.IsNullOrEmpty(bankCode))
                {
                    result.Message = ErrorCode.未傳入參數.ToString();
                    result.Error.Code = (int)ErrorCode.未傳入參數;
                    result.Error.Detail = string.Format("查詢銀行代碼是否存在 Bank 表失敗。ErrorType = {0}。", ErrorCode.未傳入參數.ToString());

                    isException = false;
                    throw new Exception();
                }
                else
                {
                    bankCode = bankCode.Trim();
                }

                if (this._IBankRepoAdapter.GetAll().Any(x => x.Code == bankCode))
                {
                    result.Data = this._IBankRepoAdapter.GetAll().Where(x => x.Code == bankCode).Select(x => x.ID).First();
                    result.IsSuccess = true;
                    result.Message = string.Format("銀行代碼 {0} 存在，且在 Bank 表的編號為 {1}。", bankCode, result.Data);
                    result.Error = null;
                }
                else
                {
                    result.Message = string.Format("找不到銀行代碼為 {0} 的銀行編號。", bankCode);
                    result.Error = null;

                    isException = false;
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = -1;

                if (isException)
                {
                    result.Message = "查詢銀行代碼是否存在 Bank 表失敗。";
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("查詢銀行代碼是否存在 Bank 表失敗。ErrorType = {0}。ErrorMessage = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        public string BankName(string bankCode)
        {
            string returnBankStr = string.Empty;
            var _GetBankId = this.GetBankId(bankCode);
            if (_GetBankId.IsSuccess == false)
            {
                return "[Error]: " + _GetBankId.Message;
            }
            var bankInfo = this._IBankRepoAdapter.GetAll();
            if (bankInfo.Any() == false)
            {
                return "[Error]: 沒有銀行資料";
            }
            string bankName = bankInfo.Where(p => p.ID == _GetBankId.Data).FirstOrDefault().Name;

            return bankName;
        }

        /// <summary>
        /// 取得 Exception Message
        /// </summary>
        /// <remarks>解決若有 InnerException 時，看不到 InnerException Message 的問題</remarks>
        /// <param name="exception">Exception</param>
        /// <returns>Exception Message</returns>
        private string GetExceptionMessage(Exception exception)
        {
            if (exception.Message.Contains("See the inner exception for details."))
            {
                return this.GetExceptionMessage(exception.InnerException);
            }
            else
            {
                return exception.Message;
            }
        }
    }
}
