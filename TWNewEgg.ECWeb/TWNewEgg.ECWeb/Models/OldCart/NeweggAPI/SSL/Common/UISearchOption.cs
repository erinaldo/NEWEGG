using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISearchOption class.
    /// </summary>
    [DataContract]
    public class UISearchOption
    {
        /// <summary>
        /// Initializes a new instance of the UISearchOption class.
        /// </summary>
        public UISearchOption()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UISearchOption class. 查询new order history时，option默认30，beginTime、endTime不用传值.
        /// </summary>
        /// <param name="customerNumber">Customer number.</param>
        /// <param name="keyword">Keyword value.</param>
        /// <param name="option">Option value.</param>
        /// <param name="beginTime">Begin time.</param>
        /// <param name="endTime">End time.</param>
        public UISearchOption(int customerNumber, string keyword, string option, string beginTime, string endTime)
        {
            this.CustomerNumber = customerNumber;
            this.Keyword = keyword;
            this.Option = option;
            this.BeginTime = beginTime;
            this.EndTime = endTime;
        }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

        /// <summary>
        /// Gets or sets PageIndex.
        /// </summary>
        [DataMember(Name = "PageIndex")]
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets Keyword.
        /// </summary>
        [DataMember(Name = "Keyword")]
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets Option.
        /// </summary>
        [DataMember(Name = "Option")]
        public string Option { get; set; }

        /// <summary>
        /// Gets or sets BeginTime.
        /// </summary>
        [DataMember(Name = "BeginTime")]
        public string BeginTime { get; set; }

        /// <summary>
        /// Gets or sets EndTime.
        /// </summary>
        [DataMember(Name = "EndTime")]
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ShowAll.
        /// </summary>
        [DataMember(Name = "ShowAll")]
        public bool ShowAll { get; set; }
    }
}
