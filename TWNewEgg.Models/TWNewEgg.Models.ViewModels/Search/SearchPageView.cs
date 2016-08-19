using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Search
{
    public class SearchPageView
    {
        public enum OrderCondition
        {
            PriceHigh2Low = 0,
            PriceLow2High = 1,
            MostRelate = 2,
            MostPolular = 3,
            StockHigh2Low = 4
        };

        public List<ItemSearchVM> searchResults { get; set; }
        public Dictionary<string, SearchCategoryVM> hotCategory { get; set; }
        public List<SearchCategoryVM> searchResultCategory { get; set; }
        public Dictionary<string, List<int>> searchResultsPrice { get; set; }
        public int resultCount { get; set; }
    }
}
