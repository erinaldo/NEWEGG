using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SearchService.Models
{
    public class DealApiModel    /////////////////////////////////////////////////this model lack country ID. by Bill 2014/03/19
    {
        public int page { get; set; }
        public int showNumber { get; set; }
        public int showAll { get; set; }
        public int showZero { get; set; }
        public List<int> brandIds { get; set; }
        public List<int> categoryIds { get; set; }
        public string orderByType { get; set; }
        public string orderBy { get; set; }
        public decimal priceCash { get; set; }
    }
}
