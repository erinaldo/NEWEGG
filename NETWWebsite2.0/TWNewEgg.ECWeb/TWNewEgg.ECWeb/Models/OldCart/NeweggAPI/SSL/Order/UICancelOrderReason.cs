using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICancelOrderReason class.
    /// </summary>
    [DataContract]
    public class UICancelOrderReason
    {
        /// <summary>
        /// Gets or sets Code.
        /// </summary>
        [DataMember(Name = "Code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }
    }
}
