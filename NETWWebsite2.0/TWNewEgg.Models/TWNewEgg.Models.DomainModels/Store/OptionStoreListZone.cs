using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 任選館的商品列表顯示區.
    /// </summary>
    public class OptionStoreListZone
    {
        /// <summary>
        /// 商品集合.
        /// </summary>
        public List<OptionStoreItemCell> ItemList { get; set; }

        /// <summary>
        /// 目前顯示的頁數.
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 目前Service顯示每頁Item的數量.
        /// </summary>
        public int PageItemCount { get; set; }
        
        /// <summary>
        /// 商品總分頁數(依照指定的每頁顯示商品數量計算而得).
        /// </summary>
        public int TotalPageCount { get; set; }
    }
}
