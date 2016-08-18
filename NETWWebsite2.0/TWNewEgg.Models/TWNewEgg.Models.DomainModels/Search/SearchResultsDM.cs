using System;
using System.Collections.Generic;
using System.Linq;

namespace TWNewEgg.Models.DomainModels.Search
{
    public class SearchResults
    {
        public List<ItemSearchDM> searchResults { get; set; }
        public Dictionary<string, SearchCategoryDM> hotCategory { get; set; }
        public List<SearchCategoryDM> searchResultCategory { get; set; }
        public Dictionary<string, List<int>> searchResultsPrice { get; set; }
        public int resultCount { get; set; }
    }
}
