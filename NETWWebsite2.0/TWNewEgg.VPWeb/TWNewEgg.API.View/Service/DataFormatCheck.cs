using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TWNewEgg.API.View.Service
{
    public class DataFormatCheck
    {
        public API.Models.ManufacturerValidateSummaryResult ValidateInputData(API.Models.Manufacturer model_InputData)
        {
            // 驗證輸入項目的結果暫存空間
            API.Models.ManufacturerValidateInfo validateInfo = new TWNewEgg.API.Models.ManufacturerValidateInfo();

            // 驗證製造商名稱
            // 驗證內容：不得為空值
            if (!string.IsNullOrEmpty(model_InputData.ManufactureName))
            {
                validateInfo.ManufactureName = true;
            }
            else
            {
                validateInfo.ManufactureName = false;
            }

            // 驗證製造商支援信箱
            // 驗證內容：有值，才驗證是否符合電子信箱格式
            if (string.IsNullOrEmpty(model_InputData.SupportEmail))
            {
                // 若沒值，不驗證，直接通過
                validateInfo.SupportEmail = true;
            }
            else
            {
                // 若有值，驗證是否符合電子信箱格式
                if (ValidateEmail(model_InputData.SupportEmail))
                {
                    validateInfo.SupportEmail = true;
                }
                else
                {
                    validateInfo.SupportEmail = false;
                }
            }

            // 驗證製造商支援網址
            // 驗證項目：有值，才驗證是否符合網址格式
            if (string.IsNullOrEmpty(model_InputData.supportURL))
            {
                // 若沒值，不驗證，直接通過
                validateInfo.supportURL = true;
            }
            else
            {
                // 若有值，驗證是否符合網址格式
                if (ValidateURL(model_InputData.supportURL))
                {
                    validateInfo.supportURL = true;
                }
                else
                {
                    validateInfo.supportURL = false;
                }
            }

            return validateInfo.SummaryResult;
        }

        /// <summary>
        /// 加上網址抬頭
        /// </summary>
        /// <param name="strURL">增加抬頭前的網址</param>
        /// <returns>增加抬頭後的網址</returns>
        public string AddURLTitle(string strURL)
        {
            return string.Format("http://{0}", strURL);
        }

        /// <summary>
        /// 檢查網址是否有抬頭
        /// </summary>
        /// <param name="strURL">被檢查網址</param>
        /// <returns>ture：有，false：沒有</returns>
        public bool CheckURLTitle(string strURL)
        {
            return (strURL.IndexOf(@"http://") == 0 || strURL.IndexOf(@"https://") == 0);
        }

        /// <summary>
        /// 檢查網址最後一個字元是否為斜線
        /// </summary>
        /// <param name="strURL">被檢查網址</param>
        /// <returns>ture：是斜線 false：不是斜線</returns>
        public bool CheckURLLastWordIsSlash(string strURL)
        {
            // 讀取製造商網址最後一個字
            string urlLastWord = strURL.Substring(strURL.Length - 1);

            return urlLastWord == "/";
        }

        /// <summary>
        /// 驗證是否符合電子信箱格式
        /// </summary>
        /// <param name="strEmail">被驗證電子信箱位址</param>
        /// <returns>ture：驗證成功，false：驗證失敗</returns>
        public bool ValidateEmail(string strEmail)
        {
            // 輸入不為空才進行電子信箱格式檢查
            if (!string.IsNullOrEmpty(strEmail))
            {
                // 指定電子信箱格式
                string strPattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

                return Regex.IsMatch(strEmail.Trim(), strPattern);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 驗證是否符合網址格式
        /// </summary>
        /// <param name="strURL">被驗證網址</param>
        /// <returns>ture：驗證成功，false：驗證失敗</returns>
        public bool ValidateURL(string strURL)
        {
            // 輸入不為空才進行網址格式檢查
            if (!string.IsNullOrEmpty(strURL))
            {
                // 指定網址格式
                string strPattern = @"^http[s]?://[\w-_.%/:?=&#]+$";

                return Regex.IsMatch(strURL.Trim(), strPattern);
            }
            else
            {
                return false;
            }
        }
    }
}