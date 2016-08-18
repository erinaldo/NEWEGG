using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Lottery
{
    public class LotteryFactory : ILotteryFactory
    {
        private IRepository<LotteryGame> _gameRepo;
        public LotteryFactory(IRepository<LotteryGame> gameRepo)
        {
            this._gameRepo = gameRepo;
        }
        public ILottery CreateLottery(int gameId)
        {
            ILottery lottery;
            LotteryGame game = this._gameRepo.Get(x => x.ID == gameId);
            lottery = (ILottery)AutofacConfig.Container.ResolveKeyed("type" + game.Type, typeof(ILottery));

            return lottery;
        }
    }
}
