using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the ComboUniqueInfo class.
    /// </summary>
    [DataContract]
    public class ComboUniqueInfo
    {
        /// <summary>
        /// Gets or sets ComboID.
        /// </summary>
        [DataMember(Name = "ComboID")]
        public int ComboID { get; set; }

        /// <summary>
        /// Gets or sets UniqueID.
        /// </summary>
        [DataMember(Name = "UniqueID")]
        public int UniqueID { get; set; }

        /// <summary>
        /// Gets or sets SubUniqueID.
        /// </summary>
        [DataMember(Name = "SubUniqueID")]
        public int SubUniqueID { get; set; }
    }
}
