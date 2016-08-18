using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.Models.DBModels.TWSQLDB;

namespace TWNewegg.DelvTypePaymentTermRepoAdapters.Interface
{
    public interface IDelvTypePaymentTermRepoAdapter
    {
        List<string> GetlistIntersectPaymentTermID_PaymentTerm(List<int> listDelvType);
    }
}
