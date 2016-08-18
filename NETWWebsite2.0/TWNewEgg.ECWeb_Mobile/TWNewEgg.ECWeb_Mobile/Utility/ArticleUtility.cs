using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
namespace TWNewEgg.ECWeb_Mobile.Utility
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
        //取字串長度,中文字當2個字元看,所以Length = 8 代表只能放中文字4個,
        public static string LimitDownTextCht(string Article, int Length)
        {
            int iLength = Encoding.Default.GetByteCount(Article);
            if (iLength <= Length) return Article;
            for (int i = 0; i < iLength; i++)
            {
                Article = Article.Substring(0, Article.Length - 2);
                iLength = Encoding.Default.GetByteCount(Article);
                if (iLength <= Length) return Article;
            }
            return Article;
        }
    }
}