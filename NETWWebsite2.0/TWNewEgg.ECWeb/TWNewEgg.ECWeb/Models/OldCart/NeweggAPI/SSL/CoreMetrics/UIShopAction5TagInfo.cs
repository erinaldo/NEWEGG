using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShopAction5TagInfo class.
    /// </summary>
    [DataContract]
    public class UIShopAction5TagInfo
    {
        /// <summary>
        /// Gets or sets CategoryID.
        /// </summary>
        [DataMember(Name = "CategoryID")]
        public string CategoryID { get; set; }

        /// <summary>
        /// Gets or sets ProductID.
        /// </summary>
        [DataMember(Name = "ProductID")]
        public string ProductID { get; set; }

        /// <summary>
        /// Gets or sets ProductName.
        /// </summary>
        [DataMember(Name = "ProductName")]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets UnitQuantity.
        /// </summary>
        [DataMember(Name = "UnitQuantity")]
        public string UnitQuantity { get; set; }

        /// <summary>
        /// Gets or sets BasePrice.
        /// </summary>
        [DataMember(Name = "BasePrice")]
        public string BasePrice { get; set; }

        /// <summary>
        /// Gets or sets CurrencyCode.
        /// </summary>
        [DataMember(Name = "CurrencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets ExtraString.
        /// </summary>
        [DataMember(Name = "ExtraString")]
        public IList<string> ExtraString { get; set; }

        /// <summary>
        /// Gets or sets AttributesString.
        /// </summary>
        [DataMember(Name = "AttributesString")]
        public IList<string> AttributesString { get; set; }
    }
}
