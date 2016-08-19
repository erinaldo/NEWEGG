using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PromoAwardLogRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PromoAwardLogRepoAdapters
{
    public class PromoAwardLogRepoAdapters: IPromoAwardLogRepoAdapters
    {
        private IRepository<PromoAwardLog> _PromoAwardLogDB;

        public PromoAwardLogRepoAdapters(IRepository<PromoAwardLog> promoAwardLog)
        {
            this._PromoAwardLogDB = promoAwardLog;
        }
        
        /// <summary>
        /// 依中獎公告的明細ID撈該明細中裡的中獎名單
        /// </summary>
        /// <param name="promoActiveID"></param>
        /// <returns></returns>
        public IQueryable<PromoAwardLog> GetPromoAwardLogList(int promoActiveID)
        {
            IQueryable<PromoAwardLog> promoAwardLoglist = null;
            promoAwardLoglist = _PromoAwardLogDB.GetAll();
            if (promoActiveID > 0)
            {
                promoAwardLoglist = promoAwardLoglist.Where(x => x.PromoActiveID == promoActiveID);
            }
            return promoAwardLoglist;
        }
        /// <summary>
        /// 匯入Excel要先刪除該明細ID裡面的所有中獎名單
        /// </summary>
        /// <param name="promoAwardLog"></param>
        /// <returns></returns>
        public bool DeletePromoAwardLog(PromoAwardLog promoAwardLog)
        {
            try {
                this._PromoAwardLogDB.Delete(promoAwardLog);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
 
        }
        /// <summary>
        /// 匯入Excel刪後的新增
        /// </summary>
        /// <param name="promoAwardLog"></param>
        /// <returns></returns>
        public PromoAwardLog CreatePromoAwardLog(PromoAwardLog promoAwardLog)
        {
            try
            {
                this._PromoAwardLogDB.Create(promoAwardLog);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return promoAwardLog;
        }
    }
}
