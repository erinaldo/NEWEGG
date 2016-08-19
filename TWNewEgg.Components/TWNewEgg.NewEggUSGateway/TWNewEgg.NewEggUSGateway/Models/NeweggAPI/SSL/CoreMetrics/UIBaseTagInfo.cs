using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIBaseTagInfo class.
    /// </summary>
    [DataContract]
	public class UIBaseTagInfo
	{
        /// <summary>
        /// Gets or sets ExtraString.
        /// </summary>
        [DataMember(Name = "ExtraString")]
		public IList<string> ExtraString { get; set; }
	}
}
