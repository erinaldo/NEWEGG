using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.PageMgmt.Interface
{
    public interface IComponentService<T>
    {
        void SaveComponentsNObjects(List<DSComponentInfo> components);
    }
}
