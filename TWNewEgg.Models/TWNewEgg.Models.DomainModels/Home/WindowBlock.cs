using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Home
{
    /// <summary>
    /// 首頁櫥窗的區塊.
    /// </summary>
    public class WindowBlock
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 顯示組合的編號, 決定Block如何堆疊排列(目前會有7種, 請參照SA文件)
        /// </summary>
        public int LayoutNumber { get; set; }

        /// <summary>
        /// 各區塊的內容集合
        /// </summary>
        public List<BlockCell> CellList { get; set; }
    }
}
