using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Finance
{
    public class FinanceDocumentCreateNoteDM
    {
        public string SalesOrderCode { get; set; }
        public int AccDocTypeCode { get; set; }
        public int SalesOrderGroupID { get; set; }
        public string TransactionID { get; set; }
        public string DocNumber { get; set; }
        public int SalesOrderType { get; set; }
        public string ReprocessingFlag { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
    }
}
