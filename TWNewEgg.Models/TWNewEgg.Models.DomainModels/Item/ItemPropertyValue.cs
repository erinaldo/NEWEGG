using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class ItemPropertyValue
    {
        public int ID { get; set; }
        public int PropertyNameID { get; set; }
        public string PropertyCode { get; set; }
        public string PropertyValue { get; set; }
        public string PropertyValueTW { get; set; }
        public string Hide { get; set; }
        public int ShowOrder { get; set; }
        public string CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public int? Updated { get; set; }
        public string UpdateUser { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public int USAData { get; set; }//判斷是否是從美蛋商品傳過來的屬性 0:一般屬性 1:美蛋屬性
        public enum UsProduct
        {
            一般屬性 = 0,
            美蛋屬性 = 1
        };
    }
}
