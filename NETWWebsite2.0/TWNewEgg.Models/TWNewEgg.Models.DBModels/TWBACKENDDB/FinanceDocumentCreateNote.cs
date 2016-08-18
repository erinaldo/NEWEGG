using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("FinanceDocumentCreateNote")]
    public class FinanceDocumentCreateNote
    {
        [Key]
        public string SalesOrderCode { get; set; }
        [Key]
        public int AccDocTypeCode { get; set; }
        public int SalesOrderGroupID { get; set; }
        public string TransactionID { get; set; }
        public string DocNumber { get; set; }

        /// <summary>
        /// 訂單類型(1=TWNewegg,2=Newegg)
        /// </summary>
        public int SalesOrderType { get; set; }

        /// <summary>
        /// 重置旗標(0=不處理,1=重新處理)
        /// </summary>
        public string ReprocessingFlag { get; set; }
        public Nullable<DateTime> CreateDate { get; set; }
    }
}
