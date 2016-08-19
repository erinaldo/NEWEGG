using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.Models.ViewModels.Search
{
    public class SearchCategoryVM
    {
        public string categoryID { get; set; }
        public string categoryName { get; set; }
        public string categoryDescr { get; set; }
        public string layer { get; set; }
        public int categoryShowOrder { get; set; }
        public int number { get; set; }
        public List<SearchCategoryVM> parentCategory { get; set; }
    }
}
