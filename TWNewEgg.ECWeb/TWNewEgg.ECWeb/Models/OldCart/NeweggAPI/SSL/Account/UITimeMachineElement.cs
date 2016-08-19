using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UITimeMachineElement class.
    /// </summary>
    [DataContract]
    public class UITimeMachineElement
    {
        /// <summary>
        /// Gets or sets Frame.
        /// </summary>
        [DataMember(Name = "Frame")]
        public string Frame { get; set; }
        
        /// <summary>
        /// Gets or sets Year.
        /// </summary>
        [DataMember(Name = "Year")]
        public int Year { get; set; }
        
        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        [DataMember(Name = "Text")]
        public string Text { get; set; }
        
        /// <summary>
        /// Gets or sets Params.
        /// </summary>
        [DataMember(Name = "Params")]
        public List<string> Params { get; set; }
        
        /// <summary>
        /// Gets or sets Images.
        /// </summary>
        [DataMember(Name = "Images")]
        public List<UIAnimationElement> Images { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether Enabled is true or false.
        /// </summary>
        [DataMember(Name = "Enabled")]
        public bool Enabled { get; set; }
    }
}
