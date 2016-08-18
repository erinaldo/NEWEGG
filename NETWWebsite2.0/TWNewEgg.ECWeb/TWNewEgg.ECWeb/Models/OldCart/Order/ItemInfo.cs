using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    [System.Xml.Serialization.XmlInclude(typeof(ItemInfoExt))]
    [Serializable]
    public class ItemInfo
    {
        /// <summary>
        /// ItemNumber
        /// </summary>
        public string ItemNumber;

        /// <summary>
        /// DefaultShippingCharge
        /// </summary>
        public decimal DefaultShippingCharge;

        /// <summary>
        /// Quantity
        /// </summary>
        public int Quantity;
    }
}