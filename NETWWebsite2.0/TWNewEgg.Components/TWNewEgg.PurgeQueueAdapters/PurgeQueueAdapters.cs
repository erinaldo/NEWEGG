using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.PurgeQueueAdapters.Interface;

namespace TWNewEgg.PurgeQueueAdapters
{
    public class PurgeQueueAdapters : IPurgeQueueAdapters
    {
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> _purgeQueue;
        private IRepository<TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest> _purgeRequest;
        public PurgeQueueAdapters(IRepository<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> purgeQueue, IRepository<TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest> purgeRequest)
        {
            this._purgeQueue = purgeQueue;
            this._purgeRequest = purgeRequest;
        }

        public void insert(TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue model)
        {
            this._purgeQueue.Create(model);
        }

        public void insertMany(List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model)
        {
            this._purgeQueue.CreateMany(model);
        }

        public void update(TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue model)
        {
            this._purgeQueue.Update(model);
        }

        public void updateMany(IEnumerable<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model)
        {
            this._purgeQueue.UpdateMany(model);
        }

        public void updateRange(List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model)
        {
            this._purgeQueue.UpdateRange(model);
        }

        public IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> read()
        {
            return this._purgeQueue.GetAll();
        }


        public void insertPurgeRequest(Models.DBModels.TWSQLDB.PurgeRequest model)
        {
            this._purgeRequest.Create(model);
        }

        

        public void updatePurgeRequest(Models.DBModels.TWSQLDB.PurgeRequest model)
        {
            this._purgeRequest.Update(model);
        }
        public void updateRangePurgeRequest(List<Models.DBModels.TWSQLDB.PurgeRequest> model)
        {
            this._purgeRequest.UpdateRange(model);
        }

        public IQueryable<Models.DBModels.TWSQLDB.PurgeRequest> readPurgeRequest()
        {
            return this._purgeRequest.GetAll();
        }
    }
}
