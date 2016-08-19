using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.VotingActivityServices.Interface;
using TWNewEgg.VotingActivityRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.VotingActivityServices
{
    public class VotingBusinessService : IVotingBusinessServices
    {
        private IVotingActivityServices mIVotingService = null;

        public VotingBusinessService(IVotingActivityServices argIVotingService)
        {
            this.mIVotingService = argIVotingService;
        }
        
        public Models.DomainModels.VotingActivity.VotingResult Voting(string argStrAccountId, string argStrAccountSource, string argStrEmail, int argNumGroupId, int argNumItemId)
        {
            Models.DomainModels.VotingActivity.VotingResult objVotingResult = Models.DomainModels.VotingActivity.VotingResult.活動尚未開始或已經結束;
            Models.DomainModels.VotingActivity.VotingActivityGroup objGroup = null;
            Models.DomainModels.VotingActivity.VotingActivityItems objItem = null;
            Models.DomainModels.VotingActivity.VotingActivityRec objRec = null;
            List<Models.DomainModels.VotingActivity.VotingActivityRec> listRec = null;
            Models.DomainModels.VotingActivity.VotingActivityRecDetail objRecDetail = null;
            DateTime dateNow = DateTime.Now;
            string strSearchDate = dateNow.Year.ToString() + "/" + dateNow.Month.ToString().PadLeft(2, '0') + "/" + dateNow.Day.ToString().PadLeft(2,'0');
            bool boolCanVote = false;
            int numVoteCount = 0;
            int numNowHour = dateNow.Hour;
            bool boolExec = false; //執行Create或Update的結果
            int numCreateId = -1;

            //取得該Group是否可投票
            objGroup = this.mIVotingService.GetVotingGroupById(argNumGroupId);
            if (objGroup == null || objGroup.OnlineStatus != 1 || objGroup.VotingStartDate > dateNow || objGroup.VotingEndDate < dateNow)
            {
                objGroup = null;
                objVotingResult = Models.DomainModels.VotingActivity.VotingResult.活動尚未開始或已經結束;
                return objVotingResult;
            }

            //取得該Item是否在活動中
            objItem = this.mIVotingService.GetVotingItemsByGroupIdAndItemId(argNumGroupId, argNumItemId);
            if (objItem == null || objItem.OnlineStatus != 1 || objItem.VotingStartDate > dateNow || objItem.VotingEndDate < dateNow)
            {
                objVotingResult = Models.DomainModels.VotingActivity.VotingResult.此商品非活動中;
                return objVotingResult;
            }

            //比對帳號投票資格
            if (objGroup.RestrictAccount == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.RestrictAccountOption.Newegg && argStrAccountSource != Models.DomainModels.VotingActivity.VotingActivityGroup.RestrictAccountOption.Newegg.ToString())
            {
                objVotingResult = Models.DomainModels.VotingActivity.VotingResult.資格不符合;
            }

            //比對帳號投票次數
            if (objGroup.RestrictType == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.RestrictTypeOption.EveryDay)
            {
                #region 每日投票
                //每日投票
                numVoteCount = 0;
                objRec = this.mIVotingService.GetVotingRecByGroupIdAndAccountAndDate(argNumGroupId, argStrAccountId, argStrAccountSource, strSearchDate);
                if (listRec != null)
                {
                    objRec = listRec.Where(x => x.VoteDate == strSearchDate).FirstOrDefault();
                }
                if (objRec == null)
                {
                    boolCanVote = true;
                }
                else
                {
                    //比對每日投票的總次數
                    numVoteCount += objRec.Rec.Sum(x => x.Counts);
                    if (objGroup.RestrictLimit > 0 && numVoteCount < objGroup.RestrictLimit)
                    {
                        boolCanVote = true;
                    }
                    else
                    {
                        objVotingResult = Models.DomainModels.VotingActivity.VotingResult.本時段投票次數已使用完畢;
                    }
                    //比對是否可以重複投票, 若是不可重複投票則取消投票資格
                    if (boolCanVote && objGroup.VotingItemRepeate == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.VotingItemRepeateOption.CantRepeate)
                    {
                        if (objRec.Rec.Where(x => x.VotingItem == argNumItemId.ToString()).FirstOrDefault() != null)
                        {
                            boolCanVote = false;
                            objVotingResult = Models.DomainModels.VotingActivity.VotingResult.重複投票;
                        }
                    }
                }
                #endregion

            }
            else if (objGroup.RestrictType == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.RestrictTypeOption.PerHour)
            {
                #region 每小時投票
                objRec = this.mIVotingService.GetVotingRecByGroupIdAndAccountAndDate(argNumGroupId, argStrAccountId, argStrAccountSource, strSearchDate);
                if (listRec != null)
                {
                    objRec = listRec.Where(x => x.VoteDate == strSearchDate).FirstOrDefault();
                }
                if (objRec == null)
                {
                    boolCanVote = true;
                }
                else
                {
                    //取得這一小時的投票次數
                    numVoteCount = 0;
                    foreach (TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRecDetail objSubRecDetail in objRec.Rec)
                    {
                        foreach (DateTime objVoteTime in objSubRecDetail.VotingTimes)
                        {
                            if (objVoteTime.Hour == numNowHour)
                            {
                                numVoteCount++;
                            }
                        }
                    }
                    //比對投票次數
                    if (objGroup.RestrictLimit > 0 && numVoteCount < objGroup.RestrictLimit)
                    {
                        boolCanVote = true;
                    }
                    else
                    {
                        objVotingResult = Models.DomainModels.VotingActivity.VotingResult.本時段投票次數已使用完畢;
                    }
                    //比對是否可以重複投票, 若是不可重複投票則取消投票資格
                    if (boolCanVote && objGroup.VotingItemRepeate == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.VotingItemRepeateOption.CantRepeate)
                    {
                        foreach (TWNewEgg.Models.DomainModels.VotingActivity.VotingActivityRecDetail objSubRecDetail in objRec.Rec)
                        {
                            if (objSubRecDetail.VotingItem != argNumItemId.ToString())
                            {
                                continue;
                            }

                            foreach (DateTime objVoteTime in objSubRecDetail.VotingTimes)
                            {
                                if (objVoteTime.Hour == numNowHour)
                                {
                                    boolCanVote = false;
                                    objVotingResult = Models.DomainModels.VotingActivity.VotingResult.重複投票;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else if (objGroup.RestrictType == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.RestrictTypeOption.Amount)
            {
                #region 總票數
                //總票數
                listRec = this.mIVotingService.GetVotingRecByGroupIdAndAccount(argNumGroupId, argStrAccountId, argStrAccountSource);
                if (listRec == null || listRec.Count <= 0)
                {
                    boolCanVote = true;
                }
                else
                {
                    numVoteCount = 0;
                    foreach (Models.DomainModels.VotingActivity.VotingActivityRec objSubRec in listRec)
                    {
                        numVoteCount += objSubRec.Rec.Sum(x => x.Counts);
                        if (objGroup.RestrictLimit > 0 && numVoteCount >= objGroup.RestrictLimit)
                        {
                            boolCanVote = false;
                            objVotingResult = Models.DomainModels.VotingActivity.VotingResult.本時段投票次數已使用完畢;
                            break;
                        }
                    }

                    if (objGroup.RestrictLimit > 0 && numVoteCount < objGroup.RestrictLimit)
                    {
                        boolCanVote = true;
                    }

                    //比對是否可以重複投票, 若是不可重複投票則取消投票資格
                    if (boolCanVote && objGroup.VotingItemRepeate == (int)Models.DomainModels.VotingActivity.VotingActivityGroup.VotingItemRepeateOption.CantRepeate)
                    {
                        foreach (Models.DomainModels.VotingActivity.VotingActivityRec objSubRec in listRec)
                        {
                            if (objSubRec.Rec.Where(x => x.VotingItem == argNumItemId.ToString()).FirstOrDefault() != null)
                            {
                                boolCanVote = false;
                                objVotingResult = Models.DomainModels.VotingActivity.VotingResult.重複投票;
                            }
                        }
                    }
                }
                #endregion
            }

            //若可以投票則進行寫入資料庫
            if (boolCanVote)
            {
                //新增或修改Rec
                objRec = this.mIVotingService.GetVotingRecByGroupIdAndAccountAndDate(argNumGroupId, argStrAccountId, argStrAccountSource, strSearchDate);
                if (objRec == null)
                {
                    //Create
                    objRec = new Models.DomainModels.VotingActivity.VotingActivityRec();
                    objRec.AccountId = argStrAccountId;
                    objRec.AccountSource = argStrAccountSource;
                    objRec.GroupId = argNumGroupId;
                    objRec.Email = argStrEmail;
                    objRec.VoteDate = strSearchDate;

                    objRec.Rec = new List<Models.DomainModels.VotingActivity.VotingActivityRecDetail>();
                    objRecDetail = new Models.DomainModels.VotingActivity.VotingActivityRecDetail();
                    objRecDetail.Counts = 1;
                    objRecDetail.VotingItem = argNumItemId.ToString();
                    objRecDetail.VotingTimes = new List<DateTime>();
                    objRecDetail.VotingTimes.Add(DateTime.Now);
                    objRec.Rec.Add(objRecDetail);

                    numCreateId = this.mIVotingService.CreateVotingRec(objRec);
                    if (numCreateId > 0)
                    {
                        objVotingResult = Models.DomainModels.VotingActivity.VotingResult.投票成功;
                    }
                    else
                    {
                        objVotingResult = Models.DomainModels.VotingActivity.VotingResult.投票失敗;
                    }
                }
                else
                {
                    //Update
                    objRecDetail = objRec.Rec.Where(x => x.VotingItem == argNumItemId.ToString()).FirstOrDefault();
                    if (objRecDetail == null)
                    {
                        objRecDetail = new Models.DomainModels.VotingActivity.VotingActivityRecDetail();
                        objRecDetail.Counts = 1;
                        objRecDetail.VotingItem = argNumItemId.ToString();
                        objRecDetail.VotingTimes = new List<DateTime>();
                        objRecDetail.VotingTimes.Add(DateTime.Now);
                        objRec.Rec.Add(objRecDetail);
                    }
                    else
                    {
                        objRecDetail.Counts += 1;
                        objRecDetail.VotingTimes.Add(DateTime.Now);
                    }

                    boolExec = this.mIVotingService.UpdateVotingRec(objRec);
                    if (boolExec)
                    {
                        objVotingResult = Models.DomainModels.VotingActivity.VotingResult.投票成功;
                    }
                    else
                    {
                        objVotingResult = Models.DomainModels.VotingActivity.VotingResult.投票失敗;
                    }
                }

                //修改該Item的總票數
                objItem.RealVoting += 1;
                this.mIVotingService.UpdateVotingItems(objItem);
            }

            return objVotingResult;
        }

        /// <summary>
        /// 取得該GroupId所有的投票結果
        /// </summary>
        /// <param name="argVotingGroupId"></param>
        /// <returns></returns>
        public List<Models.DomainModels.VotingActivity.VotingCount> GetVotingCountByGroupId(int argVotingGroupId)
        {
            throw new NotImplementedException();
        }

        public List<Models.DomainModels.VotingActivity.VotingAccount> GetVotingAccountByVotingGroupId(int argVotingGroupId)
        {
            throw new NotImplementedException();
        }

        public Models.DomainModels.VotingActivity.VotingCountItemDetail GetVotingAccountByVotingGroupIdAndItemId(int argVotingGroupId, int argVotingItemId)
        {
            throw new NotImplementedException();
        }
    }
}
