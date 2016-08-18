using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShippingInfo class.
    /// </summary>
    [DataContract]
    public class UIShippingInfo
    {
        /// <summary>
        /// Gets or sets NormalShippingText.
        /// </summary>
        [DataMember(Name = "NormalShippingText")]
        public string NormalShippingText { get; set; }

        /// <summary>
        /// Gets or sets SpecialShippingText.
        /// </summary>
        [System.Runtime.Serialization.DataMember(Name = "SpecialShippingText")]
        public string SpecialShippingText { get; set; }

        /// <summary>
        /// Gets or sets RestrictedShippingTitle.
        /// </summary>
        [DataMember(Name = "RestrictedShippingTitle")]
        public string RestrictedShippingTitle { get; set; }

        /// <summary>
        /// Gets or sets RestrictedShippingText.
        /// </summary>
        [DataMember(Name = "RestrictedShippingText")]
        public string RestrictedShippingText { get; set; }
    }
}
