using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.DataMaintain
{
    public class DataMaintainSearchCondition_DM
    {
        public enum SearchTypeDescription
        {
            CartID = 1,
            RetgoodID = 2,
            Refund2C = 3,
        }

        public int SearchType { get; set; }
        public string SearchKey { get; set; }
    }
}
