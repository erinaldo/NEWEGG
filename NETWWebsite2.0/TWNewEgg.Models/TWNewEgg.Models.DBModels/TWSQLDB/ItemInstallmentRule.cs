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
    /// 利率分期預設表
    /// </summary>
    [Table("ItemInstallmentRule")]
    public class ItemInstallmentRule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 信用卡分期 ID
        /// </summary>
        public int InstallmentID { get; set; }

        /// <summary>
        /// 毛利
        /// </summary>
        /// <remarks>商品毛利率-分期利率</remarks>
        public decimal Rate { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum ItemInstallmentRuleStatus</remarks>
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
    /// 利率分期預設表狀態
    /// </summary>
    public enum ItemInstallmentRuleStatus
    {
        // 關閉
        Disable = 0,

        // 啟用
        Enable = 1
    }
}
