using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIQASCheckResultInfo class.
    /// </summary>
    [DataContract]
    public class UIQASCheckResultInfo
    {
        /// <summary>
        /// Gets or sets UIQASAddressInfo.
        /// </summary>
        [DataMember(Name = "UIQASAddressInfo")]
        public IList<UIQASAddressInfo> AddressList;

        /// <summary>
        /// Gets or sets QASVerifyLevelTypeValue.
        /// </summary>
        [DataMember(Name = "QASVerifyLevelTypeValue")]
        public int QASVerifyLevelTypeValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether MustRefine.
        /// </summary>
        [DataMember(Name = "MustRefine")]
        public bool MustRefine { get; set; }

        /// <summary>
        /// Gets or sets WarningMessage.
        /// </summary>
        [DataMember(Name = "WarningMessage")]
        public List<string> WarningMessage { get; set; } ////Modify string[] to List<string> by Casper.

        /// <summary>
        /// Gets or sets MonikerKey.
        /// </summary>
        [DataMember(Name = "MonikerKey")]
        public string MonikerKey { get; set; }

        /// <summary>
        /// Gets a new instance of the UIQASVerifyLevelType class.
        /// </summary>
        public UIQASVerifyLevelType QASVerifyLevelType
        {
            get
            {
                return (UIQASVerifyLevelType)this.QASVerifyLevelTypeValue;
            }
        }
    }
}
