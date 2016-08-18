using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Models.DomainModels.Redeem
{
    public class CouponsLite
    {
        public string couponID { get; set; }
        public string couponName { get; set; }
        public string couponValue { get; set; }
        public string couponCategories { get; set; }
        public string couponProductID { get; set; }
        public string couponLimit { get; set; }
        public string couponEndDate { get; set; }

        /// <summary>
        /// 限定使用的item, 以分號分隔
        /// </summary>
        public string couponItems { get; set; }
    }
}
