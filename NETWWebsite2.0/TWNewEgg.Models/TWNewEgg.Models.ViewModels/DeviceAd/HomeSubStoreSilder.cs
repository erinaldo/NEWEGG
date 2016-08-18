using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.DeviceAd
{

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160621
    /// https://bitbucket.org/vincent0406/mobile-newegg/src/36f661ad362b6b9c7cdc6098121c31e4a4eaec7c/app/jsx/components/home/HomeStore.react.jsx?at=react-dev&fileviewer=file-view-default
    /// 手機版分類輪播
    /// </summary>
    public class HomeSubStoreSilderVM
    {
        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int id { get; set; }
        //public int dataId { get; set; }


        /// <summary>
        /// 名稱
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 要點選的連結
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// 圖片位置
        /// </summary>
        public string imgUrl { get; set; }

        /// <summary>
        /// ---------------------------------------------add by bruce 20160623
        /// 來自SubCategory_NormalStore與Category的ID
        /// </summary>
        public int categoryId { get; set; }

    }

}
