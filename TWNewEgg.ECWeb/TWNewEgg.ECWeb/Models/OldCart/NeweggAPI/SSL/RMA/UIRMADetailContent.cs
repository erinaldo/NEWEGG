using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMADetailContent class.
    /// </summary>
    [DataContract]
    public class UIRMADetailContent : UIQueryRMAInfo
    {
        /// <summary>
        /// Gets or sets RMADetails.
        /// </summary>
        [DataMember(Name = "RMADetails")]
        public string RMADetails { get; set; }

        /// <summary>
        /// Gets or sets ReturnInstructions1.
        /// </summary>
        [DataMember(Name = "ReturnInstructions1")]
        public string ReturnInstructions1 { get; set; }

        /// <summary>
        /// Gets or sets ReturnInstructions3.
        /// </summary>
        [DataMember(Name = "ReturnInstructions3")]
        public string ReturnInstructions3 { get; set; }

        /// <summary>
        /// Gets or sets PostLabelRMAItems.
        /// </summary>
        [DataMember(Name = "PostLabelRMAItems")]
        public List<UIQueryRMATrackingNumberItemInfo> PostLabelRMAItems { get; set; }

        /// <summary>
        /// Gets or sets PreLabelRMAItems.
        /// </summary>
        [DataMember(Name = "PreLabelRMAItems")]
        public List<UIQueryRMAItemInfo> PreLabelRMAItems { get; set; }

        /// <summary>
        /// Gets or sets RMAMemos.
        /// </summary>
        [DataMember(Name = "RMAMemos")]
        public List<UIRMAMemo> RMAMemos { get; set; }
    }
}
