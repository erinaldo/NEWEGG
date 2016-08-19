using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 排序清單
    /// </summary>
    public class SortOption
    {
        /// <summary>
        /// 排序條件的標題.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 排序條件的值,用來傳給Service進行條件排序.
        /// </summary>
        public string SortValue { get; set; }
    }
}
