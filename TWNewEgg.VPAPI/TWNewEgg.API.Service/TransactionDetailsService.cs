using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Service
{
    public class TransactionDetailsService
    {
        public enum ResponseCode
        {
            Success = 0,
            Error = 1
        }

        //Ben Tseng
        #region SP_RPT_TransDetails
        public Models.ActionResponse<List<Models.TransactionSPResult>> GetDataTransactionSP(string inputOrderNumber, string inputInvoiceNumber, string inputSettlementID, string inputSellerPartNum, string inputNewEggItemNum)
        {
            Models.ActionResponse<List<Models.TransactionSPResult>> result = new Models.ActionResponse<List<Models.TransactionSPResult>>();
            result.Body = new List<Models.TransactionSPResult>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
            var spResult = TWSellerPortalDB.Database.SqlQuery<Models.TransactionSPResult>("exec SP_RPT_TransDetails '" + inputOrderNumber + "','" + inputInvoiceNumber + "','" + inputSettlementID + "','" + inputSellerPartNum + "','" + inputNewEggItemNum + "'").ToList();
            result.Body.AddRange(spResult);
            return result;
        }

        public Models.ActionResponse<List<Models.TransactionSPResult>> PostDataTransactionSP(TWNewEgg.API.Models.TransactionSPSearch TransactionSPSearch)
        {
            Models.ActionResponse<List<Models.TransactionSPResult>> result = new Models.ActionResponse<List<Models.TransactionSPResult>>();
            result.Body = new List<Models.TransactionSPResult>();
            DB.TWSellerPortalDBContext TWSellerPortalDB = new DB.TWSellerPortalDBContext();
            if (TransactionSPSearch.inputSellerID == string.Empty || TransactionSPSearch.inputSellerID == null)
            {
                result.Code = (int)ResponseCode.Error;
                result.IsSuccess = false;
                result.Msg = "Transaction Report - inputSellerID required field is filled.";
                return result;
            }

            if (TransactionSPSearch.inputSellerID != string.Empty && TransactionSPSearch.inputSellerID != null)
            {
                if (TransactionSPSearch.inputStartDate == string.Empty || TransactionSPSearch.inputStartDate == null)
                {
                    TransactionSPSearch.inputStartDate = "1900/01/01";
                }
                if (TransactionSPSearch.inputEndDate == string.Empty || TransactionSPSearch.inputEndDate == null)
                {
                    TransactionSPSearch.inputEndDate = "9999/01/01";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("exec SP_RPT_TransDetails '");
                sb.Append(TransactionSPSearch.inputSellerID + "','");
                sb.Append(TransactionSPSearch.inputOrderNumber + "','");
                sb.Append(TransactionSPSearch.inputInvoiceNumber + "','");
                sb.Append(TransactionSPSearch.inputSettlementID + "','");
                sb.Append(TransactionSPSearch.inputSellerProductNum + "','");
                sb.Append(TransactionSPSearch.inputNewEggProductNum + "','");
                sb.Append(TransactionSPSearch.inputTransType + "','");
                sb.Append(TransactionSPSearch.inputIsOnlySettledRecords + "','");
                sb.Append(TransactionSPSearch.inputStartDate + "','");
                sb.Append(TransactionSPSearch.inputEndDate);
                sb.Append("'");
                var spResult = TWSellerPortalDB.Database.SqlQuery<Models.TransactionSPResult>(sb.ToString()).ToList();
                result.Finish(true, 0, "Success", spResult);
                return result;
            }
            return result;
        }

        #endregion


    }
}
