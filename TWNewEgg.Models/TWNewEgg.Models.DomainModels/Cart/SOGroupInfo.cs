using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Cart
{
    public class SOGroupInfo
    {
        public SOGroupInfo()
        {
            SalesOrders = new List<SOInfo>();
        }

        public enum SOGroupStatus
        {
            Initial= 0,
            NotPayed = 1,
            Payed = 2,
            Completed = 3,
            Failed = 4
        }

        public SOGroupBase Main { get; set; }
        public List<SOInfo> SalesOrders { get; set; }
        public SOGroupStatus Status { get; set; }
    }
}
