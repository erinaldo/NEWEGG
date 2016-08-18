using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the StoreMenu class.
    /// </summary>
    [DataContract]
    public class StoreMenu
    {
        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets ID.
        /// </summary>
        [DataMember(Name = "ID")]
        public int ID { get; set; }
    }
}
