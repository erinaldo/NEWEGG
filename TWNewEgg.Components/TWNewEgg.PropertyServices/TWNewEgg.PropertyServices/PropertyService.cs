using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.PropertyServices.Interface;
using TWNewEgg.Models.DomainModels.Property;
using TWNewEgg.Models.DBModels.TWSQLDB;
using TWNewEgg.Models.DBModels.TWSQLDBExtModels;
using TWNewEgg.PropertyRepoAdapters.Interface;
using TWNewEgg.Framework.AutoMapper;

namespace TWNewEgg.PropertyServices
{
    public class PropertyService : IPropertyService
    {
        private IPropertyRepoAdapter _propertyRepoAdapter;
        private string[] _hideCodes = new string[1] { "F" };
        private string[] _hideCodesforIPP = new string[2] { "F","T" };
        public PropertyService(IPropertyRepoAdapter propertyRepoAdapter)
        {
            this._propertyRepoAdapter = propertyRepoAdapter;
        }

        public List<PropertyGroup> GetPropertyGroups(int categoryId)
        {   
            List<PropertyGroup> allPropertyGroups = new List<PropertyGroup>();
            var groupsData = this._propertyRepoAdapter.GetGroups(categoryId, this._hideCodes);

            List<ItemPropertyGroup> groups = groupsData.OrderBy(x => x.ShowOrder).ToList();
            foreach (var group in groups)
            {
                var keys = this.GetPropertyNames(new List<int> { group.ID })
                    .Where(x => !(x.PNName == null || x.PNName.Trim() == string.Empty))
                    .ToList();
                
                if (keys.Count > 0)
                {
                    PropertyGroup propertyGroup = ModelConverter.ConvertTo<PropertyGroup>(group);
                    propertyGroup.GroupProperties = keys;
                    allPropertyGroups.Add(propertyGroup);
                }
            }

            return allPropertyGroups;
        }

        public List<TWNewEgg.Models.DomainModels.Property.PriceWithQty> GetPropertyPriceWithQty(List<decimal> PriceList)
        {
            decimal maxPrice = PriceList.Max();
            List<TWNewEgg.Models.DomainModels.Property.PriceWithQty> PriceWithQtyList = new List<TWNewEgg.Models.DomainModels.Property.PriceWithQty>();
            
            for (int i = 0; i < maxPrice; )
            {
                int initPrice = i;
                if (i == 0)
                {
                    i += 500;
                }
                else
                {
                    i *= 2;
                }

                TWNewEgg.Models.DomainModels.Property.PriceWithQty temp = new TWNewEgg.Models.DomainModels.Property.PriceWithQty();
                temp.ID = i;
                temp.Qty = PriceList.Where(x => x >= initPrice && x <= i).Count();
                temp.ShowOrder = 0;
                temp.minPrice = initPrice;
                temp.maxPrice = i;
                temp.Name = initPrice.ToString("#,##0") + " - " + i.ToString("#,##0");
                if (temp.Qty > 0) {
                    temp.ShowOrder = 1;
                }
                PriceWithQtyList.Add(temp);
            }

            return PriceWithQtyList;
        }

        private List<PropertyKey> GetPropertyNames(List<int> allGroupIds)
        {
            List<PropertyKey> allPropertyKeys = new List<PropertyKey>();
            var namesData = this._propertyRepoAdapter.GetNames(allGroupIds, this._hideCodes);

            List<ItemPropertyName> names = namesData.Distinct().OrderBy(x=>x.ShowOrder).ToList();
            foreach (var name in names)
            {
                var values = this.GetPropertyValues(new List<int>{name.ID})
                    .Where(x => !(x.PVName == null || x.PVName.Trim() == string.Empty))
                    .ToList();

                if (values.Count > 0)
                {
                    PropertyKey propertyKey = ModelConverter.ConvertTo<PropertyKey>(name);
                    propertyKey.PropertyValues = values;
                    allPropertyKeys.Add(propertyKey);
                }
            }

            return allPropertyKeys;
        }

        private List<PropertyValue> GetPropertyValues(List<int> allNameIds)
        {
            List<PropertyValue> allPropertyValues = new List<PropertyValue>();
            var valuesData = this._propertyRepoAdapter.GetValues(allNameIds, this._hideCodes);

            List<ItemPropertyValue> values = valuesData.Distinct().OrderBy(x => x.ShowOrder).ToList();
            foreach (var value in values)
            {
                PropertyValue propertyValue = ModelConverter.ConvertTo<PropertyValue>(value);
                allPropertyValues.Add(propertyValue);
            }

            return allPropertyValues;
        }

        #region IPP Read Property
        public List<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup> GetPropertyGroupsIPP(int categoryId)
        {
            var groupsData = this._propertyRepoAdapter.GetGroups(categoryId, _hideCodesforIPP);

            List<ItemPropertyGroup> groups = groupsData.ToList();
            List<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup> propertyGrouplist = new List<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup>();
            if (groups.Count > 0)
            {               
                propertyGrouplist = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup>>(groups);
            }           
            return propertyGrouplist;
        }

        public List<TWNewEgg.Models.DomainModels.Item.ItemPropertyName> GetPropertyNamesIPP(List<int> groupIds)
        {
            List<TWNewEgg.Models.DomainModels.Item.ItemPropertyName> propertyKeylist = new List<TWNewEgg.Models.DomainModels.Item.ItemPropertyName>();
            var nameData = this._propertyRepoAdapter.GetNames(groupIds, _hideCodesforIPP);
         
            List<ItemPropertyName> names = nameData.ToList();
            if (names.Count > 0)
            {
                propertyKeylist = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Item.ItemPropertyName>>(names);
            }
            return propertyKeylist;
        }

        public List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM> ItemPropertySearch(SearchProperty searchProperty)
        {
            List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM> itemPropertyDM = new List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM>();
            //用CategoryID撈Group、Property及Value join完的資料
            IEnumerable<DbItemProperty> itempropertydata = this._propertyRepoAdapter.GetItemPropertyInfo(searchProperty.CategoryID);
            //依條件塞選
            
            if (searchProperty.GroupID > 0)
            {
                itempropertydata = itempropertydata.Where(x => x.ItemPropertyGroup.ID == searchProperty.GroupID);
            }
            if (searchProperty.PropertyNameID > 0)
            {
                itempropertydata = itempropertydata.Where(x => x.ItemPropertyName.ID == searchProperty.PropertyNameID);
            }
            if (searchProperty.PropertyValueID > 0)
            {
                itempropertydata = itempropertydata.Where(x => x.ItemPropertyValue.ID == searchProperty.PropertyValueID);
            }
            
            //塞選完轉成List
            List<DbItemProperty> itempropertydatatmp = itempropertydata.OrderBy(x => x.ItemPropertyName.ID).ToList();
            //轉成DM model
            itemPropertyDM = ModelConverter.ConvertTo<List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM>>(itempropertydatatmp);
            return itemPropertyDM;
        }
        #endregion
        #region IPP Create Property
        //新增前確認是否有重複 (新增Group、Property和Value 但這邊只先對Group與Property做有沒有資料已存到DB的檢查 )
        //DB檢查順序 : Group => Property 
        public TWNewEgg.Models.DomainModels.Item.ItemPropertyDM CreatePropertyCheck(SearchProperty PropertyData)
        {
            TWNewEgg.Models.DomainModels.Item.ItemPropertyDM result = new TWNewEgg.Models.DomainModels.Item.ItemPropertyDM();
            //Group為必填 故有資料必為一筆 無資料則要新增 
            ItemPropertyGroup groupsData = this._propertyRepoAdapter.GetGroups(PropertyData.CategoryID, _hideCodesforIPP).Where(x => string.Compare(PropertyData.GroupNameTW, x.GroupNameTW) == 0 && string.Compare(PropertyData.GroupName, x.GroupName) == 0).FirstOrDefault();
            //Group有資料
            if (groupsData != null)
            {
                //DB Group轉為DM 塞給result
                result.ItemPropertyGroup = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup>(groupsData);
                ItemPropertyName propertydata = null;
                /*此處邏輯在美蛋商品PM自行維護屬性加上拉美蛋商品屬性時，可能會發生同樣屬性名稱，但不同PropertyCode的情況，但目前(2016/03月)實際操作較少發生，待新增需求時修改*/
                if (!string.IsNullOrEmpty(PropertyData.PropertyCode))
                {
                    propertydata = this._propertyRepoAdapter.GetNames(new List<int> { result.ItemPropertyGroup.ID }, _hideCodesforIPP).Where(x =>string.Compare( x.PropertyCode , PropertyData.PropertyCode )==0&& string.Compare(PropertyData.PropertyName, x.PropertyName) == 0 && string.Compare(PropertyData.PropertyNameTW, x.PropertyNameTW) == 0).FirstOrDefault();
                }
                else
                {
                    propertydata = this._propertyRepoAdapter.GetNames(new List<int> { result.ItemPropertyGroup.ID }, _hideCodesforIPP).Where(x => string.IsNullOrEmpty(x.PropertyCode) && string.Compare(PropertyData.PropertyName, x.PropertyName) == 0 && string.Compare(PropertyData.PropertyNameTW, x.PropertyNameTW) == 0).FirstOrDefault();
                }
                //Property有資料
                if (propertydata != null)
                {
                    //DB Property轉為DM 塞給result
                    result.ItemPropertyName = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemPropertyName>(propertydata);
                }
            }
            //result裡面若Group有資料，則表示DB要新增Property和Value；若Group和Property有資料，則表示DB只要新增Value；若無資料，則要新增Group、Property和Value
            return result;
        }
        
        /// <summary>
        /// 新增Property   DB新增順序 : Group => Property(需要GroupID) => Value(需要PropertyID)
        /// </summary>
        /// <param name="PropertyData"></param>
        /// <returns> view需要新增或修改後的GroupID和PropertyID 故回傳model中會塞入ID</returns>
        public SearchProperty CreateProperty(TWNewEgg.Models.DomainModels.Item.ItemPropertyDM itemPropertyDM)
        {
            SearchProperty result = new SearchProperty();
            //新增資料 有該筆資料則不新增
            ItemPropertyGroup itemPropertyGroup = this.CreatePropertyGroup(itemPropertyDM.ItemPropertyGroup);
            itemPropertyDM.ItemPropertyName.GroupID = itemPropertyGroup.ID;
            //新增資料 有該筆資料則不新增
            ItemPropertyName itemPropertyName = this.CreatePropertyName(itemPropertyDM.ItemPropertyName);
            itemPropertyDM.ItemPropertyValue.PropertyNameID = itemPropertyName.ID;
            //新增資料 有該筆資料則不新增
            ItemPropertyValue itemPropertyValue = this.CreatePropertyValue(itemPropertyDM.ItemPropertyValue);
            //view需要的資料
            result.GroupID = itemPropertyGroup.ID;
            result.PropertyNameID = itemPropertyName.ID;
            result.PropertyValueID = itemPropertyValue.ID;
            return result;
        }
        //新增List 
        public List<SearchProperty> CreatePropertyList(List<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM> itemPropertyDMlist)
        {
            List<SearchProperty> result = new List<SearchProperty>();
            foreach (TWNewEgg.Models.DomainModels.Item.ItemPropertyDM item in itemPropertyDMlist)
            {
                SearchProperty searchProperty = new SearchProperty();
                searchProperty = this.CreateProperty(item);
                result.Add(searchProperty);
            }
            return result;
        }
        private ItemPropertyGroup CreatePropertyGroup(TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup itemPropertyGroup)
        {
            ItemPropertyGroup result = this._propertyRepoAdapter.GetGroups((int)itemPropertyGroup.CategoryID, _hideCodesforIPP).Where(x => string.Compare(itemPropertyGroup.GroupNameTW, x.GroupNameTW) == 0 && string.Compare(itemPropertyGroup.GroupName, x.GroupName) == 0).FirstOrDefault();
            //有該筆資料則不新增
            if (result == null)
            {
                ItemPropertyGroup newitemPropertyGroup = new ItemPropertyGroup();
                newitemPropertyGroup.CategoryID = itemPropertyGroup.CategoryID;
                newitemPropertyGroup.GroupName = itemPropertyGroup.GroupName;
                newitemPropertyGroup.GroupNameTW = itemPropertyGroup.GroupNameTW;
                newitemPropertyGroup.UpdateUser = itemPropertyGroup.UpdateUser;
                newitemPropertyGroup.CreateUser = itemPropertyGroup.UpdateUser;
                newitemPropertyGroup.UpdateDate = DateTime.Now;
                newitemPropertyGroup.CreateDate = DateTime.Now;
                newitemPropertyGroup.Hide = _hideCodes[0];
                newitemPropertyGroup.ShowOrder = 0;
                result = this._propertyRepoAdapter.CreatePropertyGroup(newitemPropertyGroup);
            }
            return result;
        }
        private ItemPropertyName CreatePropertyName(TWNewEgg.Models.DomainModels.Item.ItemPropertyName itemPropertyName)
        {
            ItemPropertyName result = null;
            IEnumerable<ItemPropertyName>  itemPropertyNametmp = this._propertyRepoAdapter.GetNames(new List<int> { itemPropertyName.GroupID }, _hideCodesforIPP).Where(x =>  string.Compare(itemPropertyName.PropertyName, x.PropertyName) == 0 && string.Compare(itemPropertyName.PropertyNameTW, x.PropertyNameTW) == 0);
            if (!(string.IsNullOrEmpty(itemPropertyName.PropertyCode)) && itemPropertyNametmp.Count() > 0)
            {
                itemPropertyNametmp = itemPropertyNametmp.Where(x => string.Compare(x.PropertyCode,itemPropertyName.PropertyCode)==0).Distinct();
            }
            if (string.IsNullOrEmpty(itemPropertyName.PropertyCode) && itemPropertyNametmp.Count() > 0)
            {
                itemPropertyNametmp = itemPropertyNametmp.Where(x => string.IsNullOrEmpty(x.PropertyCode)).Distinct();
            }
            if (itemPropertyNametmp.Count() > 0)
            {
                result = itemPropertyNametmp.FirstOrDefault();
            }
            //有該筆資料則不新增
            if (result == null)
            {
                ItemPropertyName newitemPropertyName = new ItemPropertyName();
                newitemPropertyName.GroupID = itemPropertyName.GroupID;
                newitemPropertyName.PropertyName = itemPropertyName.PropertyName;
                newitemPropertyName.PropertyNameTW = itemPropertyName.PropertyNameTW;
                newitemPropertyName.PropertyCode = itemPropertyName.PropertyCode;
                newitemPropertyName.UpdateUser = itemPropertyName.UpdateUser;
                newitemPropertyName.CreateUser = itemPropertyName.UpdateUser;
                newitemPropertyName.UpdateDate = DateTime.Now;
                newitemPropertyName.CreateDate = DateTime.Now;
                newitemPropertyName.Hide = _hideCodes[0];
                newitemPropertyName.ShowOrder = 0;
                result = this._propertyRepoAdapter.CreatePropertyName(newitemPropertyName);
            }
            return result;
        }
        private ItemPropertyValue CreatePropertyValue(TWNewEgg.Models.DomainModels.Item.ItemPropertyValue itemPropertyValue)
        {
            IEnumerable<ItemPropertyValue> itemPropertyValuetmp = this._propertyRepoAdapter.GetValues(new List<int> { itemPropertyValue.PropertyNameID }, this._hideCodesforIPP);
            ItemPropertyValue result = null;
            //專門擋美蛋新增重覆
            if (itemPropertyValuetmp.Count() > 0 && itemPropertyValue.USAData == (int)TWNewEgg.Models.DomainModels.Item.ItemPropertyValue.UsProduct.美蛋屬性)
            {
                if (itemPropertyValue.PropertyCode!=null)
                {
                    itemPropertyValuetmp = itemPropertyValuetmp.Where(x => string.Compare(x.PropertyCode,itemPropertyValue.PropertyCode)==0);
                    result = itemPropertyValuetmp.FirstOrDefault();
                }
            }
            
            if (result == null)
            {
                ItemPropertyValue newitemPropertyValue = new ItemPropertyValue();
                newitemPropertyValue.PropertyNameID = itemPropertyValue.PropertyNameID;
                newitemPropertyValue.PropertyValue = itemPropertyValue.PropertyValue;
                newitemPropertyValue.PropertyValueTW = itemPropertyValue.PropertyValueTW;
                newitemPropertyValue.PropertyCode = itemPropertyValue.PropertyCode;
                newitemPropertyValue.UpdateUser = itemPropertyValue.UpdateUser;
                newitemPropertyValue.CreateUser = itemPropertyValue.UpdateUser;
                newitemPropertyValue.UpdateDate = DateTime.Now;
                newitemPropertyValue.CreateDate = DateTime.Now;
                newitemPropertyValue.Hide = _hideCodes[0];
                newitemPropertyValue.ShowOrder = 0;
                result = this._propertyRepoAdapter.CreatePropertyValue(newitemPropertyValue);
            }
            return result;
        }
        #endregion
        #region IPP Update Property
        public TWNewEgg.Models.DomainModels.Item.ItemPropertyDM UpdateProperty(TWNewEgg.Models.DomainModels.Item.ItemPropertyDM itemPropertyDM)
        {
            TWNewEgg.Models.DomainModels.Item.ItemPropertyDM result = new TWNewEgg.Models.DomainModels.Item.ItemPropertyDM();
            DbItemProperty dbItemProperty = new DbItemProperty();
            //三張表各別更新data
            dbItemProperty.ItemPropertyGroup = this.UpdatePropertyGroup(itemPropertyDM.ItemPropertyGroup);
            dbItemProperty.ItemPropertyName = this.UpdatePropertyName(itemPropertyDM.ItemPropertyName);
            dbItemProperty.ItemPropertyValue = this.UpdatePropertyValue(itemPropertyDM.ItemPropertyValue);

            result = ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemPropertyDM>(dbItemProperty);
            return result;
        }
        private ItemPropertyGroup UpdatePropertyGroup(TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup PropertyData)
        {
            //撈資料
            ItemPropertyGroup itemPropertyGroup = this._propertyRepoAdapter.GetGroups((int)PropertyData.CategoryID, _hideCodesforIPP).Where(x => x.ID == PropertyData.ID).FirstOrDefault();
            //DM to DB
            ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemPropertyGroup, ItemPropertyGroup>(PropertyData, itemPropertyGroup);
            //Update且回傳
            return this._propertyRepoAdapter.UpdatePropertyGroup(itemPropertyGroup);
        }
        private ItemPropertyName UpdatePropertyName(TWNewEgg.Models.DomainModels.Item.ItemPropertyName PropertyData)
        {
            //撈資料
            ItemPropertyName itemPropertyName = this._propertyRepoAdapter.GetNames(new List<int> { PropertyData.GroupID }, _hideCodesforIPP).Where(x => x.ID == PropertyData.ID).FirstOrDefault();
            //DM to DB
            ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemPropertyName, ItemPropertyName>(PropertyData, itemPropertyName);
            //Update且回傳
            return this._propertyRepoAdapter.UpdatePropertyName(itemPropertyName);
        }
        private ItemPropertyValue UpdatePropertyValue(TWNewEgg.Models.DomainModels.Item.ItemPropertyValue PropertyData)
        {
            //撈資料
            ItemPropertyValue itemPropertyValue = this._propertyRepoAdapter.GetValues(new List<int> { PropertyData.PropertyNameID }, _hideCodesforIPP).Where(x => x.ID == PropertyData.ID).FirstOrDefault();
            //DM to DB
            ModelConverter.ConvertTo<TWNewEgg.Models.DomainModels.Item.ItemPropertyValue, ItemPropertyValue>(PropertyData, itemPropertyValue);
            //Update且回傳
            return this._propertyRepoAdapter.UpdatePropertyValue(itemPropertyValue);
        }
        #endregion
        #region IPP Delete Property or Value
        //IPP 依使用者選擇決定刪Group或Property 但不管選誰必定要刪多筆Value及ProductProperty
        //  Group     Property      Value     ProductProperty
        //  刪一筆 =>  刪多筆    => 刪多筆 =>   刪多筆
        //  或不刪 =>  刪一筆    => 刪多筆 =>   刪多筆
        //DB刪除順序 : ProductProperty => Value => Property => Group
        public bool DeleteProperty(int categoryid, int groupid, int propertyid)
        {
            bool IsSuccess = true;
            //撈出Group
            ItemPropertyGroup itemPropertyGroup = this._propertyRepoAdapter.GetGroups(categoryid, _hideCodesforIPP).Where(x => x.ID == groupid).FirstOrDefault();
            List<ItemPropertyName> itemPropertyNamelist = new List<ItemPropertyName>();
            List<ItemPropertyValue> itemPropertyValuelist = new List<ItemPropertyValue>();
            List<ProductProperty> productPropertylist = new List<ProductProperty>();
            List<int> itempropertynameIDs = new List<int>();
            //刪除前，先撈出Property,Value,PropuctProperty
            if (propertyid < 1)
            {
                //Property沒有值 => 刪除一筆Group 和多筆Property => 先撈出多筆Property資料
                IEnumerable<ItemPropertyName> itemPropertyNamelisttmp = this._propertyRepoAdapter.GetNames(new List<int> { groupid }, _hideCodesforIPP);
                //用來撈Value及PropuctProperty
                itempropertynameIDs = itemPropertyNamelisttmp.Select(x => x.ID).ToList();
                itemPropertyNamelist = itemPropertyNamelisttmp.ToList();
            }
            //刪除Property,Value,PropuctProperty
            else
            {
                //Property有值 => 刪除一筆Property => 先撈出一筆Property資料
                IEnumerable<ItemPropertyName> itemPropertyNamelisttmp = this._propertyRepoAdapter.GetNames(new List<int> { groupid }, _hideCodesforIPP).Where(x => x.ID == propertyid);
                //用來撈Value及PropuctProperty
                itempropertynameIDs = itemPropertyNamelisttmp.Select(x => x.ID).ToList();
                itemPropertyNamelist = itemPropertyNamelisttmp.ToList();
            }
            //撈出Value
            itemPropertyValuelist = this._propertyRepoAdapter.GetValues(itempropertynameIDs, _hideCodesforIPP).ToList();
            //撈出ProductProperty
            productPropertylist = this._propertyRepoAdapter.GetAllProductProperty().Where(x => itempropertynameIDs.Contains(x.PropertyNameID)).ToList();
            //開始刪除Group,Property,Value,PropuctProperty
            //因為PropuctProperty為最後一層 所以先刪除PropuctProperty
            if (productPropertylist.Count > 0)
            {
                foreach (ProductProperty productProperty in productPropertylist)
                {
                    this._propertyRepoAdapter.DeleteProductProperty(productProperty);
                }
            }
            //在刪除Value
            if (itemPropertyValuelist.Count > 0)
            {
                foreach (ItemPropertyValue itemPropertyValue in itemPropertyValuelist)
                {
                    this._propertyRepoAdapter.DeletePropertyValue(itemPropertyValue);
                }
            }
            //最後在刪PropertyName
            if (itemPropertyNamelist.Count > 0)
            {
                foreach (ItemPropertyName itemPropertyName in itemPropertyNamelist)
                {
                    this._propertyRepoAdapter.DeletePropertyName(itemPropertyName);
                }
            }
            //判斷使用者是要刪Group還是Property 
            if (propertyid < 1)
            {
                //刪除Group
                this._propertyRepoAdapter.DeletePropertyGroup(itemPropertyGroup);
            }
            return IsSuccess;
        }
        // 只刪除使用者指定的value data 只對value這張table做刪除與Group和Property沒有任何關係
        public bool DeletePropertyValue(int propertyid,int valueid)
        {
            ItemPropertyValue itemPropertyValue = this._propertyRepoAdapter.GetValues(new List<int> { propertyid }, _hideCodesforIPP).Where(x => x.ID == valueid).FirstOrDefault();

            return this._propertyRepoAdapter.DeletePropertyValue(itemPropertyValue);
        }
        #endregion
    }
}
