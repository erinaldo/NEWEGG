using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Lottery;

namespace TWNewEgg.Lottery
{
    public interface ILotteryService
    {
        DrawResult DrawAwrad(int accountId, int gameId);
    }
}
