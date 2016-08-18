using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderTrackInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderTrackInfo
    {
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }

        /// <summary>
        /// Gets or sets State.
        /// </summary>
        [DataMember(Name = "State")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets TrackNumber.
        /// </summary>
        [DataMember(Name = "TrackNumber")]
        public string TrackNumber { get; set; }

        /// <summary>
        /// Gets or sets WarehouseNumber.
        /// </summary>
        [DataMember(Name = "WarehouseNumber")]
        public string WarehouseNumber { get; set; }

        /// <summary>
        /// Gets or sets TrackingInfoDesc.
        /// </summary>
        [DataMember(Name = "TrackingInfoDesc")]
        public string TrackingInfoDesc { get; set; }
    }
}
