using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWSQLDB
{
    [Table("edmbook")]
    public class EDMBook
    {
        public enum usedAutoSend
        {
            不使用 = 0,
            全會員寄信 = 1,
            部分會員寄信 = 2
        }

        public enum eDMDefault
        {
            前台不顯示 = 0,
            前台顯示 = 1
        }

        public EDMBook()
        {
            //this.AdvTypeCode = 0; // AdvEventType Table 中的 AdvTypeCode
            this.EDMName = string.Empty; // EDM顯示名稱
            this.ViewName = string.Empty; // 使用的View的Name
            this.RecipientsList = string.Empty;
            this.OpenLetter = 0;
            this.RepeatOpenLetter = 0;
            this.AllMembers = 0;
            this.EDMDisplay = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int AdvTypeCode { get; set; } // AdvEventType Table 中的 AdvTypeCode
        public string EDMName { get; set; } // EDM顯示名稱
        public Nullable<DateTime> StartDate { get; set; } // EDM起始時間
        public Nullable<DateTime> EndDate { get; set; } // EDM結束時間
        public string ViewName { get; set; }
        public string HtmlContext { get; set; }
        public string LettersRecord { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateUser { get; set; }
        public int Updated { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string TrackingCode { get; set; }
        public int LandingFromEdm { get; set; }
        public int OpenLetter { get; set; }
        public int RepeatOpenLetter { get; set; }
        public int AllMembers { get; set; }
        public string RecipientsList { get; set; }
        public Nullable<DateTime> AutoSendDate { get; set; }
        public int EDMDisplay { get; set; }
    }
}
