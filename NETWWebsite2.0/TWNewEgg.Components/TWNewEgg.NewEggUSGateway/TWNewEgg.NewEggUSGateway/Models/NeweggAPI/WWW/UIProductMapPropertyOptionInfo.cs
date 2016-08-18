using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIProductMapPropertyOptionInfo class.
    /// </summary>
    [DataContract]
    public class UIProductMapPropertyOptionInfo
    {
        /// <summary>
        /// Gets or sets RecommendItemNumber.
        /// </summary>
        [DataMember(Name = "RecommendItemNumber")]
        public string RecommendItemNumber { get; set; }

        /// <summary>
        /// Gets or sets ParentItemNumber.
        /// </summary>
        [DataMember(Name = "ParentItemNumber")]
        public string ParentItemNumber { get; set; }

        /// <summary>
        /// Gets or sets Properties.
        /// </summary>
        [DataMember(Name = "Properties")]
        public string Properties { get; set; }
    }
}
