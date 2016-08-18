using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PageMgmt.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using log4net;
using System.Reflection;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.PageMgmt
{
    public class ComponentService<T>:IComponentService<T>
    {
        ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IComponentDBUtil _componentDBUtil;
        private IObjectService<T> _service;
        private List<DSComponentInfo> allComponents = new List<DSComponentInfo>();
        
        public ComponentService(IComponentDBUtil componentDBUtil, IObjectService<T> service)
        {
            this._componentDBUtil = componentDBUtil;
            _service = service;
        }

        /// <summary>
        /// 儲存頁面編輯元件
        /// </summary>
        /// <param name="components"></param>
        /// <param name="currentAccountName"></param>
        public void SaveComponentsNObjects(List<DSComponentInfo> components)
        {   
            try
            {
                foreach (DSComponentInfo component in components)
                {
                    AssignServiceByComponentStatus(component);
                }
            }
            catch
            {
                throw;
            }
        }
        
        private void AssignServiceByComponentStatus(DSComponentInfo component) 
        {
            if (component.Status == ComponentInfo.ComponentStatus.Edit) 
            {
                SaveEditComponent(component);
            }
            else if (component.Status == ComponentInfo.ComponentStatus.New) 
            {
                SaveNewComponent(component);
            }
            else if (component.Status == ComponentInfo.ComponentStatus.Delete) 
            {
                SaveDeleteComponent(component);
            }
        }

        private void SaveEditComponent(DSComponentInfo component)
        {
            _service.saveEditObject(component);
            _componentDBUtil.Update(component);
        }

        private void SaveNewComponent(DSComponentInfo component)
        {
            _service.saveNewObject(component);
            component.Status = ComponentInfo.ComponentStatus.Edit;
            _componentDBUtil.Create(component);
            if (component.ObjectType == "CustomizedObject")
            {
                _service.saveEditObject(component);
            }
        }

        private void SaveDeleteComponent(DSComponentInfo component)
        {
            _service.saveDeleteObject(component);
            _componentDBUtil.Delete(component);
        }  
    }
}
