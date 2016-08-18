using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newegg.Mobile.Serialization;

namespace Newegg.Mobile.MvcApplication.Models
{
    /// <summary>
    /// Serializer Shipping method.
    /// </summary>
    [DataContract]
    public class ShippingMethodViewData
    {
        /// <summary>
        /// Initializes a new instance of the ShippingMethodViewData class.
        /// </summary>
        /// <param name="methodList">Method List.</param>
        /// <param name="orderIndex">Order Index.</param>
        public ShippingMethodViewData(List<UIShippingMethodInfo> methodList, int orderIndex)
        {
            this.Methods = new List<ShippingMethodItemViewData>();
            this.OrderIndex = orderIndex;

            methodList.ForEach(delegate(UIShippingMethodInfo method)
            {
                this.Methods.Add(new ShippingMethodItemViewData(method));
            });
        }

        /// <summary>
        /// Gets or sets Methods.
        /// </summary>
        [DataMember(Name = "ms")]
        public List<ShippingMethodItemViewData> Methods { get; set; }

        /// <summary>
        /// Gets or sets OrderIndex.
        /// </summary>
        [DataMember(Name = "index")]
        public int OrderIndex { get; set; }

        /// <summary>
        /// Get Json Methods.
        /// </summary>
        /// <returns>Return Json String.</returns>
        public string GetJsonMethods()
        {
            return SerializerFactory.CreateSerializer(SerializationType.JSON).Serialize(this);
        }
    }
}
