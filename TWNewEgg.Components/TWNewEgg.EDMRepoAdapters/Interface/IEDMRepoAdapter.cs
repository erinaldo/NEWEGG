using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.EDMRepoAdapters.Interface
{
    public interface IEDMRepoAdapter
    {
        EDMBook GetLatestEDMBook();
        EDMBook GetEDMBookByID(int edmID);
        List<EDMBook> GetEDMBookByDate(DateTime? startDate, DateTime? endDate);
    }
}
