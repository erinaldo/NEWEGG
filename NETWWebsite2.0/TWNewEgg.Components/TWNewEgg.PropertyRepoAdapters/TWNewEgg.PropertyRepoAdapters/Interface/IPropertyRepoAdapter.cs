using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;

namespace TWNewEgg.PropertyRepoAdapters.Interface
{
    public interface IPropertyRepoAdapter
    {
        List<int> FilterProductIds(List<int> productIds, int categoryId, List<int> propertyValueIds);
        IQueryable<ItemPropertyGroup> GetGroups(int categoryId, string[] hide);
        IQueryable<ItemPropertyName> GetNames(List<int> allGroupIds, string[] hide);
        IQueryable<ItemPropertyValue> GetValues(List<int> allNameIds, string[] hide);
        IQueryable<DbItemProperty> GetItemPropertyInfo(int categoryID);
        IQueryable<ProductProperty> GetAllProductProperty();
        ItemPropertyGroup CreatePropertyGroup(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup itemPropertyGroup);
        ItemPropertyName CreatePropertyName(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName itemPropertyName);
        ItemPropertyValue CreatePropertyValue(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue itemPropertyValue);
        ItemPropertyGroup UpdatePropertyGroup(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup itemPropertyGroup);
        ItemPropertyName UpdatePropertyName(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName itemPropertyName);
        ItemPropertyValue UpdatePropertyValue(TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue itemPropertyValue);
        bool DeletePropertyGroup(ItemPropertyGroup itemPropertyGroup);
        bool DeletePropertyName(ItemPropertyName itemPropertyName);
        bool DeletePropertyValue(ItemPropertyValue itemPropertyValue);
        bool DeleteProductProperty(ProductProperty productProperty);
    }
}
