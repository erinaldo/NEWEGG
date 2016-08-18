using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Activity;

namespace TWNewEgg.ActivityServices.Interface
{
    public interface IActivityService
    {
        ActivityDM GetActivityByName(string activityName);
    }
}
