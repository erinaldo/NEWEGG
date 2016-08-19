using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.SearchService.Models
{
    public class SearchResults
    {
        public List<TWNewEgg.DB.TWSQLDB.Models.ItemSearch> searchResults { get; set; }
        public Dictionary<string, SearchCategory> hotCategory { get; set; }
        public List<SearchCategory> searchResultCategory { get; set; }
        public Dictionary<string, List<int>> searchResultsPrice { get; set; }
        public int resultCount { get; set; }
    }
}
