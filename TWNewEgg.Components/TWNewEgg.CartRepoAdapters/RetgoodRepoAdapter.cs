using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.CartRepoAdapters.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters
{
    public class RetgoodRepoAdapter : IRetgoodRepoAdapter
    {
        private IBackendRepository<Retgood> _retgood;

        public RetgoodRepoAdapter(IBackendRepository<Retgood> retgood)
        {
            this._retgood = retgood;
        }

        public IQueryable<Retgood> GetAll()
        {
            return this._retgood.GetAll();
        }

        public Retgood GetRetgood(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM)
        {
            Retgood RetgoodData = new Retgood();
            switch (dataMaintainSearchCondition_DM.SearchType) { 
                case (int)DataMaintainSearchCondition_DM.SearchTypeDescription.CartID:
                    RetgoodData = _retgood.GetAll().Where(p => p.CartID == dataMaintainSearchCondition_DM.SearchKey).FirstOrDefault();
                    break;
                case (int)DataMaintainSearchCondition_DM.SearchTypeDescription.RetgoodID:
                    RetgoodData = _retgood.GetAll().Where(p => p.Code == dataMaintainSearchCondition_DM.SearchKey).FirstOrDefault();
                    break;
                default:
                    RetgoodData = _retgood.GetAll().Where(p => p.Code == dataMaintainSearchCondition_DM.SearchKey).FirstOrDefault();
                    break;
            }
            return RetgoodData;
        }

        public Retgood GetRetgood(string Code)
        {
            return this._retgood.Get(x => x.Code == Code);
        }
        public Retgood GetRetgoodbyProcessID(string ProcessId)
        {
            return this._retgood.Get(x => x.ProcessID == ProcessId);
        }
        public Retgood Update(Retgood retgood)
        {
            this._retgood.Update(retgood);
            return retgood;
        }
    }
}
