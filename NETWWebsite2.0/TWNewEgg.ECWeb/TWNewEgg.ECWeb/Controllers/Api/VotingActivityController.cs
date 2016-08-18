using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb.Auth;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.ViewModels.Track;
using TWNewEgg.Models.DomainModels.Track;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class VotingActivityController : ApiController
    {
        [HttpPost]
        public string Voting(int GroupId, int ItemId, string AccountId, string AccountSource, string AccountEmail)
        //public TWNewEgg.Models.ViewModels.VotingActivity.VotingResult Voting()
        {
            TWNewEgg.Models.ViewModels.VotingActivity.VotingResult objVotingResult = TWNewEgg.Models.ViewModels.VotingActivity.VotingResult.活動尚未開始或已經結束;
            string strAccountId = "";
            string strAccountSource = "";
            string strAccountEmail = "";

            //取得Newegg的登入資訊
            if (AccountSource == null || AccountSource.Length == 0)
            {
                //從登入者取得資訊
                strAccountSource = TWNewEgg.Models.ViewModels.VotingActivity.VotingActivityGroup.RestrictAccountOption.Newegg.ToString();
                strAccountId = NEUser.ID.ToString();
                strAccountEmail = NEUser.Email;
            }
            else
            {
                strAccountId = AccountId;
                strAccountSource = AccountSource;
                strAccountEmail = AccountEmail;
            }

            if (strAccountId.Length > 0 && strAccountSource.Length > 0)
            {
                objVotingResult = Processor.Request<TWNewEgg.Models.ViewModels.VotingActivity.VotingResult, TWNewEgg.Models.DomainModels.VotingActivity.VotingResult>("VotingBusinessService", "Voting", strAccountId, strAccountSource, strAccountEmail, GroupId, ItemId).results;
            }

            return Enum.GetName(typeof(TWNewEgg.Models.ViewModels.VotingActivity.VotingResult), objVotingResult);
        }
    }
}
