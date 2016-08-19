using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIDeviceInfo class.
    /// </summary>
    [DataContract]
    public class UIDeviceInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsPad.
        /// </summary>
        [DataMember(Name = "IsPad")]
        public bool IsPad { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNokia.
        /// </summary>
        [DataMember(Name = "IsNokia")]
        public bool IsNokia { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsIphoneOrIpod.
        /// </summary>
        [DataMember(Name = "IsIphoneOrIpod")]
        public bool IsIphoneOrIpod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBlackberry.
        /// </summary>
        [DataMember(Name = "IsBlackberry")]
        public bool IsBlackberry { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAndroid.
        /// </summary>
        [DataMember(Name = "IsAndroid")]
        public bool IsAndroid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsWindowsPhone.
        /// </summary>
        [DataMember(Name = "IsWindowsPhone")]
        public bool IsWindowsPhone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsBlackberryPlayBook.
        /// </summary>
        [DataMember(Name = "IsBlackberryPlayBook")]
        public bool IsBlackberryPlayBook { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsWindowsSystem.
        /// </summary>
        [DataMember(Name = "IsWindowsSystem")]
        public bool IsWindowsSystem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsAndroidTablet.
        /// </summary>
        [DataMember(Name = "IsAndroidTablet")]
        public bool IsAndroidTablet { get; set; }
    }
}
