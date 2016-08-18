using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TWNewEgg.Website.ECWeb.Models
{
    public class OrderInfo : OrderInfoBase
    {
        /// <summary>
        /// 國內 or 國外
        /// </summary>
        public int IsInternational;
    }
}