using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShopAction9TagInfo class.
    /// </summary>
    [DataContract]
	public class UIShopAction9TagInfo : UIShopAction5TagInfo
	{
        /// <summary>
        /// Gets or sets CustomerID.
        /// </summary>
        [DataMember(Name = "CustomerID")]
        public string CustomerID { get; set; }
		
        /// <summary>
        /// Gets or sets OrderNumber.
        /// </summary>
        [DataMember(Name = "OrderNumber")]
        public string OrderNumber { get; set; }
		
        /// <summary>
        /// Gets or sets TotalOrderValue.
        /// </summary>
        [DataMember(Name = "TotalOrderValue")]
        public string TotalOrderValue { get; set; }
	}
}
