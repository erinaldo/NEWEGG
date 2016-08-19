using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderTagInfo class.
    /// </summary>
    [DataContract]
	public class UIOrderTagInfo
	{
        /// <summary>
        /// Gets or sets OrderID.
        /// </summary>
        [DataMember(Name = "OrderID")]
        public string OrderID { get; set; }
		
        /// <summary>
        /// Gets or sets OrderSubtotal.
        /// </summary>
        [DataMember(Name = "OrderSubtotal")]
        public decimal OrderSubtotal { get; set; }
		
        /// <summary>
        /// Gets or sets OrderShipping.
        /// </summary>
        [DataMember(Name = "OrderShipping")]
        public decimal OrderShipping { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerID.
        /// </summary>
        [DataMember(Name = "CustomerID")]
        public string CustomerID { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerCity.
        /// </summary>
        [DataMember(Name = "CustomerCity")]
        public string CustomerCity { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerState.
        /// </summary>
        [DataMember(Name = "CustomerState")]
        public string CustomerState { get; set; }
		
        /// <summary>
        /// Gets or sets CustomerZip.
        /// </summary>
        [DataMember(Name = "CustomerZip")]
        public string CustomerZip { get; set; }
		
        /// <summary>
        /// Gets or sets CategoryID.
        /// </summary>
        [DataMember(Name = "CategoryID")]
        public string CategoryID { get; set; }
		
        /// <summary>
        /// Gets or sets OSK.
        /// </summary>
        [DataMember(Name = "OSK")]
        public string OSK { get; set; }
		
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
