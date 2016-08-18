using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWNewEgg.DB.TWSQLDB.Models;
using TWNewEgg.DB.TWSQLDB.Models.ExtModels;
using TWNewEgg.SearchService.Models;

namespace TWNewEgg.SearchService.Service
{
    public class ApiSelectorRepository : IApiSelector
    {
        /*
        public bool FindApiName(string apiName)
        {
            bool flag = false;
            switch (apiName)
            {
                case "SearchApi":
                    flag = true;
                    break;
                default:
                    break;
            }
            return flag;
        }
        
        */
        public DealApiModel SetDealApiModel(string apiArgs)
        {
            DealApiModel dealArg = new DealApiModel();
            dealArg.brandIds = new List<int>();
            dealArg.categoryIds = new List<int>();
            var modelData = ParseUrl(apiArgs);


            //Type tt = typeof(SalesOrderItem);
            //Type rr = typeof(dealArg);
            //var propNames = modelData.Keys.ToList();
            //foreach (var property in rr.GetProperties())
            //{
            //    if (propNames.Contains(property.Name))
            //    {
            //        rr.GetProperty(property.Name).SetValue(this, modelData[property.Name]);
            //    }
            //}
            bool convertFlag = false;
            if (modelData.ContainsKey("page"))
            {
                int page = new int();
                convertFlag = int.TryParse(modelData["page"], out page);
                if (convertFlag)
                {
                    dealArg.page = page;
                }
                else
                {
                    dealArg.page = 1;
                }
            }

            if (modelData.ContainsKey("showNumber"))
            {
                int showNumber = new int();
                convertFlag = int.TryParse(modelData["showNumber"], out showNumber);
                if (convertFlag)
                {
                    dealArg.showNumber = showNumber;
                }
                else
                {
                    dealArg.showNumber = 20;
                }
            }
            if (modelData.ContainsKey("showAll"))
            {
                int showAll = new int();
                convertFlag = int.TryParse(modelData["showAll"], out showAll);
                if (convertFlag)
                {
                    dealArg.showAll = showAll;
                }
                else
                {
                    dealArg.showAll = 0;
                }
            }

            if (modelData.ContainsKey("showZero"))
            {
                int showZero = new int();
                convertFlag = int.TryParse(modelData["showZero"], out showZero);
                if (convertFlag)
                {
                    dealArg.showZero = showZero;
                }
                else
                {
                    dealArg.showZero = 0;
                }
            }
            if (modelData.ContainsKey("priceCash"))
            {
                decimal priceCash = new decimal();
                convertFlag = decimal.TryParse(modelData["priceCash"], out priceCash);
                if (convertFlag)
                {
                    dealArg.priceCash = priceCash;
                }
                else
                {
                    dealArg.priceCash = 0;
                }
            }
            if (modelData.ContainsKey("orderByType"))
            {
                dealArg.orderByType = modelData["orderByType"];
            }
            if (modelData.ContainsKey("orderBy"))
            {
                dealArg.orderBy = modelData["orderBy"];
            }
            if (modelData.ContainsKey("brandIds"))
            {
                var brandIdsString = modelData["brandIds"];
                var splitString = new string[] { "," };
                string[] brandIdsText = brandIdsString.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

                foreach (var brandIdText in brandIdsText)
                {
                    int brandId = 0;
                    bool flag = int.TryParse(brandIdText, out brandId);
                    if (flag)
                    {
                        dealArg.brandIds.Add(brandId);
                    }
                }
            }
            if (modelData.ContainsKey("categoryIds"))
            {
                var categoryIdsString = modelData["categoryIds"];
                var splitString = new string[] { "," };
                string[] categoryIdsText = categoryIdsString.Split(splitString, StringSplitOptions.RemoveEmptyEntries);

                foreach (var categoryIdText in categoryIdsText)
                {
                    int categoryId = 0;
                    bool flag = int.TryParse(categoryIdText, out categoryId);
                    if (flag)
                    {
                        dealArg.categoryIds.Add(categoryId);
                    }
                }
            }

            return dealArg;
        }
        public SearchApiModel SetSearchApiModel(string apiArgs)
        {
            SearchApiModel searchArg = new SearchApiModel();

            //if (oriString.IndexOf(keyWord, StringComparison.OrdinalIgnoreCase) > -1)
            var modelData = ParseUrl(apiArgs);

            //Type tt = typeof(SalesOrderItem);
            //Type rr = typeof(SearchApiModel);
            //var propNames = modelData.Keys.ToList();
            //foreach (var property in rr.GetProperties())
            //{
            //    if (propNames.Contains(property.Name))
            //    {
            //        rr.GetProperty(property.Name).SetValue(this, modelData[property.Name]);
            //    }
            //}
            bool convertFlag = false;
            if (modelData.ContainsKey("SearchWord"))
            {
                searchArg.SearchWord = modelData["SearchWord"];
            }
            if (modelData.ContainsKey("SrchIn"))
            {
                searchArg.SrchIn = modelData["SrchIn"];
            }
            if (modelData.ContainsKey("Order"))
            {
                int Order = new int();
                convertFlag = int.TryParse(modelData["Order"], out Order);
                if (convertFlag)
                {
                    searchArg.Order = Order;
                }
                else
                {
                    searchArg.Order = null;
                }
            }
            if (modelData.ContainsKey("Cat"))
            {
                int Cat = new int();
                convertFlag = int.TryParse(modelData["Cat"], out Cat);
                if (convertFlag)
                {
                    searchArg.Cat = Cat;
                }
                else
                {
                    searchArg.Cat = null;
                }
            }
            if (modelData.ContainsKey("LID"))
            {
                int LID = new int();
                convertFlag = int.TryParse(modelData["LID"], out LID);
                if (convertFlag)
                {
                    searchArg.LID = LID;
                }
                else
                {
                    searchArg.LID = null;
                }
            }
            if (modelData.ContainsKey("Cty"))
            {
                int Cty = new int();
                convertFlag = int.TryParse(modelData["Cty"], out Cty);
                if (convertFlag)
                {
                    searchArg.Cty = Cty;
                }
                else
                {
                    searchArg.Cty = null;
                }
            }
            if (modelData.ContainsKey("BID"))
            {
                int BID = new int();
                convertFlag = int.TryParse(modelData["BID"], out BID);
                if (convertFlag)
                {
                    searchArg.BID = BID;
                }
                else
                {
                    searchArg.BID = null;
                }
            }
            if (modelData.ContainsKey("SID"))
            {
                int SID = new int();
                convertFlag = int.TryParse(modelData["SID"], out SID);
                if (convertFlag)
                {
                    searchArg.SID = SID;
                }
                else
                {
                    searchArg.SID = null;
                }
            }
            if (modelData.ContainsKey("minPrice"))
            {
                int minPrice = new int();
                convertFlag = int.TryParse(modelData["minPrice"], out minPrice);
                if (convertFlag)
                {
                    searchArg.minPrice = minPrice;
                }
                else
                {
                    searchArg.minPrice = null;
                }
            }
            if (modelData.ContainsKey("maxPrice"))
            {
                int maxPrice = new int();
                convertFlag = int.TryParse(modelData["maxPrice"], out maxPrice);
                if (convertFlag)
                {
                    searchArg.maxPrice = maxPrice;
                }
                else
                {
                    searchArg.maxPrice = null;
                }
            }
            if (modelData.ContainsKey("PageSize"))
            {

            }
            if (modelData.ContainsKey("Page"))
            {
                int page = new int();
                convertFlag = int.TryParse(modelData["Page"], out page);
                if (convertFlag)
                {
                    searchArg.Page = page;
                }
                else
                {
                    searchArg.Page = null;
                }
            }
            if (modelData.ContainsKey("Mode"))
            {
                searchArg.Mode = modelData["Mode"];
            }
            if (modelData.ContainsKey("Submit"))
            {
                searchArg.Submit = modelData["Submit"];
            }
            if (modelData.ContainsKey("orderCats"))
            {
                searchArg.orderCats = modelData["orderCats"];
            }

            return searchArg;
        }

        private Dictionary<string, string> ParseUrl(string apiArgs)
        {
            Dictionary<string, string> modelData = new Dictionary<string, string>();
            string baseUrl = "";
            if (apiArgs == null)
            {
                return modelData;
                //throw new ArgumentNullException("url");
            }

            baseUrl = "";

            if (apiArgs == "")
                return modelData;

            int questionMarkIndex = apiArgs.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                baseUrl = apiArgs;
                return modelData;
            }

            baseUrl = apiArgs.Substring(0, questionMarkIndex);
            if (questionMarkIndex == apiArgs.Length - 1)
                return modelData;
            string ps = apiArgs.Substring(questionMarkIndex + 1);

            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                if (!modelData.ContainsKey(m.Result("$2")))
                {
                    modelData.Add(m.Result("$2"), m.Result("$3"));
                }

                //var aaa = m.Result("$2");
                //var bbb = m.Result("$3");
            }
            return modelData;
        }
        public string ReturnURLWithApi(string apiName, string apiArgs)
        {
            string url = "";


            switch (apiName)
            {
                case "SearchApi":

                    url += "/Search/Search";

                    url += apiArgs;

                    break;
                case "DealApi":

                    //url += "/item/CategoryItem";

                    //url += apiArgs;

                    url += "/item/CategoryItem?";
                    DealApiModel dealApiUrl = SetDealApiModel(apiArgs);

                    if (dealApiUrl.categoryIds.Count != 0)
                    {
                        url += "CategoryID=" + dealApiUrl.categoryIds.First() + "&";
                    }

                    if (dealApiUrl.brandIds.Count != 0)
                    {
                        url += "BrandID=" + dealApiUrl.brandIds.First() + "&";
                    }

                    url += "length=40&";

                    switch (dealApiUrl.orderByType)
                    {
                        case "Qty":
                            url += "orderBy=0&";
                            break;
                        case "Hit":
                            url += "orderBy=1&";
                            break;
                        case "Price":
                            if (dealApiUrl.orderBy == "DESC")
                            {
                                url += "orderBy=2&";
                            }
                            else
                            {
                                url += "orderBy=3&";
                            }

                            break;
                        case "Review":
                            url += "orderBy=4&";
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    url = "";
                    break;
            }


            return url;
        }
    }
}
