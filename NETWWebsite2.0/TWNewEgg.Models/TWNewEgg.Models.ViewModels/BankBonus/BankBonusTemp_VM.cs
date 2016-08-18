﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.BankBonus
{
    public class BankBonusTemp_VM
    {

        public enum enStatus
        {
            上架 = 0,
            下架 = 1,
        };

        public int ID { get; set; }

        /// <summary>
        /// 用BankCode 去查table Bank
        /// </summary>
        public int BankID { get; set; }

        /// <summary>
        /// FK 等於審核通過的ID
        /// </summary>
        public int? BankBonusID { get; set; }

        /// <summary>
        /// Check Box
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 銀行代碼
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string PhotoName { get; set; }

        /// <summary>
        /// 上下架狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 發卡銀行名稱
        /// </summary>
        public string PublishBank { get; set; }

        /// <summary>
        /// 發卡銀行的客服連絡電話
        /// </summary>
        public string PublishBankPhone { get; set; }

        /// <summary>
        /// 最高折抵上限
        /// </summary>
        public decimal? OffsetMax { get; set; }

        /// <summary>
        /// 消費限制
        /// </summary>
        public decimal? ConsumeLimit { get; set; }

        /// <summary>
        /// 點數限制
        /// </summary>
        public decimal? PointLimit { get; set; }

        /// <summary>
        /// 折抵比例點
        /// </summary>
        public decimal? ProportionPoint { get; set; }

        /// <summary>
        ///折抵比例元
        /// </summary>
        public decimal? ProportionMoney { get; set; }

        /// <summary>
        /// 用在審核通過，永遠顯示資料為 0的 資料
        /// 舊的全部+1，建立一筆新的SerialNumberSerialNumber為0。
        /// 記錄舊的資料，不可以蓋掉舊的審核資料
        /// </summary>
        public int? SerialNumber { get; set; }

        /// <summary>
        /// 建立的時間
        /// </summary>
        public System.DateTime CreateDate { get; set; }

        /// <summary>
        /// 建立的使用者
        /// </summary>
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

        /// <summary>
        /// Verification InputData
        /// </summary>
        public string strNullErrorMsg { get; set; }
        public string strTpyeErrorMsg { get; set; }
        public string strLessZeroErrorMsg { get; set; }
        public string strDBOverflowErrorMsg { get; set; }
        public string strEndLessStartDateErrorMsg { get; set; }
        public string strBankNotExist { get; set; }
    }
}
