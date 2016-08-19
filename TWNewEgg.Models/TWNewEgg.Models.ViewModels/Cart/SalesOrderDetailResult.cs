using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Cart
{
    public class SalesOrderDetailResult
    {
        public string State { get; set; }
        public CashFlowResult PaymentResult { get; set; }
        public string SystemMessage { get; set; }
        public int SOGroupId { get; set; }
    }
}
