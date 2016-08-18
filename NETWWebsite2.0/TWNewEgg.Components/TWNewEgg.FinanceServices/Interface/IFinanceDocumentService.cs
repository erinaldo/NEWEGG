using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;

namespace TWNewEgg.FinanceServices.Interface
{
    public interface IFinanceDocumentService
    {
        ResponseMessage<string> Create(DateTime startFinanDate, DateTime endFinanDate, FinanDocTypeEnum finanDocType);
    }
}
