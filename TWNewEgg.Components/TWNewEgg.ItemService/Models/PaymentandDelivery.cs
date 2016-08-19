using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Item
{
    public class PaymentandDelivery
    {
        public Nullable<int> PayTypeID { get; set; }
        public int PayType0rateNum { get; set; }
        public int DeliverType { get; set; }
    }
}
