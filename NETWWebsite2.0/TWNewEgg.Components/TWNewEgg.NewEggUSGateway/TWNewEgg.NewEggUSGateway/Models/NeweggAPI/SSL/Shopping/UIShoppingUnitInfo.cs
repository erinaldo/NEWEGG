using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingUnitInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingUnitInfo
    {
        /// <summary>
        /// Gets or sets ItemGroupType.
        /// </summary>
        [DataMember(Name = "ItemGroupType")]
        public UIItemGroupType ItemGroupType { get; set; }
        
        /// <summary>
        /// Gets or sets ItemNumber.
        /// </summary>
        [DataMember(Name = "ItemNumber")]
        public string ItemNumber { get; set; }
        
        /// <summary>
        /// Gets or sets Quantity.
        /// </summary>
        [DataMember(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets RelatedItemNumber.
        /// </summary>
        [DataMember(Name = "RelatedItemNumber")]
        public string RelatedItemNumber { get; set; }

        /// <summary>
        /// Gets or sets SaleType.
        /// salesOrder = 1,
        /// PreSalesOrder = 2.
        /// </summary>
        [DataMember(Name = "SaleType")]
        public int SaleType { get; set; }
    }
}
