using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TWNewEgg.Models.ViewModels.Search;
using TWNewEgg.ECWeb.PrivilegeFilters;
using TWNewEgg.Models.DomainModels.Search;
using TWNewEgg.Framework.ServiceApi;

namespace TWNewEgg.ECWeb.Controllers
{
    [AllowNonSecures]
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly int SearchPageSize = 36;
        //
        // GET: /Search/

        public ActionResult Index(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats = "")
        {
            //SearchConditionDM condition = new SearchConditionDM();
            if (!string.IsNullOrEmpty(searchword))
            {
                searchword = searchword.Trim();
            }
            ViewBag.KeyWords = searchword;
            ViewBag.searchUrl = SettingSearchUrl(searchword, srchin, order, cat, lid, cty, bid, sid, minprice, maxprice, pagesize, page, mode, submit, orderCats);
            return View();
        }

        public ActionResult filter(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats = "")
        {
            SearchConditionDM condition = new SearchConditionDM();
            if (!string.IsNullOrEmpty(searchword))
            {
                searchword = searchword.Trim();
            }
            condition = SettingCondition(searchword, srchin, order, cat, lid, cty, bid, sid, minprice, maxprice, pagesize, page, mode, submit, orderCats);

            Api.SearchController searchService = new Api.SearchController();
            SearchPageView results = searchService.Get(condition);
            ViewBag.KeyWords = searchword;
            return PartialView("Partial_SearchFilter", results);
        }

        public ActionResult result(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats = "")
        {
            SearchConditionDM condition = new SearchConditionDM();
            if (!string.IsNullOrEmpty(searchword))
            {
                searchword = searchword.Trim();
            }
            condition = SettingCondition(searchword, srchin, order, cat, lid, cty, bid, sid, minprice, maxprice, pagesize, page, mode, submit, orderCats);

            Api.SearchController searchService = new Api.SearchController();
            SearchPageView results = searchService.Get(condition);
            TWNewEgg.ECWeb.Services.Page.CalculationsPage CalculationsPage = new TWNewEgg.ECWeb.Services.Page.CalculationsPage();
            ViewBag.KeyWords = searchword;
            int totalPage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(results.resultCount) / condition.PageSize));
            ViewBag.TotalPage = totalPage;
            ViewBag.NowPage = condition.Page.Value;
            ViewBag.ShowingPageList = CalculationsPage.getShowPages(totalPage, condition.Page.Value, 3);
            return PartialView("Partial_SearchResult", results);
        }

        private string SettingSearchUrl(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats = "")
        {
            string searchPath = "/search/result?";

            if (string.IsNullOrEmpty(searchword))
            {
                searchword = null;
            }
            searchPath = string.Format("{0}searchword={1}", searchPath, HttpUtility.UrlEncode(searchword));

            if (string.IsNullOrEmpty(srchin))
            {
                srchin = null;
            }
            searchPath = string.Format("{0}&srchin={1}", searchPath, HttpUtility.UrlEncode(srchin));

            if (page == null || page < 0)
            {
                page = 0;
            }
            searchPath = string.Format("{0}&page={1}", searchPath, page.Value);

            if (pagesize == null || pagesize < 20 || pagesize > 1000)
            {
                pagesize = SearchPageSize;
            }
            searchPath = string.Format("{0}&pagesize={1}", searchPath, pagesize);

            if (order != null)
            {
                searchPath = string.Format("{0}&order={1}", searchPath, order.Value);
            }


            if (cat != null)
            {
                searchPath = string.Format("{0}&cat={1}", searchPath, cat.Value);
            }

            if (lid != null)
            {
                searchPath = string.Format("{0}&lid={1}", searchPath, lid.Value);
            }

            if (cty != null)
            {
                searchPath = string.Format("{0}&cty={1}", searchPath, cty.Value);
            }

            if (bid != null)
            {
                searchPath = string.Format("{0}&bid={1}", searchPath, bid.Value);
            }

            if (sid != null)
            {
                searchPath = string.Format("{0}&sid={1}", searchPath, sid.Value);
            }

            if (minprice != null)
            {
                searchPath = string.Format("{0}&minprice={1}", searchPath, minprice.Value);
            }

            if (maxprice != null)
            {
                searchPath = string.Format("{0}&maxprice={1}", searchPath, maxprice.Value);
            }

            if (!string.IsNullOrEmpty(mode))
            {
                searchPath = string.Format("{0}&mode={1}", searchPath, HttpUtility.UrlEncode(mode));
            }

            if (!string.IsNullOrEmpty(submit))
            {
                searchPath = string.Format("{0}&submit={1}", searchPath, HttpUtility.UrlEncode(submit));
            }

            if (!string.IsNullOrEmpty(orderCats))
            {
                searchPath = string.Format("{0}&orderCats={1}", searchPath, HttpUtility.UrlEncode(orderCats));
            }

            return searchPath;
        }

        private SearchConditionDM SettingCondition(string searchword, string srchin, int? order, int? cat, int? lid, int? cty, int? bid, int? sid, int? minprice, int? maxprice, int? pagesize, int? page, string mode, string submit, string orderCats)
        {
            SearchConditionDM condition = new SearchConditionDM();
            if (string.IsNullOrEmpty(searchword))
            {
                searchword = null;
            }
            condition.SearchWord = searchword;

            if (string.IsNullOrEmpty(srchin))
            {
                srchin = null;
            }
            condition.SrchIn = srchin;

            if (page == null || page < 0)
            {
                page = 0;
            }
            condition.Page = page.Value;

            if (pagesize == null || pagesize < 20 || pagesize > 1000)
            {
                pagesize = SearchPageSize;
            }
            condition.PageSize = pagesize.Value;

            if (order != null)
            {
                condition.Order = order.Value;
            }

            if (cat != null)
            {
                condition.Cat = cat.Value;
            }

            if (lid != null)
            {
                condition.LID = lid.Value;
            }

            if (cty != null)
            {
                condition.Cty = cty.Value;
            }

            if (bid != null)
            {
                condition.BID = bid.Value;
            }

            if (sid != null)
            {
                condition.SID = sid.Value;
            }

            if (minprice != null)
            {
                condition.minPrice = minprice.Value;
            }

            if (maxprice != null)
            {
                condition.maxPrice = maxprice.Value;
            }

            if (!string.IsNullOrEmpty(mode))
            {
                condition.Mode = mode;
            }

            if (!string.IsNullOrEmpty(submit))
            {
                condition.Submit = submit;
            }

            if (!string.IsNullOrEmpty(orderCats))
            {
                condition.orderCats = orderCats;
            }

            return condition;
        }
    }
}
