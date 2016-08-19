using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderHistorySearchParameter class.
    /// </summary>
    [DataContract]
    public class UIOrderHistorySearchParameter
    {
        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets TransNo.
        /// </summary>
        [DataMember(Name = "TransNo")]
        public int TransNo { get; set; }

        /// <summary>
        /// Gets or sets PageIndex.
        /// </summary>
        [DataMember(Name = "PageIndex")]
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }

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
    }
}
