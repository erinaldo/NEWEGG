using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class QueryFunctionCondition
    {
        //public List<int> FunctionPointGroupIDs { get; set; }

        public int? SellerID { get; set; }

        public int? UserID { get; set; }

        public int? GroupID { get; set; }

        public string Language { get; set; }
    }
}
