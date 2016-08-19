using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Common
{
    public class NeweggCommonService
    {
        public enum LogiticsPartner
        {
            Others = 64,
            HiLife = 65,
            HCT = 66,
        }

        public enum MonthCode
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4, 
            E = 5,
            F = 6,
            G = 7,
            H = 8,
            I = 9,
            J = 10,
            K = 11,
            L = 12
        }

        public NeweggCommonService()
        {
        }

        /// <summary>
        /// 此函式將會自動產生流水號, 若不足碼則在左側補上傳入的strSymbol符號
        /// </summary>
        /// <param name="argNumTotalLengh">總長度</param>
        /// <param name="argStrSymbol">補齊符號</param>
        /// <returns></returns>
        public string GetNewNumber(int argNumTotalLengh, char argStrSymbol)
        {
            string strResult = "";



            return strResult;
        }
    }
}
