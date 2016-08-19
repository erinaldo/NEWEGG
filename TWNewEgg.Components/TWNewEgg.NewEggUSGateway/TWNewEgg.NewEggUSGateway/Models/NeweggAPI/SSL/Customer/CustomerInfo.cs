using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the CustomerInfo class.
    /// </summary>
    [DataContract]
    public class CustomerInfo
    {
        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets Number.
        /// </summary>
        [DataMember(Name = "Number")]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }

        /// <summary>
        /// Gets or sets Zipcode.
        /// </summary>
        [DataMember(Name = "Zipcode")]
        public string Zipcode { get; set; }

        /// <summary>
        /// Gets or sets AuthToken.
        /// </summary>
        [DataMember(Name = "AuthToken")]
        public string AuthToken { get; set; }
    }
}
