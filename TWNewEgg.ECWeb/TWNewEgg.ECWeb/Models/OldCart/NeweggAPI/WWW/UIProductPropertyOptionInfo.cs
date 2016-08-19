using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductPropertyOptionInfo class.
    /// </summary>
    [DataContract]
    public class UIProductPropertyOptionInfo
    {
        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets DisplayInfo.
        /// </summary>
        [DataMember(Name = "DisplayInfo")]
        public string DisplayInfo { get; set; }

        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [DataMember(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelected.
        /// </summary>
        [DataMember(Name = "IsSelected")]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets OptionType.
        /// </summary>
        [DataMember(Name = "OptionType")]
        public UIProductPropertyOptionType OptionType { get; set; }
    }
}
