using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TWNewEgg.Framework.AutoMapper;
using TWNewEgg.PageMgmt.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB.PageMgmt;
using TWNewEgg.DAL.Interface;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels.PageMgmt;

namespace TWNewEgg.PageMgmt
{
    public class ImageObjectService: IObjectService<ImageObject>
    {
        private IRepository<ImageObject> _image;
        private IRepository<ComponentInfo> _component;

        public ImageObjectService(IRepository<ImageObject> image, IRepository<ComponentInfo> component)
        {
            this._image = image;
            this._component = component;
        }

        public List<ImageObject> GetByComponents(List<ComponentInfo> components)
        {
            List<int> ImageIDs = components.Where(x => x.ObjectType == "Image").Select(x => x.ObjectID).ToList();
            List<ImageObject> images = _image.GetAll().Where(x => ImageIDs.Contains(x.ImageID)).ToList();

            List<ImageObject> groupImgs = new List<ImageObject>();
            foreach (ImageObject img in images)
            {
                if (img.Effect == "R")
                {
                    groupImgs.AddRange(_image.GetAll().Where(x => x.EffectGroupID == img.EffectGroupID && x.ImageID != img.ImageID));
                }
            }

            images.AddRange(groupImgs);
            return images;
        }

        public void saveEditObject(DSComponentInfo component)
        {
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);
            List<ImageObject> images = component.Image;
            ImageObject image = component.Image[0] ?? null;
            
            if (image != null)
            {   
                List<ImageObject> groupImgs = new List<ImageObject>();
                groupImgs = images.Where(x => x.EffectGroupID == image.ImageID && x.ImageID != image.ImageID).ToList();
                
                if(origin.Status != ComponentInfo.ComponentStatus.Saved || image.Effect != "R") 
                    deleteGroupImages(image);
                Update(component);
                createGroupImages(image, groupImgs);
            }
        }

        public void saveNewObject(DSComponentInfo component)
        {
            List<ImageObject> images = component.Image;
            images.RemoveAll(x => x == null);
            ImageObject image = images[0] ?? null;
            
            if (image != null)
            {
                List<ImageObject> groupImgs = new List<ImageObject>();
                groupImgs = images.Where(x => x.EffectGroupID == image.ImageID && x.ImageID != image.ImageID).ToList();

                Create(image);
                component.ObjectID = image.ImageID;

                createGroupImages(image, groupImgs);
            }
        }

        public void saveDeleteObject(DSComponentInfo component)
        {
            List<ImageObject> images = component.Image;
            images.RemoveAll(x => x == null);
            ImageObject image = images[0] ?? null;
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);

            if (image != null && origin.Status != ComponentInfo.ComponentStatus.Saved)
            {
                deleteGroupImages(image);
                Delete(image);
                component.ObjectID = image.ImageID;
            }
        }

        public bool Update(DSComponentInfo component)
        {
            ImageObject image = component.Image[0] ?? null;
            ComponentInfo origin = _component.Get(x => x.ComponentID == component.ComponentID);

            if (origin == null)
            {
                return false;
            }

            if (origin.Status == ComponentInfo.ComponentStatus.Edit)
            {
                ImageObject saveimage = _image.Get(x => x.ImageID == image.ImageID);
                if (saveimage != null)
                {
                    ModelConverter.ConvertTo<ImageObject, ImageObject>(image, saveimage);
                    _image.Update(saveimage);
                }
            }
            else if (origin.Status == ComponentInfo.ComponentStatus.Saved)
            {
                image.InUser = "SYS";
                image.InDate = DateTime.Now;
                Create(image);
                component.ObjectID = image.ImageID;
            }
            return true;
        }

        public bool Create(ImageObject image)
        {
            image.InDate = DateTime.Now;
            image.InUser = "SYS";
            _image.Create(image);
            image.EffectGroupID = image.ImageID;
            _image.Update(image);
            return true;
        }

        public bool Delete(ImageObject image)
        {
            _image.Delete(image);
            return true;
        }

        private void createGroupImages(ImageObject image, List<ImageObject> groupImgs)
        {
            if (image.Effect == "R")
            {
                foreach (ImageObject img in groupImgs)
                {
                    ImageObject saveimage = ModelConverter.ConvertTo<ImageObject>(img);
                    saveimage.EffectGroupID = image.ImageID;
                    _image.Create(saveimage);
                }
            }
        }
        private void deleteGroupImages(ImageObject image)
        {
            List<ImageObject> originGroupImgs = new List<ImageObject>();
            originGroupImgs = _image.GetAll().Where(x => x.EffectGroupID == image.ImageID && x.ImageID != image.ImageID).ToList();
            foreach (ImageObject img in originGroupImgs)
            {
                Delete(img);
            }
        }
    }
}
