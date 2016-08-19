using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNewEgg.ItemServices.Interface;
using TWNewEgg.SearchServices.Interface;
using TWNewEgg.Framework.HttpMethod;
using TWNewEgg.SearchServices.Model;

namespace TWNewEgg.SearchServices
{
    public class SolrService : CommonService, ISearchService
    {
        private string filterQuery = "{0}"; //0: {SrchIn}
        private string queryString = "{0}{1}{2}"; //0: {ItemName}, 1: {CategoryID}, 2: {PriceCash}
        private string ItemNameQuery = "ItemName:({0})";
        private string CategoryIDQuery = "{0}CategoryID:({1})";
        private string PriceCashQuery = "{0}PriceCash:[{1} TO {2}]";
        private string defTypeString = "edismax";
        //private string bfQueryString = "product(map(SellingQty,1,9999999,1),100)";
        private string bfQueryString = "product(map(SellingQty,1,9999999,1),100){0}";

        public SolrService(IItemDetailService itemDetailService) 
            : base(itemDetailService)
        {

        }
        public SearchResults TotalSearch(TWNewEgg.Models.DomainModels.Search.SearchConditionDM condition)
        {
            SearchResults returnResults = new SearchResults();
            List<ItemSearch> searchResults;
            searchResults = GetAllSimpleSearchResult(condition);
            returnResults.resultCount = searchResults.Count;

            List<SearchCategory> categoryNumber = new List<SearchCategory>(); //for saved all item's category
            Dictionary<string, SearchCategory> hotCategory = new Dictionary<string, SearchCategory>();//for saved upper level category
            if (searchResults.Count > 0)
            {
                categoryNumber = GetCategoryNumber(searchResults, condition.Cat, condition.LID, condition.Cty, condition.BID, condition.SID); //get all items' cateogry
                categoryNumber.RemoveAll(x => x.categoryDescr == "None" && x.categoryName == "None" && x.categoryShowOrder == 999);
                categoryNumber = categoryNumber.OrderBy(x => x.categoryShowOrder).ThenByDescending(x => x.number).ToList();
            }
            hotCategory = GetHotCategoryNumber(categoryNumber, condition.LID); //combine all results category to tree struct
            //ViewBag.hotCategory = hotCategory;
            if (categoryNumber.Count > 0 && categoryNumber.Count < 2) //if search results items's category only one, then check search condotion, if there have no condition then show it, if yes then clear it, coz don't need it anymore
            {
                if (condition.Cat != null && condition.LID != null && categoryNumber[0].categoryID == condition.Cat.Value.ToString() && categoryNumber[0].layer == condition.LID.Value.ToString())
                {
                    categoryNumber.Clear();
                }
            }

            Dictionary<string, List<int>> searchPrice = new Dictionary<string, List<int>>(); //First int is number, second int is low price, third int is high price
            if (searchResults.Count > 0)
            {
                decimal maximunPrice = searchResults.Max(x => x.Pricecash).Value;
                searchPrice = CountNumberPrice(maximunPrice, searchResults);
            }

            searchResults = GetPageSearchResult(condition);

            returnResults.searchResults = searchResults;
            returnResults.hotCategory = hotCategory;
            returnResults.searchResultCategory = categoryNumber;
            returnResults.searchResultsPrice = searchPrice;
            return returnResults;
        }

        public List<ItemSearch> GetAllSimpleSearchResult(TWNewEgg.Models.DomainModels.Search.SearchConditionDM condition)
        {
            List<ItemSearch> searchResults = new List<ItemSearch>();
            SolrRequestModel requestModel = new SolrRequestModel();
            SettingParameters requestParameters = new SettingParameters();
            
            if (!string.IsNullOrEmpty(condition.SrchIn))
            {
                filterQuery = string.Format(filterQuery, string.Format(ItemNameQuery, ReplaceSolrSearchSyntax(condition.SrchIn)));
            }
            else
            {
                filterQuery = string.Format(filterQuery, string.Empty);
            }

            if (!string.IsNullOrEmpty(condition.SearchWord))
            {
                queryString = string.Format(queryString, string.Format(ItemNameQuery, ReplaceSolrSearchSyntax(condition.SearchWord)), GetQueryPriceCashRange(condition), GetQueryCategoryID(condition));
            }

            requestParameters.wt = "json";
            requestParameters.start = 0;
            requestParameters.rows = 100000;
            requestParameters.fl = "CategoryID Layer PriceCash";
            requestParameters.q = queryString;
            requestParameters.fq = filterQuery;
            requestParameters.sort = GetSortString(condition.Order);
            requestModel.@params = requestParameters;

            searchResults = GetSearchResult(requestModel);

            return searchResults;
        }

        public List<ItemSearch> GetPageSearchResult(TWNewEgg.Models.DomainModels.Search.SearchConditionDM condition)
        {
            List<ItemSearch> searchResults = new List<ItemSearch>();
            SolrRequestModel requestModel = new SolrRequestModel();
            SettingParameters requestParameters = new SettingParameters();
            int startRows = 0;

            if (!string.IsNullOrEmpty(condition.SrchIn))
            {
                filterQuery = string.Format(filterQuery, string.Format(ItemNameQuery, ReplaceSolrSearchSyntax(condition.SrchIn)));
            }
            else
            {
                filterQuery = string.Format(filterQuery, string.Empty);
            }

            if (!string.IsNullOrEmpty(condition.SearchWord))
            {
                queryString = string.Format(queryString, string.Format(ItemNameQuery, ReplaceSolrSearchSyntax(condition.SearchWord)), GetQueryPriceCashRange(condition), GetQueryCategoryID(condition));
            }

            if (condition.Page == null)
            {
                condition.Page = 0;
            }
            condition.Page += 1;
            startRows = condition.PageSize * (condition.Page.Value - 1);

            requestParameters.wt = "json";
            requestParameters.start = startRows;
            requestParameters.rows = condition.PageSize;
            requestParameters.fl = string.Empty;
            requestParameters.q = queryString;
            requestParameters.fq = filterQuery;
            requestParameters.sort = GetSortString(condition.Order);
            requestModel.@params = requestParameters;

            searchResults = GetSearchResult(requestModel);

            return searchResults;
        }

        public List<ItemSearch> GetSearchResult(SolrRequestModel condition)
        {
            if (!string.IsNullOrEmpty(condition.@params.q))
            {
                condition.@params.bf = bfQueryString;
                condition.@params.defType = defTypeString;
            }
            List<ItemSearch> searchResults = new List<ItemSearch>();
            string solrAddress = System.Configuration.ConfigurationManager.AppSettings["SolrServerAddress"] ?? "";
            string solrQueryPath = System.Configuration.ConfigurationManager.AppSettings["SolrQueryPath"] ?? "";
            if (string.IsNullOrEmpty(solrAddress) || string.IsNullOrEmpty(solrQueryPath))
            {
                throw new Exception("Solr Server Info didn't setting in appsetting.config.");
            }
            
            string solrResults = HttpClientMethod.Post(
                solrAddress,
                solrQueryPath,
                TWNewEgg.Framework.Common.JSONSerialization.Serializer(condition),
                null);
            SolrResponseFullModel queryResult = new SolrResponseFullModel();

            try
            {
                int resultLength = solrResults.Length;
            }
            catch (Exception e)
            {
                throw new Exception("Too many records.");
            }

            queryResult = TWNewEgg.Framework.Common.JSONSerialization.Deserializer<SolrResponseFullModel>(solrResults);

            if (queryResult != null)
            {
                searchResults = queryResult.response.docs;
            }

            return searchResults;
        }

        private string GetQueryCategoryID(TWNewEgg.Models.DomainModels.Search.SearchConditionDM condition)
        {
            string queryCategory = string.Empty;
            if (!string.IsNullOrEmpty(condition.orderCats))
            {
                List<int> allChildCategories = GetChildCategory(condition.orderCats);
                if (allChildCategories.Count > 0)
                {
                    queryCategory = string.Format(CategoryIDQuery , " AND ", string.Join(" OR ", allChildCategories.ToArray()));
                }
            }
            return queryCategory;
        }

        private string GetQueryPriceCashRange(TWNewEgg.Models.DomainModels.Search.SearchConditionDM condition)
        {
            if (condition.minPrice == null || condition.maxPrice == null)
            {
                return string.Empty;
            }
            if (condition.minPrice >= condition.maxPrice)
            {
                return string.Empty;
            }
            return string.Format(PriceCashQuery, " AND ", condition.minPrice.Value.ToString(), condition.maxPrice.Value.ToString());
        }

        private string GetSortString(int? order)
        {
            string orderString = string.Empty;
            switch (order)
            {
                case (int)OrderCondition.PriceLow2High:
                    //價格由低到高
                    //orderString = "PriceCash asc";
                    bfQueryString = string.Format(bfQueryString, " rord(PriceCash)");
                    break;
                case (int)OrderCondition.PriceHigh2Low:
                    //價格由高到低
                    //orderString = "PriceCash desc";
                    bfQueryString = string.Format(bfQueryString, " ord(PriceCash)");
                    break;
                case (int)OrderCondition.MostRelate:
                    bfQueryString = string.Format(bfQueryString, "");
                    break;
                case (int)OrderCondition.MostPolular:
                    //最有人氣，參考方式(銷售量?)
                    //orderString = "QtyReg desc";
                    bfQueryString = string.Format(bfQueryString, " ord(QtyReg)");
                    break;
                case (int)OrderCondition.StockHigh2Low:
                    //庫存量從多到少，參考方式(銷售量?)
                    //orderString = "SellingQty desc";
                    bfQueryString = string.Format(bfQueryString, " ord(SellingQty)");
                    break;
                case (int)OrderCondition.CreateNew2Old:
                    //新到舊
                    //orderString = "UpdateDate desc";
                    bfQueryString = string.Format(bfQueryString, " ord(UpdateDate)");
                    break;
                case (int)OrderCondition.CreateOld2New:
                    //舊到新
                    //orderString = "UpdateDate asc";
                    bfQueryString = string.Format(bfQueryString, " rord(UpdateDate)");
                    break;
                default:
                    bfQueryString = string.Format(bfQueryString, "");
                    break;
            }
            return orderString;
        }

        private string ReplaceSolrSearchSyntax(string searchWords)
        {
            string results = string.Empty;
            char[] replaceSyntax = {'+', '-', '!', '(', ')', '{', '}', '[', ']', '^', '\"', '~', '*', '?', ':', '\\', '/'};
            string[] replaceDoubleSyntax = {"&&", "||"};
            foreach (var singleWord in searchWords)
            {
                bool replaceFlag = false;
                foreach (var singleSyntax in replaceSyntax)
                {
                    if (singleWord == singleSyntax)
                    {
                        replaceFlag = true;
                        break;
                    }
                    else
                    {
                        replaceFlag = false;
                    }
                }
                if (replaceFlag)
                {
                    results += "\\" + singleWord;
                }
                else
                {
                    results += singleWord;
                }
            }
            foreach (var doubleSyntax in replaceDoubleSyntax)
            {
                results = results.Replace(doubleSyntax, "\\" + doubleSyntax);
            }
            return results;
        }
    }
}
