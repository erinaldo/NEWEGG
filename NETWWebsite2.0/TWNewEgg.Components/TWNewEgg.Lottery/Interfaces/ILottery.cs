using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Lottery;
namespace TWNewEgg.Lottery
{
    public interface ILottery
    {
        LotteryAward TakeAward(int accountId, LotteryAward award);
        DrawResult Draw(int accountId, int lotteryId);
        QualificationResult IsQualified(int accountId, int lotteryId);
        void UpdateLottery(int lotteryId);
    }
}
