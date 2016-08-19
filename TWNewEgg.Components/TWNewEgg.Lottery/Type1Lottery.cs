using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Models.DomainModels.Lottery;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.Lottery
{
    public class Type1Lottery : ILottery
    {
        private int _LotteryType = 1;
        private IRepository<LotteryAward> _awardRepo;
        private IRepository<LotteryGame> _gameRepo;
        private IRepository<LotteryLog> _logRepo;
        private IRepository<Account> _accountRepo;

        public Type1Lottery()
        {
            this._awardRepo = AutofacConfig.Container.Resolve<IRepository<LotteryAward>>();
            this._logRepo = AutofacConfig.Container.Resolve<IRepository<LotteryLog>>();
            this._gameRepo = AutofacConfig.Container.Resolve<IRepository<LotteryGame>>();
            this._accountRepo = AutofacConfig.Container.Resolve<IRepository<Account>>();
        }

        public Type1Lottery(IRepository<LotteryAward> awardRepo, IRepository<LotteryLog> logRepo, IRepository<LotteryGame> gameRepo
            ,IRepository<Account> accountRepo)
        {
            this._awardRepo = awardRepo;
            this._logRepo = logRepo;
            this._gameRepo = gameRepo;
            this._accountRepo = accountRepo;
        }

        public DrawResult Draw(int accountId, int lotteryId)
        {
            DrawResult result = new DrawResult();
            LotteryGame game = this._gameRepo.GetAll().FirstOrDefault(x => x.ID == lotteryId && x.Status == "Y");
            if (game == null)
            {
                result.Message = "找不到此抽獎活動";
                result.State = ((int)DrawResult.StateCode.NotExists).ToString();
                return result;
            }
            else if (game.StartDate != null && DateTime.Compare(DateTime.Now, game.StartDate.Value) < 0)
            {
                result.Message = "活動尚未開始";
                result.State = ((int)DrawResult.StateCode.NotBegin).ToString();
                return result;
            }
            else if (this.IsGameClosed(game))
            {
                this.CloseGame(game);
                result.Message = "活動已經結束";
                result.State = ((int)DrawResult.StateCode.IsOver).ToString();
                return result;
            }

            LotteryAward finalAward = null;
            List<LotteryAward> awards = this._awardRepo.GetAll().Where(x=>x.LotteryID == lotteryId && x.Type != 0 && x.Status != "N" && x.Probability != 0).ToList();
            if (awards.Count != 0)
            {
                int awardIndex = this.GetRandomNumber(awards);
                finalAward = this.FindAwardThroughAwardIndex(awards, awardIndex);
                this.TakeAward(accountId, finalAward);
            }
            else
            {
                finalAward = this._awardRepo.Get(x => x.Type == 0 && x.LotteryID == lotteryId && x.Status != "N" && x.Probability != 0);
                if (finalAward != null)
                {
                    this.TakeAward(accountId, finalAward);
                }
            }

            result.Game = new Game();
            ModelConverter.ConvertTo<LotteryGame, Game>(game, result.Game);
            if (finalAward != null)
            {
                result.Award = new Award();
                ModelConverter.ConvertTo<LotteryAward, Award>(finalAward, result.Award);
                result.State = ((int)DrawResult.StateCode.Finished).ToString();
            }
            else
            {
                result.State = ((int)DrawResult.StateCode.Error).ToString(); ;
            }
            result.Message = "抽獎程序完成";
            return result;
        }

        private bool IsGameClosed(LotteryGame game)
        {
            return game.EndDate != null && (DateTime.Compare(DateTime.Now, game.EndDate.Value) > 0 || game.Status == "N");
        }

        private void CloseGame(LotteryGame game)
        {
            if (game.Status != "N")
            {
                game.Status = "N";
                this._gameRepo.Update(game);
            }
        }

        private int GetRandomNumber(List<LotteryAward> awards)
        {
            int totalcnt = 0;
            foreach (LotteryAward award in awards)
            {
                totalcnt += award.Probability;
            }

            Random random = new Random(Guid.NewGuid().GetHashCode());
            int awardOrder = random.Next(0, totalcnt - 1);
            return awardOrder;
        }

        private LotteryAward FindAwardThroughAwardIndex(List<LotteryAward> awards, int awardIndex)
        {
            LotteryAward finalAward = null;
            foreach (LotteryAward award in awards)
            {
                awardIndex -= award.Probability;
                if (awardIndex <= 0)
                {
                    finalAward = award;
                    break;
                }
            }

            return finalAward;
        }

        public LotteryAward TakeAward(int accountId, LotteryAward award)
        {
            LotteryAward finalAward = this._awardRepo.Get(x => x.ID == award.ID);
            if (finalAward.Type != 0)
            {
                finalAward.Probability -= 1;
                if (finalAward.Probability == 0)
                {
                    finalAward.Status = "N";
                }

                this._awardRepo.Update(finalAward);
            }

            LotteryLog log = new LotteryLog()
            {
                AccountID = accountId,
                AwardID = award.ID,
                LotteryID = award.LotteryID,
                CreateDate = DateTime.Now,
                CreateUser = "System"
            };

            this._logRepo.Create(log);
            return finalAward;
        }

        public QualificationResult IsQualified(int accountId, int lotteryId)
        {
            LotteryGame game = this._gameRepo.GetAll().FirstOrDefault(x => 
                x.ID == lotteryId 
                && x.Status == "Y");
            
            if(game == null)
            {
                return new QualificationResult()
                {
                    IsQualified=false,
                    ReasonCode = ((int)DrawResult.StateCode.IsOver).ToString()
                };
            }

            //Account account = this._accountRepo.Get(x => x.ID == accountId && DateTime.Compare(game.StartDate.Value, x.Registeron.Value) <= 0);
            DateTime dateCondiction = Convert.ToDateTime("2015-11-30 23:59:59");
            Account account = this._accountRepo.Get(x => x.ID == accountId && x.Registeron != null &&  x.Registeron <= dateCondiction);
            
            if(account == null)
            {
                return new QualificationResult()
                {
                    IsQualified = false,
                    ReasonCode = ((int)DrawResult.StateCode.NotQualified).ToString()
                };
            }

            bool hasDrawedBefore = this._logRepo.GetAll().Any(x => x.AccountID == accountId && x.LotteryID == lotteryId);

            if (hasDrawedBefore)
            {
                return new QualificationResult()
                {
                    IsQualified = false,
                    ReasonCode = ((int)DrawResult.StateCode.HasDrawedBefore).ToString()
                };
            }
            else
            {
                return new QualificationResult()
                {
                    IsQualified = true
                };
            }
        }

        public void UpdateLottery(int lotteryId)
        {
            LotteryGame game = this._gameRepo.Get(x => x.ID == lotteryId);
            if (this.IsGameClosed(game))
            {
                this.CloseGame(game);
            }
        }
    }
}
