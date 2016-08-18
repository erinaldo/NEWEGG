using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIConversionEventTagInfo class.
    /// </summary>
    [DataContract]
	public class UIConversionEventTagInfo
	{
        /// <summary>
        /// Gets or sets ConversionEventID.
        /// </summary>
        [DataMember(Name = "ConversionEventID")]
        public string ConversionEventID { get; set; }

        /// <summary>
        /// Gets or sets ConversionPoints.
        /// </summary>
        [DataMember(Name = "ConversionPoints")]
        public int ConversionPoints { get; set; }

        /// <summary>
        /// Gets or sets AttributesString.
        /// </summary>
        [DataMember(Name = "AttributesString")]
        public IList<string> AttributesString { get; set; }

        /// <summary>
        /// Gets or sets CategoryID.
        /// </summary>
        [DataMember(Name = "CategoryID")]
        public string CategoryID { get; set; }

        /// <summary>
        /// Gets or sets ConversionEventActionType.
        /// </summary>
        [DataMember(Name = "ConversionEventActionType")]
        public string ConversionEventActionType { get; set; }
	}
}
