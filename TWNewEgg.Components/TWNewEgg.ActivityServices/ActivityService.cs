using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ActivityRepoAdapters.Interface;
using TWNewEgg.ActivityServices.Interface;
using TWNewEgg.Models.DomainModels.Activity;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ActivityServices
{
    public class ActivityService : IActivityService
    {
        IActivityRepoAdapter _activityRepoAdapter;

        public ActivityService(IActivityRepoAdapter activityRepoAdapter)
        {
            this._activityRepoAdapter = activityRepoAdapter;
        }

        public ActivityDM GetActivityByName(string activityName)
        {
            if (string.IsNullOrEmpty(activityName))
            {
                return null;
            }

            Activity oriActivity = _activityRepoAdapter.GetActivityByName(activityName);

            if (oriActivity == null)
            {
                return null;
            }

            if (oriActivity.ShowType == 1)
            {
                return null;
            }

            return ConvertActivityToDM(oriActivity);
        }

        private ActivityDM ConvertActivityToDM(Activity dbActivity)
        {
            ActivityDM domainModel = new ActivityDM();

            domainModel.ID = dbActivity.ID;
            domainModel.Name = dbActivity.Name;
            domainModel.HtmlContext = dbActivity.HtmlContext;
            domainModel.ShowType = dbActivity.ShowType;
            domainModel.MetaTitle = dbActivity.MetaTitle;
            domainModel.MetaKeyWord = dbActivity.MetaKeyWord;
            domainModel.MetaDescription = dbActivity.MetaDescription;
            domainModel.ActionType = dbActivity.ActionType;
            domainModel.SectionInfor = dbActivity.SectionInfor;

            return domainModel;
        }

    }
}
