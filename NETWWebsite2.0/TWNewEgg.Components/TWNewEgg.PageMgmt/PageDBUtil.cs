using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using TWNewEgg.PageMgmt.Interface;

namespace TWNewEgg.PageMgmt
{
    public class PageDBUtil :IPageDBUtil
    {
        private log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName);

        private IRepository<PageInfo> _pageInfo;
        private IRepository<ComponentInfo> _components;
        private IComponentService<TextObject> _TextComponentService;
        private IComponentService<ImageObject> _ImageComponentService;
        private IComponentService<VideoInfo> _VideoComponentService;
        private IComponentService<DynamicObject> _DynamicComponentService;
        public PageDBUtil(IRepository<PageInfo> pageInfo, IRepository<ComponentInfo> components,
            IComponentService<TextObject> TextComponentService,
            IComponentService<ImageObject> ImageComponentService,
            IComponentService<VideoInfo> VideoComponentService,
            IComponentService<DynamicObject> DynamicComponentService)
        {
            this._pageInfo = pageInfo;
            this._components = components;
            this._TextComponentService = TextComponentService;
            this._ImageComponentService = ImageComponentService;
            this._VideoComponentService = VideoComponentService;
            this._DynamicComponentService = DynamicComponentService;
        }

        public PageInfo getPage(int pageID)
        {
            return _pageInfo.Get(x => x.PageID == pageID);
        }

        public List<PageInfo> getPages()
        {
            List<PageInfo> pages;
            if (this._pageInfo.GetAll().Any())
            {
                pages = _pageInfo.GetAll().GroupBy(x => x.Path).ToList().Select(grp => grp.OrderByDescending(x => x.PageID).First()).ToList();
            }
            else
            {
                pages = new List<PageInfo>();
            }
            return pages;
        }

        public List<PageInfo> getAll()
        {
            List<PageInfo> pages = _pageInfo.GetAll().ToList();
            return pages;
        }

        public PageInfo getLastPage(string Path)
        {
            PageInfo p = _pageInfo.GetAll().Where(x => x.Path == Path) //已上線且同頁面
                .OrderByDescending(x => x.PageID).FirstOrDefault();
            return p;
        }

        public PageInfo getLastActivePage(string Path)
        {
            PageInfo p = _pageInfo.GetAll().Where(x => x.Path == Path && x.Status == PageInfo.PageStatus.Active) //已上線且同頁面
                .OrderByDescending(x => x.PageID).FirstOrDefault();
            return p;
        }

        public PageInfo createPage(PageInfo page)
        {
            PageInfo newPage = null;
            if (page != null)
            {
                try
                {
                    newPage = ModelConverter.ConvertTo<PageInfo>(page);
                    newPage.Status = PageInfo.PageStatus.Editing;
                    newPage.InUser = "SYS";
                    newPage.InDate = DateTime.Now;
                    _pageInfo.Create(newPage);
                }
                catch (Exception e) {
                    logger.Error(e);
                    throw; 
                }
            }
            return newPage;
        }

        public bool savePage(DSPageInfo page)
        {
            bool success = true;
            try
            {
                PageInfo savePage = _pageInfo.Get(x => x.PageID == page.PageID);
                DateTime dt = savePage.InDate;
                ModelConverter.ConvertTo<DSPageInfo, PageInfo>(page, savePage);
                savePage.InDate = dt;
                savePage.Status = PageInfo.PageStatus.Editing;
                _pageInfo.Update(savePage);
                StartSavingComponents(page.ComponentInfo);
            }
            catch 
            {
                success = false;
                throw;
            }
            return success;
        }

        private void StartSavingComponents(List<DSComponentInfo> components)
        {
            List<DSComponentInfo> textComponents = components.Where(x => x.ObjectType == "Text").ToList();
            _TextComponentService.SaveComponentsNObjects(textComponents);

            List<DSComponentInfo> imageComponents = components.Where(x => x.ObjectType == "Image").ToList();
            _ImageComponentService.SaveComponentsNObjects(imageComponents);

            List<DSComponentInfo> videoComponents = components.Where(x => x.ObjectType == "Video").ToList();
            _VideoComponentService.SaveComponentsNObjects(videoComponents);

            List<DSComponentInfo> dataComponents = components.Where(x => x.ObjectType == "Dynamic").ToList();
            _DynamicComponentService.SaveComponentsNObjects(dataComponents);
        }

        public bool Delete(PageInfo page)
        {
            bool success = true;
            try
            {
                PageInfo savePage = _pageInfo.Get(x => x.PageID == page.PageID);
                _pageInfo.Delete(savePage);
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public bool AuditPage(PageInfo page)
        {
            bool success = true;
            try
            {
                PageInfo savePage = _pageInfo.Get(x => x.PageID == page.PageID);
                savePage.Status = PageInfo.PageStatus.Waiting; //送審
                _pageInfo.Update(savePage);
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public bool Reject(PageInfo page)
        {
            bool success = true;
            try
            {
                PageInfo savePage = _pageInfo.Get(x => x.PageID == page.PageID);
                savePage.Status = PageInfo.PageStatus.Reject; //編輯
                _pageInfo.Update(savePage);
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public bool LaunchPage(PageInfo page)
        {
            bool success = true;
            try
            {
                PageInfo savePage = _pageInfo.Get(x => x.PageID == page.PageID);
                savePage.Status = PageInfo.PageStatus.Active; //上線
                _pageInfo.Update(savePage);
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }
    }
}
