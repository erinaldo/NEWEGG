using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.DAL.Interface;
using TWNewEgg.ItemRepoAdapters.Interface;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters
{
    public class ImageUrlReferenceRepoAdapter :IImageUrlReferenceRepoAdapter
    {
        private IRepository<ImageUrlReference> _imageUrlReferenceRepo;

        public ImageUrlReferenceRepoAdapter(IRepository<ImageUrlReference> imageUrlReferenceRepo)
        {
            this._imageUrlReferenceRepo = imageUrlReferenceRepo;
        }
        public IQueryable<ImageUrlReference> GetAll()
        {
            return this._imageUrlReferenceRepo.GetAll();
        }

        public ImageUrlReference GetImagePath(int itemId,int size,int SizeIndex)
        {
            var result = this._imageUrlReferenceRepo.Get(x => x.ItemID == itemId && x.Size == size && x.SizeIndex == SizeIndex);
            return result;
        }

        public Dictionary<int, List<ImageUrlReference>> GetAllImagePath(List<int> itemIds)
        {
            itemIds = itemIds.Distinct().ToList();
            List<ImageUrlReference> list = this._imageUrlReferenceRepo.GetAll().Where(x => itemIds.Contains(x.ItemID)).ToList();
            Dictionary<int, List<ImageUrlReference>> RTN = new Dictionary<int, List<ImageUrlReference>>();
            foreach (int itemId in itemIds)
            {
                RTN.Add(itemId, list.Where(x=>x.ItemID==itemId).ToList());
            }
            return RTN;
        }
        public ImageUrlReference UpdateUrlReference(ImageUrlReference ImageUrlReference)
        {
            _imageUrlReferenceRepo.Update(ImageUrlReference);
            return ImageUrlReference;
        }
        public ImageUrlReference AddImageUrlReference(ImageUrlReference ImageUrlReferenceTemp) 
        {

            try
            {
                //檢查ItemID是否已經存在
                ImageUrlReference ImageUrlReference;
                ImageUrlReference = _imageUrlReferenceRepo.Get(x => x.ItemID == ImageUrlReferenceTemp.ItemID);
                if (ImageUrlReference == null)
                {
               
                    ImageUrlReferenceTemp.CreateUser = ImageUrlReferenceTemp.UpdateUser;
                   
                    _imageUrlReferenceRepo.Create(ImageUrlReferenceTemp);
                }
                return ImageUrlReferenceTemp;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<ImageUrlReference> AddManyImageUrlReference(List<ImageUrlReference> ImageUrlReferenceTemp)
        {

            try
            {
                //檢查ItemID是否已經存在
                this._imageUrlReferenceRepo.CreateMany(ImageUrlReferenceTemp);
                return ImageUrlReferenceTemp;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
