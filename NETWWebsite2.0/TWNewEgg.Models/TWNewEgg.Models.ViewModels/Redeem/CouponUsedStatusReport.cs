using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.ViewModels.Redeem
{
    public class CouponsUsedStatusReport
    {
        /// <summary>
        /// Event Id
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Event Name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 使用狀態
        /// </summary>
        public Coupon.CouponUsedStatusOption CouponUsedStatusType { get; set; }

        /// <summary>
        /// 總數
        /// </summary>
        public int Count { get; set; }
    }
}
