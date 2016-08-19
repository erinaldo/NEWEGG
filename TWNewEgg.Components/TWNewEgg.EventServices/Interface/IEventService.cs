using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Event;
using TWNewEgg.Models.DomainModels.Answer;

namespace TWNewEgg.EventServices.Interface
{
    public interface IEventService
    {
        List<Event> GetEvents(List<int> eventIDs);
        List<SalesOrderItemInfo> GetSOItemByCodes(List<string> Codes);
    }
}
