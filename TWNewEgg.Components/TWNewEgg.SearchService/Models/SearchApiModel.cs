using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SearchService.Models
{
    public class SearchApiModel
    {
        public string SearchWord { get; set; }
        public string SrchIn { get; set; }
        public Nullable<int> Order { get; set; }
        public Nullable<int> Cat { get; set; }
        public Nullable<int> LID { get; set; }
        public Nullable<int> Cty { get; set; }
        public Nullable<int> BID { get; set; }
        public Nullable<int> SID { get; set; }
        public Nullable<int> minPrice { get; set; }
        public Nullable<int> maxPrice { get; set; }
        public int PageSize { get; set; }
        public Nullable<int> Page { get; set; }
        public string Mode { get; set; }
        public string Submit { get; set; }
        public string orderCats { get; set; }
    }
}
