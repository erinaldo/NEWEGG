using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.FinanceRepoAdapters.Interface
{
    public interface ISapBapiAccDocumentRepoAdapter
    {
        void SaveDocument(SapDocumentInfo sapInfo);

        IEnumerable<SapBapiAccDocumentInfo> GetData(DateTime startDate, DateTime endDate, AccountsDocumentType.DocTypeEnum docType, List<string> soCodeList);

        List<CustomerInfo> GetCustomerData(DateTime startDate, DateTime endDate, FinanceDataListFinanceData twNewEggData);

        List<CustomerInfo> GetCustomerDataByCartID(List<string> cartIDList, FinanceDataListFinanceData twNewEggData);

        List<CustomerInfo> GetCustomerDataByDocNumber(List<string> docNumberList, FinanceDataListFinanceData twNewEggData);

        List<SAPLogInfo> GetSAPLog(DateTime startDate, DateTime endDate, List<string> docTypeList, SAPLogInfo.LogTypeEnum logType, List<string> cartIDList);

        bool RedoFinanceDocument(string soCode, int docTypeCode);
    }
}
