using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.DeviceAd
{   

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160615
    /// 行動設備的廣告設定
    /// </summary>
    public class DeviceAdSetVM
    {
        ///// <summary>
        ///// Related to Status
        ///// </summary>
        //public enum FlagEnum
        //{
        //    phone = 0,
        //    pad = 1,
        //    pc = 2
        //};

        //public static string GetFlagName(FlagEnum flag)
        //{
        //    return Enum.Parse(typeof(FlagEnum), flag.ToString()).ToString();
        //}


        /// <summary>
        /// 系統流水編號
        /// IDENTITY(1000,1) NOT NULL
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 子層時是:標題文字
        /// 有父層時才會有標題文字
        /// </summary>
        public string SubName { get; set; }

        ///// <summary>
        ///// 值是NULL=沒有父親, 有值時父親是ID
        ///// </summary>
        //public int Parent { get; set; }

        ///// <summary>
        ///// phone=手機, pad=平版, pc=桌機
        ///// </summary>
        //public string Flag { get; set; }

        ///// <summary>
        ///// 顯示順序
        ///// </summary>
        //public int Showorder { get; set; }

        ///// <summary>
        ///// 創建者
        ///// </summary>
        //public string CreateUser { get; set; }

        ///// <summary>
        ///// 創建日期
        ///// </summary>
        //public DateTime CreateDate { get; set; }

        ///// <summary>
        ///// 最後修改者
        ///// </summary>
        //public string UpdateUser { get; set; }

        ///// <summary>
        ///// 最後修改日期
        ///// </summary>
        //public DateTime? UpdateDate { get; set; }

    }
}
