using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using TWNewEgg.Models.DomainModels.PageMgmt;
using TWNewEgg.PageMgmt.Interface;

namespace TWNewEgg.PageMgmt
{
    public class PageMgmtAdapter:IPageMgmtAdapter
    {
        private IPageDBUtil _pageDBUtil;
        private IComponentDBUtil _componentDBUtil;
        private IComponentService<TextObject> _TextComponentService;
        private IComponentService<VideoInfo> _VideoComponentService;
        private IComponentService<ImageObject> _ImageComponentService;
        private IComponentService<DynamicObject> _DynamicComponentService;
        private IObjectService<TextObject> _textObjService;
        private IObjectService<VideoInfo> _videoObjService;
        private IObjectService<ImageObject> _imageObjService;
        private IObjectService<DynamicObject> _dataService;
        public PageMgmtAdapter(IPageDBUtil pageDBUtil, 
            IComponentDBUtil componentDBUtil, 
            IObjectService<TextObject> textObjService, 
            IObjectService<VideoInfo> videoObjService, IObjectService<ImageObject> imageService,
            IObjectService<DynamicObject> dataService,
            IComponentService<TextObject> TextComponentService,
            IComponentService<VideoInfo> VideoComponentService,
            IComponentService<ImageObject> ImageComponentService,
            IComponentService<DynamicObject> DynamicComponentService)
        {
            this._pageDBUtil = pageDBUtil;
            this._componentDBUtil = componentDBUtil;
            this._TextComponentService = TextComponentService;
            this._videoObjService = videoObjService;
            this._textObjService = textObjService;
            this._imageObjService = imageService;
            this._dataService = dataService;
        }

        public PageData EditPage(string path, bool isEditPage)
        {
            PageData data;
            List<ComponentInfo> editComponents = new List<ComponentInfo>();
            List<TextObject> texts = new List<TextObject>();
            List<ImageObject> images = new List<ImageObject>();
            List<VideoInfo> videos = new List<VideoInfo>();
            List<DynamicObject> dynamics = new List<DynamicObject>();
            PageInfo lastPage = _pageDBUtil.getLastPage(path);
            PageInfo editPage = new PageInfo();

            if (lastPage != null)
            {
                List<ComponentInfo> components = _componentDBUtil.getComponents(lastPage.PageID);
                if (isEditPage && lastPage.Status == PageInfo.PageStatus.Active)
                {
                    editPage = _pageDBUtil.createPage(lastPage);
                    editComponents = _componentDBUtil.createComponents(components, editPage.PageID);
                }
                else
                {
                    editPage = lastPage;
                    editComponents = components;
                }

                texts = _textObjService.GetByComponents(editComponents);
                videos = _videoObjService.GetByComponents(editComponents);
                images = _imageObjService.GetByComponents(editComponents);
                dynamics = _dataService.GetByComponents(editComponents);
            }

            data = new PageData
            {
                page = editPage,
                editComponents = editComponents,
                texts = texts,
                videos = videos,
                images = images,
                dynamics = dynamics
            };

            return data;
        }

        public PageData GetActivePage(string Path)
        {
            List<ComponentInfo> editComponents = new List<ComponentInfo>();
            List<TextObject> texts = new List<TextObject>();
            List<ImageObject> images = new List<ImageObject>();
            List<VideoInfo> videos = new List<VideoInfo>();
            List<DynamicObject> dynamics = new List<DynamicObject>();
            PageInfo lastPage = _pageDBUtil.getLastActivePage(Path);

            if (lastPage != null)
            {
                List<ComponentInfo> components = _componentDBUtil.getComponents(lastPage.PageID);
                editComponents = components;
                texts = _textObjService.GetByComponents(editComponents);
                videos = _videoObjService.GetByComponents(editComponents);
                images = _imageObjService.GetByComponents(editComponents);
                dynamics = _dataService.GetByComponents(editComponents);
            }

            PageData data = new PageData()
            {
                page = lastPage,
                editComponents = editComponents,
                texts = texts,
                videos = videos,
                images = images,
                dynamics = dynamics
            };
            return data;
        }

        public void NewPage(PageInfo page)
        {
            try
            {
                _componentDBUtil.deleteComponents(page);
            }
            catch 
            {
                throw;
            }
        }

        public void SavePage(DSPageInfo page)
        {
            try
            {
                PageInfo savePage = _pageDBUtil.getPage(page.PageID);
                List<PageInfo> pages = _pageDBUtil.getPages().Where(x => x.Path == page.Path).ToList();
                if (savePage.Path != page.Path && pages.Count > 0)
                {
                    throw new Exception("網頁名稱已存在");
                }
                else
                {
                    _pageDBUtil.savePage(page);
                    StartSavingComponents(page.ComponentInfo);
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeletePage(PageInfo page)
        {
            try
            {
                List<PageInfo> pages = _pageDBUtil.getAll().Where(x => x.Path == page.Path).ToList();
                foreach (PageInfo p in pages)
                {
                    _pageDBUtil.Delete(p);
                    _componentDBUtil.deleteComponents(p);
                }
            }
            catch
            {
                throw;
            }
        }

        public void CancelEdit(PageInfo page)
        {
            try
            {
                if (page.Status == PageInfo.PageStatus.Editing || page.Status == PageInfo.PageStatus.Reject)
                {
                    _pageDBUtil.Delete(page);
                    _componentDBUtil.deleteComponents(page);
                }
            }
            catch { throw; }
        }

        public void AddPage(PageInfo page)
        {
            try
            {
                List<PageInfo> pages = _pageDBUtil.getPages().Where(x => x.Path == page.Path).ToList();
                if (pages.Count > 0)
                {
                    throw new Exception("網頁名稱已存在");
                }
                else
                {
                    _pageDBUtil.createPage(page);
                }
            }
            catch
            {
                throw;
            }
        }

        public string AuditPage(PageInfo page)
        {
            string response = "";
            try
            {
                if (page.Status == PageInfo.PageStatus.Editing || page.Status == PageInfo.PageStatus.Reject)
                {
                    _pageDBUtil.AuditPage(page);
                    
                    response = "success";
                }
            }
            catch (Exception e)
            {
                response = "fail:" + e.Message;
            }

            return response;
        }

        public string LaunchPage(PageInfo page)
        {
            string response = "fail: 狀態必須是編輯中。";
            try
            {
                if (page.Status == PageInfo.PageStatus.Editing)
                {
                    using (TransactionScope tran = new TransactionScope())
                    {
                        _pageDBUtil.LaunchPage(page);
                        _componentDBUtil.LaunchComponents(page);
                        tran.Complete();
                        
                        response = "success";
                    }
                }
            }
            catch (Exception e)
            {
                response = "fail:" + e.Message;
            }

            return response;
        }

        private void StartSavingComponents(List<DSComponentInfo> components)
        {
            List<DSComponentInfo> textComponents = components.Where(x => x.ObjectType == "Text").ToList();
            _TextComponentService.SaveComponentsNObjects(textComponents);

            List<DSComponentInfo> videoComponents = components.Where(x => x.ObjectType == "Video").ToList();
            _VideoComponentService.SaveComponentsNObjects(videoComponents);

            List<DSComponentInfo> imageComponents = components.Where(x => x.ObjectType == "Image").ToList();
            _ImageComponentService.SaveComponentsNObjects(imageComponents);

            List<DSComponentInfo> dataComponents = components.Where(x => x.ObjectType == "Dynamic").ToList();
            _DynamicComponentService.SaveComponentsNObjects(dataComponents);
        }
    }
}
