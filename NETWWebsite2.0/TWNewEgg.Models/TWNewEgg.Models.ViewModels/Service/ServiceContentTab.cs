using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Service
{
    /// <summary>
    /// 存放服務說明的各分頁(Tab)內容
    /// </summary>
    public class ServiceContentTab
    {
        /// <summary>
        /// 分頁標題
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 區塊集合
        /// </summary>
        public List<ServiceContentBlock> BlockList { get; set; }
    }
}
