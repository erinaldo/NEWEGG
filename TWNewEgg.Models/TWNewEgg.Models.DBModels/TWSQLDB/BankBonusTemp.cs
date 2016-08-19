using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    /// <summary>
    /// 折抵紅利，待審核
    /// </summary>
    [Table("BankBonusTemp")]
    public class BankBonusTemp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// FK 等於審核通過的ID
        /// </summary>
        public int? BankBonusID { get; set; }

        /// <summary>
        /// Bank 表的銀行編號
        /// </summary>
        public int BankID { get; set; }

        /// <summary>
        /// 銀行代碼
        /// </summary>
        [Required]
        public string BankCode { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        [Required]
        public string PhotoName { get; set; }

        /// <summary>
        /// 上下架狀態
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// 發卡銀行名稱
        /// </summary>
        [Required]
        public string PublishBank { get; set; }

        /// <summary>
        /// 發卡銀行的客服連絡電話
        /// </summary>
        [Required]
        public string PublishBankPhone { get; set; }

        /// <summary>
        /// 最高折抵上限
        /// </summary>
        [Required]
        public decimal OffsetMax { get; set; }

        /// <summary>
        /// 消費限制
        /// </summary>
        [Required]
        public decimal ConsumeLimit { get; set; }

        /// <summary>
        /// 點數限制
        /// </summary>
        [Required]
        public decimal PointLimit { get; set; }

        /// <summary>
        /// 折抵比例點
        /// </summary>
        [Required]
        public decimal ProportionPoint { get; set; }

        /// <summary>
        ///折抵比例元
        /// </summary>
        [Required]
        public decimal ProportionMoney { get; set; }

        /// <summary>
        /// 用在審核通過，永遠顯示資料為 0的 資料
        /// 舊的全部+1，建立一筆新的SerialNumberSerialNumber為0。
        /// 記錄舊的資料，不可以蓋掉舊的審核資料
        /// </summary>
        public int? SerialNumber { get; set; }

        /// <summary>
        /// 建立的時間
        /// </summary>
        [Required]
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立的使用者
        /// </summary>
        [Required]
        public string CreateUser { get; set; }

        /// <summary>
        /// 更新次數
        /// </summary>
        public int? Updated { get; set; }

        /// <summary>
        /// 更新的使用者
        /// </summary>
        public string UpdateUser { get; set; }

        /// <summary>
        /// 更新的日期
        /// </summary>
        public Nullable<System.DateTime> UpdateDate { get; set; }

        /// <summary>
        /// 審核的使用者
        /// </summary>
        public string AuditUser { get; set; }
        
        /// <summary>
        /// 審核的日期
        /// </summary>
        public Nullable<System.DateTime> AuditDate { get; set; }
        
        /// <summary>
        /// 規則說明
        /// </summary>
        public string DescriptionFormat { get; set; }
    }
}
