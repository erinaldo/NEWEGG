using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;

namespace TWNewEgg.FinanceServices.Interface
{
    public interface ISapDocumentService
    {
        ResponseMessage<List<SapBapiAccDocumentDM>> GetData(DocConditionDM condition);

        ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> GetCustomerData(DateTime nowDate);

        ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> GetCustomerDataByCartID(DateTime nowDate, List<string> cartIDList);

        ResponseMessage<List<ZNETW_CUSTOMERCUSTOMERDATA>> GetCustomerDataByDocNumber(DateTime nowDate, List<string> docNumberList);

        ResponseMessage<List<SAPLogDM>> GetSAPLog(DateTime startDate, DateTime endDate, FinanDocTypeEnum docType, SAPLogDM.LogTypeEnum logType, List<string> cartIDList);

        ResponseMessage<List<string>> RedoFinanceDocument(List<string> salesOrderList);
    }
}
