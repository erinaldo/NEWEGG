using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASVerifyLevelType class.
    /// </summary>
    [DataContract]
    public enum UIQASVerifyLevelType
    {
        /// <summary>
        /// Gets or sets None.
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Gets or sets Verified.
        /// </summary>
        [EnumMember]
        Verified = 1,

        /// <summary>
        /// Gets or sets InteractionRequired.
        /// </summary>
        [EnumMember]
        InteractionRequired = 2,

        /// <summary>
        /// Gets or sets PremisesPartial.
        /// </summary>
        [EnumMember]
        PremisesPartial = 3,

        /// <summary>
        /// Gets or sets StreetPartial.
        /// </summary>
        [EnumMember]
        StreetPartial = 4,

        /// <summary>
        /// Gets or sets Multiple.
        /// </summary>
        [EnumMember]
        Multiple = 5,

        /// <summary>
        /// Gets or sets VerifiedPlace.
        /// </summary>
        [EnumMember]
        VerifiedPlace = 6,

        /// <summary>
        /// Gets or sets VerifiedStreet.
        /// </summary>
        [EnumMember]
        VerifiedStreet = 7,
    }
}
