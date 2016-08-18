using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
	/// <summary>
    /// Initializes a new instance of the UIItemType class.
	/// </summary>
    [DataContract]
	public class UIItemType
	{
		/// <summary>
        /// Gets or sets a value indicating whether shopping cart IsDiIsDigitalRiverItem.
		/// </summary>
        [DataMember(Name = "IsDigitalRiverItem")]
		public bool IsDigitalRiverItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping IsDVDItem.
		/// </summary>
        [DataMember(Name = "IsDVDItem")]
		public bool IsDVDItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cartIsOpenBoxItem.
		/// </summary>
        [DataMember(Name = "IsOpenBoxItem")]
		public bool IsOpenBoxItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cart IsRIsRecertifiedItem.
		/// </summary>
        [DataMember(Name = "IsRecertifiedItem")]
		public bool IsRecertifiedItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping IsAITItem.
		/// </summary>
        [DataMember(Name = "IsAITItem")]
		public bool IsAITItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping caIsPhoneItem.
		/// </summary>
        [DataMember(Name = "IsPhoneItem")]
		public bool IsPhoneItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cart IsSIsServicePlanItem.
		/// </summary>
        [DataMember(Name = "IsServicePlanItem")]
		public bool IsServicePlanItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cartIsSimCardItem.
		/// </summary>
        [DataMember(Name = "IsSimCardItem")]
		public bool IsSimCardItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cart IsIsRestrictedItem.
		/// </summary>
        [DataMember(Name = "IsRestrictedItem")]
		public bool IsRestrictedItem { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cartIsComboBundle.
		/// </summary>
        [DataMember(Name = "IsComboBundle")]
		public bool IsComboBundle { get; set; }

		/// <summary>
        /// Gets or sets a value indicating whether shopping cart IIsSimplxityItem.
		/// </summary>
        [DataMember(Name = "IsSimplxityItem")]
		public bool IsSimplxityItem { get; set; }
	}
}
