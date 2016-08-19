using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewEgg.PaymentTermRepoAdapters.Interface
{
    public interface IPaymentTermRepoAdapters
    {
        bool Create(PaymentTerm argObjPaymentTerm);
        List<PaymentTerm> ReadAll();
        bool Update(PaymentTerm argObjPaymentTerm);
    }
}
