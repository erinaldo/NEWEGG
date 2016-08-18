using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TWNewEgg.Models.DBModels.TWBACKENDDB
{
    [Table("ChartOfAccountsProfile")]
    public class ChartOfAccountsProfile
    {
        [Key]
        public int AccDocTypeCode { get; set; }
        [Key]
        public int DeliverTypeCode { get; set; }
        [Key]
        public int Seq { get; set; }
        public int ItemNo { get; set; }
        public string AccNumber { get; set; }
        public string AccPattern { get; set; }
        public string SignFlag { get; set; }
        public string ProfitCtr { get; set; }
        public string UseFlag { get; set; }
        public Nullable<DateTime> UseDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
