using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderHistoryDetailParameter class.
    /// </summary>
    [DataContract]
    public class UIOrderHistoryDetailParameter
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
        /// Gets or sets PreSONumber.
        /// </summary>
        [DataMember(Name = "PreSONumber")]
        public int PreSONumber { get; set; }
       
        /// <summary>
        /// Gets or sets OrderDate.
        /// </summary>
        [DataMember(Name = "OrderDate")]
        public string OrderDate { get; set; }
       
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
