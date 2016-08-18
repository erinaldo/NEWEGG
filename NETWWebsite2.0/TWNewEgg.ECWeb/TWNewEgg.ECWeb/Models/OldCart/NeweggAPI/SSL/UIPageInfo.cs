using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPageInfo class.
    /// </summary>
    [DataContract]
    public class UIPageInfo
    {
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
    }
}
