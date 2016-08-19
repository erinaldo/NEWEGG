using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.VotingActivity;
using TWNewEgg.VotingActivityRepoAdapters;

namespace TWNewEgg.VotingActivityServices.Interface
{
    public interface IVotingActivityServices
    {
        #region VotingActivityGroup 投票活動
        /// <summary>
        /// 取得所有的投票活動列表
        /// </summary>
        /// <returns></returns>
        List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup> GetAllVotingGroup();

        /// <summary>
        /// 取得符合OnlineStatus設定的投票活動
        /// </summary>
        /// <param name="argOnlineStatus">OnlineStatus</param>
        /// <returns>IQueryable</returns>
        List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup> GetVotingGroupByOnlineStatus(int argOnlineStatus);

        /// <summary>
        /// 取得有效的投票活動
        /// </summary>
        /// <returns>IQueryable</returns>
        List<TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup> GetActiveVotingGroup();

        /// <summary>
        /// 根據Id取得投票活動
        /// </summary>
        /// <param name="argId">VotingActivityGroup Id</param>
        /// <returns>IQueryable</returns>
        TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup GetVotingGroupById(int argId);

        /// <summary>
        /// 新增投票活動
        /// </summary>
        /// <param name="argVotingGroup">欲新增的投票活動物件</param>
        /// <returns>新增成功回傳流水編號, 新增失敗回傳-1</returns>
        int CreateVotingActivityGroup(TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup argVotingGroup);

        /// <summary>
        /// 修改VotingActivityGroup物件
        /// </summary>
        /// <param name="argVotingGroup">修改的物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateVotingActivityGroup(TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityGroup argVotingGroup);
        #endregion

        #region VotingActivityItems 投票活動的Item

        /// <summary>
        /// 取得該GroupId下所有的VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityItems> GetAllVotingItemsByGroupId(int argGroupId);

        /// <summary>
        /// 取得該GroupId下, 符合OnlineStatus設定的VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <param name="argOnlineStatus">OnlineStatus</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityItems> GetVotingItemsByGroupIdAndOnlineStatus(int argGroupId, int argOnlineStatus);

        /// <summary>
        /// 根據GroupId取得旗下有效的VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityItems> GetActiveVotingItemsByGroupId(int argGroupId);

        /// <summary>
        /// 根據GroupId與ItemId取得VotingActivityItems
        /// </summary>
        /// <param name="argGroupId">VotingActivityGroup Id</param>
        /// <param name="argItemId">Item Id</param>
        /// <returns>IQueryable</returns>
        VotingActivityItems GetVotingItemsByGroupIdAndItemId(int argGroupId, int argItemId);

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

        #region VotingActivityRec 投票活動的記錄
        /// <summary>
        /// 根據帳號取得投票記錄
        /// </summary>
        /// <param name="argStrAccountId">帳號</param>
        /// <param name="argStrAccountSource">帳號來源</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityRec> GetVotingRecByAccocunt(string argStrAccountId, string argStrAccountSource);

        /// <summary>
        /// 查詢當日所有的投票記錄
        /// </summary>
        /// <param name="argSearchDate">查詢當日, 格式為yyyy/MM/dd</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityRec> GetVotingRecByDate(string argSearchDate);

        /// <summary>
        /// 根據帳號與日期, 查詢該Account的投票記錄
        /// </summary>
        /// <param name="argStrAccountId">Account Id</param>
        /// <param name="argStrAccountSource">Account Source, ex: Newegg, FB, Yahoo, Google+, etc.</param>
        /// <param name="argSearchDate">日期, yyyy/MM/dd</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityRec> GetVotingRecByAccountAndDate(string argStrAccountId, string argStrAccountSource, string argSearchDate);

        /// <summary>
        /// 查詢該Group的所有投票記錄
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <returns>IQueryable</returns>
        List<VotingActivityRec> GetVotingRecByGroupId(int argGroupId);

        /// <summary>
        /// 查詢該Group及Item項的所有投票記錄
        /// </summary>
        /// <param name="argGroupId"></param>
        /// <param name="ItemId"></param>
        /// <returns>IQueryable</returns>
        List<VotingActivityRec> GetVotingRecByGroupIdAndItemId(int argGroupId, int ItemId);

        /// <summary>
        /// 查詢該User投該Group的所有記錄
        /// </summary>
        /// <param name="argGroupId">VotingActivityGroup Id</param>
        /// <param name="argStrAccountId">Account Id</param>
        /// <param name="argStrAccountSource">Account Source</param>
        /// <returns></returns>
        List<VotingActivityRec> GetVotingRecByGroupIdAndAccount(int argGroupId, string argStrAccountId, string argStrAccountSource);

        /// <summary>
        /// 取得該User當日投該Group的所有記錄
        /// </summary>
        /// <param name="argGroupId">VotingActivityGroup Id</param>
        /// <param name="argStrAccountId">Account Id</param>
        /// <param name="argStrAccountSource">Account Source</param>
        /// <param name="argSearchDate">Search Date: yyyy/mm/dd</param>
        /// <returns></returns>
        VotingActivityRec GetVotingRecByGroupIdAndAccountAndDate(int argGroupId, string argStrAccountId, string argStrAccountSource, string argSearchDate);

        /// <summary>
        /// 新增一筆投票記錄
        /// </summary>
        /// <param name="argVotingRec">要新增的投票物件</param>
        /// <returns>新增成功, 回傳流水編號; 新增失敗, 回傳-1</returns>
        int CreateVotingRec(VotingActivityRec argVotingRec);

        /// <summary>
        /// 修改投票記錄
        /// </summary>
        /// <param name="argVotingRec">欲修改的投票物件</param>
        /// <returns>修改成功回傳true, 修改失敗回傳false</returns>
        bool UpdateVotingRec(VotingActivityRec argVotingRec);
        #endregion
    }
}
