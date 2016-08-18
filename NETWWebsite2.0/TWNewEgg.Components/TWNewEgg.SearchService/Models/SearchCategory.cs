using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.SearchService.Models
{
    public class SearchCategory
    {
        public string categoryID { get; set; }
        public string categoryName { get; set; }
        public string categoryDescr { get; set; }
        public string layer { get; set; }
        public int categoryShowOrder { get; set; }
        public int number { get; set; }
        public List<SearchCategory> parentCategory { get; set; }
    }
}
