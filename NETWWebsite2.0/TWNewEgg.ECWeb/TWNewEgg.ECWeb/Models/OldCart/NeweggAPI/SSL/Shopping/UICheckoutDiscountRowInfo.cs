using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckoutDiscountRowInfo class.
    /// </summary>
    [DataContract]
    public class UICheckoutDiscountRowInfo
	{
        /// <summary>
        /// Gets or sets qty.
        /// </summary>
        [DataMember(Name = "Qty")]
        public int Qty { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }
		
        /// <summary>
        /// Gets or sets Price.
        /// </summary>
        [DataMember(Name = "Price")]
        public string Price { get; set; }
	}
}
