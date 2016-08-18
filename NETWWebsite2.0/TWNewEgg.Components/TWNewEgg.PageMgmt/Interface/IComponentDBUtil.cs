using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;

namespace TWNewEgg.PageMgmt.Interface
{
    public interface IComponentDBUtil
    {
        List<ComponentInfo> getComponents(int pageID);
        List<ComponentInfo> createComponents(List<ComponentInfo> components, int newPageID);
        bool Update(ComponentInfo component);
        bool Create(ComponentInfo component);
        bool Delete(ComponentInfo component);
        bool deleteComponents(PageInfo page);
        ComponentInfo addComponent(int PageID, string ObjectType, int ObjectID);
        void LaunchComponents(PageInfo page);
    }
}
