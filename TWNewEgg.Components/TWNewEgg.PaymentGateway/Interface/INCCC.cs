using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.PaymentGateway;

namespace TWNewEgg.PaymentGateway.Interface
{
    public interface INCCC
    {
        string Pay(NCCCInput inputData);
        NCCCResult CheckPayResultByOrderId(string orderGroupId);
        NCCCResult CheckPayResultByKey(string key);
    }
}
