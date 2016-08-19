using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PageMgmt.Interface;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.PageMgmt
{
    public class TextObjectService:IObjectService<TextObject>
    {
        private IRepository<TextObject> _text;
        private IRepository<ComponentInfo> _component;
        public TextObjectService(IRepository<TextObject> text, IRepository<ComponentInfo> component)
        {
            this._text = text;
            this._component = component;
        }

        public List<TextObject> GetByComponents(List<ComponentInfo> components)
        {
            List<int> textIDs = components.Where(x => x.ObjectType == "Text").Select(x => x.ObjectID).ToList();
            List<TextObject> texts = _text.GetAll().Where(x => textIDs.Contains(x.TextID)).ToList();
            return texts;
        }

        public void saveEditObject(DSComponentInfo component)
        {
            TextObject text = component.Text;
            if (text != null)
            {
                Update(component, text);
                component.ObjectID = text.TextID;
            }
        }

        public void saveNewObject(DSComponentInfo component)
        {
            TextObject text = component.Text;
            if (text != null)
            {
                Create(text);
                component.ObjectID = text.TextID;
            }
        }

        public void saveDeleteObject(DSComponentInfo component)
        {
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);
            TextObject text = component.Text;
            if (text != null && origin.Status != ComponentInfo.ComponentStatus.Saved)
            {
                Delete(text);
            }
        }

        private bool Update(DSComponentInfo component, TextObject text)
        {
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);

            if (origin == null)
            {
                return false;
            }

            if (origin.Status == ComponentInfo.ComponentStatus.Edit)
            {
                TextObject saveText = _text.Get(x => x.TextID == text.TextID);
                if (saveText != null)
                {
                    ModelConverter.ConvertTo<TextObject, TextObject>(text, saveText);
                    _text.Update(saveText);
                }
            }
            else if (origin.Status == ComponentInfo.ComponentStatus.Saved)
            {
                text.InUser = "SYS";
                text.InDate = DateTime.Now;
                _text.Create(text);
            }

            return true;
        }
        
        private bool Create(TextObject text)
        {
            text.InDate = DateTime.Now;
            text.InUser = "SYS";
            _text.Create(text);
            return true;
        }

        private bool Delete(TextObject text)
        {
            _text.Delete(text);
            return true;
        }
    }
}
