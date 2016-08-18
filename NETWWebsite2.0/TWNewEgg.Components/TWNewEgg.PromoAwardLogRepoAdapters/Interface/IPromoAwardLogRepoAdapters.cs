using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PromoAwardLogRepoAdapters.Interface
{
    public interface IPromoAwardLogRepoAdapters
    {
       
        IQueryable<PromoAwardLog> GetPromoAwardLogList(int promoActiveID);
        bool DeletePromoAwardLog(PromoAwardLog promoAwardLog);
        PromoAwardLog CreatePromoAwardLog(PromoAwardLog promoAwardLog);
    }
}
