using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class StorageDetailService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }
        //Ben Tseng
        #region SP_RPT_TransDetails

        public Models.ActionResponse<List<Models.StorageDetailSPResult>> GetDataStorageDetail(string inputSellerName,string inputSellerID,string ProductID,string SellerProductID)
        {
            Models.ActionResponse<List<Models.StorageDetailSPResult>> result = new Models.ActionResponse<List<Models.StorageDetailSPResult>>();
            result.Body = new List<Models.StorageDetailSPResult>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
            var spResult = TWSellerPortalDB.Database.SqlQuery<Models.StorageDetailSPResult>("exec SP_RPT_Storage '" + inputSellerName + "','" + inputSellerID + "','" + ProductID + "','" + SellerProductID + "'").ToList();
            result.Body.AddRange(spResult);
            return result;
        }

        public Models.ActionResponse<List<Models.StorageDetailSPResult>> PostDataStorageDetail(TWNewEgg.API.Models.StorageDetailSPSearch StorageDetailSPSearch)
        {
            Models.ActionResponse<List<Models.StorageDetailSPResult>> result = new Models.ActionResponse<List<Models.StorageDetailSPResult>>();
            result.Body = new List<Models.StorageDetailSPResult>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
            if (StorageDetailSPSearch.inputSellerID == string.Empty || StorageDetailSPSearch.inputSellerID == null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "StorageDetail Report - inputSellerID required field is filled.";
                return result;
            }

            if (StorageDetailSPSearch.inputSellerID != string.Empty && StorageDetailSPSearch.inputSellerID != null)
            {
                var spResult = TWSellerPortalDB.Database.SqlQuery<Models.StorageDetailSPResult>("exec SP_RPT_Storage '" + StorageDetailSPSearch.inputSellerName + "','" + StorageDetailSPSearch.inputSellerID + "','" + StorageDetailSPSearch.inputNeweggProductID + "','" + StorageDetailSPSearch.inputSellerProductID + "'").ToList();
                result.Finish(true, 0, "Success", spResult);
                return result;
            }
            return result;
        }
        #endregion
    }
}
