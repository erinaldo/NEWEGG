using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.EDM
{
    public class EDMBookDM
    {
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
