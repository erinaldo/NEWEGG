using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.EventServices.Interface;
using TWNewEgg.Models.DomainModels.Event;
using TWNewEgg.EventRepoAdapters.Interface;
using TWNewEgg.EventRepoAdapters;
using TWNewEgg.Models.DomainModels.Answer;
using TWNewEgg.CartRepoAdapters.Interface;

namespace TWNewEgg.EventServices
{
    public class EventService : IEventService
    {
        private IEventRepoAdapter _iEventRepoAdapter;
        private ISORepoAdapter _iSORepoAdapter;
        public EventService(IEventRepoAdapter iEventRepoAdapter,ISORepoAdapter iSORepoAdapter)
        {
            this._iEventRepoAdapter = iEventRepoAdapter;
            this._iSORepoAdapter = iSORepoAdapter;
        }
        public List<Event> GetEvents(List<int> eventIDs)
        {
            List<Event> rtn = new List<Event>();
            List<TWNewEgg.Models.DBModels.TWSQLDB.Event> data = this._iEventRepoAdapter.Event_GetAll().Where(x => eventIDs.Contains(x.id)).ToList();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.Event item in data)
            {
                rtn.Add(new Event
                {
                    id = item.id,
                    name = item.name,
                    creator = item.creator,
                    couponcategories = item.couponcategories,
                    couponmarketnumber = item.couponmarketnumber,
                    couponmax = item.couponmax,
                    couponsum = item.couponsum,
                    couponreget = item.couponreget,
                    datestart = item.datestart,
                    dateend = item.dateend,
                    couponactiveusagedays = item.couponactiveusagedays,
                    visible = item.visible,
                    note = item.note,
                    grantstart = item.grantstart,
                    grantend = item.grantend,
                    grantstatus = item.grantstatus,
                    couponvalidstart = item.couponvalidstart,
                    couponvalidend = item.couponvalidend,
                    value = item.value,
                    couponactivetype = item.couponactivetype,
                    filter = item.filter,
                    filterfileusage = item.filterfileusage,
                    filterfileid = item.filterfileid,
                    createdate = item.createdate,
                    createuser = item.createuser,
                    updated = item.updated,
                    updatedate = item.updatedate,
                    updateuser = item.updateuser,
                    limit = item.limit,
                    limitdescription = item.limitdescription,
                    coupondescription = item.coupondescription,
                    eventdescription = item.eventdescription
                });
            }
            return rtn;
        }
        public List<SalesOrderItemInfo> GetSOItemByCodes(List<string> Codes)
        {
            List<SalesOrderItemInfo> rtn = new List<SalesOrderItemInfo>();
            List<TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem> data = _iSORepoAdapter.GetSOItemsByCodes(Codes).ToList();
            foreach (TWNewEgg.Models.DBModels.TWSQLDB.SalesOrderItem item in data)
            {
                rtn.Add(new SalesOrderItemInfo
                {
                    Code = item.Code,
                    SalesorderCode = item.SalesorderCode,
                    ProductID = item.ProductID,
                    ProductlistID = item.ProductlistID,
                    Name = item.Name,
                    ActID = item.ActID,
                    ItemID = item.ItemID,
                    ItemlistID = item.ItemlistID
                });
            }             
            return rtn;
        }
    }
}
