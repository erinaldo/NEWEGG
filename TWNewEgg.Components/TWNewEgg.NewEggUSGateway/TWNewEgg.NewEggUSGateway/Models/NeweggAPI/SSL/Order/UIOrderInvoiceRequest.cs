using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderInvoiceRequest class.
    /// </summary>
    [DataContract]
    public class UIOrderInvoiceRequest
    {
        /// <summary>
        /// Gets or sets InvoiceNumber.
        /// </summary>
        [DataMember(Name = "InvoiceNumber")]
        public int InvoiceNumber { get; set; }
       
        /// <summary>
        /// Gets or sets SONumber.
        /// </summary>
        [DataMember(Name = "SONumber")]
        public int SONumber { get; set; }
       
        /// <summary>
        /// Gets or sets InvoiceDate.
        /// </summary>
        [DataMember(Name = "InvoiceDate")]
        public string InvoiceDate { get; set; }
       
        /// <summary>
        /// Gets or sets CustomerNumber.
        /// </summary>
        [DataMember(Name = "CustomerNumber")]
        public int CustomerNumber { get; set; }
       
        /// <summary>
        /// Gets or sets LoginName.
        /// </summary>
        [DataMember(Name = "LoginName")]
        public string LoginName { get; set; }
    }
}
