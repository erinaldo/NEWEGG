using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class SummaryService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        //Ben Tseng
        #region SP_RPT_Summary

        public Models.ActionResponse<List<Models.SettlementInfo>> GetDataSummary(int sellerID, string beginDate, string endDate)
        {
            Models.ActionResponse<List<Models.SettlementInfo>> result = new Models.ActionResponse<List<Models.SettlementInfo>>();
            result.Body = new List<Models.SettlementInfo>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
            var spResult = TWSellerPortalDB.Database.SqlQuery<Models.SettlementInfo>("exec SP_RPT_Summary '" + sellerID + "','" + beginDate + "','" + endDate +"'").ToList();
            result.Body.AddRange(spResult);
            return result;
        }

        public Models.ActionResponse<List<Models.SummarySPResult>> PostDataSummary(TWNewEgg.API.Models.SummarySPSrarch SummarySPSrarch)
        {
            Models.ActionResponse<List<Models.SummarySPResult>> result = new Models.ActionResponse<List<Models.SummarySPResult>>();
            result.Body = new List<Models.SummarySPResult>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();

            if (SummarySPSrarch.inputSellerID == string.Empty || SummarySPSrarch.inputSellerID == null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Summary Report - inputSellerID required field is filled.";
                return result;
            }

            if (SummarySPSrarch.inputSellerID != string.Empty && SummarySPSrarch.inputSellerID != null)
            {
                //Modified and comment out by Ted 20140314
                //DateTime StartDate = DateTime.MinValue;
                //DateTime EndDate = DateTime.MinValue;
                //DateTime.TryParse(SummarySPSrarch.inputStartDate, out StartDate);
                //DateTime.TryParse(SummarySPSrarch.inputEndDate, out EndDate);

                //Built by Ben, Modified by Ted 20140314
                if (SummarySPSrarch.inputStartDate == string.Empty || SummarySPSrarch.inputStartDate == null)
                {
                    SummarySPSrarch.inputStartDate = DateTime.MinValue.ToShortDateString(); //"1900/01/01"; 改使用MinValue modified by Ted 20140314
                }
                if (SummarySPSrarch.inputEndDate == string.Empty || SummarySPSrarch.inputEndDate == null)
                {
                    SummarySPSrarch.inputEndDate = DateTime.MaxValue.ToShortDateString();  //"9999/01/01"; 改使用MaxValue modified by Ted 20140314
                }

                //string StartDate = DateTime.TryParse(SummarySPSrarch.inputSellerID);
                //string EndDate = "";

                //改為使用string.Format //modified
                string sqlcmd = string.Format("exec SP_RPT_Summary '{1}', '{1}', '{2}'", SummarySPSrarch.inputSellerID, SummarySPSrarch.inputStartDate, SummarySPSrarch.inputEndDate);
                var spResult = TWSellerPortalDB.Database.SqlQuery<Models.SummarySPResult>(sqlcmd).ToList();
                result.Finish(true, 0, "Success", spResult);
                return result;
            }
            return result;
        }

        #endregion
    }

}
