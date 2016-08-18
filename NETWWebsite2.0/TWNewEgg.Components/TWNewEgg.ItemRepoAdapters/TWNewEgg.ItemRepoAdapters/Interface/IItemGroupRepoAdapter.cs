using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels;

namespace TWNewEgg.ItemRepoAdapters.Interface
{
    public interface IItemGroupRepoAdapter
    {
        #region ItemGroup
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> GetAllItemGroup();
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroup> GetItemGroupById(int argNumId);
        #endregion

        #region ItemGroupProperty
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> GetAllItemGroupProperty();
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> GetItemGroupPropertyByGroupId(int argNumGroupId);
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupProperty> GetItemGroupPropertyByGroupIdAndPropertyId(int argNumGroupId, int argNumPropertyId);
        #endregion

        #region ItemGroupDetailProperty
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> GetAllItemGroupDetailProperty();
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> GetItemGroupDetailPropertyByGroupId(int argNumGroupId);
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemGroupDetailProperty> GetItemGroupDetailPropertyByItemId(int argNumItemId);
        #endregion

        #region ItemPropertyGroup
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> GetAllPropertyGroup();
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> GetPropertyGroupById(int argNumId);
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyGroup> GetPropertyGroupByCategoryId(int argNumCategoryId);
        #endregion

        #region ItemPropertyName
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> GetAllPropertyName();
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> GetPropertyNameById(int argNumId);
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyName> GetPropertyNameByGroupId(int argNumGroupId);
        #endregion

        #region ItemPropertyValue
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> GetAllPropertyValue();
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> GetPropertyValueById(int argNumId);
        IQueryable<TWNewEgg.Models.DBModels.TWSQLDB.ItemPropertyValue> GetPropertyValueByPropertyNameId(int argNumNameId);
        #endregion
    }
}
