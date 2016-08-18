using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TWNewEgg.DB.TWBACKENDDB.Models
{
    [Table("FinDocTransLog")]
    public class FinDocTransLog
    {
        public enum ActionTypeStatus
        {
            U = 0,
            R =1
        }

        public enum ResultTypeStatus
        {
            E = 0,
            S = 1
        }

        public FinDocTransLog() {
            this.ActionType = "U";
            this.ResultType = "S";
            this.CreateDate = DateTime.UtcNow.AddHours(8);
        }

        [Key]
        public int Index { get; set; }
        public string ActionType { get; set; }
        public string DocType { get; set; }
        public string DocNo { get; set; }
        public string FileName { get; set; }
        public string ResultType { get; set; }
        public string Reason { get; set; }
        public string TransactionNumber { get; set; }
        public string TransTime { get; set; }
        public DateTime CreateDate { get; set; }
    }

}