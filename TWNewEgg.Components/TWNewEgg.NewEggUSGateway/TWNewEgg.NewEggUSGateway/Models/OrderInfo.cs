using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.NewEggUSGateway.Models
{
    public class OrderInfo : OrderInfoBase
    {
        /// <summary>
        /// 國內 or 國外
        /// </summary>
        public int IsInternational;
    }
}
