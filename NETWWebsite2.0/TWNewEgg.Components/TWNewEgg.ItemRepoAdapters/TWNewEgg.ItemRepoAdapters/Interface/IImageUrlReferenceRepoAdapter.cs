using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IImageUrlReferenceRepoAdapter
    {
        IQueryable<ImageUrlReference> GetAll();
        ImageUrlReference GetImagePath(int itemId,int size,int SizeIndex);
        Dictionary<int, List<ImageUrlReference>> GetAllImagePath(List<int> itemIds);
        ImageUrlReference AddImageUrlReference(ImageUrlReference ImageUrlReferenceTemp);
        List<ImageUrlReference> AddManyImageUrlReference(List<ImageUrlReference> ImageUrlReferenceTemp);
    }
}
