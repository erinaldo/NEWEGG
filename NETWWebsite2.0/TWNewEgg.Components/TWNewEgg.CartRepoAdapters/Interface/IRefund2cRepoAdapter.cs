using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.CartRepoAdapters.Interface
{
    public interface IRefund2cRepoAdapter
    {
        IQueryable<refund2c> GetAll();
        refund2c GetRefund2c(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM);
        refund2c GetRefund2c(string Code);
        refund2c GetRefund2cbyProcessID(string ProcessId);
        refund2c Update(refund2c refund2c);
    }
}
