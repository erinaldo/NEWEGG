using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UISearchProvider class.
    /// </summary>
    [DataContract]
    public enum UISearchProvider
    {
        /// <summary>
        /// Gets enum type of Endeca.
        /// </summary>
        [EnumMember]
        Endeca = 0,
        
        /// <summary>
        /// Gets enum type of DataBase.
        /// </summary>
        [EnumMember]
        DataBase = 1,
    }
}
