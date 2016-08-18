using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDBExtModels;
using TWNewEgg.Models.DomainModels.Message;
using TWNewEgg.Models.DomainModels.Finance;

namespace TWNewEgg.FinanceServices.Interface
{
    public interface ICompanyFinanceDataService
    {
        FinanceDataList GetAll();
        FinanceDataListFinanceData Get(DateTime nowDate);
        FinanceDataListFinanceData Get(DateTime nowDate, List<FinanceDataListFinanceData> finanList);
    }
}
