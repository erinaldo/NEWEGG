using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters
{
    public class Datamaintain_logRepoAdapter : IDatamaintain_logRepoAdapter
    {
        private IBackendRepository<Datamaintain_log> _Datamaintain_log;

        public Datamaintain_logRepoAdapter(IBackendRepository<Datamaintain_log> Datamaintain_log)
        {
            this._Datamaintain_log = Datamaintain_log;
        }

        public Datamaintain_log Create(Datamaintain_log Datamaintain_log)
        {
            this._Datamaintain_log.Create(Datamaintain_log);
            return Datamaintain_log;
        }
    }
}
