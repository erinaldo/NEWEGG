using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.GroupBuy.Interface;
using TWNewEgg.GroupBuyServices.Interface;
using TWNewEgg.Models.DomainModels.GroupBuy;

namespace TWNewEgg.GroupBuyServices
{
    public class GetOldGroupBuy : IGetOldGroupBuyServices
    {
        IGroupBuy _groupBuyService;

        public GetOldGroupBuy(IGroupBuy groupBuyService)
        {
            this._groupBuyService = groupBuyService;
        }

        public List<GroupBuyViewInfo> QueryViewInfo(GroupBuyQueryCondition condition)
        {
            List<GroupBuyViewInfo> GroupBuyViewInfotemp = this._groupBuyService.QueryViewInfo(condition);
            return GroupBuyViewInfotemp;
        }
    }
}
