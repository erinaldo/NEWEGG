using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Initializes a new instance of the UIShoppingCartListInfo class.
    /// </summary>
    [DataContract]
    public class UIShoppingCartListInfo
    {
        /// <summary>
        /// Gets or sets ShoppingItemList.
        /// </summary>
        [DataMember(Name = "ShoppingItemList")]
        public List<UIShoppingItemInfo> ShoppingItemList { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCombo.
        /// </summary>
        [DataMember(Name = "IsCombo")]
        public bool IsCombo { get; set; }

        /// <summary>
        /// Gets or sets ComboID.
        /// </summary>
        [DataMember(Name = "ComboID")]
        public int ComboID { get; set; }

        /// <summary>
        /// Gets or sets ComboType.
        /// </summary>
        [DataMember(Name = "ComboType")]
        public int ComboType { get; set; }

        /// <summary>
        /// Gets or sets PrimaryItemNumber.
        /// </summary>
        [DataMember(Name = "PrimaryItemNumber")]
        public string PrimaryItemNumber { get; set; }

        /// <summary>
        /// Gets or sets CaseID.
        /// </summary>
        [DataMember(Name = "CaseID")]
        public int CaseID { get; set; }

        /// <summary>
        /// Gets or sets IndexID.
        /// </summary>
        [DataMember(Name = "IndexID")]
        public int IndexID { get; set; }

        /// <summary>
        /// Gets or sets ItemQty.
        /// </summary>
        [DataMember(Name = "ItemQty")]
        public int ItemQty { get; set; }

        /// <summary>
        /// Gets or sets TotalPrice.
        /// </summary>
        [DataMember(Name = "TotalPrice")]
        public string TotalPrice { get; set; }

        /// <summary>
        /// Gets or sets ExtendPrice.
        /// </summary>
        [DataMember(Name = "ExtendPrice")]
        public string ExtendPrice { get; set; }

        /// <summary>
        /// Gets or sets ExtendUnitPrice.
        /// </summary>
        [DataMember(Name = "ExtendUnitPrice")]
        public string ExtendUnitPrice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnableEdited.
        /// </summary>
        [DataMember(Name = "IsEnableEdited")]
        public bool IsEnableEdited { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsSinglePreSelectCombo.
        /// </summary>
        [DataMember(Name = "IsSinglePreSelectCombo")]
        public bool IsSinglePreSelectCombo { get; set; }

        /// <summary>
        /// Gets or sets SinglePreSelectItem.
        /// </summary>
        [DataMember(Name = "SinglePreSelectItem")]
        public string SinglePreSelectItem { get; set; }

        /// <summary>
        /// Gets or sets ItemMapPriceMarkType.
        /// </summary>
        [DataMember(Name = "ItemMapPriceMarkType")]
        public UIMapPriceMarkType ItemMapPriceMarkType { get; set; }

        /// <summary>
        /// Gets or sets RelatedItemNumber.
        /// </summary>
        [DataMember]
        public string RelatedItemNumber { get; set; }
    }
}
