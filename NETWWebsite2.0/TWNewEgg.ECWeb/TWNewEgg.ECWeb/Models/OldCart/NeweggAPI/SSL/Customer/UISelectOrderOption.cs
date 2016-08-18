using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Select order option.
    /// </summary>
    [DataContract]
    public class UISelectOrderOption
    {
        /// <summary>
        /// Initializes a new instance of the UISelectOrderOption class.
        /// </summary>
        public UISelectOrderOption()
        {
            this.OptionValue = "30";
            this.OptionText = "in 30 Days";
        }

        /// <summary>
        /// Gets or sets OptionValue.
        /// </summary>
        [DataMember(Name = "OptionValue")]
        public string OptionValue { get; set; }

        /// <summary>
        /// Gets or sets OptionText.
        /// </summary>
        [DataMember(Name = "OptionText")]
        public string OptionText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelected.
        /// </summary>
        [DataMember(Name = "IsSelected")]
        public bool IsSelected { get; set; }
    }
}
