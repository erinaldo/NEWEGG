using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PurgeQueueAdapters.Interface
{
    public interface IPurgeQueueAdapters
    {
        void insert(PurgeQueue model);
        void insertMany(List<PurgeQueue> model);
        void update(TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue model);
        void updateMany(IEnumerable<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model);
        void updateRange(List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeQueue> model);
        IQueryable<PurgeQueue> read();

        void insertPurgeRequest(PurgeRequest model);
        void updatePurgeRequest(TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest model);
        void updateRangePurgeRequest(List<TWNewEgg.Models.DBModels.TWSQLDB.PurgeRequest> model);
        IQueryable<PurgeRequest> readPurgeRequest();
    }
}
