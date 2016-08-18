using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.DataMaintain;

namespace TWNewEgg.DataMaintainServices.Interface
{
    public interface IDataMaintainService
    {
        CartDataMaintain_DM GetCartDataMaintain(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM);
        RetgoodDataMaintain_DM GetretgoodDataMaintain(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM);
        refund2cDataMaintain_DM Getrefund2cDataMaintain(DataMaintainSearchCondition_DM dataMaintainSearchCondition_DM);
    }
}
