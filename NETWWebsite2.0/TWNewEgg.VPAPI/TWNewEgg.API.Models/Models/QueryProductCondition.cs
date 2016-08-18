using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.API.Models
{
    public class QueryProductCondition
    {
        public int ProductID { get; set; }
        public string SellerProductID { get; set; }
        public string Keyword { get; set; }
        public int SellerID { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Skip { get; set; }
        public int PageSize { get; set; }
    }
}
