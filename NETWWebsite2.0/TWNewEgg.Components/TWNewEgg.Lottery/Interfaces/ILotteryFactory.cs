using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Lottery
{
    public interface ILotteryFactory
    {
        ILottery CreateLottery(int gameId);
    }
}
