using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.PageMgmt.Interface
{
    public interface IPageDBUtil
    {
        PageInfo getPage(int pageID);
        List<PageInfo> getPages();
        List<PageInfo> getAll();
        PageInfo getLastPage(string Path);
        PageInfo getLastActivePage(string Path);
        PageInfo createPage(PageInfo page);
        bool savePage(DSPageInfo page);
        bool Delete(PageInfo page);
        bool AuditPage(PageInfo page);
        bool Reject(PageInfo page);
        bool LaunchPage(PageInfo page);
    }
}
