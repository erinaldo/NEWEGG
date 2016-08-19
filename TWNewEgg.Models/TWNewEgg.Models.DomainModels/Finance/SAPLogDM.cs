using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Finance
{
    public class SAPLogDM
    {
        public enum LogTypeEnum
        {
            Fail = 0,
            Sussess = 1,
            ALL = 2
        }

        public string DocType { get; set; }
        public string DocTypeDesc { get; set; }
        public FinanceDocumentCreateNoteDM FinanceDocumentCreateNote { get; set; }
        public FinDocTransLogDM FinDocTransLog { get; set; }

        public string SellerIsCheck { get; set; }
    }
}
