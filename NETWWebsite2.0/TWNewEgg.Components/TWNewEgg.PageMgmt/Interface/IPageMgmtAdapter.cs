using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using TWNewEgg.Models.DomainModels.PageMgmt;

namespace TWNewEgg.PageMgmt.Interface
{
    public interface IPageMgmtAdapter
    {
        void AddPage(PageInfo page);
        void DeletePage(PageInfo page);
        void CancelEdit(PageInfo page);
        PageData EditPage(string path, bool isEditPage);
        PageData GetActivePage(string Path);
        void NewPage(PageInfo page);
        void SavePage(DSPageInfo page);
        string AuditPage(PageInfo page);
        string LaunchPage(PageInfo page);
    }
}
