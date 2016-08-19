using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Item Attribute.
    /// </summary>
    [Serializable]
    public enum ItemAttribute
    {
        /// <summary>
        /// None Item.
        /// </summary>
        None = 0,

        /// <summary>
        /// Single Item.
        /// </summary>
        SingleItem = 1,

        /// <summary>
        /// Normal Combo Primary Item.
        /// </summary>
        NormalComboPrimaryItem = 2,

        /// <summary>
        /// Normal Combo Price Gift.
        /// </summary>
        NormalComboPriceGift = 3,

        /// <summary>
        /// Normal Combo Zero Price Gift.
        /// </summary>
        NormalComboZeroPriceGift = 4,

        /// <summary>
        /// Auto Add Combo Primary Item.
        /// </summary>
        AutoAddComboPrimaryItem = 5,

        /// <summary>
        /// Auto Add Combo Item.
        /// </summary>
        AutoAddComboItem = 6,

        /// <summary>
        /// Promotion Code Gift.
        /// </summary>
        PromotionCodeGift = 7,

        /// <summary>
        /// CellPhone Plan Item.
        /// </summary>
        CellPhonePlanItem = 8,

        /// <summary>
        /// CellPhone Phone Item.
        /// </summary>
        CellPhonePhoneItem = 9,

        /// <summary>
        /// CellPhone SIM Card.
        /// </summary>
        CellPhoneSIMCard = 10,
        
        /// <summary>
        /// CellPhone Accessory Item.
        /// </summary>
        CellPhoneAccessoryItem = 11,
    }
}
