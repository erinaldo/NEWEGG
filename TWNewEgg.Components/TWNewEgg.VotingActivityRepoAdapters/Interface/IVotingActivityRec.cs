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
    /// 投票記錄
    /// </summary>
    public interface IVotingActivityRec
    {
        /// <summary>
        /// 根據帳號取得投票記錄
        /// </summary>
        /// <param name="argStrAccountId">帳號</param>
        /// <param name="argStrAccountSource">帳號來源</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityRec> GetVotingRecByAccocunt(string argStrAccountId, string argStrAccountSource);

        /// <summary>
        /// 查詢當日所有的投票記錄
        /// </summary>
        /// <param name="argSearchDate">查詢當日, 格式為yyyy/MM/dd</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityRec> GetVotingRecByDate(string argSearchDate);

        /// <summary>
        /// 根據帳號與日期, 查詢該Account的投票記錄
        /// </summary>
        /// <param name="argStrAccountId">Account Id</param>
        /// <param name="argStrAccountSource">Account Source, ex: Newegg, FB, Yahoo, Google+, etc.</param>
        /// <param name="argSearchDate">日期, yyyy/MM/dd</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityRec> GetVotingRecByAccountAndDate(string argStrAccountId, string argStrAccountSource, string argSearchDate);

        /// <summary>
        /// 查詢該User投該Group的所有記錄
        /// </summary>
        /// <param name="argGroupId">VotingActivityGroup Id</param>
        /// <param name="argStrAccountId">Account Id</param>
        /// <param name="argStrAccountSource">Account Source</param>
        /// <returns></returns>
        IQueryable<VotingActivityRec> GetVotingRecByGroupIdAndAccount(int argGroupId, string argStrAccountId, string argStrAccountSource);

        /// <summary>
        /// 取得該User當日投該Group的所有記錄
        /// </summary>
        /// <param name="argGroupId">VotingActivityGroup Id</param>
        /// <param name="argStrAccountId">Account Id</param>
        /// <param name="argStrAccountSource">Account Source</param>
        /// <param name="argSearchDate">Search Date: yyyy/mm/dd</param>
        /// <returns></returns>
        IQueryable<VotingActivityRec> GetVotingRecByGroupIdAndAccountAndDate(int argGroupId, string argStrAccountId, string argStrAccountSource, string argSearchDate);

        /// <summary>
        /// 查詢該Group的所有投票記錄
        /// </summary>
        /// <param name="argGroupId">Group Id</param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityRec> GetVotingRecByGroupId(int argGroupId);

        /// <summary>
        /// 查詢該Group及Item項的所有投票記錄
        /// </summary>
        /// <param name="argGroupId"></param>
        /// <param name="ItemId"></param>
        /// <returns>IQueryable</returns>
        IQueryable<VotingActivityRec> GetVotingRecByGroupIdAndItemId(int argGroupId, int ItemId);

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

    }
}