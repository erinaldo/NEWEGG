using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VendorModels.DBModels.Model;

namespace TWNewEgg.Seller_MailLogRepoAdapters.Interface
{
    public interface ISeller_MailLogRepoAdapters
    {
        void Create(Seller_MailLog model);
        IQueryable<Seller_MailLog> GetAll();
        void Update(Seller_MailLog model);
    }
}
