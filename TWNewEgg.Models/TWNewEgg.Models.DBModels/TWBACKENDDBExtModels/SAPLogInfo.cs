using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWBACKENDDB;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.Models.DBModels.TWBACKENDDBExtModels
{
    public class SAPLogInfo
    {
        public enum LogTypeEnum
        {
            Fail = 0,
            Sussess = 1,
            ALL = 2
        }

        public string DocType { get; set; }
        public string DocTypeDesc { get; set; }
        public FinanceDocumentCreateNote FinanceDocumentCreateNote { get; set; }
        public FinDocTransLog FinDocTransLog { get; set; }

        public string SellerIsCheck { get; set; }
    }
}
