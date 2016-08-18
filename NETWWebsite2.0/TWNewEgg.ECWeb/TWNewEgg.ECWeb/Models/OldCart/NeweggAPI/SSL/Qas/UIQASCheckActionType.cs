using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASCheckActionType class.
    /// </summary>
    [DataContract]
    public enum UIQASCheckActionType
    {
        /// <summary>
        /// Gets or sets Search.
        /// </summary>
        [EnumMember]
        Search = 1,

        /// <summary>
        /// Gets or sets Refine.
        /// </summary>
        [EnumMember]
        Refine = 2,
    }
}
