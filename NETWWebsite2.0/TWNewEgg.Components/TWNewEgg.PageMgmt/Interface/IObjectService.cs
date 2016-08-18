using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.PageMgmt.Interface
{
    public interface IObjectService<T>
    {
        void saveEditObject(DSComponentInfo component);
        void saveNewObject(DSComponentInfo component);
        void saveDeleteObject(DSComponentInfo component);
        List<T> GetByComponents(List<ComponentInfo> components);
    }
}
