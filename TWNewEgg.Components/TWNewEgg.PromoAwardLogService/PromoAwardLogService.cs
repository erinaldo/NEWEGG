using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PromoAwardLogService.Interface;
using TWNewEgg.Models.DomainModels.PromoActive;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.PromoAwardLogRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DomainModels.PromoAwardLog;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.PromoActiveRepoAdapters;
using TWNewEgg.PromoActiveRepoAdapters.Interface;

namespace TWNewEgg.PromoAwardLogService
{
    public class PromoAwardLogService: IPromoAwardLogService
    {
        private IPromoAwardLogRepoAdapters _promoAwardLogRepoAdapters;
        private IAccountRepoAdapter _accountRepoAdapter;
        private IPromoActiveRepoAdapters _promoActiveRepoAdapters;
        public PromoAwardLogService(IPromoAwardLogRepoAdapters promoAwardLogRepoAdapters, IAccountRepoAdapter accountRepoAdapter, IPromoActiveRepoAdapters promoActiveRepoAdapters)
        {
            this._promoAwardLogRepoAdapters = promoAwardLogRepoAdapters;
            this._accountRepoAdapter = accountRepoAdapter;
            this._promoActiveRepoAdapters = promoActiveRepoAdapters;
        }
        /// <summary>
        /// 搜尋中獎名單 (IPP Excel匯出以及中獎公告顯示是否已上傳Excel)
        /// </summary>
        /// <param name="promoActiveID"></param>
        /// <returns></returns>
        public List<PromoAwardLogDM> GetPromoAwardLogList(int promoActiveID)
        {
            //用中獎公告ID來撈該table的List中獎名單
            List<PromoAwardLog> PromoAwardLogData = _promoAwardLogRepoAdapters.GetPromoAwardLogList(promoActiveID).ToList();
            List<PromoAwardLogDM> retutnrtest = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<PromoAwardLogDM>>(PromoAwardLogData);
            
            return retutnrtest;
        }
        /// <summary>
        /// 先刪名單再新增名單(IPP Excel匯入名單功能)
        /// </summary>
        /// <param name="promoAwardLogDMList"></param>
        /// <param name="promoActiveID"></param>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public List<PromoAwardLogDM> UpdatePromoAwardLog(List<PromoAwardLogDM> promoAwardLogDMList, int promoActiveID, string UserName)
        {
            //用來回傳
            List<PromoAwardLogDM> promoAwardLogDMListresult = new List<PromoAwardLogDM>();
            //撈資料
            List<PromoAwardLog> deletepromoAwardLogList = _promoAwardLogRepoAdapters.GetPromoAwardLogList(promoActiveID).ToList();
            //有資料則刪除
            if (deletepromoAwardLogList.Count != 0)
            {
                foreach (PromoAwardLog depromoAwardLog in deletepromoAwardLogList)
                {
                    _promoAwardLogRepoAdapters.DeletePromoAwardLog(depromoAwardLog);
                }
            }
            List<PromoAwardLog> promoAwardLogList = ModelConverter.ConvertTo<List<PromoAwardLog>>(promoAwardLogDMList);
            //新增資料
            foreach (PromoAwardLog promoAwardLog in promoAwardLogList)
            {
                promoAwardLog.UpdateDate = DateTime.UtcNow.AddHours(8);
                promoAwardLog.UpdateUser = UserName;
                promoAwardLog.CreateDate = DateTime.UtcNow.AddHours(8);
                promoAwardLog.CreateUser = UserName;
                promoAwardLog.PromoActiveID = promoActiveID;
                //用Email到Account table比對 撈會員的ID
                Account account = _accountRepoAdapter.GetAccount(promoAwardLog.Email);
                if (account != null)
                {
                    promoAwardLog.AccountID = account.ID;
                }
                else
                {
                    promoAwardLog.AccountID = 0;
                }
                PromoAwardLog _promoAwardLog = _promoAwardLogRepoAdapters.CreatePromoAwardLog(promoAwardLog);
                PromoAwardLogDM _promoAwardLogDM = ModelConverter.ConvertTo<PromoAwardLogDM>(_promoAwardLog);
                promoAwardLogDMListresult.Add(_promoAwardLogDM);
            }
            
            return promoAwardLogDMListresult;
        }
        #region 中獎名單明細
        public List<TWNewEgg.Models.DomainModels.PromoAwardLog.PromoAwardLogDM> Detail_PromoAwardLogDM(string promoActiveId = "")
        {
            TWNewEgg.Models.DomainModels.Message.ResponseMessage<List<PromoAwardLogDM>> result = new Models.DomainModels.Message.ResponseMessage<List<PromoAwardLogDM>>();
            //判斷是否有傳入查詢的 id
            if (string.IsNullOrEmpty(promoActiveId) == true)
            {
                return null;
            }
            IEnumerable<PromoAwardLog> PromoAwardLogDetail = null;
            #region 轉換傳過來的 id 為 int 並判斷是否有錯誤
            int int_promoActiveId = 0;
            int.TryParse(promoActiveId, out int_promoActiveId);
            if (int_promoActiveId == 0)
            {
                return null;
            }
            #endregion
            #region 利用活動 id 讀取對應的活動相關資料
            var _promoActivity = this._promoActiveRepoAdapters.GetPromoActive(int_promoActiveId);
            //判斷是否有這個活動
            if (_promoActivity == null)
            {
                return null;
            }
            #endregion
            //取出活動名稱
            string promoActivityName = _promoActivity.Name;
            //讀取所有對應活動的中獎名單資料
            PromoAwardLogDetail = this._promoAwardLogRepoAdapters.GetPromoAwardLogList(int_promoActiveId);
            //判斷是否有中獎名單的資料
            if (PromoAwardLogDetail.Any() == false)
            {
                return null;
            }
            List<TWNewEgg.Models.DomainModels.PromoAwardLog.PromoAwardLogDM> returnModelPromoAwardLogDM = new List<PromoAwardLogDM>();
            returnModelPromoAwardLogDM = TWNewEgg.Framework.AutoMapper.ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.PromoAwardLog.PromoAwardLogDM>>(PromoAwardLogDetail);
            if (returnModelPromoAwardLogDM.Count == 0 || returnModelPromoAwardLogDM == null)
            {
                return returnModelPromoAwardLogDM;
            }
            //利用第 0 的位置暫時放活動名稱
            returnModelPromoAwardLogDM[0].activityName = string.IsNullOrEmpty(promoActivityName) == null ? string.Empty : promoActivityName;
            return returnModelPromoAwardLogDM;
        }
        #endregion
    }
}
