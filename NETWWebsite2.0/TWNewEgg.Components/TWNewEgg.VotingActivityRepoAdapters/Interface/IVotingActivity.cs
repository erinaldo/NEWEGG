using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL.Interface;

namespace TWNewEgg.VotingActivityRepoAdapters.Interface
{
    /// <summary>
    /// 投票活動
    /// </summary>
    public interface IVotingActivity
    {
        #region VotingActivityGroup 投票活動

        /// <summary>
        /// 取得所有的投票活動列表
        /// </summary>
        /// <returns></returns>
        IQueryable<VotingActivityGroup> GetAllVotingGroup();

        /// <summary>
        /// 取得符合OnlineStatus設定的投票活動
        /// </summary>
        /// <param name="argOnlineStatus">OnlineStatus</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityGroup> GetVotingGroupByOnlineStatus(int argOnlineStatus);

        /// <summary>
        /// 取得有效的投票活動
        /// </summary>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityGroup> GetActiveVotingGroup();

        /// <summary>
        /// 根據Id取得投票活動
        /// </summary>
        /// <param name="argId">VotingActivityGroup Id</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityGroup> GetVotingGroupById(int argId);

        /// <summary>
        /// 新增投票活動
        /// </summary>
        /// <param name="argVotingGroup">欲新增的投票活動物件</param>
        /// <returns>新增成功回傳流水編號, 新增失敗回傳-1</returns>
        int CreateVotingActivityGroup(VotingActivityGroup argVotingGroup);

        /// <summary>
        /// 修改VotingActivityGroup物件
        /// </summary>
        /// <param name="argVotingGroup">修改的物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateVotingActivityGroup(VotingActivityGroup argVotingGroup);
        #endregion

        #region VotingActivityItems 投票活動的Item

        /// <summary>
        /// 取得該GroupId下所有的VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityItems> GetAllVotingItemsByGroupId(int argGroupId);

        /// <summary>
        /// 取得該GroupId下, 符合OnlineStatus設定的VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <param name="argOnlineStatus">OnlineStatus</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityItems> GetVotingItemsByGroupIdAndOnlineStatus(int argGroupId, int argOnlineStatus);

        /// <summary>
        /// 根據GroupId取得旗下有效的VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityItems> GetActiveVotingItemsByGroupId(int argGroupId);

        /// <summary>
        /// 根據GroupId與ItemId取得VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">VotingActivityGroup Id</param>
        /// <param name="argItemId">Item Id</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityItems> GetVotingItemsByGroupIdAndItemId(int argGroupId, int argItemId);

        /// <summary>
        /// 新增投票活動的Items物件
        /// </summary>
        /// <param name="argVotingItems">新增的物件</param>
        /// <returns>新增成功, 回傳true, 新增失敗, 回傳false</returns>
        bool CreateVotingItems(VotingActivityItems argVotingItems);

        /// <summary>
        /// 修改VotingActivityItems
        /// </summary>
        /// <param name="argVotingItems">修改的物件</param>
        /// <returns>true:修改成功, false:修改失敗</returns>
        bool UpdateVotingItems(VotingActivityItems argVotingItems);
        #endregion
    }
}