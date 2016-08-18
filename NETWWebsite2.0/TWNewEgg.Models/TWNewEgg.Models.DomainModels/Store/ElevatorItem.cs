using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// Store頁面，直購館電梯顯示的文字，要與櫥窗的標題可對應.
    /// </summary>
    public class ElevatorItem
    {
        /// <summary>
        /// 電梯個別項目顯示的文字.
        /// </summary>
        public string Title { get; set; }
    }
}
