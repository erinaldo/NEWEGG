using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.PromoAwardLog;


namespace TWNewEgg.PromoAwardLogService.Interface
{
    public interface IPromoAwardLogService
    {
        List<PromoAwardLogDM> GetPromoAwardLogList(int promoActiveID);
        List<PromoAwardLogDM> UpdatePromoAwardLog(List<PromoAwardLogDM> promoAwardLogDMList, int promoActiveID, string UserName);

        List<PromoAwardLogDM> Detail_PromoAwardLogDM(string promoActiveId = "");
    }
}
