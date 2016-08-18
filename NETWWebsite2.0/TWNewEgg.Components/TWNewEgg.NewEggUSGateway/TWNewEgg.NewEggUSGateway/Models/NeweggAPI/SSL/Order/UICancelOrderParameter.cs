using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UICancelOrderParameter class.
    /// </summary>
    [DataContract]
    public class UICancelOrderParameter
    {
        /// <summary>
        /// Gets or sets PreSoNumber.
        /// </summary>
        [DataMember(Name = "PreSoNumber")]
        public int PreSoNumber { get; set; }
        
        /// <summary>
        /// Gets or sets SoNumber.
        /// </summary>
        [DataMember(Name = "SoNumber")]
        public int SoNumber { get; set; }
        
        /// <summary>
        /// Gets or sets CutomerNumber.
        /// </summary>
        [DataMember(Name = "CutomerNumber")]
        public int CutomerNumber { get; set; }
        
        /// <summary>
        /// Gets or sets ReasonCode.
        /// </summary>
        [DataMember(Name = "ReasonCode")]
        public string ReasonCode { get; set; }
        
        /// <summary>
        /// Gets or sets ReasonDescription.
        /// </summary>
        [DataMember(Name = "ReasonDescription")]
        public string ReasonDescription { get; set; }
    }
}
