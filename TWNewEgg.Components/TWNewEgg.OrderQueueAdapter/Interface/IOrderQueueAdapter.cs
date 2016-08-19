using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.OrderQueueAdapter.Interface
{
    public interface IOrderQueueAdapter
    {
        void orderQueueInsert(OrderQueue _orderQueueInsertModel);
        void orderQueueDelete(OrderQueue _orderQueueDeleteModel);
        void orderQueueLogInsert(OrderQueueLog _orderQueueLogInsertModel);
    }
}
