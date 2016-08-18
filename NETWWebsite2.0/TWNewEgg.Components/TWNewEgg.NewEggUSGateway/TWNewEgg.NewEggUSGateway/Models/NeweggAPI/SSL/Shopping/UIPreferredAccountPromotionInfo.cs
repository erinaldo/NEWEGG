using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIPreferredAccountPromotionInfo class.
    /// </summary>
    [DataContract]
    public class UIPreferredAccountPromotionInfo
    {
        /// <summary>
        /// Gets or sets NPAConfirmOffRadioBoxDescription.
        /// </summary>
        [DataMember(Name = "NPAConfirmOffRadioBoxDescription")]
        public string NPAConfirmOffRadioBoxDescription { get; set; }

        /// <summary>
        /// Gets or sets NPAConfirmOnRadioBoxDescription.
        /// </summary>
        [DataMember(Name = "NPAConfirmOnRadioBoxDescription")]
        public string NPAConfirmOnRadioBoxDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether EnableNPAConfirmCheckBox.
        /// </summary>
        [DataMember(Name = "EnableNPAConfirmCheckBox")]
        public bool EnableNPAConfirmCheckBox { get; set; }

        /// <summary>
        /// Gets or sets NPAConfirmCheckBoxDescription.
        /// </summary>
        [DataMember(Name = "NPAConfirmCheckBoxDescription")]
        public string NPAConfirmCheckBoxDescription { get; set; }

        /// <summary>
        /// Gets or sets ChooseYourOffer.
        /// </summary>
        [DataMember(Name = "ChooseYourOffer")]
        public string ChooseYourOffer { get; set; }

        /// <summary>
        /// Gets or sets SelectedMark.
        /// </summary>
        [DataMember(Name = "SelectedMark")]
        public string SelectedMark { get; set; }
    }
}
