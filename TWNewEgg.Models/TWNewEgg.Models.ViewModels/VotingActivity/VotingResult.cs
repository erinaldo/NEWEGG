using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.VotingActivity
{
    public enum VotingResult
    {
        投票成功 = 1,
        投票失敗 = 2,
        本時段投票次數已使用完畢 = 3,
        活動尚未開始或已經結束 = 4,
        重複投票 = 5,
        此商品非活動中 = 6,
        資格不符合 = 7
    }
}
