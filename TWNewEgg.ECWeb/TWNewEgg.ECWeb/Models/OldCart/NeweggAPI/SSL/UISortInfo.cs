using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISortInfo class.
    /// </summary>
    [DataContract]
    public class UISortInfo
    {
        /// <summary>
        /// Gets or sets SortField.
        /// </summary>
        [DataMember(Name = "SortField")]
        public string SortField { get; set; }

        /// <summary>
        /// Gets or sets SortType.
        /// </summary>
        [DataMember(Name = "SortType")]
        public SortOrderType SortType { get; set; }
    }
}
