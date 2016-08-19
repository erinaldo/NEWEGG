using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class SellerFinancialService
    {
        ManageAccountService MA = new ManageAccountService();

        private DB.TWSellerPortalDBContext db = new DB.TWSellerPortalDBContext();
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        #region Seller_Financial table
        /// <summary>
        /// Get table Seller_Financial Data by seller ID
        /// </summary>
        /// <param name="Seller">Table Seller_Financial Seller Name</param>
        public Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> GetSeller_Financial(string Seller, int type)
        {
            int sellerid = 0;
            Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial> Financial = new Models.ActionResponse<DB.TWSELLERPORTALDB.Models.Seller_Financial>();
            Financial.Body = new DB.TWSELLERPORTALDB.Models.Seller_Financial();
            if (type == 0)
            {
                //用ID查詢
                Int32.TryParse(Seller, out sellerid);
            }
            else if (type == 1)
            {
                //用名稱查詢
                sellerid = MA.SellerID(Seller);
            }
            Financial.Body = db.Seller_Financial.Where(x => x.SellerID == sellerid).FirstOrDefault<DB.TWSELLERPORTALDB.Models.Seller_Financial>();
            if (Financial.Body == null)
            {
                Financial.Msg = "Table Seller_Financial Can't find this seller ID!";
                Financial.Code = 1;
                Financial.IsSuccess = false;
            }
            else
            {
                Financial.Msg = "Success";
                Financial.Code = 0;
                Financial.IsSuccess = true;
            }
            return Financial;
        }
        /// <summary>
        /// Save table Seller_Financial
        /// 1. If table Seller_Financial have data , save table
        /// 2. If table Seller_Financial have not data , create table
        /// </summary>
        /// <param name="Financial">Table Seller_Financial Seller Data</param>
        public Models.ActionResponse<string> SaveSeller_Financial(DB.TWSELLERPORTALDB.Models.Seller_Financial Financial)
        {
            Models.ActionResponse<string> massage = CheckInputData(Financial);

            if (massage.IsSuccess)
            {
                DateTime dt = DateTime.UtcNow.AddHours(8);
                dt.GetDateTimeFormats('r')[0].ToString();
                Convert.ToDateTime(dt);
                bool HaveData = false;

                if (db.Seller_Financial.Find(Financial.SellerID) == null)
                {
                    HaveData = true;
                }

                try
                {
                    if (HaveData)
                    {
                        Financial.UpdateDate = dt;
                        Financial.InDate = dt;
                        Financial.InUserID = Financial.UpdateUserID;

                        db.Seller_Financial.Add(Financial);
                        db.SaveChanges();
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Body = "新增成功。";
                    }
                    else
                    {
                        DB.TWSELLERPORTALDB.Models.Seller_Financial SaveData = new DB.TWSELLERPORTALDB.Models.Seller_Financial();
                        SaveData = db.Seller_Financial.Find(Financial.SellerID);

                        SaveData.BankAccountNumber = Financial.BankAccountNumber;
                        SaveData.BankAddress = Financial.BankAddress;
                        SaveData.BankCity = Financial.BankCity;
                        SaveData.BankCountryCode = Financial.BankCountryCode;
                        SaveData.BankName = Financial.BankName;
                        SaveData.BankBranchName = Financial.BankBranchName;
                        SaveData.BankCode = Financial.BankCode;
                        SaveData.BankBranchCode = Financial.BankBranchCode;
                        SaveData.BankState = Financial.BankState;
                        SaveData.BankZipCode = Financial.BankZipCode;
                        SaveData.BeneficiaryAddress = Financial.BeneficiaryAddress;
                        SaveData.BeneficiaryCity = Financial.BeneficiaryCity;
                        SaveData.BeneficiaryName = Financial.BeneficiaryName;
                        SaveData.BeneficiaryState = Financial.BeneficiaryState;
                        SaveData.BeneficiaryZipcode = Financial.BeneficiaryZipcode;
                        SaveData.BeneficiaryCountryCode = Financial.BeneficiaryCountryCode;

                        SaveData.SWIFTCode = Financial.SWIFTCode;
                        SaveData.UpdateUserID = Financial.UpdateUserID;

                        SaveData.UpdateDate = dt;

                        db.Entry(SaveData).State = EntityState.Modified;
                        db.SaveChanges();
                        massage.IsSuccess = true;
                        massage.Code = 0;
                        massage.Msg = "儲存成功。";
                    }
                }
                catch (Exception ex)
                {
                    massage.IsSuccess = false;
                    massage.Code = 1;
                    massage.Msg = "儲存失敗，請聯繫客服人員。";
                    logger.Info(string.Format("儲存財務資訊失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                }
            }

            return massage;
        }
        #endregion Seller_Financial table

        #region 檢查輸入資料

        /// <summary>
        /// 檢查必填欄位是否填值
        /// </summary>
        /// <param name="inputData">要檢查的UpdateInfo</param>
        /// <returns>回傳錯誤訊息，若是Empty則正確</returns>
        public Models.ActionResponse<string> CheckInputData(DB.TWSELLERPORTALDB.Models.Seller_Financial inputData)
        {
            Models.ActionResponse<string> result = new Models.ActionResponse<string>();
            result.Body = string.Empty;
            result.IsSuccess = true;
            result.Msg = string.Empty;

            // SWIFT碼
            if (!string.IsNullOrEmpty(inputData.SWIFTCode))
            {
                if (inputData.SWIFTCode.Length > 50)
                {
                    result.Msg += "SWIFT碼不可超過50個字。 ";
                }

                if (!ISNumberAndAlphabet(inputData.BankAccountNumber))
                {
                    result.Msg += "SWIFT碼僅能為數字和英文字母。 ";
                }
            }

            // 銀行名稱
            if (!string.IsNullOrEmpty(inputData.BankName))
            {
                if (inputData.BankName.Length > 60)
                {
                    result.Msg += "銀行名稱不可超過60個字。 ";
                }
            }
            else
            {
                result.Msg += "銀行名稱為必填。 ";
            }

            // 銀行代碼
            if (!string.IsNullOrEmpty(inputData.BankCode))
            {
                if (inputData.BankCode.Length > 15)
                {
                    result.Msg += "銀行代碼不可超過15個字。 ";
                }

                if (!ISOnlyNumber(inputData.BankCode))
                {
                    result.Msg += "銀行代碼僅能為數字。 ";
                }
            }
            else
            {
                result.Msg += "銀行代碼為必填。 ";
            }

            // 分行名稱
            if (!string.IsNullOrEmpty(inputData.BankBranchName))
            {
                if (inputData.BankBranchName.Length > 60)
                {
                    result.Msg += "分行名稱不可超過60個字。 ";
                }
            }
            else
            {
                result.Msg += "分行名稱為必填。 ";
            }

            // 分行代碼
            if (!string.IsNullOrEmpty(inputData.BankBranchCode))
            {
                if (inputData.BankBranchCode.Length > 15)
                {
                    result.Msg += "分行代碼不可超過15個字。 ";
                }

                if (!ISOnlyNumber(inputData.BankBranchCode))
                {
                    result.Msg += "分行代碼僅能為數字。 ";
                }
            }
            else
            {
                result.Msg += "分行代碼為必填。 ";
            }

            // 銀行帳號
            if (!string.IsNullOrEmpty(inputData.BankAccountNumber))
            {
                if (inputData.BankAccountNumber.Length > 50)
                {
                    result.Msg += "銀行帳號不可超過50個字。 ";
                }

                if (!ISOnlyNumber(inputData.BankAccountNumber))
                {
                    result.Msg += "銀行帳號僅能為數字。 ";
                }
            }
            else
            {
                result.Msg += "銀行帳號為必填。 ";
            }

            // 銀行所在國家/地區(此為下拉式選單填寫項目，若錯誤則改為記錄log)
            if (inputData.BankCountryCode.Length > 2)
            {
                logger.Info(string.Format("銀行所在國家/地區輸入字元數超過2字元(BankCountryCode = {0})", inputData.BankCountryCode));
            }

            // 銀行所在州/省
            if (!string.IsNullOrEmpty(inputData.BankState) && inputData.BankState.Length > 20)
            {
                result.Msg += "銀行所在州/省不可超過20個字。 ";
            }

            // 銀行地址
            if (!string.IsNullOrEmpty(inputData.BankAddress) && inputData.BankAddress.Length > 150)
            {
                result.Msg += "銀行地址不可超過150個字。 ";
            }

            // 銀行所在城市
            if (!string.IsNullOrEmpty(inputData.BankCity) && inputData.BankCity.Length > 20)
            {
                result.Msg += "銀行所在城市不可超過20個字。 ";
            }

            // 銀行郵遞區號
            if (!string.IsNullOrEmpty(inputData.BankZipCode) && inputData.BankZipCode.Length > 10)
            {
                result.Msg += "銀行郵遞區號不可超過10個字。 ";

                if (!ISNumberAndAlphabet(inputData.BankAccountNumber))
                {
                    result.Msg += "銀行郵遞區號僅能為數字和英文字母。 ";
                }
            }

            // 帳戶名稱
            if (!string.IsNullOrEmpty(inputData.BeneficiaryName))
            {
                if (inputData.BeneficiaryName.Length > 60)
                {
                    result.Msg += "帳戶名稱不可超過60個字。 ";
                }
            }
            else
            {
                result.Msg += "帳戶名稱為必填。 ";
            }

            // 發票國家/區域(此為下拉式選單填寫項目，若錯誤則改為記錄log)
            if (!string.IsNullOrEmpty(inputData.BeneficiaryCountryCode))
            {
                if (inputData.BeneficiaryCountryCode.Length > 2)
                {
                    logger.Info(string.Format("發票國家/區域輸入字元數超過2字元(BankCountryCode = {0})", inputData.BeneficiaryCountryCode));
                }
                else if (inputData.BeneficiaryCountryCode != "CN" && inputData.BeneficiaryCountryCode != "CA" && inputData.BeneficiaryCountryCode != "TW" && inputData.BeneficiaryCountryCode != "US" && inputData.BeneficiaryCountryCode != "HK")
                {
                    logger.Info(string.Format("發票國家/區域輸入內容錯誤(BankCountryCode = {0})", inputData.BeneficiaryCountryCode));
                }
            }
            else
            {
                result.Msg += "發票國家/區域為必填。 ";
            }

            // 發票州/省
            if (!string.IsNullOrEmpty(inputData.BeneficiaryState))
            {
                if (inputData.BeneficiaryState.Length > 20)
                {
                    result.Msg += "發票州/省不可超過20個字。 ";
                }
            }
            else
            {
                result.Msg += "發票州/省為必填。 ";
            }

            // 發票地址
            if (!string.IsNullOrEmpty(inputData.BeneficiaryAddress))
            {
                if (inputData.BeneficiaryAddress.Length > 150)
                {
                    result.Msg += "發票地址不可超過150個字。 ";
                }
            }
            else
            {
                result.Msg += "發票地址為必填。 ";
            }

            // 發票城市
            if (!string.IsNullOrEmpty(inputData.BeneficiaryCity))
            {
                if (inputData.BeneficiaryCity.Length > 20)
                {
                    result.Msg += "發票城市不可超過20個字。 ";
                }
            }
            else
            {
                result.Msg += "發票城市為必填。 ";
            }

            // 發票郵遞區號
            if (!string.IsNullOrEmpty(inputData.BeneficiaryZipcode))
            {
                if (inputData.BeneficiaryZipcode.Length > 10)
                {
                    result.Msg += "發票郵遞區號不可超過10個字。 ";
                }

                if (!ISNumberAndAlphabet(inputData.BankAccountNumber))
                {
                    result.Msg += "發票郵遞區號僅能為數字和英文字母。 ";
                }
            }
            else
            {
                result.Msg += "發票郵遞區號為必填。 ";
            }

            if (!string.IsNullOrEmpty(result.Msg))
            {
                result.IsSuccess = false;
                result.Code = (int)Models.ResponseCode.Error;
            }
            else
            {
                result.Code = (int)Models.ResponseCode.Success;
            }

            return result;
        }

        /// <summary>
        /// 檢查文字內容是否只有數字
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool ISOnlyNumber(string value)
        {
            Regex reg = new Regex(@"^[\x30-\x39]+$");

            if (reg.IsMatch(value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 檢查文字內容是否只有數字和英文字母
        /// </summary>
        /// <param name="value">待檢查內容</param>
        /// <returns>檢查結果</returns>
        private bool ISNumberAndAlphabet(string value)
        {
            Regex reg = new Regex(@"^[\x30-\x39A-Za-z]+$");

            if (reg.IsMatch(value))
            {
                return true;
            }

            return false;
        }

        #endregion 檢查輸入資料

        /// <summary>
        /// 取得 Exception 錯誤訊息
        /// </summary>
        /// <param name="ex">Exception 內容</param>
        /// <returns>Exception 錯誤訊息</returns>
        private string GetExceptionMessage(System.Exception ex)
        {
            string errorMessage = string.Empty;

            if (ex.Message.IndexOf("See the inner exception for details.") != -1)
            {
                errorMessage = ex.InnerException.Message;

                if (errorMessage.IndexOf("See the inner exception for details.") != -1)
                {
                    errorMessage = GetExceptionMessage(ex.InnerException);
                }
            }
            else
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}
