using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PromoActiveRepoAdapters.Interface
{
    public interface IPromoActiveRepoAdapters
    {
        IQueryable<PromoActive> GetAllPromoActive();
        #region IPP
        //單筆資料
        PromoActive GetPromoActive(int ID);
        //更新
        PromoActive UpdatePromoActiveDetail(TWNewEgg.Models.DBModels.TWSQLDB.PromoActive promoActive);
        //新增
        PromoActive CreatePromoActiveDetail(TWNewEgg.Models.DBModels.TWSQLDB.PromoActive promoActive);
        #endregion
    }
}
