using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.VotingActivity
{
    public class VotingActivityRecDetail
    {
        /// <summary>
        /// Item Id
        /// </summary>
        public string VotingItem { get; set; }

        /// <summary>
        /// 投票次數
        /// </summary>
        public int Counts { get; set; }

        /// <summary>
        /// 投票時間
        /// </summary>
        public List<DateTime> VotingTimes { get; set; }
    }
}
