using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Text;
using System.Web;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPager class.
    /// </summary>
    [DataContract]
    public class UIPager
    {
        /// <summary>
        /// Initializes a new instance of the UIPager class.
        /// </summary>
        /// <param name="pageInfo">UI PageInfo.</param>
        public UIPager(UIPageInfo pageInfo)
        {
            this.EndPage = 1;
            this.StartPage = 1;
            this.ExtendPage = 5;

            this.PageSize = pageInfo.PageSize;
            this.PageNumber = pageInfo.PageNumber;
            this.TotalCount = pageInfo.TotalCount;
            this.PageCount = pageInfo.PageCount;

            this.CalcPage();
            this.SetStatus();
            this.BuildUrl();
        }

        /// <summary>
        /// Gets or sets TotalCount.
        /// </summary>
        [DataMember(Name = "TotalCount")]
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets PageSize.
        /// </summary>
        [DataMember(Name = "PageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets PageNumber.
        /// </summary>
        [DataMember(Name = "PageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets PageCount.
        /// </summary>
        [DataMember(Name = "PageCount")]
        public int PageCount { get; set; }

        /// <summary>
        /// Gets or sets ExtendPage.
        /// </summary>
        [DataMember(Name = "ExtendPage")]
        public int ExtendPage { get; set; }

        /// <summary>
        /// Gets or sets StartPage.
        /// </summary>
        [DataMember(Name = "StartPage")]
        public int StartPage { get; set; }

        /// <summary>
        /// Gets or sets EndPage.
        /// </summary>
        [DataMember(Name = "EndPage")]
        public int EndPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PreviousEnable.
        /// </summary>
        [DataMember(Name = "PreviousEnable")]
        public bool PreviousEnable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether NextEnable.
        /// </summary>
        [DataMember(Name = "NextEnable")]
        public bool NextEnable { get; set; }

        /// <summary>
        /// Gets or sets Url.
        /// </summary>
        [DataMember(Name = "Url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets QueryString.
        /// </summary>
        [DataMember(Name = "QueryString")]
        public NameValueCollection QueryString { get; set; }

        /// <summary>
        /// Gets or sets Params.
        /// </summary>
        [DataMember(Name = "Params")]
        public Dictionary<string, string> Params { get; set; }

        /// <summary>
        /// Calculate page.
        /// </summary>
        private void CalcPage()
        {
            if (this.PageCount <= 1)
            {
                this.PageCount = (this.TotalCount % this.PageSize == 0) ? (this.TotalCount / this.PageSize) : ((this.TotalCount / this.PageSize) + 1);
            }

            if (this.PageCount < 1)
            {
                this.PageCount = 1;
            }

            if (this.ExtendPage < 3)
            {
                this.ExtendPage = 2;
            }

            if (this.PageCount > this.ExtendPage)
            {
                if (this.PageNumber - (this.ExtendPage / 2) > 0)
                {
                    if (this.PageNumber + (this.ExtendPage / 2) < this.PageCount)
                    {
                        this.StartPage = this.PageNumber - (this.ExtendPage / 2);
                        this.EndPage = this.StartPage + this.ExtendPage - 1;
                    }
                    else
                    {
                        this.EndPage = this.PageCount;
                        this.StartPage = this.EndPage - this.ExtendPage + 1;
                    }
                }
                else
                {
                    this.EndPage = this.ExtendPage;
                }
            }
            else
            {
                this.StartPage = 1;
                this.EndPage = this.PageCount;
            }
        }

        /// <summary>
        /// Build Url.
        /// </summary>
        private void BuildUrl()
        {
            this.Params = new Dictionary<string, string>();
            this.QueryString = HttpContext.Current.Request.QueryString;
            if (this.QueryString != null && this.QueryString.AllKeys != null && this.QueryString.AllKeys.Length > 0)
            {
                foreach (string paramName in this.QueryString.AllKeys)
                {
                    if (!string.IsNullOrEmpty(paramName)
                        && !string.Equals("Page", paramName, StringComparison.CurrentCultureIgnoreCase)
                        && !this.Params.ContainsKey(paramName))
                    {
                        this.Params.Add(paramName, this.QueryString[paramName].Trim());
                    }
                }
            }

            this.Params.Add("Page", "{0}");
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> dic in this.Params)
            {
                sb.AppendFormat("{0}={1}&", dic.Key, dic.Value);
            }

            ////this.Url = HttpContext.Current.Request.Url.ToString().Split('?')[0] + "?" + sb.ToString().Trim('&');
            var controlllerName = HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("controller");
            var actionName = HttpContext.Current.Request.RequestContext.RouteData.GetRequiredString("action");
            this.Url = "/" + controlllerName + (actionName.ToLower() == "index" ? string.Empty : ("/" + actionName)) + "?" + sb.ToString().Trim('&');
        }

        /// <summary>
        /// Set Status.
        /// </summary>
        private void SetStatus()
        {
            this.PreviousEnable = (this.PageNumber <= 1) ? false : true;
            this.NextEnable = (this.PageNumber > (this.PageCount - 1)) ? false : true;
        }
    }
}
