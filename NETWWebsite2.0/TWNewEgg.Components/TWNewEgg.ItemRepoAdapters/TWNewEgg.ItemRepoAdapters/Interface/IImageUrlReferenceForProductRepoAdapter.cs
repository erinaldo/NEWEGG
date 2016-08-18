using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IImageUrlReferenceForProductRepoAdapter
    {
        IQueryable<ImageUrlReferenceForProduct> GetAll();
        ImageUrlReferenceForProduct GetImagePath(int itemId, int size, int SizeIndex);
        Dictionary<int, List<ImageUrlReferenceForProduct>> GetAllImagePath(List<int> itemIds);
        ImageUrlReferenceForProduct AddImageUrlReferenceForProduct(ImageUrlReferenceForProduct ImageUrlReferenceTemp);
        List<ImageUrlReferenceForProduct> AddManyImageUrlReferenceForProduct(List<ImageUrlReferenceForProduct> ImageUrlReferenceForProductTemp);
    }
}
