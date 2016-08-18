using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.FinanceRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DBModels.TWSQLDB;
using System.Data.Objects.SqlClient;

namespace TWNewEgg.FinanceRepoAdapters
{
    public class AccountsProfileRepoAdapter : IAccountsProfileRepoAdapter
    {
        log4net.ILog _logger;

        private IBackendRepository<AccountsDocumentType> _accDocTypeRepo;
        private IBackendRepository<DeliverType> _deliverTypeRepo;
        private IBackendRepository<GLAccounts> _glAccounts;
        private IBackendRepository<ChartOfAccountsProfile> _chartAccProfileRepo;

        public AccountsProfileRepoAdapter(IBackendRepository<AccountsDocumentType> accDocTypeRepo, IBackendRepository<DeliverType> deliverTypeRepo, IBackendRepository<GLAccounts> glAccounts, 
            IBackendRepository<ChartOfAccountsProfile> chartAccProfileRepo)
        {
            this._logger = log4net.LogManager.GetLogger(LoggerInfo.FinanceLog);

            this._accDocTypeRepo = accDocTypeRepo;
            this._deliverTypeRepo = deliverTypeRepo;
            this._glAccounts = glAccounts;
            this._chartAccProfileRepo = chartAccProfileRepo;
        }

        public IQueryable<AccountsDocumentType> GetAccDocument(AccountsDocumentType.DocTypeEnum docType)
        {
            string strDocType = docType.ToString();
            return this._accDocTypeRepo.GetAll()
                .Where(x => x.DocType ==  strDocType);
        }

        public IQueryable<DeliverType> GetDeliverType()
        {
            return this._deliverTypeRepo.GetAll();
        }

        public IQueryable<ChartOfAccountsProfile> GetChartOfAccPorfile(int accDocTypeCode, int? deliverTypeCode)
        {
            DateTime nowDate = DateTime.UtcNow.AddHours(8);

            var list = this._chartAccProfileRepo.GetAll()
                .Where(x => x.AccDocTypeCode == accDocTypeCode &&
                (
                x.UseFlag == "1" ||
                (x.UseFlag == "8" && nowDate < x.UseDate) ||
                (x.UseFlag == "9" && nowDate >= x.UseDate)
                ));

            if (deliverTypeCode != null)
                list = list.Where(x => x.DeliverTypeCode == deliverTypeCode.Value);

            list = list.OrderBy(x => x.AccDocTypeCode).ThenBy(x => x.DeliverTypeCode).ThenBy(x => x.Seq);

            return list;
        }
    }
}
