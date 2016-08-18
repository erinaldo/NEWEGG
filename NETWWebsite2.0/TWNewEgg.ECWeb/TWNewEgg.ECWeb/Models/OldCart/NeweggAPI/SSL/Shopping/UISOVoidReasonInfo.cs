using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISOVoidReasonInfo class.
    /// </summary>
    [DataContract]
    public class UISOVoidReasonInfo
	{
        /// <summary>
        /// Gets or sets ReasonID.
        /// </summary>
        [DataMember(Name = "ReasonID")]
        public int ReasonID { get; set; }
		
        /// <summary>
        /// Gets or sets ReasonDescription.
        /// </summary>
        [DataMember(Name = "ReasonDescription")]
        public string ReasonDescription { get; set; }
	}
}
