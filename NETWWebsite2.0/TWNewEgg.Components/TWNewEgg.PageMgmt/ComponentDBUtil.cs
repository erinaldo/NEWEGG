using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PageMgmt.Interface;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;

namespace TWNewEgg.PageMgmt
{
    public class ComponentDBUtil:IComponentDBUtil
    {
        private IRepository<ComponentInfo> _componentInfo;
        private IRepository<TextObject> _text;
        public ComponentDBUtil(IRepository<ComponentInfo> componentInfo, IRepository<TextObject> text)
        {
            this._componentInfo = componentInfo;
            this._text = text;
        }

        public List<ComponentInfo> getComponents(int pageID)
        {
            return _componentInfo.GetAll().Where(x => x.PageID == pageID).ToList();
        }

        public List<ComponentInfo> createComponents(List<ComponentInfo> components, int newPageID)
        {
            List<ComponentInfo> newList = new List<ComponentInfo>();
            foreach (ComponentInfo component in components)
            {
                ComponentInfo newComp = ModelConverter.ConvertTo<ComponentInfo>(component);
                newComp.PageID = newPageID;
                newComp.Status = ComponentInfo.ComponentStatus.Saved;
                _componentInfo.Create(newComp);
                newList.Add(newComp);
            }
            return newList;
        }

        public bool Update(ComponentInfo component)
        {
            bool success = true;
            try
            {
                ComponentInfo saveComp = _componentInfo.Get(x => x.ComponentID == component.ComponentID);
                if (saveComp != null)
                {
                    saveComp.Height = component.Height;
                    saveComp.HitCount = component.HitCount;
                    saveComp.Width = component.Width;
                    saveComp.XIndex = component.XIndex;
                    saveComp.YIndex = component.YIndex;
                    saveComp.ZIndex = component.ZIndex;
                    saveComp.ObjectID = component.ObjectID;
                    saveComp.Status = component.Status;
                    saveComp.Index = component.Index;

                    _componentInfo.Update(saveComp);
                }
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public bool Create(ComponentInfo component)
        {
            bool success = true;
            try
            {
                component.InDate = DateTime.Now;
                component.InUser = "SYS";
                ComponentInfo saveComp = ModelConverter.ConvertTo<ComponentInfo>(component);
                _componentInfo.Create(saveComp);
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public bool Delete(ComponentInfo component)
        {
            bool success = true;
            try
            {
                ComponentInfo deleteComp = _componentInfo.Get(x => x.ComponentID == component.ComponentID);
                if (deleteComp != null)
                {
                    _componentInfo.Delete(deleteComp);
                }
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public bool deleteComponents(PageInfo page)
        {
            bool success = true;
            try
            {
                List<ComponentInfo> components = _componentInfo.GetAll().Where(x => x.PageID == page.PageID).ToList();
                foreach (ComponentInfo component in components)
                {
                    _componentInfo.Delete(component);
                }
            }
            catch
            {
                success = false;
                throw;
            }
            return success;
        }

        public ComponentInfo addComponent(int PageID, string ObjectType, int ObjectID)
        {
            ComponentInfo component = new ComponentInfo();
            component.ObjectType = ObjectType;
            component.PageID = PageID;
            component.ObjectID = ObjectID;
            return component;
        }


        public void LaunchComponents(PageInfo page)
        {
            List<ComponentInfo> components = _componentInfo.GetAll().Where(x => x.PageID == page.PageID).ToList();
            if (components != null)
            {
                foreach (ComponentInfo component in components)
                {
                    component.Status = ComponentInfo.ComponentStatus.Saved;
                    _componentInfo.Update(component);
                }
            }
        }
    }
}
