using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newegg.Mobile.MvcApplication.Models;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the IphoneVersionControl class.
    /// </summary>
    [DataContract]
    public class IphoneVersionControl
    {
        /// <summary>
        /// Gets or sets AppStoreURL.
        /// </summary>
        [DataMember(Name = "AppStoreURL")]
        public string AppStoreURL { get; set; }

        /// <summary>
        /// Gets or sets DeviceType.
        /// </summary>
        [DataMember(Name = "DeviceType")]
        public int DeviceType { get; set; }

        /// <summary>
        /// Gets or sets AvailableVersions.
        /// </summary>
        [DataMember(Name = "AvailableVersions")]
        public List<string> AvailableVersions { get; set; }

        /// <summary>
        /// Gets or sets ClientVersion.
        /// </summary>
        [DataMember(Name = "ClientVersion")]
        public string ClientVersion { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        [DataMember(Name = "Description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets PayPal.
        /// </summary>
        [DataMember(Name = "PayPal")]
        public PayPalConfiguration PayPal { get; set; }

        /// <summary>
        /// Gets or sets PushNotification.
        /// </summary>
        [DataMember(Name = "PushNotification")]
        public PushNotificationConfiguration PushNotification { get; set; }

        /// <summary>
        /// Gets or sets TimeMachine.
        /// </summary>
        [DataMember(Name = "TimeMachine")]
        public TimeMachineConfiguration TimeMachine { get; set; }
    }
}
