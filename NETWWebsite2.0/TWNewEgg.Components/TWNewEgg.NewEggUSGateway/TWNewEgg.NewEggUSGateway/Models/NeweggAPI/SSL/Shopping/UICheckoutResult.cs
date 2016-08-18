using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICheckoutResult class.
    /// </summary>
    [DataContract]
	public class UICheckoutResult
	{
        /// <summary>
        /// Gets or sets SoOrderNumbers.
        /// </summary>
        [DataMember(Name = "SoOrderNumbers")]
        public string SoOrderNumbers { get; set; }
	}
}
