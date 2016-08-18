using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.DataMaintain
{
    public class refund2cDataMaintain_DM
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int? Amount { get; set; }
        public string BankID { get; set; }
        public string BankName { get; set; }
        public string SubBankName { get; set; }
        public string AccountNO { get; set; }
        public string AccountName { get; set; }
        public int? Updated { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateUser { get; set; }
        public string CartID { get; set; }
        public string ProcessID { get; set; }
        public string UpdateNote { get; set; }
    }
}
