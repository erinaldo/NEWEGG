using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Redeem.Service.PromotionGiftExportService
{
    public class PromotionGiftImportCheckDataService : IPromotionGiftImportCheckDataService
    {
        public ActionResponse<List<PromotionGiftImportCheckData>> CheckImportDate(List<PromotionGiftImportCheckData> PromotionGiftImportModelList)
        {
            ActionResponse<List<PromotionGiftImportCheckData>> result = new ActionResponse<List<PromotionGiftImportCheckData>>();
            int intConut = 0;
            int intPromotionGiftItemID;
            string strStatus;

            try
            {
                foreach (var item in PromotionGiftImportModelList)
                {
                    intConut++;
                    //verification init
                    string strNullError = "";
                    string strTypeError = "";
                    string strPromotionGiftItemIDRange = "";
                    string strDuplicatePromotionGiftItemID = "";
                    string strStatusRange = "";

                    //PromotionGiftItemId 
                    if (string.IsNullOrEmpty(item.PromotionGiftItemId))
                    {
                        item.errorRow = intConut;
                        strNullError += "活動ID";
                    }
                    else if (Int32.TryParse(item.PromotionGiftItemId, out intPromotionGiftItemID) == false)
                    {
                        item.errorRow = intConut;
                        strTypeError += "活動ID";
                    }
                    else if (Int32.TryParse(item.PromotionGiftItemId, out intPromotionGiftItemID))
                    {
                        if (intPromotionGiftItemID < 0 || intPromotionGiftItemID > 2147483647)
                        {
                            item.errorRow = intConut;
                            strPromotionGiftItemIDRange += "活動ID";
                        }

                        //檢查重複的PromotionGiftItemID
                        var _PromotionGiftItemIDList = PromotionGiftImportModelList.Where(x => x.PromotionGiftItemId == item.PromotionGiftItemId);
                        if (_PromotionGiftItemIDList.Count() > 1)
                        {
                            item.errorRow = intConut;
                            strDuplicatePromotionGiftItemID += "活動ID";
                        }
                    }

                    //status
                    if (string.IsNullOrEmpty(item.status))
                    {
                        item.errorRow = intConut;
                        strNullError += "上下架狀態";
                    }
                    else
                    {
                        strStatus = item.status.ToLower();

                        if (!(strStatus == "上線" || strStatus == "下線" || strStatus == "testing")) 
                        {
                            item.errorRow = intConut;
                            strStatusRange += "上下架狀態";
                        }
                    }

                    item.ImportExcelNullError = strNullError;
                    item.ImportExcelTypeError = strTypeError;
                    item.ImportExcelItemIDRangeError = strPromotionGiftItemIDRange;
                    item.ImportExcelCheckDuplicateItemIDError = strDuplicatePromotionGiftItemID;
                    item.ImportExcelStatusRangeError = strStatusRange;
                }

                string NullTotal = "";
                string TypeTotal = "";
                string PromotionGiftItemIDRangeTotal = "";
                string DuplicatePromotionGiftItemIDTotal = "";
                string statusRangeTotal = "";
                foreach (var item in PromotionGiftImportModelList)
                {
                    NullTotal += item.ImportExcelNullError;
                    TypeTotal += item.ImportExcelTypeError;
                    PromotionGiftItemIDRangeTotal += item.ImportExcelItemIDRangeError;
                    DuplicatePromotionGiftItemIDTotal += item.ImportExcelCheckDuplicateItemIDError;
                    statusRangeTotal += item.ImportExcelStatusRangeError;
                }

                if (string.IsNullOrEmpty(NullTotal) && string.IsNullOrEmpty(TypeTotal) && string.IsNullOrEmpty(PromotionGiftItemIDRangeTotal) && string.IsNullOrEmpty(DuplicatePromotionGiftItemIDTotal) && string.IsNullOrEmpty(statusRangeTotal))
                {
                    result.Code = (int)ResponseCode.Success;
                    result.IsSuccess = true;
                    result.Body = PromotionGiftImportModelList;
                    result.Msg = "Success 沒有找到任何錯誤";
                }
                else
                {
                    result.Code = (int)ResponseCode.Error;
                    result.IsSuccess = false;
                    result.Body = PromotionGiftImportModelList;
                    result.Msg = "fail 有欄位填寫錯誤";
                }
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = (int)ResponseCode.Error;
                result.Body = PromotionGiftImportModelList;
                if (ex.InnerException != null)
                {
                    result.Msg = ex.InnerException.Message + ex.InnerException.StackTrace;
                }
                else
                {
                    result.Msg = ex.Message;
                }
                return result;
            }
        }
    }//End class
}//End namespace
