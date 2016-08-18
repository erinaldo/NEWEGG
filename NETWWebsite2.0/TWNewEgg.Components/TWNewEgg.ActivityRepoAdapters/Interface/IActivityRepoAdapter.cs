using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ActivityRepoAdapters.Interface
{
    public interface IActivityRepoAdapter
    {
        Activity GetActivityByName(string activityName);
        List<Activity> GetActivityByDate(DateTime? startDate, DateTime? endDate);
    }
}
