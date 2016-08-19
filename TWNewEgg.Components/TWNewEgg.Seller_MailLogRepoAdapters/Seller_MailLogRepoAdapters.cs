using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Seller_MailLogRepoAdapters.Interface;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_MailLogRepoAdapters
{
    public class Seller_MailLogRepoAdapters : ISeller_MailLogRepoAdapters
    {
        private ITWSELLERPORTALDBRepository<Seller_MailLog> _seller_MailLog;

        public Seller_MailLogRepoAdapters(ITWSELLERPORTALDBRepository<Seller_MailLog> seller_MailLog)
        {
            this._seller_MailLog = seller_MailLog;
        }

        public void Create(Seller_MailLog model)
        {
            this._seller_MailLog.Create(model);
        }

        public IQueryable<Seller_MailLog> GetAll()
        {
            return this._seller_MailLog.GetAll();
        }

        public void Update(Seller_MailLog model)
        {
            this._seller_MailLog.Update(model);
        }
    }
}
