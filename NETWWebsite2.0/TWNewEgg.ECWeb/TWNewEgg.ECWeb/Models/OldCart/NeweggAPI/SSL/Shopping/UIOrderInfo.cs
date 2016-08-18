using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIOrderInfo class.
    /// </summary>
    [DataContract]
    public class UIOrderInfo
    {
        /// <summary>
        /// Gets or sets SOAmount.
        /// </summary>
        [DataMember(Name = "SOAmount")]
        public string SOAmount { get; set; }

        /// <summary>
        /// Gets or sets TaxAmount.
        /// </summary>
        [DataMember(Name = "TaxAmount")]
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets ShippingCharge.
        /// </summary>
        [DataMember(Name = "ShippingCharge")]
        public decimal ShippingCharge { get; set; }

        /// <summary>
        /// Gets or sets ShippingMethodInfoList.
        /// </summary>
        [DataMember(Name = "ShippingMethodInfoList")]
        public List<UIShippingMethodInfo> ShippingMethodInfoList { get; set; }

        /// <summary>
        /// Gets or sets ShippingMethodNote.
        /// </summary>
        [DataMember(Name = "ShippingMethodNote")]
        public string ShippingMethodNote { get; set; }

        /// <summary>
        /// Gets or sets OrderItemType.
        /// </summary>
        [DataMember(Name = "OrderItemType")]
        public int OrderItemType { get; set; }

        /// <summary>
        /// Gets or sets OrderType.
        /// </summary>
        [DataMember(Name = "OrderType")]
        public int OrderType { get; set; }

        /// <summary>
        /// Gets or sets SODate.
        /// </summary>
        [DataMember(Name = "SODate")]
        public string SODate { get; set; }

        /// <summary>
        /// Gets or sets SONumber.
        /// </summary>
        [DataMember(Name = "SONumber")]
        public int SONumber { get; set; }

        /// <summary>
        /// Gets or sets Status.
        /// </summary>
        [DataMember(Name = "Status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets InvoiceNumber.
        /// </summary>
        [DataMember(Name = "InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNoncancelable.
        /// </summary>
        [DataMember(Name = "IsNoncancelable")]
        public bool IsNoncancelable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCorrelativeComboSO.
        /// </summary>
        [DataMember(Name = "IsCorrelativeComboSO")]
        public bool IsCorrelativeComboSO { get; set; }

        /// <summary>
        /// Gets or sets OrderItemInfoList.
        /// </summary>
        [DataMember(Name = "OrderItemInfoList")]
        public List<UIOrderItemInfo> OrderItemInfoList { get; set; }

        /// <summary>
        /// Gets or sets OrderDisplayFeeInfo.
        /// </summary>
        [DataMember(Name = "OrderDisplayFeeInfo")]
        public UIOrderDisplayFeeInfo OrderDisplayFeeInfo { get; set; }

        /// <summary>
        /// Gets or sets OrderItemsSubtotalInfo.
        /// </summary>
        [DataMember(Name = "OrderItemsSubtotalInfo")]
        public string OrderItemsSubtotalInfo { get; set; }

        /// <summary>
        /// Gets or sets SellerInfo.
        /// </summary>
        [DataMember(Name = "SellerInfo")]
        public UISellerInfo SellerInfo { get; set; }
    }
}