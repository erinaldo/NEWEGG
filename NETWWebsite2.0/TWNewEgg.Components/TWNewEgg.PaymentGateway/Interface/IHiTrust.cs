using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DomainModels.PaymentGateway;

namespace TWNewEgg.PaymentGateway.Interface
{
    public interface IHiTrust
    {
        HiTrustQueryInput HiTrustQueryInData(string OrderNumber);
        string Pay(HiTrustInput inputData);
        HiTrustQueryData CheckPayResult(int Id);
    }
}
