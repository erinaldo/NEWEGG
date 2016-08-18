using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Property;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 主要提供給任選館的資訊.
    /// </summary>
    public class OptionStoreInfo
    {
        /// <summary>
        /// 麵包屑選單
        /// </summary>
        public Breadcrumbs Breadcurmbs { get; set; }
        
        /// <summary>
        /// 任選館的活動名稱,會顯示在已選購清單作為提示文字.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 活動顯示的單一Banner.
        /// </summary>
        public StoreBanner SingleBanner { get; set; }

        /// <summary>
        /// 小廣告Banner,有可能為空值.
        /// </summary>
        public List<StoreBanner> PullDownAdvList { get; set; }

        /// <summary>
        /// 可去結帳的金額限制.
        /// </summary>
        public decimal LimitAmount { get; set; }
        
        /// <summary>
        /// 左邊屬性過濾區塊.
        /// </summary>
        public List<PropertyGroup> PropertyFilter { get; set; }
        
        /// <summary>
        /// 排序清單.
        /// </summary>
        public List<SortOption> SortOptionList { get; set; }
        
        /// <summary>
        /// 產品列表區域(包含分頁顯示那塊).
        /// </summary>
        public OptionStoreListZone ListZone { get; set; }
    }
}
