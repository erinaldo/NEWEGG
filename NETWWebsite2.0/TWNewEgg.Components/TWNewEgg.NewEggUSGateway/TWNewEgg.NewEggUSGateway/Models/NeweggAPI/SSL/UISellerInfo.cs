using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISellerInfo class.
    /// </summary>
    [DataContract]
    public class UISellerInfo
    {
        /// <summary>
        /// Gets or sets SellerId.
        /// </summary>
        [DataMember(Name = "SellerId")]
        public string SellerId { get; set; }

        /// <summary>
        /// Gets or sets SellerName.
        /// </summary>
        [DataMember(Name = "SellerName")]
        public string SellerName { get; set; }
    }
}
