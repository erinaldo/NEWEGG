using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;

namespace TWNewEgg.FinanceRepoAdapters.Interface
{
    public interface IAccountsProfileRepoAdapter
    {
        IQueryable<AccountsDocumentType> GetAccDocument(AccountsDocumentType.DocTypeEnum docType);
        IQueryable<DeliverType> GetDeliverType();
        IQueryable<ChartOfAccountsProfile> GetChartOfAccPorfile(int accDocTypeCode, int? deliverTypeCode);
    }
}
