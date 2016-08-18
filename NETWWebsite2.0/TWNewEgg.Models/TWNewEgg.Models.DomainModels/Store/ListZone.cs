using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 一般分類頁面、直購館的產品列表區塊.
    /// </summary>
    public class ListZone
    {
        /// <summary>
        /// 產品的集合.
        /// </summary>
        public List<StoreItemCell> ItemList { get; set; }
    }
}
