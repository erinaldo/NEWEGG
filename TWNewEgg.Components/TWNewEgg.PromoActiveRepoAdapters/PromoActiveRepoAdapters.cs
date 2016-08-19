using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PromoActiveRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PromoActiveRepoAdapters
{
    public class PromoActiveRepoAdapter:IPromoActiveRepoAdapters
    {
        private IRepository<PromoActive> _PromoActiveDB;

        public PromoActiveRepoAdapter(IRepository<PromoActive> promoActive)
        {
            this._PromoActiveDB = promoActive;
        }
        /// <summary>
        /// 讀取所有活動的資料
        /// </summary>
        /// <returns></returns>
        public IQueryable<PromoActive> GetAllPromoActive()
        {
            IQueryable<PromoActive> _IqueryablePromoActive = null;
            _IqueryablePromoActive = _PromoActiveDB.GetAll().AsQueryable();
            return _IqueryablePromoActive;
        }
        #region IPP
        /// <summary>
        /// 明細設定需撈單筆資料
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public PromoActive GetPromoActive(int ID)
        {
            PromoActive _IqueryablePromoActive = null;
            if (ID != null && ID != 0)
            {
                _IqueryablePromoActive = _PromoActiveDB.Get(x => x.ID == ID);
            }
            
            return _IqueryablePromoActive;
        }
        /// <summary>
        /// 更新單筆資料
        /// </summary>
        /// <param name="promoActive"></param>
        /// <returns></returns>
        public PromoActive UpdatePromoActiveDetail(TWNewEgg.Models.DBModels.TWSQLDB.PromoActive promoActive)
        {
            try
            {
                this._PromoActiveDB.Update(promoActive);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return promoActive;
           
        }
        /// <summary>
        /// 新增單筆資料
        /// </summary>
        /// <param name="promoActive"></param>
        /// <returns></returns>
        public PromoActive CreatePromoActiveDetail(TWNewEgg.Models.DBModels.TWSQLDB.PromoActive promoActive)
        {
            try
            {
                _PromoActiveDB.Create(promoActive);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return promoActive;

        }
        #endregion
    }
}
