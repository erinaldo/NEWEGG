using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.PaymentGateway
{
    public class AllPayItem
    {
        /// <summary>
        /// 商品名稱。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 商品單價
        /// </summary>
        public Decimal Price { get; set; }

        /// <summary>
        /// 幣別單位。(例如：元)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public string Quantity { get; set; }

    }
}
