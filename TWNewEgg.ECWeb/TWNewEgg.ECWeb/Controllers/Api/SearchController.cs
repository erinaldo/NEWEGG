using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TWNewEgg.ECWeb.PrivilegeFilters.Api;
using TWNewEgg.ECWeb.Utility;
using TWNewEgg.Framework.ServiceApi;
using TWNewEgg.Models.DomainModels.Item;
using TWNewEgg.Models.DomainModels.Search;
using TWNewEgg.Models.ViewModels.Item;
using TWNewEgg.Models.ViewModels.Search;

namespace TWNewEgg.ECWeb.Controllers.Api
{
    public class SearchController : ApiController
    {
        // GET api/search/5
        [AllowNonSecures]
        [AllowAnonymous]
        public SearchPageView Get(SearchConditionDM condition)
        {
            var results = Processor.Request<SearchPageView, SearchResults>("SolrService", "TotalSearch", condition);
            if (string.IsNullOrEmpty(results.error))
            {
                //results.results.resultCount = results.results.searchResults.Count;
                if (results.results.resultCount > 0)
                {
                    Dictionary<int, List<ItemUrl>> itemUrlDictionary = new Dictionary<int, List<ItemUrl>>();
                    List<int> itemIDs = results.results.searchResults.Select(x => x.ID).ToList();
                    if (itemIDs.Count < 10000 && itemIDs.Count > 0)
                    {
                        itemUrlDictionary = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", itemIDs).results;
                    }
                    else if (itemIDs.Count >= 10000)
                    {
                        for (int i = 0; i < (itemIDs.Count / 500); i++)
                        {
                            var itemUrlResults = Processor.Request<Dictionary<int, List<ItemUrl>>, Dictionary<int, List<ImageUrlReferenceDM>>>("ItemImageUrlService", "GetItemImagePath", itemIDs.Skip(i * 500).Take(500));
                            if (string.IsNullOrEmpty(itemUrlResults.error))
                            {
                                foreach (var singleDiction in itemUrlResults.results)
                                {
                                    itemUrlDictionary.Add(singleDiction.Key, singleDiction.Value);
                                }
                            }
                        }
                    }
                    for (int i = 0; i < results.results.searchResults.Count; i++)
                    {
                        if (!itemUrlDictionary.ContainsKey(results.results.searchResults[i].ID))
                        {
                            continue;
                        }
                        if (itemUrlDictionary[results.results.searchResults[i].ID].Where(x => x.Size == 300).Count() > 0)
                        {
                            var singleImgUrl = itemUrlDictionary[results.results.searchResults[i].ID].Where(x => x.Size == 300).FirstOrDefault();
                            if (singleImgUrl == null)
                            {
                                continue;
                            }
                            if (singleImgUrl.ImageUrl.IndexOf("newegg.com/") >= 0)
                            {
                                results.results.searchResults[i].PhotoName = singleImgUrl.ImageUrl;
                            }
                            else
                            {
                                results.results.searchResults[i].PhotoName = ImageUtility.GetImagePath(singleImgUrl.ImageUrl);
                            }
                        }
                    }
                }
                return results.results;
            }
            return null;
        }
    }
}
