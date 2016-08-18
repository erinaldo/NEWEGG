using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.MobileStore
{
    /// <summary>
    /// 手機版 - Store頁面的頁籤區塊
    /// </summary>
    public class MStoreTab
    {
        /// <summary>
        /// 頁籤的編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 頁籤的標題
        /// </summary>
        public string Title { get; set; }
    }
}
