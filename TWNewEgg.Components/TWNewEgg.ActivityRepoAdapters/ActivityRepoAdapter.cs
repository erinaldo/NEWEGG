using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ActivityRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ActivityRepoAdapters
{
    public class ActivityRepoAdapter : IActivityRepoAdapter
    {
        private IRepository<Activity> _activityDB;

        public ActivityRepoAdapter(IRepository<Activity> activityDB)
        {
            this._activityDB = activityDB;
        }

        public Models.DBModels.TWSQLDB.Activity GetActivityByName(string activityName)
        {
            return this._activityDB.GetAll().Where(x => x.Name == activityName).FirstOrDefault();
        }

        public List<Models.DBModels.TWSQLDB.Activity> GetActivityByDate(DateTime? startDate, DateTime? endDate)
        {
            IQueryable<Activity> activityDB = this._activityDB.GetAll();
            if (startDate == null && endDate == null)
            {
                return null;
            }
            if (startDate != null)
            {
                activityDB = activityDB.Where(x => x.CreateDate >= startDate.Value);
            }
            if (endDate != null)
            {
                activityDB = activityDB.Where(x => x.CreateDate < endDate.Value);
            }
            return activityDB.ToList();
        }
    }
}
