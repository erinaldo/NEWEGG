using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL.Interface;
using TWNewEgg.VotingActivityRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.VotingActivityRepoAdapters
{
    public class VotingActivityRepo : IVotingActivity
    {
        private IRepository<VotingActivityGroup> _RepoVotingGroup;
        private IRepository<VotingActivityItems> _RepoVotingItems;

        public VotingActivityRepo(IRepository<VotingActivityGroup> argRepoVotingGroup, IRepository<VotingActivityItems> argRepoVotingItems)
        {
            this._RepoVotingGroup = argRepoVotingGroup;
            this._RepoVotingItems = argRepoVotingItems;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityGroup> GetAllVotingGroup()
        {
            return this._RepoVotingGroup.GetAll();
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityGroup> GetVotingGroupByOnlineStatus(int argOnlineStatus)
        {
            IQueryable<VotingActivityGroup> objResult = null;

            objResult = this._RepoVotingGroup.GetAll().Where(x => x.OnlineStatus == argOnlineStatus);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityGroup> GetActiveVotingGroup()
        {
            IQueryable<VotingActivityGroup> objResult = null;
            DateTime dateNow = DateTime.Now;

            objResult = this._RepoVotingGroup.GetAll().Where(x => x.OnlineStatus == 1 && x.DisplayStartDate <= dateNow && x.DisplayEndDate >= dateNow);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityGroup> GetVotingGroupById(int argId)
        {
            IQueryable<VotingActivityGroup> objResult = null;

            objResult = this._RepoVotingGroup.GetAll().Where(x => x.Id == argId);

            return objResult;

        }

        public int CreateVotingActivityGroup(Models.DBModels.TWSQLDB.VotingActivityGroup argVotingGroup)
        {
            int numId = -1;

            try
            {
                this._RepoVotingGroup.Create(argVotingGroup);
                numId = argVotingGroup.Id;
            }
            catch (Exception ex)
            {
                numId = -1;
            }

            return numId;
        }

        public bool UpdateVotingActivityGroup(Models.DBModels.TWSQLDB.VotingActivityGroup argVotingGroup)
        {
            bool boolExec = false;

            try
            {
                this._RepoVotingGroup.Update(argVotingGroup);
                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
            }

            return boolExec;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityItems> GetAllVotingItemsByGroupId(int argGroupId)
        {
            IQueryable<VotingActivityItems> objResult = null;

            objResult = this._RepoVotingItems.GetAll().Where(x => x.GroupId == argGroupId);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityItems> GetVotingItemsByGroupIdAndOnlineStatus(int argGroupId, int argOnlineStatus)
        {
            IQueryable<VotingActivityItems> objResult = null;

            objResult = this._RepoVotingItems.GetAll().Where(x => x.GroupId == argGroupId && x.OnlineStatus == argOnlineStatus).OrderBy(x=>x.ShowOrder);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityItems> GetActiveVotingItemsByGroupId(int argGroupId)
        {
            IQueryable<VotingActivityItems> objResult = null;
            DateTime dateNow = DateTime.Now;

            objResult = this._RepoVotingItems.GetAll().Where(x => x.GroupId == argGroupId && x.OnlineStatus == 1 && x.DisplayStartDate <= dateNow && x.DisplayEndDate >= dateNow).OrderBy(x=>x.ShowOrder);

            return objResult;
        }

        public IQueryable<Models.DBModels.TWSQLDB.VotingActivityItems> GetVotingItemsByGroupIdAndItemId(int argGroupId, int argItemId)
        {
            IQueryable<VotingActivityItems> objResult = null;

            objResult = this._RepoVotingItems.GetAll().Where(x => x.GroupId == argGroupId && x.ItemId == argItemId);

            return objResult;
        }

        public bool CreateVotingItems(Models.DBModels.TWSQLDB.VotingActivityItems argVotingItems)
        {
            if (argVotingItems == null)
            {
                return false;
            }

            bool boolExec = false;

            try
            {
                this._RepoVotingItems.Create(argVotingItems);
                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
            }

            return boolExec;
        }

        public bool UpdateVotingItems(Models.DBModels.TWSQLDB.VotingActivityItems argVotingItems)
        {
            if (argVotingItems == null)
            {
                return false;
            }

            bool boolExec = false;
            Models.DBModels.TWSQLDB.VotingActivityItems objTempItem = null;

            try
            {

                objTempItem = this._RepoVotingItems.Get(x => x.GroupId == argVotingItems.GroupId && x.ItemId == argVotingItems.ItemId);
                objTempItem.Description = argVotingItems.Description;
                objTempItem.DisplayEndDate = argVotingItems.DisplayEndDate;
                objTempItem.DisplayStartDate = argVotingItems.DisplayStartDate;
                objTempItem.InitVoting = argVotingItems.InitVoting;
                objTempItem.OnlineStatus = argVotingItems.OnlineStatus;
                objTempItem.RealVoting = argVotingItems.RealVoting;
                objTempItem.ShowOrder = argVotingItems.ShowOrder;
                objTempItem.SystemVoting = argVotingItems.SystemVoting;
                objTempItem.Title = argVotingItems.Title;
                objTempItem.UpdateDate = DateTime.Now;
                objTempItem.UpdateUser = argVotingItems.UpdateUser;
                objTempItem.VotingEndDate = argVotingItems.VotingEndDate;
                objTempItem.VotingStartDate = argVotingItems.VotingStartDate;
                this._RepoVotingItems.Update(objTempItem);
                boolExec = true;
            }
            catch (Exception ex)
            {
                boolExec = false;
            }

            return boolExec;
        }

        public bool UpdateVotingItems(List<Models.DBModels.TWSQLDB.VotingActivityItems> argListVotingItems)
        {
            throw new NotImplementedException();
        }
    }
}