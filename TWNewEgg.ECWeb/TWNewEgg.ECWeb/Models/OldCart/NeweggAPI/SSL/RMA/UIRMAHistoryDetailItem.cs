using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistoryDetailItem class.
    /// </summary>
    [DataContract]
    public class UIRMAHistoryDetailItem : UIRMAHistoryDetailItemBase
    {
        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        [DataMember(Name = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets SubTitle.
        /// </summary>
        [DataMember(Name = "SubTitle")]
        public List<string> SubTitle { get; set; }

        /// <summary>
        /// Gets or sets PropertiesValues.
        /// </summary>
        [DataMember(Name = "PropertiesValues")]
        public string PropertiesValues { get; set; }
    }
}
