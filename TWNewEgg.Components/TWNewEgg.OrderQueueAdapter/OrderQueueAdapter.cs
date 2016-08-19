using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.OrderQueueAdapter.Interface;

namespace TWNewEgg.OrderQueueAdapter
{
    public class OrderQueueAdapter : IOrderQueueAdapter
    {
        private IRepository<OrderQueue> _orderQueue;
        private IRepository<OrderQueueLog> _orderQueueLog;
        public OrderQueueAdapter(IRepository<OrderQueue> orderQueue, IRepository<OrderQueueLog> orderQueueLog)
        {
            this._orderQueue = orderQueue;
            this._orderQueueLog = orderQueueLog;
        }
        public void orderQueueInsert(OrderQueue _orderQueueInsertModel)
        {
            this._orderQueue.Create(_orderQueueInsertModel);
        }
        public void orderQueueDelete(OrderQueue _orderQueueDeleteModel)
        {
            this._orderQueue.Delete(_orderQueueDeleteModel);
        }
        public void orderQueueLogInsert(OrderQueueLog _orderQueueLogInsertModel)
        {
            this._orderQueueLog.Create(_orderQueueLogInsertModel);
        }
        
    }
}
