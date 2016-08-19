using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Answer
{
    public class AnswerInfo
    {

        public ProbelmBase Probelm { get; set; }
        public List<AnswerBase> AnswerList { get; set; }
        public AccountInfo Account { get; set;}
        public SalesOrderInfo SalesOrder { get; set; }
        public List<SalesOrderItemInfo> SalesOrderItem { get; set; }
    }
}
