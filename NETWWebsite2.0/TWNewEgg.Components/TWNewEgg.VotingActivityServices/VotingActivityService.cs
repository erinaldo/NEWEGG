using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VotingActivityServices.Interface;
using TWNewEgg.VotingActivityRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.VotingActivityServices
{
    public class VotingActivityService : IVotingActivityServices
    {
        private IVotingActivity mIVotingActivity = null;
        private IVotingActivityRec mIVotingActivityRec = null;

        /// <summary>
        /// 建構函式
        /// </summary>
        public VotingActivityService(IVotingActivity argIVotingActivity, IVotingActivityRec argIVotingActivityRec)
        {
            this.mIVotingActivity = argIVotingActivity;
            this.mIVotingActivityRec = argIVotingActivityRec;
        }

        #region VotingActivityGroup 投票活動
        public List<Models.DomainModels.VotingActivity.VotingActivityGroup> GetAllVotingGroup()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup> listResult = null;

            objSearch = this.mIVotingActivity.GetAllVotingGroup();
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>>(listTemp);
                }
                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityGroup> GetVotingGroupByOnlineStatus(int argOnlineStatus)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup> listResult = null;

            objSearch = this.mIVotingActivity.GetVotingGroupByOnlineStatus(argOnlineStatus);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>>(listTemp);
                }
                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityGroup> GetActiveVotingGroup()
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup> listResult = null;

            objSearch = this.mIVotingActivity.GetActiveVotingGroup();
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>>(listTemp);
                }
                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public Models.DomainModels.VotingActivity.VotingActivityGroup GetVotingGroupById(int argId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup> objSearch = null;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup objTemp = null;
            TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup objResult = null;

            objSearch = this.mIVotingActivity.GetVotingGroupById(argId);
            if (objSearch != null)
            {
                objTemp = objSearch.FirstOrDefault();
                if (objTemp != null)
                {
                    objResult = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup>(objTemp);
                }
                objTemp = null;
                objSearch = null;
            }

            return objResult;
        }

        public int CreateVotingActivityGroup(Models.DomainModels.VotingActivity.VotingActivityGroup argVotingGroup)
        {
            if (argVotingGroup == null)
            {
                return -1;
            }

            int numId = -1;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup objTemp = null;

            objTemp = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup>(argVotingGroup);
            numId = this.mIVotingActivity.CreateVotingActivityGroup(objTemp);
            objTemp = null;

            return numId;
        }

        public bool UpdateVotingActivityGroup(Models.DomainModels.VotingActivity.VotingActivityGroup argVotingGroup)
        {
            if (argVotingGroup == null)
            {
                return false;
            }

            bool boolExec = false; ;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup objTemp = null;
            if (objTemp != null)
            {
                objTemp = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityGroup>(argVotingGroup);
                boolExec = this.mIVotingActivity.UpdateVotingActivityGroup(objTemp);
            }

            objTemp = null;

            return boolExec;
        }

        #endregion

        #region VotingActivityItems 投票活動的Item

        public List<Models.DomainModels.VotingActivity.VotingActivityItems> GetAllVotingItemsByGroupId(int argGroupId)
        {
            if (argGroupId <= 0)
            {
                return null;
            }

            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> listTemp;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems> listResult = null;

            objSearch = this.mIVotingActivity.GetAllVotingItemsByGroupId(argGroupId);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityItems> GetVotingItemsByGroupIdAndOnlineStatus(int argGroupId, int argOnlineStatus)
        {
            if (argGroupId <= 0)
            {
                return null;
            }

            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> listTemp;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems> listResult = null;

            objSearch = this.mIVotingActivity.GetVotingItemsByGroupIdAndOnlineStatus(argGroupId, argOnlineStatus);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityItems> GetActiveVotingItemsByGroupId(int argGroupId)
        {
            if (argGroupId <= 0)
            {
                return null;
            }

            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> listTemp;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems> listResult = null;

            objSearch = this.mIVotingActivity.GetActiveVotingItemsByGroupId(argGroupId);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public Models.DomainModels.VotingActivity.VotingActivityItems GetVotingItemsByGroupIdAndItemId(int argGroupId, int argItemId)
        {
            if (argGroupId <= 0 || argItemId <= 0)
            {
                return null;
            }

            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems> objSearch = null;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems objTemp;
            TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems objResult = null;

            objSearch = this.mIVotingActivity.GetVotingItemsByGroupIdAndItemId(argGroupId, argItemId);
            if (objSearch != null)
            {
                objTemp = objSearch.FirstOrDefault();
                if (objTemp != null)
                {
                    objResult = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityItems>(objTemp);
                }

                objTemp = null;
                objSearch = null;
            }

            return objResult;
        }

        public bool CreateVotingItems(Models.DomainModels.VotingActivity.VotingActivityItems argVotingItems)
        {
            if (argVotingItems == null)
            {
                return false;
            }

            bool boolExec = false;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems objTemp = null;

            objTemp = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems>(argVotingItems);
            boolExec = this.mIVotingActivity.CreateVotingItems(objTemp);

            return boolExec;
        }

        public bool UpdateVotingItems(Models.DomainModels.VotingActivity.VotingActivityItems argVotingItems)
        {
            if (argVotingItems == null)
            {
                return false;
            }

            bool boolExec = false;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems objTemp = null;

            objTemp = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityItems>(argVotingItems);
            boolExec = this.mIVotingActivity.UpdateVotingItems(objTemp);

            return boolExec;
        }

        #endregion

        #region VotingActivityRec 投票活動的記錄

        public List<Models.DomainModels.VotingActivity.VotingActivityRec> GetVotingRecByAccocunt(string argStrAccountId, string argStrAccountSource)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec> listResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByAccocunt(argStrAccountId, argStrAccountSource);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityRec> GetVotingRecByDate(string argSearchDate)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec> listResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByDate(argSearchDate);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityRec> GetVotingRecByAccountAndDate(string argStrAccountId, string argStrAccountSource, string argSearchDate)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec> listResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByAccountAndDate(argStrAccountId, argStrAccountSource, argSearchDate);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityRec> GetVotingRecByGroupIdAndAccount(int argGroupId, string argStrAccountId, string argStrAccountSource)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec> listResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByGroupIdAndAccount(argGroupId, argStrAccountId, argStrAccountSource);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public Models.DomainModels.VotingActivity.VotingActivityRec GetVotingRecByGroupIdAndAccountAndDate(int argGroupId, string argStrAccountId, string argStrAccountSource, string argSearchDate)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec objTemp = null;
            TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec objResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByGroupIdAndAccountAndDate(argGroupId, argStrAccountId, argStrAccountSource, argSearchDate);
            if (objSearch != null)
            {
                objTemp = objSearch.FirstOrDefault();
                if (objTemp != null)
                {
                    objResult = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>(objTemp);
                    objTemp = null;
                }
                objSearch = null;
            }

            return objResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityRec> GetVotingRecByGroupId(int argGroupId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec> listResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByGroupId(argGroupId);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public List<Models.DomainModels.VotingActivity.VotingActivityRec> GetVotingRecByGroupIdAndItemId(int argGroupId, int argItemId)
        {
            IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> objSearch = null;
            List<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec> listTemp = null;
            List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec> listResult = null;

            objSearch = this.mIVotingActivityRec.GetVotingRecByGroupIdAndItemId(argGroupId, argItemId);
            if (objSearch != null)
            {
                listTemp = objSearch.ToList();
                if (listTemp != null && listTemp.Count > 0)
                {
                    listResult = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRec>>(listTemp);
                }

                listTemp = null;
                objSearch = null;
            }

            return listResult;
        }

        public int CreateVotingRec(Models.DomainModels.VotingActivity.VotingActivityRec argVotingRec)
        {
            if (argVotingRec == null)
            {
                return -1;
            }

            int numId = -1;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec objTemp = null;

            objTemp = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec>(argVotingRec);
            numId = this.mIVotingActivityRec.CreateVotingRec(objTemp);

            return numId;
        }

        public bool UpdateVotingRec(Models.DomainModels.VotingActivity.VotingActivityRec argVotingRec)
        {
            if (argVotingRec == null)
            {
                return false;
            }

            bool boolExec = false;
            TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec objTemp = null;

            objTemp = ModelConverter.ConvertTo<TWNewEgg.Models.DBModels.TWSQLDB.VotingActivityRec>(argVotingRec);
            boolExec = this.mIVotingActivityRec.UpdateVotingRec(objTemp);

            return boolExec;
        }

        #endregion
    }
}
