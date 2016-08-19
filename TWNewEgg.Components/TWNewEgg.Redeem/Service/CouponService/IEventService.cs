using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Redeem;

namespace TWNewEgg.Redeem.Service.CouponService
{
    public interface IEvent
    {
        int addEvent(Models.DomainModels.Redeem.Event arg_oEvent);
        bool editEvent(Models.DomainModels.Redeem.Event arg_oEvent);
        Models.DomainModels.Redeem.Event getEventById(int arg_nEventId);
        List<Models.DomainModels.Redeem.Event> getEventList();
        List<Models.DomainModels.Redeem.Event> getEventListByEventName(string arg_strEventName);
        List<Models.DomainModels.Redeem.Event> getEventListByManager(string arg_strManager);
        bool checkEventCouponNumber(string arg_strCouponNumber);
        bool checkEventCouponNumberByEventId(string arg_strCouponNumber, int arg_nEventId);
        Models.DomainModels.Redeem.Event GetActiveEventById(int arg_nEventId);
        List<Models.DomainModels.Redeem.Event> GetEvents(List<int> eventIDs);
        List<Models.DomainModels.Answer.SalesOrderItemInfo> GetSOItemByCodes(List<string> salesOrderCodes);
        List<Models.DomainModels.Redeem.Event> GetWaitingForGrantingList();
    }
}
