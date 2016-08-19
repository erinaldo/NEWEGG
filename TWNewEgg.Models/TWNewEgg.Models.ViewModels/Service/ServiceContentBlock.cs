using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Service
{
    /// <summary>
    /// 服務說明分業內的各區塊
    /// </summary>
    public class ServiceContentBlock
    {
        /// <summary>
        /// 區塊標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 區塊的內容, 目前僅支援直接輸出Html, 未來或許再考慮其他格式.
        /// </summary>
        public string HtmlContent { get; set; }
    }
}
