using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.API.Models
{
    public class BatchExamineModel : TWNewEgg.API.Models.ItemSketch
    {
        public BatchExamineModel()
        {
            colorsizeProperty = new List<colorSizeModel>();
        }
        public List<colorSizeModel> colorsizeProperty { get; set; }
        public List<oneArrayProperty> oneArrayProperty { get; set; }
    }
    public class propertyModel
    {
        public propertyModel()
        {
            proValueId = -1;
            proQty = -1;
        }
        public int proValueId { get; set; }
        public int proQty { get; set; }
    }
    public class oneArrayProperty
    {
        public oneArrayProperty()
        {
            firstGroupVauleId = -1;
            Qty = -1;
        }
        public int firstGroupVauleId { get; set; }
        public string inputValue { get; set; }
        public int Qty { get; set; }
    }
    public class colorSizeModel
    {
        public colorSizeModel()
        {
            colorValueId = -1;
            listProperty = new List<propertyModel>();
        }
        public int colorValueId { get; set; }
        public string inputValue { get; set; }
        public List<propertyModel> listProperty { get; set; }
    }
    public class propertyJoinModel
    {
        public int ItemPropertyValue_ID { get; set; }
        public int ItemPropertyValue_propertyNameID { get; set; }
        public string ItemPropertyValue_propertyValue { get; set; }
        public string ItemPropertyValue_propertyValueTW { get; set; }
        public int ItemPropertyName_groupId { get; set; }
        public int ItemPropertyName_ID { get; set; }
        public string ItemPropertyName_propertyName { get; set; }
        public string ItemPropertyName_propertyNameTW { get; set; }
    }


    public class sketchPropertyExamine : TWNewEgg.API.Models.ItemSketch
    {
        //List<TWNewEgg.API.Models.PropertyColorSize> propertyCS { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public string definitions { get; set; }
        public int group_id { get; set; }
    }
    public class PropertyColorSize
    {
        public string color { get; set; }
        public string size { get; set; }
    }
    public class ItemGroupJoinitemsketchListData
    {
        public int SellerId { get; set; }
        public Nullable<int> ItemTempId { get; set; }
        public string ValueName { get; set; }
        public Nullable<int> GroupValueID { get; set; }
        public int MasterPropertyID { get; set; }
        public int PropertyID { get; set; }
        public int groupID { get; set; }
        public int ValueID { get; set; }
        public string definitions { get; set; }
        public string propertyValue { get; set; }
        public Nullable<int> itemid { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> PriceCash { get; set; }
        public string ProductName { get; set; }
    }
    public class ItemTempJoinProductTemp
    {
        public int itemtemp_id { get; set; }
        public int? item_id { get; set; }
        public int productTemp_id { get; set; }
        public int? product_id { get; set; }

    }
    public enum OneOrTwoDimension
    {
        OneDimension = 1,
        TwoDimension = 2
    }
}
