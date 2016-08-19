using System;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the ProductDetailsViewData class.
    /// </summary>
    [DataContract]
    public class ProductDetailsViewData
    {
        /// <summary>
        /// Gets or sets SpecificationInfo.
        /// </summary>
        [DataMember(Name = "SpecificationInfo")]
        public UIProductSpecificationInfo SpecificationInfo { get; set; }

        /// <summary>
        /// Gets or sets ReturnPolicyInfo.
        /// </summary>
        [DataMember(Name = "ReturnPolicyInfo")]
        public UIReturnPolicyInfo ReturnPolicyInfo { get; set; }
    }
}
