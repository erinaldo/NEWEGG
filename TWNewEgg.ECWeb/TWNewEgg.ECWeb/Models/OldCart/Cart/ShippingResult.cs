using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class ShippingResult
    {
        public ShippingResult()
        {
            this.ShipFeeFlag = false;
            this.ShippingTotal = 0;
            this.JMShipping = 0;
            this.ShippingTotalbutJM = 0;
            this.ShippingTempStore = new List<ItemCartBox>();
        }
        public bool ShipFeeFlag { get; set; } // 是否含運費
        public decimal ShippingTotal { get; set; }
        public decimal JMShipping { get; set; }
        public decimal ShippingTotalbutJM { get; set; }
        public List<ItemCartBox> ShippingTempStore { get; set; }
    }
}