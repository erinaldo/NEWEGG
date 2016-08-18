using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.VotingActivity
{
    /// <summary>
    /// 投票記錄
    /// </summary>
    public class VotingActivityRec
    {
        /// <summary>
        /// 流水編號
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 投票活動的GroupId, refer RateActivityGroup.Id
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 帳號Id, 因各家帳號儲存方式不同, 故以String儲存
        /// </summary>
        public string AccocuntId { get; set; }

        /// <summary>
        /// 帳號來源, Newegg, FB, Yahoo, Google+, etc.
        /// </summary>
        public string AccountSource { get; set; }

        /// <summary>
        /// 投票日期, yyyy/MM/dd
        /// </summary>
        public string RateDate { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 投票記錄的詳細資訊
        /// </summary>
        public List<VotingActivityRecDetail> Rec { get; set; }
    }
}
