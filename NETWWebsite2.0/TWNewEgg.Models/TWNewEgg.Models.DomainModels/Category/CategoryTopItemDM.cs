using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Category
{
    public class CategoryTopItemDM
    {
        public enum itemType
        {
            銷售TOP = 1,
            推薦商品 = 2
        };
        public enum showAll
        {
            不顯示 = 0,
            顯示 = 1
        };
        public int? CategoryID { get; set; }
        public int? ItemID { get; set; }
        /// <summary>
        /// 1：銷售TOP10
        /// 2：推薦商品
        /// </summary>
        public int? ItemType { get; set; }
        /// <summary>
        /// 顯示排序，1開始
        /// </summary>
        public int? Showorder { get; set; }
        /// <summary>
        /// 0：不顯示；1：顯示
        /// </summary>
        public int? ShowAll { get; set; }
        public System.DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
    }

    public class TopItemDM
    {
        public TopItemDM()
        {
            this.ItemList = new List<CategoryTopItemDM>();
        }
        public int? CategoryID { get; set; }
        public int? ItemType { get; set; }
        public System.DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public List<CategoryTopItemDM> ItemList { get; set; }
    }
}
