using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.API.View.Service
{
    public class GetManufacturerListService
    {
        // 連接至 API
        TWNewEgg.API.Models.Connector connector = new API.Models.Connector();
        private log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);
        public TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.View.ItemListManufacturer>> GetManufacturer()
        {
            // API 製造商清單
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.Models.Manufacturer>> apiResult = new API.Models.ActionResponse<List<API.Models.Manufacturer>>();
            TWNewEgg.API.Models.ActionResponse<List<TWNewEgg.API.View.ItemListManufacturer>> returnResult = new API.Models.ActionResponse<List<ItemListManufacturer>>();
            List<TWNewEgg.API.View.ItemListManufacturer> result = new List<ItemListManufacturer>();
            // 顯示在下拉式選單的製造商清單
            
            // 指定搜尋類型
            TWNewEgg.API.Models.SearchDataModel searchData = new API.Models.SearchDataModel();
            searchData.SearchType = API.Models.SearchType.SearchofficialALLInfo;
            try
            {
                // 讀取 API 製造商清單
                apiResult = connector.SearchManufacturerInfo(searchData);
                apiResult.IsSuccess = true;
            }
            catch (Exception ex)
            {
                log.Info(string.Format("取得製造商清單失敗(exception); ExceptionMessage = {0}; StackTrace = {1}.", GetExceptionMessage(ex), ex.StackTrace));
                apiResult.IsSuccess = false;
            }
            if (apiResult.IsSuccess)
            {
                if (apiResult.Body.Count > 0)
                {
                    // 將API 製造商清單轉成下拉式選單的製造商清單 Model
                    result = apiResult.Body.Select(x => new TWNewEgg.API.View.ItemListManufacturer
                    {
                        ManufactureName = x.ManufactureName,
                        SN = x.SN
                    }).ToList();

                    // 將製造商清單依製造商名稱排序
                    result = result.OrderBy(x => x.ManufactureName).ToList();
                    returnResult.IsSuccess = true;
                    returnResult.Body = result;
                }
                else
                {
                    log.Info("查無製造商資料。");
                }
            }
            else
            {
                log.Info(string.Format("取得製造商清單失敗; APIMessage = {0}.", apiResult.Msg));
            }
            return returnResult;
        }


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