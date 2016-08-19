using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TWNewEgg.VotingActivityRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.VotingActivityRepoAdapters
{
    public class VotingActivityRecRepo : IVotingActivityRec
    {
        private IRepository<VotingActivityRec> _RepoVotingRec;

        public VotingActivityRecRepo(IRepository<VotingActivityRec> argRepoVotingRec)
        {
            this._RepoVotingRec = argRepoVotingRec;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityRec> GetVotingRecByAccocunt(string argStrAccountId, string argStrAccountSource)
        {
            IQueryable<VotingActivityRec> objResult = null;

            objResult = this._RepoVotingRec.GetAll().Where(x => x.AccountId == argStrAccountId && x.AccountSource == argStrAccountSource);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityRec> GetVotingRecByDate(string argSearchDate)
        {
            IQueryable<VotingActivityRec> objResult = null;


            objResult = this._RepoVotingRec.GetAll().Where(x => x.VoteDate == argSearchDate);

            return objResult;
        }

        public IQueryable<VotingActivityRec> GetVotingRecByAccountAndDate(string argStrAccountId, string argStrAccountSource, string argSearchDate)
        {
            IQueryable<VotingActivityRec> objResult = null;


            objResult = this._RepoVotingRec.GetAll().Where(x => x.VoteDate == argSearchDate && x.AccountId == argStrAccountId && x.AccountSource == argStrAccountSource);

            return objResult;
        }

        public IQueryable<VotingActivityRec> GetVotingRecByGroupIdAndAccount(int argGroupId, string argStrAccountId, string argStrAccountSource)
        {
            IQueryable<VotingActivityRec> objResult = null;

            objResult = this._RepoVotingRec.GetAll().Where(x => x.GroupId == argGroupId && x.AccountId == argStrAccountId && x.AccountSource == argStrAccountSource);

            return objResult;
        }

        public IQueryable<VotingActivityRec> GetVotingRecByGroupIdAndAccountAndDate(int argGroupId, string argStrAccountId, string argStrAccountSource, string argSearchDate)
        {
            IQueryable<VotingActivityRec> objResult = null;

            objResult = this._RepoVotingRec.GetAll().Where(x => x.GroupId == argGroupId && x.AccountId == argStrAccountId && x.AccountSource == argStrAccountSource && x.VoteDate == argSearchDate);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityRec> GetVotingRecByGroupId(int argGroupId)
        {
            IQueryable<VotingActivityRec> objResult = null;

            objResult = this._RepoVotingRec.GetAll().Where(x => x.GroupId == argGroupId);

            return objResult;

        }

        public IQueryable<VotingActivityRec> GetVotingRecByGroupIdAndItemId(int argGroupId, int argItemId)
        {
            IQueryable<VotingActivityRec> objResult = null;

            objResult = this._RepoVotingRec.GetAll().Where(x => x.GroupId == argGroupId && x.Rec.IndexOf("\"VotingItem\":\"" + argItemId.ToString() + "\"") >= 0);

            return objResult;
        }

        public int CreateVotingRec(Models.DBModels.TWSQLDB.VotingActivityRec argVotingRec)
        {
            if (argVotingRec == null)
            {
                return -1;
            }

            int numId = -1;

            try
            {
                this._RepoVotingRec.Create(argVotingRec);
                numId = argVotingRec.Id;
            }
            catch (Exception ex)
            {
                numId = -1;
            }

            return numId;
        }

        public bool UpdateVotingRec(Models.DBModels.TWSQLDB.VotingActivityRec argVotingRec)
        {
            bool boolExec = false;
            Models.DBModels.TWSQLDB.VotingActivityRec objTempRec = null;

            try
            {
                objTempRec = this._RepoVotingRec.Get(x => x.GroupId == argVotingRec.GroupId && x.AccountId == argVotingRec.AccountId && x.AccountSource == argVotingRec.AccountSource && x.VoteDate == argVotingRec.VoteDate);
                if (objTempRec != null)
                {
                    objTempRec.Rec = argVotingRec.Rec;
                    objTempRec.Email = argVotingRec.Email;

                    this._RepoVotingRec.Update(objTempRec);
                    boolExec = true;
                }
            }
            catch (Exception ex)
            {
                boolExec = false;
            }

            return boolExec;
        }
    }
}