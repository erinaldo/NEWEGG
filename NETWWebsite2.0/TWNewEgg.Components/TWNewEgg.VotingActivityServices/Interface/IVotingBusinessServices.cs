using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.VotingActivity;
using TWNewEgg.VotingActivityRepoAdapters;

namespace TWNewEgg.VotingActivityServices.Interface
{
    public interface IVotingBusinessServices
    {
        /// <summary>
        /// 進行投票
        /// </summary>
        /// <param name="argStrAccountId"></param>
        /// <param name="argStrAccountSource"></param>
        /// <param name="argStrEmail"></param>
        /// <param name="argNumGroupId"></param>
        /// <param name="argNumItemId"></param>
        /// <returns></returns>
        VotingResult Voting(string argStrAccountId, string argStrAccountSource, string argStrEmail, int argNumGroupId, int argNumItemId);

        /// <summary>
        /// 取得該Group的投票結果
        /// </summary>
        /// <param name="argVotingGroupId">VotingActivityGroup Id</param>
        /// <returns></returns>
        List<VotingCount> GetVotingCountByGroupId(int argVotingGroupId);

        /// <summary>
        /// 取得參與該Group投票的Account
        /// </summary>
        /// <param name="argVotingGroupId">VotingActivityGroup Id</param>
        /// <returns></returns>
        List<VotingAccount> GetVotingAccountByVotingGroupId(int argVotingGroupId);

        /// <summary>
        /// 取得參與此投票Item的Account
        /// </summary>
        /// <param name="argVotingGroupId"></param>
        /// <param name="argVotingItemId"></param>
        /// <returns></returns>
        VotingCountItemDetail GetVotingAccountByVotingGroupIdAndItemId(int argVotingGroupId, int argVotingItemId);

    }
}
