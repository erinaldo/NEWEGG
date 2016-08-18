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
    /// 銀行與分期對照表
    /// </summary>
    [Table("BankInstallment")]
    public class BankInstallment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// 銀行代碼
        /// </summary>
        public int BankCode { get; set; }

        /// <summary>
        /// 分期期數ID
        /// </summary>
        public int InstallmentID { get; set; }

        /// <summary>
        /// PayType表的ID
        /// </summary>
        public int PayTypeID { get; set; }

        /// <summary>
        /// 狀態
        /// </summary>
        /// <remarks>enum BankInstallmentStatus</remarks>
        public int Status { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        /// <remarks>0為最新，同樣的 ItemID 被更新時，舊的資料序號全部自動+1</remarks>
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
    /// 銀行與分期對照表狀態
    /// </summary>
    public enum BankInstallmentStatus
    { 
        // 關閉
        Disable = 0,

        // 啟用
        Enable = 1
    }
}
