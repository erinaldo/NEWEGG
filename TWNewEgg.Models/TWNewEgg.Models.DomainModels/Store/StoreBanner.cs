using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Store
{
    /// <summary>
    /// 通用的Banner屬性，用於Store頁面，直購館，任選館.
    /// </summary>
    public class StoreBanner
    {
        /// <summary>
        /// 編號
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Banner要顯示的文字.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Banner的圖片網址.
        /// </summary>
        public string Image { get; set; }
        
        /// <summary>
        /// 點擊Banner後要跳轉的網址.
        /// </summary>
        public string Url { get; set; }
    }
}
