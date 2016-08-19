using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.VotingActivity
{
    /// <summary>
    /// 投票結果統計
    /// </summary>
    public class VotingCount
    {
        /// <summary>
        /// VotingActivityGroupId
        /// </summary>
        public int VotingGroupId { get; set; }

        /// <summary>
        /// 個別VotingActivityItems的統計結果
        /// </summary>
        public List<VotingCountItemDetail> VotingCountDetail { get; set; }
    }

    /// <summary>
    /// 投票Items的統計
    /// </summary>
    public class VotingCountItemDetail
    {
        /// <summary>
        /// Item Id
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 得票數
        /// </summary>
        public int VotingItemAmount { get; set; }

        /// <summary>
        /// 不重複的投票帳號
        /// </summary>
        public List<VotingAccount> VotingAccount { get; set; }
    }

    /// <summary>
    /// 投票帳號
    /// </summary>
    public class VotingAccount
    {
        /// <summary>
        /// 帳號Id
        /// </summary>
        public string VotingAccountId { get; set; }

        /// <summary>
        /// 帳號來源
        /// </summary>
        public string VotingAccountSource { get; set; }
    }
}
