using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Item;

namespace TWNewEgg.ItemServices.Interface
{
    public interface IItemImageUrlService
    {        
        Dictionary<int,List<ImageUrlReferenceDM>> GetItemImagePath(List<int> itemIDs);
        List<ImageUrlReferenceDM> GetSingleItemImagePath(int itemID);
    }
}