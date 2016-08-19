using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderSearchOption class.
    /// </summary>
    [DataContract]
    public class UIOrderSearchOption : UISearchOption
    {
        /// <summary>
        /// Initializes a new instance of the UIOrderSearchOption class.
        /// </summary>
        public UIOrderSearchOption()
        {
        }

        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets TransNo.
        /// </summary>
        [DataMember(Name = "TransNo")]
        public int TransNo { get; set; }
    }
}
