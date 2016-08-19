using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TWNewEgg.SearchServices.Model
{
    public class SearchResults
    {
        public List<ItemSearch> searchResults { get; set; }
        public Dictionary<string, SearchCategory> hotCategory { get; set; }
        public List<SearchCategory> searchResultCategory { get; set; }
        public Dictionary<string, List<int>> searchResultsPrice { get; set; }
        public int resultCount { get; set; }
    }
}
