using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.SearchServices.Model;

namespace TWNewEgg.SearchServices.Interface
{
    public interface ISearchService
    {
        SearchResults TotalSearch(TWNewEgg.Models.DomainModels.Search.SearchConditionDM condition);
    }
}
