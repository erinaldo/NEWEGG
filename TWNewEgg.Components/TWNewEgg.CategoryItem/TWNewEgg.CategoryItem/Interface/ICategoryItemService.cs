using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DomainModels.Category;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.CategoryItem.Interface
{
    public interface ICategoryItemService
    {
        List<ItemInfo> GetCategoryItems(CategoryItemConditions conditions);
        List<ItemInfo> GetCategoryItemsTopRows(CategoryItemConditions conditions, int num);
        CategoryAreaInfo GetCategoryAreaInfo(CategoryItemConditions conditions);
        Breadcrumbs GetBreadCrumbs(int categoryID);
        List<MainZone> GetCategoryBanner(int categoryID);
    }
}
