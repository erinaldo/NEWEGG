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
    /// 信用卡分期(此表僅記錄分期的「期數」)
    /// </summary>
    /// <remarks>此表要與 BankInstallment 對照使用</remarks>
    [Table("Installment")]
    public class Installment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 分期期數
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum InstallmentStatus</remarks>
        public int Status { get; set; }

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
    /// 信用卡分期狀態
    /// </summary>
    public enum InstallmentStatus
    {
        // 關閉
        Disable = 0,

        // 啟用
        Enable = 1
    }
}
