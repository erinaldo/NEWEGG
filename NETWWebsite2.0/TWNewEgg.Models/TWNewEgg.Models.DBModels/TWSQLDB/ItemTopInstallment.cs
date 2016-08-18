using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 賣場最高分期期數
    /// </summary>
    [Table("ItemTopInstallment")]
    public class ItemTopInstallment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 項目編號
        /// </summary>
        /// <remarks>此欄位在新增新的一筆資料時值才會 +1，使用時與 SerialNumber 對照使用</remarks>
        public int Edition { get; set; }

        /// <summary>
        /// 賣場編號
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 最高分期期數
        /// </summary>
        public int TopInstallment { get; set; }

        /// <summary>
        /// 開始時間(同Item不可重複區間)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 結束時間(同Item不可重複區間)
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum ItemTopInstallmentStatus</remarks>
        public int Status { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        /// <remarks>0為最新，同樣的 ID 被更新時，舊的資料序號全部自動+1</remarks>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 建立人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateUser { get; set; }
    }

    /// <summary>
    /// 賣場最高分期期數狀態
    /// </summary>
    public enum ItemTopInstallmentStatus
    {
        // 關閉
        Disable = 0,

        // 啟用
        Enable = 1
    }
}
