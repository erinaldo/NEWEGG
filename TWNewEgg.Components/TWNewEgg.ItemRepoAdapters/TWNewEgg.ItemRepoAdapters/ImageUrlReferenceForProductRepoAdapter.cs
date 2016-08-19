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
    public class ImageUrlReferenceForProductRepoAdapter : IImageUrlReferenceForProductRepoAdapter
    {
       private IRepository<ImageUrlReferenceForProduct> _ImageUrlReferenceForProductRepo;

         public ImageUrlReferenceForProductRepoAdapter(IRepository<ImageUrlReferenceForProduct> imageUrlReferenceRepo)
        {
            this._ImageUrlReferenceForProductRepo = imageUrlReferenceRepo;
        }
        public IQueryable<ImageUrlReferenceForProduct> GetAll()
        {
            return this._ImageUrlReferenceForProductRepo.GetAll();
        }

        public ImageUrlReferenceForProduct GetImagePath(int ProductID, int size, int SizeIndex)
        {
            var result = this._ImageUrlReferenceForProductRepo.Get(x => x.ProductID == ProductID && x.Size == size && x.SizeIndex == SizeIndex);
            return result;
        }

        public Dictionary<int, List<ImageUrlReferenceForProduct>> GetAllImagePath(List<int> ProductIDs)
        {
            ProductIDs = ProductIDs.Distinct().ToList();
            List<ImageUrlReferenceForProduct> list = this._ImageUrlReferenceForProductRepo.GetAll().Where(x => ProductIDs.Contains(x.ProductID)).ToList();
            Dictionary<int, List<ImageUrlReferenceForProduct>> RTN = new Dictionary<int, List<ImageUrlReferenceForProduct>>();
            foreach (int ProductID in ProductIDs)
            {
                RTN.Add(ProductID, list.Where(x => x.ProductID == ProductID).ToList());
            }
            return RTN;
        }
        public ImageUrlReferenceForProduct UpdateUrlReference(ImageUrlReferenceForProduct ImageUrlReference)
        {
            _ImageUrlReferenceForProductRepo.Update(ImageUrlReference);
            return ImageUrlReference;
        }
        public ImageUrlReferenceForProduct AddImageUrlReferenceForProduct(ImageUrlReferenceForProduct ImageUrlReferenceTemp) 
        {

            try
            {
                //檢查ItemID是否已經存在
                ImageUrlReferenceForProduct ImageUrlReference;
                ImageUrlReference = _ImageUrlReferenceForProductRepo.Get(x => x.ProductID == ImageUrlReferenceTemp.ProductID);
                if (ImageUrlReference == null)
                {
               
                    ImageUrlReferenceTemp.CreateUser = ImageUrlReferenceTemp.UpdateUser;

                    _ImageUrlReferenceForProductRepo.Create(ImageUrlReferenceTemp);
                }
                return ImageUrlReferenceTemp;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<ImageUrlReferenceForProduct> AddManyImageUrlReferenceForProduct(List<ImageUrlReferenceForProduct> ImageUrlReferenceForProductTemp)
        {

            try
            {
                //檢查ItemID是否已經存在
                this._ImageUrlReferenceForProductRepo.CreateMany(ImageUrlReferenceForProductTemp);
                return ImageUrlReferenceForProductTemp;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
