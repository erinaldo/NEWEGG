using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Lottery;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Lottery
{
    public class LotteryService : ILotteryService
    {
        private ILottery _lottery;
        private ILotteryFactory _factory;
        private static object syncRoot = new Object();

        public LotteryService(ILotteryFactory factory)
        {
            this._factory = factory;
        }
        
        public DrawResult DrawAwrad(int accountId, int gameId)
        {
            try
            {
                lock (syncRoot)
                {
                    ILottery lottery = this._factory.CreateLottery(gameId);
                    QualificationResult qualificationResult = lottery.IsQualified(accountId, gameId);
                    bool isQualified = qualificationResult.IsQualified;
                    DrawResult result = new DrawResult();
                    if (isQualified)
                    {
                        result = lottery.Draw(accountId, gameId);
                        lottery.UpdateLottery(gameId);
                    }
                    else
                    {
                        result.State = qualificationResult.ReasonCode;
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                DrawResult result = new DrawResult();
                result.State = ((int)DrawResult.StateCode.Error).ToString();
                result.Message = e.ToString();
                return result;
            }
        }
    }
}
