using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace TWNewEgg.Website.ECWeb.Service
{
    public class AddressRestrictions
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        private string cityStreetKeywordsRule = System.Configuration.ConfigurationManager.AppSettings["CityStreetKeywordsRule"];
        //private string cityStreetRestrictions = System.Configuration.ConfigurationManager.AppSettings["CityStreetRestrictions"];

        /// <summary>
        /// 檢核收件人地址是否不符合設定規則
        /// </summary>
        /// <param name="cityAddr">城市</param>
        /// <param name="streetAddr">鄉鎮市區街道</param>
        /// <param name="exceptionStr">可使用的特殊符號，若無則直接給予空字串</param>
        /// <returns>返回檢核結果，空字串表示檢核通過，非空字串則代表不通過檢核</returns>
        //public string CheckingCompleteAddress(string cityAddr, string streetAddr, string exceptionStr)
        //{
        //    // 排除特殊符號
        //    string delivAddress = CheckingSpecialSymbols(cityAddr, streetAddr, exceptionStr).FirstOrDefault().Value;
        //    if(delivAddress.Length <= 0)
        //    {
        //        logger.Error("地址檢核不通過，該地址為空違反配送地址設訂規則");
        //        return "地址缺漏請重新填寫";
        //    }

        //    List<string> checkAddress = cityStreetRestrictions.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
        //    foreach (string address in checkAddress)
        //    {
        //        string trimAddress = address.Trim();
        //        if (address.Trim().Length > 0)
        //        {
        //            // 驗證縣市
        //            string cityCheck = string.Empty;
        //            // 驗證鄉鎮市區
        //            string countyCheck = string.Empty;
        //            // 是否需驗證鄉鎮市區
        //            // 不驗證鄉鎮市區
        //            if (address.IndexOf("#") == -1)
        //            {
        //                cityCheck = trimAddress;
        //                List<string> cityList = cityCheck.Split(new char[] { '[', ']', '|' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
        //                foreach (string city in cityList)
        //                {
        //                    string subCity = city.Trim();
        //                    // delivAddress內含不允許的縣市
        //                    if (delivAddress.IndexOf(subCity) != -1)
        //                    {
        //                        // 返回檢核不通過訊息
        //                        logger.Error("地址檢核不通過，該地址為 [" + delivAddress + "]違反配送地址設訂規則");
        //                        return "我們提供的商品配送區域僅限於台灣本島，請重新填寫地址";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                // 需驗證鄉鎮市區
        //                cityCheck = trimAddress.Split('#')[0];
        //                countyCheck = trimAddress.Split('#')[1];
        //                List<string> cityList = cityCheck.Split(new char[] { '[', ']', '|' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
        //                List<string> countyList = countyCheck.Split(new char[] { '[', ']', '|' }).Where(s => !string.IsNullOrEmpty(s)).ToList();
        //                foreach (string city in cityList)
        //                {
        //                    string subCity = city.Trim();
        //                    // delivAddress內含不允許的縣市
        //                    if (delivAddress.IndexOf(subCity) != -1)
        //                    {
        //                        foreach (string county in countyList)
        //                        {
        //                            string subCounty = county.Trim();
        //                            // delivAddress內含不允許的鄉鎮市區
        //                            if (delivAddress.IndexOf(subCounty) != -1)
        //                            {
        //                                // 返回檢核不通過訊息
        //                                logger.Error("地址檢核不通過，該地址為 [" + delivAddress + "]違反配送地址設訂規則");
        //                                return "我們提供的商品配送區域僅限於台灣本島，請重新填寫地址";
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return "";
        //}

        /// <summary>
        /// 檢核收件人地址是否不符合關鍵字設定規則
        /// </summary>
        /// <param name="cityAddr">城市</param>
        /// <param name="streetAddr">鄉鎮市區街道</param>
        /// <param name="exceptionStr">可使用的特殊符號，若無則直接給予空字串</param>
        /// <returns>返回檢核結果，空字串表示檢核通過，非空字串則代表不通過檢核</returns>
        public string CheckingAddressKeyword(string cityAddr, string streetAddr, string exceptionStr)
        {
            // 排除特殊符號
            string delivAddress = CheckingSpecialSymbols(cityAddr, streetAddr, exceptionStr).FirstOrDefault().Value;
            if (delivAddress.Length <= 0)
            {
                logger.Error("地址檢核不通過，該地址為空違反配送地址設訂規則");
                return "地址缺漏請重新填寫";
            }

            List<string> checkAddress = cityStreetKeywordsRule.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
            foreach (string address in checkAddress)
            {
                string trimAddress = address.Trim();
                // delivAddress內含不允許的縣市鄉鎮市區島嶼
                if (delivAddress.IndexOf(trimAddress) != -1)
                {
                    // 返回檢核不通過訊息
                    logger.Error("地址檢核不通過，該地址為 [" + delivAddress + "]違反配送地址設訂規則");
                    return "我們提供的商品配送區域僅限於台灣本島，請重新填寫地址";
                }
            }

            return "";
        }

        /// <summary>
        /// 檢查地址是否包含特殊符號
        /// </summary>
        /// <param name="cityAddr">城市</param>
        /// <param name="streetAddr">鄉鎮市區街道</param>
        /// <param name="exceptionStr">可使用的特殊符號，若無則直接給予空字串</param>
        /// <returns>若無特殊符號則返回原地址，Dictionary中的key為true否則false，value則存新地址</returns>
        public Dictionary<bool, string> CheckingSpecialSymbols(string cityAddr, string streetAddr, string exceptionStr)
        {
            Dictionary<bool, string> result = new Dictionary<bool, string>();
            if (streetAddr != null && streetAddr.Length > 0)
            {
                streetAddr = streetAddr.Replace(" ", "");
            }

            string delivAddress = cityAddr + streetAddr;
            char[] checkCharArray;
            char[] exceptionStrArray = exceptionStr.ToCharArray();

            if (delivAddress.Length > 0)
            {
                checkCharArray = delivAddress.ToCharArray();
                string newAddress = string.Empty;
                newAddress = delivAddress;
                foreach (char subChar in checkCharArray)
                {
                    if (Regex.Matches(subChar.ToString(), "[\x21-\x2F\x3A-\x40\x5B-\x60\x7B-\x7E]").Count > 0)
                    {
                        char getChar = exceptionStrArray.Where(x => x == subChar).FirstOrDefault();
                        if (getChar == '\0')
                        {
                            newAddress = newAddress.Replace(subChar.ToString(), "");
                        }
                    }
                }

                delivAddress = newAddress;
            }

            if (delivAddress != cityAddr + streetAddr)
            {
                result.Add(false, delivAddress);
            }
            else
            {
                result.Add(true, delivAddress);
            }

            return result;
        }
    }
}
