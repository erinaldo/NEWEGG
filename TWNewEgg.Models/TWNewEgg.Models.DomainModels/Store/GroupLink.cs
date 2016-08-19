using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// Store頁面顯示的櫥窗群組連結，依照Store或直購館會有不同的排列(由UI判斷處理)
    /// </summary>
    public class GroupLink
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 櫥窗群組連結的標題.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 櫥窗群組連結的網址.
        /// </summary>
        public string Url { get; set; }
    }
}
