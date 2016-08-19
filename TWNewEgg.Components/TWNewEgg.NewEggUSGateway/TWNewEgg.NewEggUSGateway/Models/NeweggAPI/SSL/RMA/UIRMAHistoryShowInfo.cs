using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIRMAHistoryShowInfo class.
    /// </summary>
    [DataContract]
    public class UIRMAHistoryShowInfo : UIQueryRMAInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsShowCreateLabel.
        /// </summary>
        [DataMember(Name = "IsShowCreateLabel")]
        public bool IsShowCreateLabel { get; set; }

        /// <summary>
        /// Gets or sets CreateLabelUrl.
        /// </summary>
        [DataMember(Name = "CreateLabelUrl")]
        public string CreateLabelUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowTrackShipment.
        /// </summary>
        [DataMember(Name = "IsShowTrackShipment")]
        public bool IsShowTrackShipment { get; set; }

        /// <summary>
        /// Gets or sets TrackShipmentUrl.
        /// </summary>
        [DataMember(Name = "TrackShipmentUrl")]
        public string TrackShipmentUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowRePrintLabel.
        /// </summary>
        [DataMember(Name = "IsShowRePrintLabel")]
        public bool IsShowRePrintLabel { get; set; }

        /// <summary>
        /// Gets or sets RePrintLabelUrl.
        /// </summary>
        [DataMember(Name = "RePrintLabelUrl")]
        public string RePrintLabelUrl { get; set; }

        /// <summary>
        /// Gets or sets RMAQueryDetailUrl.
        /// </summary>
        [DataMember(Name = "RMAQueryDetailUrl")]
        public string RMAQueryDetailUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsShowCommercialInvoice.
        /// </summary>
        [DataMember(Name = "IsShowCommercialInvoice")]
        public bool IsShowCommercialInvoice { get; set; }

        /// <summary>
        /// Gets or sets CommercialInvoiceUrl.
        /// </summary>
        [DataMember(Name = "CommercialInvoiceUrl")]
        public string CommercialInvoiceUrl { get; set; }
    }
}
