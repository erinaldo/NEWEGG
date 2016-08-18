using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Finance
{
    public class FinDocTransLogDM
    {
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
