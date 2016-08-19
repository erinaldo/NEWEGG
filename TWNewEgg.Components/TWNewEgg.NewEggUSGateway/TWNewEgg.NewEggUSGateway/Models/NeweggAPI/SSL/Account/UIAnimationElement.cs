using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIAnimationElement class.
    /// </summary>
    [DataContract]
    public class UIAnimationElement
    {
        /// <summary>
        /// Gets or sets Element.
        /// </summary>
        [DataMember(Name = "Element")]
        public string Element { get; set; }
        
        /// <summary>
        /// Gets or sets ImgName.
        /// </summary>
        [DataMember(Name = "ImgName")]
        public string ImgName { get; set; }
        
        /// <summary>
        /// Gets or sets Animation.
        /// </summary>
        [DataMember(Name = "Animation")]
        public string Animation { get; set; }
        
        /// <summary>
        /// Gets or sets RGB.
        /// </summary>
        [DataMember(Name = "RGB")]
        public string RGB { get; set; }
    }
}
