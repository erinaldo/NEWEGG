using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.GroupBuy;

namespace TWNewEgg.GroupBuyServices.Interface
{
    public interface IGetOldGroupBuyServices
    {
        List<GroupBuyViewInfo> QueryViewInfo(GroupBuyQueryCondition condition);
    }
}
