using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.Store;

namespace TWNewEgg.Models.DomainModels.DeviceAd
{

    /// <summary>
    /// 依據 BSATW-177 手機改版需求增加---------------add by bruce 20160606
    /// 行動設備的廣告內文
    /// </summary>
    public class DeviceAdContentDM
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
        /// 廣告位置
        /// 是屬於哪一個行動設備的廣告設定, DeviceAdSet.ID
        /// </summary>
        public int DeviceAdSetID { get; set; }

        /// <summary>
        /// 名稱, 輪播時叫:標題．生活提案時叫:大標題．美國直購時叫:說明．促案時叫:文字內容．全館分類叫:館別名稱．
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名稱2, 生活提案時叫:小標題
        /// </summary>
        public string Name2 { get; set; }


        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// del=已刪除了
        /// ---------------------------------------------add by bruce 20160623
        /// 若CategoryID有值這裡代表是屬於這個CategoryID的index
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 顯示順序
        /// </summary>
        public int Showorder { get; set; }

        /// <summary>
        /// 是否顯示, 顯示:show|1, 不顯示:hide|0
        /// </summary>
        public string ShowAll { get; set; }

        /// <summary>
        /// 要點選的連結
        /// </summary>
        public string Clickpath { get; set; }

        /// <summary>
        /// 圖片位置
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// ---------------------------------------------add by bruce 20160623
        /// 來自SubCategory_NormalStore與Category的ID
        /// </summary>
        public int CategoryID { get; set; }
        
        /// <summary>
        /// 創建者
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 創建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最後修改者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 最後修改日期
        /// </summary>
        public DateTime? UpdateDate { get; set; }

    }
}
