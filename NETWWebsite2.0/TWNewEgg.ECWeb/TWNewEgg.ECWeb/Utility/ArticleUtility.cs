using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.ECWeb.Utility
{
    public class ArticleUtility
    {
        /// <summary>
        /// 把過長的字串去除, 並加上...
        /// </summary>
        /// <param name="Article">原始的字串.</param>
        /// <param name="Length">需要留下的長度.</param>
        /// <returns>刪減過後的字串.</returns>
        public static string LimitDownText(string Article, int Length)
        {
            string result = Article;

            if (!string.IsNullOrEmpty(Article) && Article.Length > Length)
            {
                result = Article.Substring(0, Length) + "...";
            }

            return result;
        }
    }
}