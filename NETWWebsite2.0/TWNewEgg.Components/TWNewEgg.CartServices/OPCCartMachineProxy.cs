using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.AccountRepoAdapters.Interface;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.CartServices.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.Autofac;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.OrderQueueAdapter.Interface;
using Autofac;
using System.Diagnostics;

namespace TWNewEgg.CartServices
{
    public class OPCCartMachineProxy : ICartMachineProxy
    {

        private ICartMachine _cartMachine;
        private IOrderQueueAdapter _orderQueueAdapter;
        private ISORepoAdapter _sORepoAdapter;
        public OPCCartMachineProxy(ICartMachine cartMachine, IOrderQueueAdapter orderQueueAdapter, ISORepoAdapter sORepoAdapter)
        {
            this._cartMachine = cartMachine;
            this._orderQueueAdapter = orderQueueAdapter;
            this._sORepoAdapter = sORepoAdapter;
        }

        private void iniInitialMachine(int soGroupId)
        {
            this._cartMachine.InitialMachine(soGroupId);
        }

        public void CheckPayment(int soGroupId)
        {
            this.iniInitialMachine(soGroupId);
            this._cartMachine.CheckPayment();
        }

        public void Cancel(int soGroupId)
        {
            this.iniInitialMachine(soGroupId);
            this._cartMachine.Cancel();
        }

        public void TransactToBackend(int soGroupId)
        {
            this.iniInitialMachine(soGroupId);
            this._cartMachine.TransactToBackend();
        }

        public void Pay(int soGroupId)
        {
            throw new NotImplementedException();
        }

        public void PayComplete(int soGroupId)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(0);
            string lockedBy = this.GetType().Name + "." + sf.GetMethod().Name;

            try
            {
                OrderQueue orderQueue = this.createOrderQueueModel(soGroupId);
                this.orderQueueInsert(orderQueue);

                this.ProcessPayComplete(orderQueue);

                OrderQueueLog orderQueueLog = this.createOrderQueueLogModel(orderQueue.OrderNumber, "System", lockedBy, StatusType.success, string.Empty);
                this.orderQueueLogInsert(orderQueueLog);
            }
            catch (Exception error)
            {
                string innerExceptionStr = error.ToString();
                OrderQueueLog orderQueueLog = this.createOrderQueueLogModel(soGroupId, "System", lockedBy, StatusType.error, innerExceptionStr);
                this.orderQueueLogInsert(orderQueueLog);
                throw;
            }
        }

        private void ProcessPayComplete(OrderQueue orderQueue)
        {
            try
            {
                this.iniInitialMachine(orderQueue.OrderNumber);
                this._cartMachine.PayComplete();
            }
            catch (Exception e)
            {
                throw new Exception("執行OPCCartMachine時發生例外", e);
            }
            finally
            {
                this.orderQueueDelete(orderQueue);
            }
        }

        private void orderQueueInsert(OrderQueue _orderQueueModel)
        {
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IRepository<OrderQueue> IRorderQueue = scope.Resolve<IRepository<OrderQueue>>();
                if (IRorderQueue.GetAll().Where(p => p.OrderNumber == _orderQueueModel.OrderNumber).Any() == false)
                {
                    IRorderQueue.Create(_orderQueueModel);
                }
                else
                {
                    throw new Exception("訂單已存在資料表並在處理中");
                }
            }
        }
        private void orderQueueLogInsert(OrderQueueLog _orderQueueLogModel)
        {
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IRepository<OrderQueueLog> IRorderQueueLog = scope.Resolve<IRepository<OrderQueueLog>>();
                IRorderQueueLog.Create(_orderQueueLogModel);
            }
        }
        private void orderQueueDelete(OrderQueue _orderQueueModel)
        {
            using (var scope = AutofacConfig.Container.BeginLifetimeScope())
            {
                IRepository<OrderQueue> IRorderQueue = scope.Resolve<IRepository<OrderQueue>>();
                if (IRorderQueue.GetAll().Where(p => p.OrderNumber == _orderQueueModel.OrderNumber).Any() == true)
                {
                    IRorderQueue.Delete(_orderQueueModel);
                }
            }
        }


        public OrderQueue createOrderQueueModel(int soGroupId = -1)
        {
            OrderQueue orderQueue = new OrderQueue { OrderNumber = soGroupId };
            return orderQueue;
        }
        public OrderQueueLog createOrderQueueLogModel(int soGroupId, string createUser, string lookedBy, StatusType statusType, string errMsg)
        {
            OrderQueueLog orderQueueLog = new OrderQueueLog
            {
                OrderNumber = soGroupId,
                CreateUser = createUser,
                LockedBy = lookedBy,
                Status = (int)statusType,
                ErrMsg = errMsg
            };
            return orderQueueLog;
        }
    }
}
