using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.EDMRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.EDMRepoAdapters
{
    public class EDMRepoAdapter : IEDMRepoAdapter
    {
        private IRepository<EDMBook> _edmBookDB;

        public EDMRepoAdapter(IRepository<EDMBook> edmBookDB)
        {
            this._edmBookDB = edmBookDB;
        }

        public EDMBook GetLatestEDMBook()
        {
            EDMBook latestEDM = new EDMBook();
            DateTime dateTime = DateTime.UtcNow.AddHours(8);
            latestEDM = this._edmBookDB.GetAll().Where(x => x.EDMDisplay == (int)EDMBook.eDMDefault.前台顯示 && x.HtmlContext != null && x.StartDate < dateTime && x.EndDate > dateTime).OrderByDescending(x => x.StartDate).FirstOrDefault();
            if (latestEDM == null)
            {
                latestEDM = this._edmBookDB.GetAll().Where(x => x.HtmlContext != null).OrderByDescending(x => x.StartDate).FirstOrDefault();
            }
            return latestEDM;
        }

        public EDMBook GetEDMBookByID(int edmID)
        {
            EDMBook latestEDM = new EDMBook();
            latestEDM = this._edmBookDB.GetAll().Where(x => x.ID == edmID).FirstOrDefault();
            return latestEDM;
        }

        public List<EDMBook> GetEDMBookByDate(DateTime? startDate, DateTime? endDate)
        {
            throw new NotImplementedException();
        }
    }
}
