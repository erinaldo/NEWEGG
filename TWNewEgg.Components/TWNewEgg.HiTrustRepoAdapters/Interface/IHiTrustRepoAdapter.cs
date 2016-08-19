using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.HiTrustRepoAdapters.Interface
{
    public interface IHiTrustRepoAdapter
    {
        IQueryable<HiTrust> GetAllHiTrustSetting();
        IQueryable<HiTrustTrans> GetAllHiTrustTrans();
        IQueryable<HiTrustQuery> GetAllHiTrustQuery();
        HiTrustTrans UpdateHiTrustTrans(HiTrustTrans HiTrustTransData);
        HiTrustQuery UpdateHiTrustQuery(HiTrustQuery HiTrustQueryData);
        HiTrustTransLog UpdateHiTrustTransLog(HiTrustTransLog HiTrustTransLogData);
        HiTrustQueryLog UpdateHiTrustQueryLog(HiTrustQueryLog HiTrustQueryLogData);
    }
}
