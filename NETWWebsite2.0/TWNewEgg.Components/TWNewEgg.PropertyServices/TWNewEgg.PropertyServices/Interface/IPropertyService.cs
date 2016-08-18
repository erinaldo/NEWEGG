using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Property;

namespace TWNewEgg.PropertyServices.Interface
{
    public interface IPropertyService
    {
        List<PropertyGroup> GetPropertyGroups(int categoryId);
        List<TWNewEgg.Models.DomainModels.Property.PriceWithQty> GetPropertyPriceWithQty(List<decimal> PriceList);
        //IPP Read
        List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM> ItemPropertySearch(SearchProperty searchProperty);
        /// <summary>
        /// 查詢Group--IPP新增屬性AddProperty使用
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        List<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup> GetPropertyGroupsIPP(int categoryId);
        /// <summary>
        /// 查詢Name--IPP新增屬性AddProperty使用
        /// </summary>
        /// <param name="groupIds"></param>
        /// <param name="nameIds"></param>
        /// <returns></returns>
        List<TWNewEgg.Models.DomainModels.Item.ItemPropertyName> GetPropertyNamesIPP(List<int> groupIds);

        //IPP Check before Create
        TWNewEgg.Models.DomainModels.Item.ItemPropertyDM CreatePropertyCheck(SearchProperty PropertyData);
        //IPP Create
        SearchProperty CreateProperty(TWNewEgg.Models.DomainModels.Item.ItemPropertyDM itemPropertyDM);
        //IPP Create List
        List<SearchProperty> CreatePropertyList(List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM> itemPropertyDMlist);
        //IPP Update
        TWNewEgg.Models.DomainModels.Item.ItemPropertyDM UpdateProperty(TWNewEgg.Models.DomainModels.Item.ItemPropertyDM itemPropertyDM);
        //IPP Delete 
        bool DeleteProperty(int categoryid, int groupid, int propertyid);
        //IPP Delete Value
        bool DeletePropertyValue(int propertyid, int valueid);
    }
}
