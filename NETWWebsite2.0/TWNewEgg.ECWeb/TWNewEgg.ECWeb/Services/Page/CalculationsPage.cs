using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
namespace TWNewEgg.ECWeb.Services.Page
{
    public class CalculationsPage
    {
        /// <summary>
        /// 計算View上顯示頁數
        /// </summary>
        /// <param name="totalPage"></param>
        /// <param name="showPage"></param>
        /// <param name="negativeRange"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.ViewModels.Page.ShowPage> getShowPages(int totalPage, int showPage = 1, int negativeRange = 3)
        {
            string maxPageString = System.Configuration.ConfigurationManager.AppSettings["DisplayMaximumPage"];
            int maximumPage = new int();
            if (!string.IsNullOrEmpty(maxPageString))
            {
                if (int.TryParse(maxPageString, out maximumPage))
                {
                    if (totalPage > maximumPage)
                    {
                        totalPage = maximumPage;
                    }
                }
            }

            List<TWNewEgg.Models.ViewModels.Page.ShowPage> result = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
            result = Paging(totalPage, showPage, negativeRange);

            return result;
        }

        /// <summary>
        /// 計算 View 上顯示頁數(不限制最大頁)
        /// </summary>
        /// <param name="totalPage"></param>
        /// <param name="showPage"></param>
        /// <param name="negativeRange"></param>
        /// <returns></returns>
        public List<TWNewEgg.Models.ViewModels.Page.ShowPage> GetShowPageWithoutLimit(int totalPage, int showPage = 1, int negativeRange = 3)
        {
            List<TWNewEgg.Models.ViewModels.Page.ShowPage> result = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
            result = Paging(totalPage, showPage, negativeRange);

            return result;
        }

        /// <summary>
        /// 計算View上顯示頁數(原計算邏輯)
        /// </summary>
        /// <param name="totalPage"></param>
        /// <param name="showPage"></param>
        /// <param name="negativeRange"></param>
        /// <returns></returns>
        private List<TWNewEgg.Models.ViewModels.Page.ShowPage> Paging(int totalPage, int showPage = 1, int negativeRange = 3)
        {
            List<TWNewEgg.Models.ViewModels.Page.ShowPage> ShowPageList = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();
            List<TWNewEgg.Models.ViewModels.Page.ShowPage> ShowPageListOut = new List<TWNewEgg.Models.ViewModels.Page.ShowPage>();

            int Begin = showPage - negativeRange;
            int End = showPage + 3;
            if (totalPage >= (negativeRange * 2) + 1)
            {
                if (Begin < 1)
                {
                    End = End - (Begin - 1);
                    Begin = 1;
                }

                if (End > totalPage)
                {
                    Begin = Begin - (End - totalPage);
                    End = totalPage;
                }


                for (int i = Begin; i <= End; i++)
                {
                    TWNewEgg.Models.ViewModels.Page.ShowPage ShowPagestemp = new TWNewEgg.Models.ViewModels.Page.ShowPage();
                    ShowPagestemp.PageNumber = i;
                    ShowPagestemp.PageNumberString = i.ToString();
                    ShowPageList.Add(ShowPagestemp);
                }

                if (ShowPageList.Where(x => x.PageNumber == 1).ToList().Count == 0)
                {
                    TWNewEgg.Models.ViewModels.Page.ShowPage ShowPagestemp = new TWNewEgg.Models.ViewModels.Page.ShowPage();
                    ShowPagestemp.PageNumber = 1;
                    if (ShowPageList.Where(x => x.PageNumber == 2).ToList().Count == 0)
                    {
                        ShowPagestemp.PageNumberString = "1 ... ";
                    }
                    else
                    {
                        ShowPagestemp.PageNumberString = "1";
                    }
                    ShowPageList.Add(ShowPagestemp);
                }

                if (ShowPageList.Where(x => x.PageNumber == totalPage).ToList().Count == 0)
                {
                    TWNewEgg.Models.ViewModels.Page.ShowPage ShowPagestemp = new TWNewEgg.Models.ViewModels.Page.ShowPage();
                    ShowPagestemp.PageNumber = totalPage;
                    if (ShowPageList.Where(x => x.PageNumber == (totalPage - 1)).ToList().Count == 0)
                    {
                        ShowPagestemp.PageNumberString = " ... " + totalPage.ToString();
                    }
                    else
                    {
                        ShowPagestemp.PageNumberString = totalPage.ToString();
                    }
                    ShowPageList.Add(ShowPagestemp);
                }

                var ShowPageListGroup = ShowPageList.GroupBy(x => x.PageNumber).OrderBy(x => x.Key).ToList();
                foreach (var temp in ShowPageListGroup)
                {
                    ShowPageListOut.Add(temp.FirstOrDefault());
                }
            }
            else
            {
                for (int i = 1; i <= totalPage; i++)
                {
                    TWNewEgg.Models.ViewModels.Page.ShowPage ShowPagestemp = new TWNewEgg.Models.ViewModels.Page.ShowPage();
                    ShowPagestemp.PageNumber = i;
                    ShowPagestemp.PageNumberString = i.ToString();
                    ShowPageListOut.Add(ShowPagestemp);
                }
            }

            return ShowPageListOut;
        }

    }
}