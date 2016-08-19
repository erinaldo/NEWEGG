using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View.Service
{
    public class TaxID_IdentityCard_CheckService
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        #region 統一編號檢查
        public TWNewEgg.API.Models.ActionResponse<string> TaxIDCheck(string TaxID)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            #region 初始化回傳的 TWNewEgg.API.Models.ActionResponse<string> result
            result.IsSuccess = true;
            result.Msg = "檢查成功, 無錯誤";
            #endregion

            #region 是否有輸入統編
            if (string.IsNullOrEmpty(TaxID) == true)
            {
                result.IsSuccess = false;
                result.Msg = "請輸入統一編號";
                return result;
            }
            #endregion
            #region 統編格式為八碼
            if (TaxID.Length != 8)
            {
                result.IsSuccess = false;
                result.Msg = "統編長度必須為 8 碼";
                return result;
            }
            #endregion
            #region 統編一定是數字不會有任何英文符號
            //統編一定是數字不會有任何英文符號
            if (System.Text.RegularExpressions.Regex.IsMatch(TaxID, "^[0-9]*$") == false)
            {
                result.IsSuccess = false;
                result.Msg = "統編格式錯誤";
                return result;
            }
            #endregion
            #region 轉換驗證 不可為 00000000
            //轉換驗證 不可為"00000000"
            try
            {
                int CompanyCodeInt = Convert.ToInt32(TaxID);
                if (CompanyCodeInt == 0)
                {
                    result.IsSuccess = false;
                    result.Msg = "統編不可以為 00000000";
                }
                else
                {
                    result.IsSuccess = true;
                }
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "統編格式轉換錯誤";
                logger.Error("[Message]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[ExceptionInnerMsg]: " + this.ExceptionInnerMsg(error));
            }
            if (result.IsSuccess == false)
            {
                return result;
            }
            #endregion
            #region 把統編轉換成數字 List
            TWNewEgg.API.Models.ActionResponse<List<int>> TaxId_PeopleIdToList = this.ListIntTaxId_PeopleId(TaxID);
            if (TaxId_PeopleIdToList.IsSuccess == false)
            {
                result.IsSuccess = false;
                result.Msg = TaxId_PeopleIdToList.Msg;
                return result;
            }
            #endregion
            #region 開始對統編格式做檢查
            int[] tbNum = new[] { 1, 2, 1, 2, 1, 2, 4, 1 };
            int total = 0, i = 0, temp;
            foreach (int TaxIntId in TaxId_PeopleIdToList.Body)
            {
                temp = TaxIntId;
                temp *= tbNum[i];
                total += (temp / 10) + temp % 10;
                i++;
            }
            //total%10==0 或 當TaxIntId第7個數字為7時，total%10可為9
            if ((total % 10 != 0) && (TaxId_PeopleIdToList.Body[6] != 7 || total % 10 != 9))
            {
                result.IsSuccess = false;
                result.Msg = "統一編號輸入錯誤!";
                return result;
            }
            #endregion

            return result;
        }
        #endregion
        #region 身分證檢查
        public TWNewEgg.API.Models.ActionResponse<string> IdentityCardCheck(string IdentityCard)
        {
            TWNewEgg.API.Models.ActionResponse<string> result = new API.Models.ActionResponse<string>();
            result.IsSuccess = true;
            result.Msg = "格式無錯誤";
            #region 身分證字號不全為數字
            if (System.Text.RegularExpressions.Regex.IsMatch(IdentityCard, "^[0-9]*$") == true)
            {
                result.IsSuccess = false;
                result.Msg = "統編/身分證字號格式錯誤";
                return result;
            }
            #endregion
            #region 身分證格式為一碼英文九碼數字
            if (IdentityCard.Length != 10)
            {
                result.IsSuccess = false;
                result.Msg = "身分證格式為一碼英文九碼數字";
                return result;
            }
            #endregion
            #region 身分證英文後面的第一個數字必須為 1 或 2
            if (IdentityCard[1] != 49 && IdentityCard[1] != 50)
            {
                result.IsSuccess = false;
                result.Msg = "請輸入正確的身分證號碼!";
                return result;
            }
            #endregion
            #region 開始對身分證格式規則做檢查
            IdentityCard = IdentityCard.ToUpper();
            var temp = 0;
            var total = 0;
            var idNum = new[] { 1, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            //驗證係數
            var IdRep = new[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
            //英文代碼 A=10 B=11...
            int[] Id = new int[11];
            Id[0] = IdRep[IdentityCard[0] - 65] / 10;
            Id[1] = IdRep[IdentityCard[0] - 65] % 10;
            for (var i = 2; i <= idNum.Length; i++)
            {
                Id[i] = IdentityCard[i - 1] - 48;
            }
            for (var i = 0; i < idNum.Length; i++)
            {
                temp = Id[i] * idNum[i];
                total += temp;
            }
            //temp暫存相乘數字
            //total紀錄temp全部相加
            //10 - total%10 == ID最後一碼 即合法身分證
            if (!((10 - (total % 10)) % 10 == Id[10]))
            {
                result.IsSuccess = false;
                result.Msg = "身分證輸入錯誤";
                return result;
            }
            #endregion
            return result;
        }
        #endregion
        #region 抓取 InnerException error msg
        public string ExceptionInnerMsg(Exception error)
        {
            string returnMsg = string.Empty;
            returnMsg = error.InnerException == null ? "" : error.InnerException.Message;

            return returnMsg;
        }
        #endregion
        #region 轉換統編字串為數字陣列
        public TWNewEgg.API.Models.ActionResponse<List<int>> ListIntTaxId_PeopleId(string TaxId_PeopleId)
        {
            TWNewEgg.API.Models.ActionResponse<List<int>> result = new API.Models.ActionResponse<List<int>>();
            if (string.IsNullOrEmpty(TaxId_PeopleId) == true)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                return result;
            }
            List<int> tempListInt = new List<int>();
            var charArray = TaxId_PeopleId.ToCharArray();
            try
            {
                for (int i = 0; i < charArray.Length; i++)
                {
                    tempListInt.Add(Convert.ToInt16(charArray[i].ToString()));
                }
                result.IsSuccess = true;
                result.Body = tempListInt;
            }
            catch (Exception error)
            {
                result.IsSuccess = false;
                result.Msg = "資料錯誤";
                logger.Error("[Msg]: " + error.Message + " ;[StackTrace]: " + error.StackTrace + " ;[innerExceptionMsg]: " + this.ExceptionInnerMsg(error));
            }
            return result;
        }
        #endregion
    }
}