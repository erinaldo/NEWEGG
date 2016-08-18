using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TWNewEgg.API.Service
{
    public class SettlementService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }
        
        //Ben Tseng
        #region exec SP_RPT_Settlement

        public Models.ActionResponse<List<Models.SettlementSPResult>> GetDataSettlement(int inputSellerID, string inputStartDate, string inputEndDate)
         {
             Models.ActionResponse<List<Models.SettlementSPResult>> result = new Models.ActionResponse<List<Models.SettlementSPResult>>();
             result.Body = new List<Models.SettlementSPResult>();
             DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
             var spResult = TWSellerPortalDB.Database.SqlQuery<Models.SettlementSPResult>("exec SP_RPT_Settlement '" + inputSellerID + "','" + inputStartDate + "','" + inputEndDate + "'").ToList();
             result.Body.AddRange(spResult);
             return result;
         }

        public Models.ActionResponse<List<Models.SettlementSPResult>> PostDataSettlement(TWNewEgg.API.Models.SettlementSPSearch SettlementSPSearch)
        {
            Models.ActionResponse<List<Models.SettlementSPResult>> result = new Models.ActionResponse<List<Models.SettlementSPResult>>();
            result.Body = new List<Models.SettlementSPResult>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
            if (SettlementSPSearch.inputSellerID == string.Empty || SettlementSPSearch.inputSellerID == null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Settlement Report - inputSellerID required field is filled.";
                return result;
            }

            if (SettlementSPSearch.inputSellerID != string.Empty && SettlementSPSearch.inputSellerID != null)
            {
                if (SettlementSPSearch.inputStartdate == string.Empty || SettlementSPSearch.inputStartdate == null)
                {
                    SettlementSPSearch.inputStartdate = "1900/01/01";
                }
                if (SettlementSPSearch.inputEndDate == string.Empty || SettlementSPSearch.inputEndDate == null)
                {
                    SettlementSPSearch.inputEndDate = "9999/01/01";
                }
                var spResult = TWSellerPortalDB.Database.SqlQuery<Models.SettlementSPResult>("exec SP_RPT_Settlement '" + SettlementSPSearch.inputSellerID + "','" + SettlementSPSearch.inputStartdate + "','" + SettlementSPSearch.inputEndDate + "'").ToList();
                result.Finish(true, 0, "Success", spResult);
                return result;
            }
            return result;
        }
        #endregion
    }
}
