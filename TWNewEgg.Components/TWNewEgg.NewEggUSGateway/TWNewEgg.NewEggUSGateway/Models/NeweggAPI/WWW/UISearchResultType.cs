using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Gets or sets UISearchResultType.
    /// </summary>
    [DataContract]
    public enum UISearchResultType
    {
        /// <summary>
        /// Gets enum type of InvalidItem.
        /// </summary>
        [EnumMember]
        InvalidItem = 0, ////it must be the first one
        
        /// <summary>
        /// Gets enum type of ActiveItem.
        /// </summary>
        [EnumMember]
        ActiveItem = 1,
        
        /// <summary>
        /// Gets enum type of WildCardItem.
        /// </summary>
        [EnumMember]
        WildCardItem = 2,
        
        /// <summary>
        /// Gets enum type of DeactiveItem.
        /// </summary>
        [EnumMember]
        DeactiveItem = 3,

        /// <summary>
        /// Gets enum type of DeactiveSimilarItem.
        /// </summary>
        [EnumMember]
        DeactiveSimilarItem = 4,

        /// <summary>
        /// Gets enum type of NoItem.
        /// </summary>
        [EnumMember]
        NoItem = 5,

        /// <summary>
        /// Gets enum type of TrimmedSuggestionItem.
        /// </summary>
        [EnumMember]
        TrimmedSuggestionItem = 6,
    }
}
