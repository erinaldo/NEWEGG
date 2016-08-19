using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TWNewEgg.Models.ViewModels.DeviceAd
{

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160615
    /// https://bitbucket.org/vincent0406/mobile-newegg/src/36f661ad362b6b9c7cdc6098121c31e4a4eaec7c/app/jsx/actions/home/LifeProjectAction.jsx?at=react-dev&fileviewer=file-view-default
    /// </summary>
    public class HomeLifeProjectVM
    {
        public string tabTitle { get; set; }
        //public List<TWNewEgg.Models.ViewModels.DeviceAd.SubLifeProjectVM> subTabs { get; set; }
        public SubLifeProjectVM sub { get; set; }
    }

    public class SubLifeProjectVM
    {
        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// color: "#ff0000"
        /// </summary>
        public string color { get; set; }

        /// <summary>
        /// 要點選的連結
        /// </summary>
        public string link { get; set; }

        /// <summary>
        /// 圖片位置
        /// </summary>
        public string imgUrl { get; set; }

        /// <summary>
        /// 文字
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 文字2
        /// </summary>
        public string desc2 { get; set; }

        ///// <summary>
        ///// 要點選的連結
        ///// </summary>
        //public string link { get; set; }

        ///// <summary>
        ///// 圖片位置
        ///// </summary>
        //public string imgUrl { get; set; }

        ///// <summary>
        ///// 名稱
        ///// </summary>
        //public string title { get; set; }

        ///// <summary>
        ///// 名稱2
        ///// </summary>
        //public string subDesc { get; set; }


    }

    //public class SubLifeProject2
    //{

    //    /// <summary>
    //    /// color: "#ff0000"
    //    /// </summary>
    //    public string color { get; set; }

    //    /// <summary>
    //    /// 要點選的連結
    //    /// </summary>
    //    public string link { get; set; }

    //    /// <summary>
    //    /// 圖片位置
    //    /// </summary>
    //    public string imgUrl { get; set; }

    //    /// <summary>
    //    /// 文字
    //    /// </summary>
    //    public string desc { get; set; }

    //    /// <summary>
    //    /// 文字2
    //    /// </summary>
    //    public string desc2 { get; set; }




    //}


}
