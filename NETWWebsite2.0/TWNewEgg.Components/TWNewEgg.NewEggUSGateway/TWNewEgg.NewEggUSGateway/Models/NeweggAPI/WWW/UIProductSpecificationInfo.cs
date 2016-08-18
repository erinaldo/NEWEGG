using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
	/// <summary>
    /// Initializes a new instance of the UIProductSpecificationInfo class.
	/// </summary>
    [DataContract]
	public class UIProductSpecificationInfo
	{
		/// <summary>
        /// Gets or sets NeweggItemNumber.
		/// </summary>
        [DataMember(Name = "NeweggItemNumber")]
		public string NeweggItemNumber { get; set; }

		/// <summary>
        /// Gets or sets Title.
		/// </summary>
        [DataMember(Name = "Title")]
		public string Title { get; set; }

		/// <summary>
        /// Gets or sets SpecificationGroupList.
		/// </summary>
        [DataMember(Name = "SpecificationGroupList")]
		public List<UIProductSpecificationGroupInfo> SpecificationGroupList { get; set; }
	}
}
