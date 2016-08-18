using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.HotWords
{
    public class HotWords
    {
        /// <summary>
        /// 系統流水編號
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 顯示順序
        /// </summary>
        public int Showorder { get; set; }

        /// <summary>
        /// 是否顯示, 1:顯示, 0:不顯示
        /// </summary>
        public int ShowAll { get; set; }

        /// <summary>
        /// 點選連結
        /// </summary>
        public string Clickpath { get; set; }

        /// <summary>
        /// 分類Id
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { get; set; }

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
