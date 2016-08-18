using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductPropertyOptionType class.
    /// </summary>
    [DataContract]
    public enum UIProductPropertyOptionType
    {
        /// <summary>
        /// Gets enum type of IconText.
        /// </summary>
        [EnumMember]
        IconText = 0,

        /// <summary>
        /// Gets enum type of IconColor.
        /// </summary>
        [EnumMember]
        IconColor = 1,

        /// <summary>
        /// Gets enum type of IconImage.
        /// </summary>
        [EnumMember]
        IconImage = 2,
    }
}
