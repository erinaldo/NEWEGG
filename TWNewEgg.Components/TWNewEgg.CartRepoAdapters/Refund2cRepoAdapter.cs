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
    public class Refund2cRepoAdapter : IRefund2cRepoAdapter
    {
        private IBackendRepository<refund2c> _refund2c;

        public Refund2cRepoAdapter(IBackendRepository<refund2c> refund2c)
        {
            this._refund2c = refund2c;
        }

        public IQueryable<refund2c> GetAll()
        {
            return this._refund2c.GetAll();
        }

        public refund2c GetRefund2c(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM)
        {
            refund2c refund2cData = new refund2c();
            string Searchkey = dataMaintainSearchCondition_DM.SearchKey;

            switch (dataMaintainSearchCondition_DM.SearchType) { 
                case (int)DataMaintainSearchCondition_DM.SearchTypeDescription.CartID:
                    refund2cData = _refund2c.GetAll().Where(p => p.CartID == Searchkey).FirstOrDefault();
                    break;
                case (int)DataMaintainSearchCondition_DM.SearchTypeDescription.Refund2C:
                    refund2cData = _refund2c.GetAll().Where(p => p.Code == Searchkey).FirstOrDefault();
                    break;
                default:
                    refund2cData = _refund2c.GetAll().Where(p => p.Code == Searchkey).FirstOrDefault();
                    break;
            }
            return refund2cData;
        }

        public refund2c GetRefund2c(string Code)
        {
            return this._refund2c.Get(x => x.Code == Code);
        }
        public refund2c GetRefund2cbyProcessID(string ProcessId)
        {
            return this._refund2c.Get(x => x.ProcessID == ProcessId);
        }
        public refund2c Update(refund2c refund2c)
        {
            this._refund2c.Update(refund2c);
            return refund2c;
        }
    }
}
