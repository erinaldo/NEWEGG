using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.BankBonusServices.Interface;
using TWNewEgg.BankBonusRepoAdapters.Interface;
using TWNewEgg.BankServices.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.BankBonus;
using TWNewEgg.Models.DomainModels.Message;


namespace TWNewEgg.BankBonusServices
{
    /// <summary>
    /// 信用卡紅利折抵
    /// </summary>
    public class BankBonusService : IBankBonusService
    {
        private IBankBonusRepoAdapter _IBankBonusRepoAdapter;
        private IBankService _IBankService;

        public enum ErrorCode
        { 
            未傳入參數 = 1,
            傳入參數錯誤 = 2,
            例外發生 = 3,
            查無資料 = 4
        }

        public BankBonusService(IBankBonusRepoAdapter bankBonusRepoAdapters, IBankService bankService)
        {
            this._IBankBonusRepoAdapter = bankBonusRepoAdapters;
            this._IBankService = bankService;
        }

        #region BankBonus

        /// <summary>
        /// 取得生效的信用卡紅利折抵項目
        /// </summary>
        /// <remarks>只取 SerialNumber 為 0 的</remarks>
        /// <returns>生效的信用卡紅利折抵項目清單</returns>
        public ResponseMessage<List<BankBonus_DM>> GetAllBankBonus()
        {
            ResponseMessage<List<BankBonus_DM>> result = new ResponseMessage<List<BankBonus_DM>>();
            result.Data = new List<BankBonus_DM>();
            List<BankBonus> bankBonusCell = null;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            try
            {
                // 讀取 DB
                stepMessage = "讀取 DB ";
                bankBonusCell = _IBankBonusRepoAdapter.GetAllBankBonus().ToList();

                if (bankBonusCell.Count > 0)
                {
                    // 轉換 DBModel 到 DomainModel
                    stepMessage = "轉換 DBModel 到 DomainModel ";
                    result.Data = ModelConverter.ConvertTo<List<BankBonus_DM>>(bankBonusCell);
                }

                if (result.Data.Count > 1)
                {
                    // 優先排序為上/下架(上架>下架)，次要排序為排序(1>2>3)
                    stepMessage = "排序";
                    result.Data = result.Data.OrderBy(x => x.Status).ThenBy(x => x.Order).ToList();
                }

                stepMessage = "填寫回傳訊息";

                if (result.Data.Count == 0)
                {
                    result.Message = "查無生效的信用卡紅利折抵項目。";
                }
                else
                {
                    result.Message = string.Format("成功取得 {0} 筆生效的信用卡紅利折抵項目。", result.Data.Count);
                }

                result.IsSuccess = true;
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = string.Format("{0}失敗。", stepMessage);
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = ex.ToString();
                result.Error.Detail = string.Format("取得生效的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                    ErrorCode.例外發生.ToString(),
                    stepMessage,
                    this.GetExceptionMessage(ex),
                    ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 取得單筆生效的信用卡紅利折抵項目
        /// </summary>
        /// <param name="id">生效的信用卡紅利折抵項目編號</param>
        /// <returns>單筆生效的信用卡紅利折抵項目</returns>
        public ResponseMessage<BankBonus_DM> GetBankBonusById(int id)
        {
            ResponseMessage<BankBonus_DM> result = new ResponseMessage<BankBonus_DM>();
            BankBonus bankbonus = null;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            try
            {
                if (id > 0)
                {
                    // 讀取 DB
                    stepMessage = "讀取 DB ";
                    bankbonus = this._IBankBonusRepoAdapter.GetAllBankBonus().Where(x => x.ID == id).First();

                    if (bankbonus != null)
                    {
                        // 轉換 DBModel 到 DomainModel
                        stepMessage = "轉換 DBModel 到 DomainModel";
                        result.Data = ModelConverter.ConvertTo<BankBonus_DM>(bankbonus);

                        stepMessage = "填寫回傳訊息";
                        result.Message = string.Format("成功取得編號為 {0} 的生效信用卡紅利折抵項目。", id);
                        result.IsSuccess = true;
                        result.Error = null;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Data = null;
                        result.Message = string.Format("查無編號為 {0} 的生效信用卡紅利折抵項目。", id);
                        result.Error.Code = (int)ErrorCode.查無資料;
                        result.Error.Detail = string.Format("取得單筆生效的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorMessage = 查無編號為 {1} 的生效信用卡紅利折抵項目。", ErrorCode.查無資料.ToString(), id);
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Data = null;
                    result.Message = "生效的信用卡紅利折抵項目編號，不可小於等於 0。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("取得單筆生效的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorMessage = 生效的信用卡紅利折抵項目編號，不可小於等於 0。", ErrorCode.傳入參數錯誤.ToString());
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = string.Format("{0}失敗。", stepMessage);
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = string.Format("取得單筆生效的信用卡紅利折抵項目失敗。生效的信用卡紅利折抵項目編號(ID) = {4}。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                    ErrorCode.例外發生.ToString(),
                    stepMessage,
                    this.GetExceptionMessage(ex),
                    ex.ToString(),
                    id);
            }

            return result;
        }

        /// <summary>
        /// 取得上架的信用卡紅利折抵項目
        /// </summary>
        /// <returns>上架的信用卡紅利折抵項目清單</returns>
        public ResponseMessage<List<BankBonus_DM>> GetAllEffectiveBankBonus()
        {
            ResponseMessage<List<BankBonus_DM>> result = new ResponseMessage<List<BankBonus_DM>>();
            result.Data = new List<BankBonus_DM>();
            List<BankBonus> bankBonusCell = null;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            try
            {
                // 讀取 DB
                stepMessage = "讀取 DB ";
                bankBonusCell = _IBankBonusRepoAdapter.GetAllEffectiveBankBonus().ToList();

                if (bankBonusCell.Count > 0)
                {
                    // 轉換 DBModel 到 DomainModel
                    stepMessage = "轉換 DBModel 到 DomainModel ";
                    result.Data = ModelConverter.ConvertTo<List<BankBonus_DM>>(bankBonusCell);
                }

                if (result.Data.Count > 1)
                {
                    // 依排序欄位由小到大排序
                    stepMessage = "排序";
                    result.Data = result.Data.OrderBy(x => x.Order).ToList();
                }

                stepMessage = "填寫回傳訊息";

                if (result.Data.Count == 0)
                {
                    result.Message = "查無上架的信用卡紅利折抵項目。";
                }
                else
                {
                    result.Message = string.Format("成功取得 {0} 筆上架的信用卡紅利折抵項目。", result.Data.Count);
                }

                result.IsSuccess = true;
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = string.Format("讀取信用卡紅利折抵項目失敗。");
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = ex.ToString();
                result.Error.Detail = string.Format("取得上架的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                    ErrorCode.例外發生.ToString(),
                    stepMessage,
                    this.GetExceptionMessage(ex),
                    ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 取得目前可用的最大排序值
        /// </summary>
        /// <returns>排序值</returns>
        private ResponseMessage<int> GetMaxOrder()
        {
            ResponseMessage<int> result = new ResponseMessage<int>();
            result.Data = -1;

            try
            {
                // 判斷生效區的資料庫內是否有資料
                if (this._IBankBonusRepoAdapter.GetAllBankBonus().Select(x => x.ID).ToList().Count > 0)
                {
                    // 取得目前生效區的資料庫內最大的排序值
                    result.Data = this._IBankBonusRepoAdapter.GetAllBankBonus().Max(x => x.Order);
                    result.Data++;
                }
                else
                {
                    result.Data = 1;
                }

                result.IsSuccess = true;
                result.Message = "取得目前可用的最大排序值成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = -1;
                result.Message = string.Format("取得目前可用的最大排序值失敗。");
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = string.Format("取得目前可用的最大排序值失敗。ErrorType = {0}。ExceptionMessage = {1}。Exception = {2}。",
                    ErrorCode.例外發生.ToString(),
                    this.GetExceptionMessage(ex),
                    ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 儲存排序
        /// </summary>
        /// <param name="bankBonusCell_DM">要更改排序的信用卡紅利折抵項目內容</param>
        public ResponseMessage<bool> UpdateOrder(List<BankBonus_DM> bankBonusCell_DM)
        {
            #region 變數宣告

            ResponseMessage<bool> result = new ResponseMessage<bool>();

            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            // 更新清單的排序值
            List<int> updateOrderCell = null;

            // 更新清單的項目編號
            List<int> updateidCell = null;

            // DB 的排序值
            List<int> dbOrderCell = null;

            // DB Model 暫存
            List<BankBonus> bankBonusCell = null;

            // 取得重複的排序值
            ResponseMessage<List<int>> getRepateOrder = null;

            #endregion 變數宣告

            try
            {
                #region 檢查傳入參數

                stepMessage = "檢查傳入參數";

                if (bankBonusCell_DM == null || bankBonusCell_DM.Count == 0)
                {
                    result.Message = ErrorCode.未傳入參數.ToString();
                    result.Error.Code = (int)ErrorCode.未傳入參數;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.未傳入參數.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 檢查排序值是否有小於 0
                if (bankBonusCell_DM.Any(x => x.Order <= 0))
                {
                    result.Message = "排序值不可小於等於 0。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 檢查更新人是否有填寫
                if (bankBonusCell_DM.Any(x => x.UpdateUser == null))
                {
                    result.Message = "更新人(UpdateUser)未填寫。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 檢查更新人是否為同一人
                if (bankBonusCell_DM.GroupBy(x => x.UpdateUser).Count() != 1)
                {
                    result.Message = "同一次的更新清單中，不可以有 2 個以上的更新人(UpdateUser)。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 檢查更新清單中，是否有重複的排序值
                if (bankBonusCell_DM.Count != bankBonusCell_DM.GroupBy(x => x.Order).Count())
                {
                    result.Message = "更新排序的清單中，排序值不可重複。";

                    getRepateOrder = this.GetRepateOrder(bankBonusCell_DM);

                    if (getRepateOrder.IsSuccess)
                    {
                        result.Message += "重複的排序值：";

                        for (int i = 0; i < getRepateOrder.Data.Count; i++)
                        {
                            if (i > 0)
                            {
                                result.Message += "、";
                            }

                            result.Message += getRepateOrder.Data[i];
                        }

                        result.Message += "。";
                    }

                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 取得更新清單的排序值
                updateOrderCell = bankBonusCell_DM.Select(x => x.Order).ToList();

                // 取得更新清單的項目編號
                updateidCell = bankBonusCell_DM.Select(x => x.ID).ToList();

                // 取得除了更新清單的項目以外，其它已存在項目的排序值
                dbOrderCell = _IBankBonusRepoAdapter.GetAllBankBonus().Where(x => !updateidCell.Contains(x.ID)).Select(x => x.Order).ToList();

                // 檢查更新清單的排序值，是否已存在其它項目的排序值中
                if (dbOrderCell.Any(x => updateOrderCell.Contains(x)))
                {
                    result.Message = "排序值已存在。";

                    getRepateOrder = this.GetRepateOrder(updateOrderCell, dbOrderCell);

                    if (getRepateOrder.IsSuccess)
                    {
                        result.Message += "重複的排序值：";

                        for (int i = 0; i < getRepateOrder.Data.Count; i++)
                        {
                            if (i > 0)
                            {
                                result.Message += "、";
                            }

                            result.Message += getRepateOrder.Data[i];
                        }

                        result.Message += "。";
                    }

                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                #endregion 檢查傳入參數

                // 轉換 DomainModel 到 DBModel
                stepMessage = "轉換 DomainModel 到 DBModel ";
                bankBonusCell = ModelConverter.ConvertTo<List<BankBonus>>(bankBonusCell_DM);

                if (bankBonusCell == null)
                {
                    throw new Exception("轉換 DomainModel 到 DBModel 失敗。");
                }

                stepMessage = "儲存排序";
                this._IBankBonusRepoAdapter.UpdateOrder(bankBonusCell);

                stepMessage = "填寫回傳訊息";
                result.IsSuccess = true;
                result.Data = true;
                result.Message = "儲存排序成功。";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;

                if (isException)
                {
                    result.Message = string.Format("{0}失敗。", stepMessage);
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("儲存排序失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        stepMessage,
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 取得重複的排序值(傳入參數內比對)
        /// </summary>
        /// <param name="bankBonusCell_DM">要更改排序的信用卡紅利折抵項目內容</param>
        /// <returns>重複的排序值清單</returns>
        private ResponseMessage<List<int>> GetRepateOrder(List<BankBonus_DM> bankBonusCell_DM)
        {
            ResponseMessage<List<int>> result = new ResponseMessage<List<int>>();
            List<int> orderCell = null;

            try
            {
                // 取得所有的排序值(不重複)
                orderCell = bankBonusCell_DM.Select(x => x.Order).Distinct().ToList();

                foreach (int order in orderCell)
                {
                    if (bankBonusCell_DM.Where(x => x.Order == order).ToList().Count > 1)
                    {
                        result.Data.Add(order);
                    }
                }

                result.IsSuccess = true;
                result.Message = "取得重複的排序值成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = "取得重複的排序值失敗(傳入參數內比對)。";
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = string.Format("取得重複的排序值失敗(傳入參數內比對)。ErrorType = {0}。ExceptionMessage = {1}。Exception = {2}。",
                    ErrorCode.例外發生.ToString(),
                    this.GetExceptionMessage(ex),
                    ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 取得重複的排序值(傳入參數和 DB 內的其他項目比對)
        /// </summary>
        /// <param name="updateOrderCell">更新清單的排序值</param>
        /// <param name="dbOrderCell">DB 的排序值(不包含更新清單中的)</param>
        /// <returns>重複的排序值清單</returns>
        private ResponseMessage<List<int>> GetRepateOrder(List<int> updateOrderCell, List<int> dbOrderCell)
        {
            ResponseMessage<List<int>> result = new ResponseMessage<List<int>>();

            try
            {
                foreach (int dbOrder in dbOrderCell)
                {
                    if (updateOrderCell.Contains(dbOrder))
                    {
                        result.Data.Add(dbOrder);
                    }
                }

                result.IsSuccess = true;
                result.Message = "取得重複的排序值成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = "取得重複的排序值失敗(傳入參數和 DB 內的其他項目比對)。";
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = string.Format("取得重複的排序值失敗(傳入參數和 DB 內的其他項目比對)。ErrorType = {0}。ExceptionMessage = {1}。Exception = {2}。",
                    ErrorCode.例外發生.ToString(),
                    this.GetExceptionMessage(ex),
                    ex.ToString());
            }

            return result;
        }

        #endregion BankBonus

        #region BankBonusTemp

        /// <summary>
        /// 新增待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp_DM">要新增的信用卡紅利折抵項目內容</param>
        public ResponseMessage<bool> CreateBankBonusTemp(BankBonusTemp_DM bankBonusTemp_DM)
        {
            #region 變數宣告

            ResponseMessage<bool> result = new ResponseMessage<bool>();

            // 新增項目
            BankBonusTemp bankBonusTemp = null;

            // 取得銀行編號
            ResponseMessage<int> getBankId = null;

            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            #endregion 變數宣告

            try
            {
                #region 檢查傳入參數

                stepMessage = "檢查傳入參數";

                if (bankBonusTemp_DM == null)
                {
                    result.Message = ErrorCode.未傳入參數.ToString();
                    result.Error.Code = (int)ErrorCode.未傳入參數;
                    result.Error.Detail = string.Format("新增待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.未傳入參數.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(bankBonusTemp_DM.BankCode))
                {
                    result.Message = "未填寫銀行代碼(BankCode)";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("新增待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 判斷新建項目的銀行代碼，是否已存在待審區
                if (this._IBankBonusRepoAdapter.GetAllBankBonusTemp().Any(x => bankBonusTemp_DM.BankCode.Contains(x.BankCode)))
                {
                    result.Message = string.Format("銀行代碼重複，銀行代碼 {0} 已存在待審區中。", bankBonusTemp_DM.BankCode);
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("新增待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 若為建立新項目，而非生效區編輯，則要檢查銀行代碼是否重複
                if (bankBonusTemp_DM.BankBonusID == null)
                {
                    // 判斷新建項目的銀行代碼，是否已存在生效區
                    if (this._IBankBonusRepoAdapter.GetAllBankBonus().Any(x => bankBonusTemp_DM.BankCode.Contains(x.BankCode)))
                    {
                        result.Message = string.Format("銀行代碼重複，銀行代碼 {0} 已存在生效區中。", bankBonusTemp_DM.BankCode);
                        result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                        result.Error.Detail = string.Format("新增待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                            ErrorCode.傳入參數錯誤.ToString(),
                            stepMessage);

                        isException = false;
                        throw new Exception();
                    }
                }

                #endregion 檢查傳入參數

                // 轉換 DomainModel 到 DBModel
                stepMessage = "轉換 DomainModel 到 DBModel ";
                bankBonusTemp = ModelConverter.ConvertTo<BankBonusTemp>(bankBonusTemp_DM);

                if (bankBonusTemp == null)
                {
                    throw new Exception("轉換 DomainModel 到 DBModel 失敗。");
                }

                #region 填寫銀行編號

                // 若沒有銀行編號，則先取得銀行編號
                if (bankBonusTemp.BankID == 0)
                {
                    stepMessage = "取得 Bank 表的銀行編號";

                    // 取得 Bank 表的銀行編號
                    getBankId = this.GetBankId(bankBonusTemp.BankCode);

                    if (getBankId.IsSuccess)
                    {
                        bankBonusTemp.BankID = getBankId.Data;
                    }
                    else
                    {
                        result.Message = getBankId.Message;
                        result.Error = getBankId.Error;

                        isException = false;
                        throw new Exception();
                    }

                    getBankId = null;
                }

                #endregion 填寫銀行編號

                // 新增待審的信用卡紅利折抵項目
                stepMessage = "新增待審的信用卡紅利折抵項目";
                this._IBankBonusRepoAdapter.CreateBankBonusTemp(bankBonusTemp);

                stepMessage = "填寫回傳訊息";
                result.Data = true;
                result.IsSuccess = true;
                result.Message = "新增待審的信用卡紅利折抵項目成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.IsSuccess = false;

                if (isException)
                {
                    result.Message = "新增待審的信用卡紅利折抵項目失敗。";
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("新增待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 刪除待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="idCell">要刪除的信用卡紅利折抵項目編號</param>
        public ResponseMessage<bool> DeleteBankBonusTemp(List<int> idCell)
        {
            #region 變數宣告

            ResponseMessage<bool> result = new ResponseMessage<bool>();

            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            #endregion 變數宣告

            try
            {
                #region 檢查傳入參數

                stepMessage = "檢查傳入參數";

                if (idCell == null || idCell.Count == 0)
                {
                    result.Message = ErrorCode.未傳入參數.ToString();
                    result.Error.Code = (int)ErrorCode.未傳入參數;
                    result.Error.Detail = string.Format("刪除待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.未傳入參數.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                if (idCell.Any(x => x < 0))
                {
                    result.Message = "要刪除的信用卡紅利折抵項目編號中，有小於 0 的值。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("刪除待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                #endregion 檢查傳入參數

                stepMessage = "刪除待審的信用卡紅利折抵項目";
                this._IBankBonusRepoAdapter.DeleteBankBonusTemp(idCell);

                stepMessage = "填寫回傳訊息";
                result.IsSuccess = true;
                result.Data = true;
                result.Message = "刪除待審的信用卡紅利折抵項目成功";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;

                if (isException)
                {
                    result.Message = string.Format("{0}失敗。", stepMessage);
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("刪除待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        stepMessage,
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 取得待審的信用卡紅利折抵項目
        /// </summary>
        /// <returns>待審的信用卡紅利折抵項目清單</returns>
        public ResponseMessage<List<BankBonusTemp_DM>> GetAllBankBonusTemp()
        {
            #region 變數宣告

            ResponseMessage<List<BankBonusTemp_DM>> result = new ResponseMessage<List<BankBonusTemp_DM>>();
            result.Data = new List<BankBonusTemp_DM>();

            // DB Model 暫存
            List<BankBonusTemp> bankBonusTempCell = null;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            #endregion 變數宣告

            try
            {
                // 讀取 DB
                stepMessage = "讀取 DB ";
                bankBonusTempCell = this._IBankBonusRepoAdapter.GetAllBankBonusTemp().ToList();

                if (bankBonusTempCell.Count > 0)
                {
                    // 轉換 DBModel 到 DomainModel
                    stepMessage = "轉換 DBModel 到 DomainModel ";
                    result.Data = ModelConverter.ConvertTo<List<BankBonusTemp_DM>>(bankBonusTempCell);
                    bankBonusTempCell = null;
                }

                if (result.Data.Count > 1)
                {
                    // 優先排序為最後異動時間(11/20>11/19>11/15)，次要排序為為上/下架(上架>下架)
                    stepMessage = "排序";
                    result.Data  = result.Data.OrderByDescending(x => x.UpdateDate).ThenBy(x => x.Status).ToList();
                }

                stepMessage = "填寫回傳訊息";

                if (result.Data.Count == 0)
                {
                    result.Message = "查無待審的信用卡紅利折抵項目。";
                }
                else
                {
                    result.Message = string.Format("成功取得 {0} 筆待審的信用卡紅利折抵項目。", result.Data.Count);
                }

                result.IsSuccess = true;
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;
                result.Message = string.Format("{0}失敗。", stepMessage);
                result.Error.Code = (int)ErrorCode.例外發生;
                result.Error.Detail = string.Format("取得待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                    ErrorCode.例外發生.ToString(),
                    stepMessage,
                    this.GetExceptionMessage(ex),
                    ex.ToString());
            }

            return result;
        }

        /// <summary>
        /// 取得單筆待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="id">待審的信用卡紅利折抵項目編號</param>
        /// <returns>單筆待審的信用卡紅利折抵項目清單</returns>
        public ResponseMessage<BankBonusTemp_DM> GetBankBonusTempById(int id)
        {
            #region 變數宣告

            ResponseMessage<BankBonusTemp_DM> result = new ResponseMessage<BankBonusTemp_DM>();

            // DB Model 暫存
            BankBonusTemp bankBonusTemp = null;
            
            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            #endregion 變數宣告

            try
            {
                #region 檢查傳入參數

                stepMessage = "檢查傳入參數";

                if (id <= 0)
                {
                    result.Message = "待審的信用卡紅利折抵項目編號，不可小於等於 0。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("取得單筆待審的信用卡紅利折抵項目失敗。待審的信用卡紅利折抵項目編號(ID) = {2}。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage,
                        id);

                    isException = false;
                    throw new Exception();
                }

                #endregion 檢查傳入參數

                // 讀取 DB
                stepMessage = "讀取 DB ";
                bankBonusTemp = this._IBankBonusRepoAdapter.GetAllBankBonusTemp().Where(x => x.ID == id).FirstOrDefault();

                if (bankBonusTemp == null)
                {
                    result.Message = string.Format("查無編號為 {0} 的待審信用卡紅利折抵項目。", id);
                    result.Error.Code = (int)ErrorCode.查無資料;
                    result.Error.Detail = string.Format("取得單筆待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorMessage = 查無編號為 {1} 的待審信用卡紅利折抵項目。", ErrorCode.查無資料.ToString(), id);

                    isException = false;
                    throw new Exception();
                }

                // 轉換 DBModel 到 DomainModel
                stepMessage = "轉換 DBModel 到 DomainModel ";
                result.Data = ModelConverter.ConvertTo<BankBonusTemp_DM>(bankBonusTemp);

                if (result.Data == null)
                {
                    throw new Exception("轉換 DBModel 到 DomainModel 失敗。");
                }

                stepMessage = "填寫回傳訊息";
                result.Message = string.Format("成功取得編號為 {0} 的待審信用卡紅利折抵項目。", id);
                result.IsSuccess = true;
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;

                if (isException)
                {
                    result.Message = string.Format("{0}失敗。", stepMessage);
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("取得單筆待審的信用卡紅利折抵項目失敗。待審的信用卡紅利折抵項目編號(ID) = {4}。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        stepMessage,
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 更新待審的信用卡紅利折抵項目
        /// </summary>
        /// <param name="bankBonusTemp_DM">要更新的信用卡紅利折抵項目內容</param>
        public ResponseMessage<bool> UpdateBankBonusTemp(BankBonusTemp_DM bankBonusTemp_DM)
        {
            #region 變數宣告

            ResponseMessage<bool> result = new ResponseMessage<bool>();

            // DB Model 暫存
            BankBonusTemp bankBonusTemp = null;

            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            #endregion 變數宣告

            try
            {
                #region 檢查傳入參數

                stepMessage = "檢查傳入參數";

                if (bankBonusTemp_DM == null)
                {
                    result.Message = ErrorCode.未傳入參數.ToString();
                    result.Error.Code = (int)ErrorCode.未傳入參數;
                    result.Error.Detail = string.Format("更新待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.未傳入參數.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                #endregion 檢查傳入參數

                // 轉換 DomainModel 到 DBModel
                stepMessage = "轉換 DomainModel 到 DBModel";
                bankBonusTemp = ModelConverter.ConvertTo<BankBonusTemp>(bankBonusTemp_DM);

                if (bankBonusTemp == null)
                {
                    throw new Exception("轉換 DomainModel 到 DBModel 失敗。");
                }

                stepMessage = "更新待審的信用卡紅利折抵項目";
                this._IBankBonusRepoAdapter.UpdateBankBonusTemp(bankBonusTemp);

                stepMessage = "填寫回傳訊息";
                result.IsSuccess = true;
                result.Data = true;
                result.Message = "更新待審的信用卡紅利折抵項目成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = false;

                if (isException)
                {
                    result.Message = string.Format("{0}失敗。", stepMessage);
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("更新待審的信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        stepMessage,
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        /// <summary>
        /// 審核通過信用卡紅利折抵項目
        /// </summary>
        /// <param name="idCell">待審的信用卡紅利折抵項目編號</param>
        /// <param name="updateUser">更新者</param>
        /// <returns>審核通過的信用卡紅利折抵項目清單</returns>
        public ResponseMessage<List<BankBonus_DM>> ApproveBankBonus(List<int> idCell, string updateUser)
        {
            #region 變數宣告

            ResponseMessage<List<BankBonus_DM>> result = new ResponseMessage<List<BankBonus_DM>>();

            // 是否真的發生 Exception (控制回傳的錯誤訊息，是否要讀取 Exception 的)
            bool isException = true;

            // 目前執行流程(用在發生 exception 時，寫入回傳訊息)
            string stepMessage = null;

            // 待審清單
            List<BankBonusTemp> bankBonusTempCell = null;

            // 原項目編號清單(BankBonusID 有值的，也就是在生效區已經有資料的)
            List<int> bankBonusIdCell = null;

            // 銀行代碼清單
            List<string> bankCodeCell = null;

            // 審核通過清單
            List<BankBonus> bankBonusCell = null;

            // 前一次的變更記錄清單
            List<BankBonus> bankBonusLastLogCell = null;

            // 目前可用的排序值
            ResponseMessage<int> maxOrder = null;

            // 生效區圖片路徑
            string photoUrl = ConfigurationManager.AppSettings["HttpImageFolderAuditPass"];

            #endregion 變數宣告

            try
            {
                #region 檢查傳入參數

                stepMessage = "檢查傳入參數";

                if (idCell == null || idCell.Count == 0)
                {
                    result.Message = ErrorCode.未傳入參數.ToString();
                    result.Error.Code = (int)ErrorCode.未傳入參數;
                    result.Error.Detail = string.Format("審核通過信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.未傳入參數.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                // 若傳入參數中，有任何值小於 0
                if (idCell.Any(x => x < 0))
                {
                    result.Message = "待審的信用卡紅利折抵項目編號中，有小於 0 的值。";
                    result.Error.Code = (int)ErrorCode.傳入參數錯誤;
                    result.Error.Detail = string.Format("審核通過信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。",
                        ErrorCode.傳入參數錯誤.ToString(),
                        stepMessage);

                    isException = false;
                    throw new Exception();
                }

                #endregion 檢查傳入參數

                #region 讀取 BankBonusTemp 要審核的項目

                // 讀取要審核的待審項目
                stepMessage = "讀取要審核的待審項目";
                bankBonusTempCell = this._IBankBonusRepoAdapter.GetAllBankBonusTemp().Where(x => idCell.Contains(x.ID)).ToList();

                if (bankBonusTempCell == null || bankBonusTempCell.Count == 0)
                {
                    throw new Exception("讀取待審清單失敗。");
                }

                if (bankBonusTempCell.Count != idCell.Count)
                {
                    throw new Exception("傳入參數的數量與待審核清單數量不相同。");
                }

                #endregion 讀取 BankBonusTemp 要審核的項目

                // 取得目前可用的最大排序值(因為只取 SerialNumber = 0 的排序值，所以在更動 SerialNumber 前，先設定好目前可用的最大排序值)
                stepMessage = "取得目前可用的最大排序值";
                maxOrder = this.GetMaxOrder();

                #region 判斷待審項目中，是否已存在原項目，有則先將原項目的 SerialNumber + 1

                // 若待審項目中，有任何已存在原項目(BankBonusID 有值的，也就是在生效區已經有資料的)，則先將原項目的 SerialNumber + 1
                if (bankBonusTempCell.Any(x => x.BankBonusID.HasValue))
                {
                    stepMessage = "將原有項目的 SerialNumber + 1 ";

                    // 收集原項目編號
                    bankBonusIdCell = bankBonusTempCell.Where(x => x.BankBonusID.HasValue).Select(x => x.BankBonusID.Value).ToList();

                    if (bankBonusIdCell != null)
                    {
                        // 依原項目編號，將生效區所有符合的項目 SerialNumber 欄位 + 1
                        this._IBankBonusRepoAdapter.UpdateSerialNumber(bankBonusIdCell, updateUser);
                    }
                    else
                    {
                        throw new Exception("收集已存在原項目的銀行代碼失敗。");
                    }

                    // 讀取前一次的更新記錄(因為待審區沒有 order 欄位，所以要讀取修改前的 order 值)
                    stepMessage = "讀取前一次的更新記錄";
                    bankBonusLastLogCell = this._IBankBonusRepoAdapter.GetLastLog(bankBonusIdCell).ToList();

                    if (bankBonusLastLogCell == null)
                    {
                        throw new Exception("讀取前一次的更新記錄失敗。");
                    }

                    // 清除原項目編號清單(從 待審Model 轉到 生效Model 後，因為 生效Model 沒有 BankBonusID，所以轉 Model 之後的邏輯，將以 BankCode 為主)
                    bankBonusIdCell = null;
                }

                #endregion 判斷待審項目中，是否已存在原項目，有則先將原項目的 SerialNumber + 1

                #region 組合要新增的 BankBonus Model

                // 轉換 待審Model 到 生效Model
                stepMessage = "轉換 待審Model 到 生效Model ";
                bankBonusCell = ModelConverter.ConvertTo<List<BankBonus>>(bankBonusTempCell);

                if (bankBonusCell == null)
                {
                    throw new Exception("轉換 待審Model 到 生效Model 失敗。");
                }

                if (maxOrder.IsSuccess == false)
                {
                    throw new Exception(maxOrder.Message);
                }

                // 組合審核通過的資料
                stepMessage = "組合審核通過的資料";
                foreach (BankBonus bankBonus in bankBonusCell)
                {
                    bankBonus.ID = 0;

                    // SerialNumber 欄位，給定 0 值
                    bankBonus.SerialNumber = 0;

                    // 判斷 Order 給值方式，若該筆審核資料的銀行代碼有在前一次的更新記錄清單中，則視為有原項目
                    if ((bankBonusLastLogCell != null && bankBonusLastLogCell.Count > 0) && bankBonusLastLogCell.Any(x => x.BankCode == bankBonus.BankCode))
                    {
                        // 有原項目的，則給原項目的 Order 值
                        bankBonus.Order = bankBonusLastLogCell.Where(x => x.BankCode == bankBonus.BankCode).Select(x => x.Order).First();
                    }
                    else
                    {
                        // 新增項目，則給目前可用的最大排序值
                        bankBonus.Order = maxOrder.Data;
                        maxOrder.Data++;
                    }

                    // 重新命名圖片名稱(LogoImage_ + 銀行代碼 + 原來的副檔名)
                    bankBonus.PhotoName = photoUrl + "/LogoImage_" + bankBonus.BankCode + bankBonus.PhotoName.Substring(bankBonus.PhotoName.IndexOf('.'), bankBonus.PhotoName.Length - bankBonus.PhotoName.IndexOf('.'));

                    // 填寫審核人
                    bankBonus.AuditUser = updateUser;
                    bankBonus.UpdateUser = updateUser;
                }

                #endregion 組合要新增的 BankBonus Model

                // 將通過審核的生效項目，增加到生效區
                stepMessage = "將通過審核的項目加到生效區";
                this._IBankBonusRepoAdapter.CreateBankBonus(bankBonusCell);

                // 清除審核通過清單
                bankBonusCell = null;

                // 刪除將通過審核的待審項目
                stepMessage = "刪除將通過審核的待審項目";
                this._IBankBonusRepoAdapter.DeleteBankBonusTemp(idCell);

                #region 重新讀取審核通過的項目，並回傳

                stepMessage = "重新讀取審核通過的項目";

                // 讀取所有送審的銀行代碼
                bankCodeCell = bankBonusTempCell.Select(x => x.BankCode).ToList();

                // 清除待審清單
                bankBonusTempCell = null;

                if (bankCodeCell == null)
                {
                    throw new Exception("讀取所有送審的銀行代碼失敗。");
                }

                // 重新讀取審核通過的項目
                bankBonusCell = this._IBankBonusRepoAdapter.GetAllBankBonus().Where(x => bankCodeCell.Contains(x.BankCode)).ToList();

                if (bankBonusCell == null || bankBonusCell.Count == 0)
                {
                    throw new Exception("重新讀取審核通過的項目失敗。");
                }

                // 轉換 DBModel 到 DomainModel
                stepMessage = "轉換 DBModel 到 DomainModel ";
                result.Data = ModelConverter.ConvertTo<List<BankBonus_DM>>(bankBonusCell);

                if (result.Data == null || result.Data.Count == 0)
                {
                    throw new Exception("轉換 DBModel 到 DomainModel 失敗。");
                }

                #endregion 重新讀取審核通過的項目，並回傳

                stepMessage = "填寫回傳訊息";
                result.IsSuccess = true;
                result.Message = "審核通過信用卡紅利折抵項目成功。";
                result.Error = null;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Data = null;

                if (isException)
                {
                    result.Message = string.Format("{0}失敗。", stepMessage);
                    result.Error.Code = (int)ErrorCode.例外發生;
                    result.Error.Detail = string.Format("審核通過信用卡紅利折抵項目失敗。ErrorType = {0}。ErrorStep = {1}失敗。ExceptionMessage = {2}。Exception = {3}。",
                        ErrorCode.例外發生.ToString(),
                        stepMessage,
                        this.GetExceptionMessage(ex),
                        ex.ToString());
                }
            }

            return result;
        }

        #endregion BankBonusTemp

        /// <summary>
        /// 取得 Bank 表的銀行編號
        /// </summary>
        /// <param name="bankCode">銀行代號</param>
        /// <returns>銀行編號</returns>
        private ResponseMessage<int> GetBankId(string bankCode)
        {
            return this._IBankService.GetBankId(bankCode);
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
