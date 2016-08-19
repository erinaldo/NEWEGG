using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PageMgmt.Interface;

namespace TWNewEgg.PageMgmt
{
    public class DynamicObjectService : IObjectService<DynamicObject>
    {
        private IRepository<DynamicObject> _data;
        private IRepository<ComponentInfo> _component;
        public DynamicObjectService(IRepository<DynamicObject> data, IRepository<ComponentInfo> component)
        {
            this._data = data;
            this._component = component;
        }

        public List<DynamicObject> GetByComponents(List<ComponentInfo> components)
        {
            List<int> DataIDs = components.Where(x => x.ObjectType == "Dynamic").Select(x => x.ObjectID).ToList();
            List<DynamicObject> datas = _data.GetAll().Where(x => DataIDs.Contains(x.DynamicID)).ToList();
            return datas;
        }

        public void saveEditObject(DSComponentInfo component)
        {
            DynamicObject data = component.Dynamic;
            if (data != null)
            {
                Update(component, data);
                component.ObjectID = data.DynamicID;
            }
        }

        public void saveNewObject(DSComponentInfo component)
        {
            DynamicObject data = component.Dynamic;
            if (data != null)
            {
                Create(data);
                component.ObjectID = data.DynamicID;
            }
        }

        public void saveDeleteObject(DSComponentInfo component)
        {
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);
            DynamicObject data = component.Dynamic;
            if (data != null && origin.Status != ComponentInfo.ComponentStatus.Saved)
            {
                Delete(data);
            }
        }

        private bool Update(DSComponentInfo component, DynamicObject data)
        {
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);

            if (origin == null)
            {
                return false;
            }

            if (origin.Status == ComponentInfo.ComponentStatus.Edit)
            {
                DynamicObject saveObj = _data.Get(x => x.DynamicID == data.DynamicID);
                if (saveObj != null)
                {
                    ModelConverter.ConvertTo<DynamicObject, DynamicObject>(data, saveObj);
                    _data.Update(saveObj);
                }
            }
            else if (origin.Status == ComponentInfo.ComponentStatus.Saved)
            {
                data.InUser = "SYS";
                data.InDate = DateTime.Now;
                _data.Create(data);
            }
            return true;
        }

        private bool Create(DynamicObject data)
        {
            data.InDate = DateTime.Now;
            data.InUser = "SYS";
            _data.Create(data);
            return true;
        }

        private bool Delete(DynamicObject data)
        {
            _data.Delete(data);
            return true;
        }
    }
}
