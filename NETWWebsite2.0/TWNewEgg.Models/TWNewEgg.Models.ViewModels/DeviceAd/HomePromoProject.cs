using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.DeviceAd
{

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160615
    /// 促銷文案
    /// </summary>
    public class HomePromoProjectVM
    {
        
        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 要點選的連結
        /// </summary>
        public string link { get; set; }

    }

}
