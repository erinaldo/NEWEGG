using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Service
{
    /// <summary>
    /// 服務說明頁的所有資訊.
    /// </summary>
    public class ServicePageInfo
    {
        /// <summary>
        /// 各分頁的內容集合
        /// </summary>
        public List<ServiceContentTab> TabList { get; set; }

        /// <summary>
        /// 預設要顯示的Tab編號(從0開始)
        /// </summary>
        public int DefaultTabIndex { get; set; }

        /// <summary>
        /// 預設要打開顯示的Block編號(從0開始)
        /// </summary>
        public int DefaultBlockIndex { get; set; }
    }
}
